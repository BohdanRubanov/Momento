using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Momento.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Registration
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter your Nickname")]
        public string Nickname { get; set; } = null!;

        [Required(ErrorMessage = "Enter your Email")]
        [EmailAddress(ErrorMessage = "Enter a valid Email address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Enter Password")]
        [MinLength(8, ErrorMessage = "Your password must be at least 8 characters")]
        public string Password { get; set; } = null!;

        public string? Status { get; set; }

        public string? AvatarUrl { get; set; }

        // Навигационное свойство для коллекции публикаций пользователя
        public virtual List<Publication> Publications { get; set; } = new List<Publication>();
    }
}