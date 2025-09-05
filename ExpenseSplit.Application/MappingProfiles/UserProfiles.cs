using AutoMapper;
using ExpenseSplit.Common.RequestDTOs.User;
using ExpenseSplit.Common.ResponseDTOs.AuthDTOs;
using ExpenseSplit.Common.ResponseDTOs.User;
using ExpenseSplit.Domain.Entities;

namespace ExpenseSplit.Application.MappingProfiles;

public class UserProfiles : Profile
{
    public UserProfiles()
    {
        CreateMap<RegisterUserRequest, User>()
                 .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                 .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                 .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
        CreateMap<UserResponse, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.ProfilePicture))
                .ReverseMap();

        CreateMap<LoginResponse, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.ProfilePicture))
            .ReverseMap();


        CreateMap<UpdateProfileRequest, User>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
