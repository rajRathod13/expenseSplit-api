using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.RequestDTOs.User;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Common.ResponseDTOs.User;
using MediatR;

namespace ExpenseSplit.Application.Commands.UserCommands;

public class UpdateProfileCommand : IRequest<BaseResponse<UserResponse>>
{
    public UpdateProfileRequest UpdateProfileRequest { get; set; }
}

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, BaseResponse<UserResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProfileCommandHandler(IUnitOfWork uow,
        IMapper mapper,
        IFileService fileService,
        ICurrentUserService currentUserService)
    {
        _uow = uow;
        _mapper = mapper;
        _fileService = fileService;
        _currentUserService = currentUserService;
    }
    public async Task<BaseResponse<UserResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var email = _currentUserService.Email;
        if(string.IsNullOrWhiteSpace(email))
            return BaseResponse<UserResponse>.Fail($"Unauthorize user. Please login first.");

        var existingUser = await _uow.UserRepository.FindUserbyEmailAsync(email);
        if (existingUser is null)
            return BaseResponse<UserResponse>.Fail($"User not found with email - {email}");

        if (request.UpdateProfileRequest.ProfilePicture is not null)
        {
            var newPath = await _fileService.SaveFileAsync(request.UpdateProfileRequest.ProfilePicture, "ProfilePicture", existingUser.Id);

            if (string.IsNullOrWhiteSpace(newPath))
                return BaseResponse<UserResponse>.Fail("An error occured during updating the profile picture.");

            existingUser.ProfilePicture = newPath;
        }

        existingUser.FullName = request.UpdateProfileRequest.FullName;

        await _uow.UserRepository.UpdateProfileAsync(existingUser);
        var result = await _uow.SaveChangesAsync(cancellationToken);
        if (result > 0) {
            var mappedResponse = _mapper.Map<UserResponse>(existingUser);
            return BaseResponse<UserResponse>.Success(mappedResponse, "Profile details updated successfully.");
        }

        return BaseResponse<UserResponse>.Fail("An error occured during updating the profile.");
    }
}
