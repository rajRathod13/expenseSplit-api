using AutoMapper;
using ExpenseSplit.Common.RequestDTOs.Group;
using ExpenseSplit.Common.RequestDTOs.GroupCategory;
using ExpenseSplit.Common.ResponseDTOs.GroupCategory;
using ExpenseSplit.Common.ResponseDTOs.GroupDetail;
using ExpenseSplit.Common.ResponseDTOs.Invitation;
using ExpenseSplit.Common.ResponseDTOs.UserGroup;
using ExpenseSplit.Domain.Entities;

namespace ExpenseSplit.Application.MappingProfiles;

public class GropProfiles : Profile
{
    public GropProfiles()
    {
        CreateMap<GroupCategoryMaster, AddGroupCategoryRequest>().ReverseMap();
        CreateMap<GroupCategoryMaster, GroupCategoryResponse>().ReverseMap();

        CreateMap<AddGroupRequest, GroupDetail>().ReverseMap();
        CreateMap<GroupDetailResponse,GroupDetail>()
            .ForMember(desc => desc.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn))
            .ForMember(desc => desc.User, opt => opt.MapFrom(src => src.User))
            .ForMember(desc => desc.UserGroupRefs, opt => opt.MapFrom(src => src.UserGroups))
            .ReverseMap();

        CreateMap<UserGroupResponse, UserGroupRef>()
            .ReverseMap();

        CreateMap<InvitationResponse, Invitation>()
           .ForMember(desc => desc.Group, opt => opt.MapFrom(src => src.Group))
           .ForMember(desc => desc.InviterUser, opt => opt.MapFrom(src => src.InviterUser))
           .ForMember(desc => desc.InvitedUser, opt => opt.MapFrom(src => src.InvitedUser))
           .ReverseMap();
    }
}
