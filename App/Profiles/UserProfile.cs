using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using TestShopApp.App.Models.User;
using TestShopApp.Common.Data;

namespace TestShopApp.App.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UpdateUserDto, User>();
        }
    }
}
