using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TestShopApp.App.Models.Group
{
    public class GroupMemberDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsHeadman { get; set; }
    }
}
