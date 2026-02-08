using System.ComponentModel.DataAnnotations;
namespace MVC.Models;

public class vm_UpdateProfile
{
     public int UserID { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    public string? Gender { get; set; }
    // public string? Status { get; set; }
    // public string? Role { get; set; }
    public IFormFile? User_ProfileImage { get; set; }
    public string? ProfileImage { get; set; }
}