using System.ComponentModel.DataAnnotations;

namespace TestShopApp.Domain.Base;

public class GroupStyles
{
    [Required]
    public string GroupNumber { get; set; }
    
    [Required]
    public string CustomHeadmanPrefix { get; set; }
    
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? BackgroundColor { get; set; }
    public string? CardBorderColor { get; set; }
}