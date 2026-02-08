using MVC.Models;
namespace MVC.interfaces;

public interface ILoginRegisterRepository
{
    t_Users Login(vm_Login user);
}