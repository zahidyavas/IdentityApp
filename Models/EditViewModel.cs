using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Models
{
    public class EditViewModel
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        public IList<string> SelectedRoles { get; set; } = new List<string>();
    }
}
