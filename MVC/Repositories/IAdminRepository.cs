using MVC.Models;
namespace MVC.interfaces;

public interface IAdminRepository
{
    List<t_Users> GetAllUsers();
        bool ToggleStatus(int id);
        List<t_transaction> GetTransactionsByUser(int userId);
}