using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using TestShopApp.App.Models.User;
using TestShopApp.Domain.Base;

namespace TestShopApp.App.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UpdateUserProfileDto, User>();
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
