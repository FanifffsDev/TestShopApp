using System.ComponentModel.DataAnnotations;

namespace TestShopApp.App.Models
{
    public class TelegramInitDataRawDto
    {
        [Required]
        public string InitData { get; set; }

        [Required]
        public long Timestamp { get; set; }
    }
}
