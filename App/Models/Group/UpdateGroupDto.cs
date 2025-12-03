using System.ComponentModel.DataAnnotations;

namespace TestShopApp.App.Models.Group;

public class UpdateGroupDto
{
    [Required]
    public string GroupNumber { get; set; }
    
    [StringLength(50, MinimumLength = 6, ErrorMessage = "Название должна быть от 6 до 50 символов")]
    [RegularExpression(@"^(?! )[a-zA-Zа-яА-ЯёЁ\s0-9]+(?<! )$", ErrorMessage = "Название не может содержать специальные символы")]
    public string? GroupName { get; set; }
}