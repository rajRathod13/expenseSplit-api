using AutoMapper;
using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.RequestDTOs.GroupCategory;
using ExpenseSplit.Common.ResponseDTOs;
using ExpenseSplit.Common.ResponseDTOs.GroupCategory;
using ExpenseSplit.Domain.Entities;
using MediatR;

namespace ExpenseSplit.Application.Commands.GroupCategory;

public class AddGroupCategoryCommand : IRequest<BaseResponse<GroupCategoryResponse>>
{
    public AddGroupCategoryRequest AddGroupCategoryRequest { get; set; }
}


public class AddGroupCategoryCommandHandler : IRequestHandler<AddGroupCategoryCommand, BaseResponse<GroupCategoryResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public AddGroupCategoryCommandHandler(IUnitOfWork uow,
        IFileService fileService,
        IMapper mapper)
    {
        _uow = uow;
        _fileService = fileService;
        _mapper = mapper;
    }
    public async Task<BaseResponse<GroupCategoryResponse>> Handle(AddGroupCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryId = Guid.NewGuid();
        var filePath = await _fileService.SaveFileAsync(request.AddGroupCategoryRequest.CategoryIcon, "GroupCategory", categoryId.ToString());
        
        var mappedEntity = _mapper.Map<GroupCategoryMaster>(request.AddGroupCategoryRequest);
        mappedEntity.CategoryId = categoryId;
        mappedEntity.CategoryIcon = filePath;
        mappedEntity.CreatedOn = DateTime.UtcNow;
        
        var result = await _uow.GroupCategoryMasterRepository.AddAsync(mappedEntity);
        await _uow.SaveChangesAsync(cancellationToken);
        var mappedResponse = _mapper.Map<GroupCategoryResponse>(result);
        if (result is not null)
            return BaseResponse<GroupCategoryResponse>.Success(mappedResponse, "Group category inserted successfully.");
        
        return BaseResponse<GroupCategoryResponse>.Fail("An error occured during inserting the record.");
    }
}