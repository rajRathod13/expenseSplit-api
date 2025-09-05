using AutoMapper;
using ExpenseSplit.Common.RequestDTOs.Expense;
using ExpenseSplit.Common.ResponseDTOs.ExpenseDTO;
using ExpenseSplit.Domain.Entities;

namespace ExpenseSplit.Application.MappingProfiles;

public class ExpenseProfiles : Profile
{
    public ExpenseProfiles()
    {
        CreateMap<UpsertExpenseRequest, Expense>()
            .ForMember(d => d.SplitDetails, o => o.Ignore())
            .ReverseMap();

        CreateMap<SplitDetailDTO, SplitDetail>()
            .ForMember(desc => desc.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(desc => desc.Percentage, opt => opt.MapFrom(src => src.Percentage))
            .ForMember(desc => desc.ShareAmount, opt => opt.MapFrom(src => src.ShareAmount))
            .ReverseMap();

        CreateMap<ExpenseResponse, Expense>()
            .ForMember(desc => desc.SplitDetails, opt => opt.MapFrom(src => src.SplitDetails))
            .ForMember(desc => desc.User, opt => opt.MapFrom(src => src.User))
            .ForMember(desc => desc.GroupDetail, opt => opt.MapFrom(src => src.GroupDetail))
            .ForMember(desc => desc.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn))
            .ReverseMap();

        CreateMap<SplitDetailResponse, SplitDetail>()
            .ForMember(desc => desc.User, opt => opt.MapFrom(src => src.User))
            .ReverseMap();
    }
}
