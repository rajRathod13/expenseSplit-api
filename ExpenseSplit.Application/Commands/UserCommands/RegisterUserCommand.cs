using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.RequestDTOs.User;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Common.ResponseDTOs.User;
using ExpenseSplit.Domain.Entities;
using ExpenseSplit.Domain.Interfaces;
using MediatR;

namespace ExpenseSplit.Application.Commands.UserCommands;

public class RegisterUserCommand : IRequest<BaseResponse<UserResponse>>
{
    public RegisterUserRequest RegisterUserRequest { get; set; }
}

public class RegisteruserCommandHandler : IRequestHandler<RegisterUserCommand, BaseResponse<UserResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public RegisteruserCommandHandler(IUnitOfWork uow,
        IFileService fileService,
        IMapper mapper)
    {
        _uow = uow;
        _fileService = fileService;
        _mapper = mapper;
    }
    public async Task<BaseResponse<UserResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _uow.UserRepository.FindUserbyEmailAsync(request.RegisterUserRequest.Email);
        if (existingUser is not null)
            throw new Exception($"User already exist with email - {request.RegisterUserRequest.Email}");

        var userId = Guid.NewGuid().ToString();
        var profilePicturePath = await _fileService.SaveFileAsync(request.RegisterUserRequest.ProfilePictureImage, "ProfilePicture", userId);

        var mappedUser = _mapper.Map<User>(request.RegisterUserRequest);
        mappedUser.Id = userId;
        mappedUser.ProfilePicture = profilePicturePath;
        mappedUser.UserName = request.RegisterUserRequest.Email;
        mappedUser.CreatedOn = DateTime.UtcNow;

        var entity = await _uow.UserRepository.RegisterUserAsync(mappedUser, request.RegisterUserRequest.Password);
        if (entity is null)
            return BaseResponse<UserResponse>.Fail("Something went wrong.");

        var mappedResponse = _mapper.Map<UserResponse>(entity);
        return BaseResponse<UserResponse>.Success(mappedResponse, "Registration done successfully.");
    }
}
