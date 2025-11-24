using System.ComponentModel.DataAnnotations;

namespace TestShopApp.App.Models.User
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Имя должно быть от 2 до 20 символов")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ\s]+$", ErrorMessage = "Имя может содержать только буквы")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Фамилия должна быть от 2 до 20 символов")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ\s]+$", ErrorMessage = "Фамилия может содержать только буквы")]
        public string LastName { get; set; }

        [StringLength(20, MinimumLength = 2, ErrorMessage = "Отчество должно быть от 2 до 20 символов")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ\s]*$", ErrorMessage = "Отчество может содержать только буквы")]
        public string? ThirdName { get; set; }

        //[StringLength(6, ErrorMessage = "Номер группы не должен превышать 6 символов")]
        //[RegularExpression(@"^[0-9]*$", ErrorMessage = "Группа может содержать только цифры")]
        //public string? Group { get; set; }

        [StringLength(100, ErrorMessage = "Название предмета не должно превышать 100 символов")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ\s]*$", ErrorMessage = "Предмет может содержать только буквы")]
        public string? Subject { get; set; }
    }
}
