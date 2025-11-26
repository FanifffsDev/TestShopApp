using System.ComponentModel.DataAnnotations;

namespace TestShopApp.App.Models.Group;

public class CreateGroupDto
{
    [Required(ErrorMessage = "Номер группы обязателен")]
    [StringLength(6, ErrorMessage = "Номер группы не может превышать символов")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Номер группы может содержать только цифры")]
    public string Number { get; set; }

    [Required(ErrorMessage = "Название обязательно")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "Название должна быть от 2 до 50 символов")]
    [RegularExpression(@"^(?! )[a-zA-Zа-яА-ЯёЁ\s0-9]+(?<! )$", ErrorMessage = "Название не может содержать специальные символы")]
    public string Name { get; set; }
}