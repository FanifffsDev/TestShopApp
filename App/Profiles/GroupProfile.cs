using AutoMapper;
using TestShopApp.App.Models.Group;
using TestShopApp.Common.Data;

namespace TestShopApp.App.Profiles;

public class GroupProfile : Profile
{
    public GroupProfile()
    {
        CreateMap<Group, SafeGroupDto>();
    }
}