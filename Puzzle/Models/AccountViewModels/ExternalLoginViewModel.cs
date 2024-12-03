using System.ComponentModel.DataAnnotations;

namespace Puzzle.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
