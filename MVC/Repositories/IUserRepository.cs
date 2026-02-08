using MVC.Models;
namespace MVC.interfaces;

public interface IUserRepository
{
    Task<int> Register(t_Users user);
    //Change user password
    bool ChangePassword(int id, string currentPassword, string newPassword);

    //update profile
    t_Users GetById(int id);
    bool UpdateUser(t_Users user);
}