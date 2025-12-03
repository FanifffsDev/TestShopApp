using System.ComponentModel.DataAnnotations;

namespace TestShopApp.App.Models.Group;

public class UpdateGroupStylesDto
{
    [Required]
    public string GroupNumber { get; set; }
    
    [StringLength(50, MinimumLength = 6, ErrorMessage = "Название должна быть от 6 до 50 символов")]
    [RegularExpression(@"^(?! )[a-zA-Zа-яА-ЯёЁ\s0-9]+(?<! )$", ErrorMessage = "Название не может содержать специальные символы")]
    public string? GroupName { get; set; }
    
    [StringLength(14, MinimumLength = 1, ErrorMessage = "Custom headman prefix should be at least 1 character long")]
    [RegularExpression(@"^(?! )[a-zA-Zа-яА-ЯёЁ\s0-9]+(?<! )$", ErrorMessage = "Prefix ")]
    public string? CustomHeadmanPrefix { get; set; }
    
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
}