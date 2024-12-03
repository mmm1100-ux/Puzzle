using System.ComponentModel.DataAnnotations;

namespace Puzzle.Models.AccountViewModels
{
    public class LoginViewModel
    {
        //[Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Mobile { get; set; }

        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
