using System.ComponentModel.DataAnnotations;

namespace MVC.Models;

public class vm_ChangePassword
{
    [Required (ErrorMessage = "Current password is required.")]
    public string CurrentPassword { get; set;}

    [Required(ErrorMessage = "New password is required.")]
    [MinLength(6, ErrorMessage = "Password must be atleast 6 charachters.")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
    [MinLength(6, ErrorMessage = "Password must be atleast 6 charachters.")]
    public string ConfirmPassword { get; set; }
}
