using AutoMapper;
using TestShopApp.App.Models.Group;
using TestShopApp.Domain.Base;

namespace TestShopApp.App.Profiles;

public class GroupProfile : Profile
{
    public GroupProfile()
    {
        CreateMap<Group, SafeGroupDto>();

        CreateMap<User, GroupMemberDto>()
            .ForMember(dest => dest.IsHeadman, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.HeadmanOf) && src.HeadmanOf == src.GroupNumber)); ;
    }
}