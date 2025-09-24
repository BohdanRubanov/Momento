using System.ComponentModel.DataAnnotations;

namespace Momento.Models
{
    public class Publication
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public string? PhotoUrl { get; set; } // Уберите Required - мы заполняем его программно

        public string Caption { get; set; }

        // Сделайте навигационное свойство nullable или не Required
        public virtual Registration? User { get; set; }
    }
}