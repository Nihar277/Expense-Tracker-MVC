using System.ComponentModel.DataAnnotations;

namespace MVC.Models;

public class t_Users
{
    [Key]
    public int UserID { get; set; }

    [Required(ErrorMessage = "name is required.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm Password is required.")]
    public string ConfirmPassword { get; set; }

    public string? Gender { get; set; }

    public string? Status { get; set; }

    public string? ProfileImage { get; set; }

    public IFormFile? User_ProfileImage { get; set; }
    
    public string? Role { get; set; }

}