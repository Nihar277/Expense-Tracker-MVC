using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVC.Models;

namespace MVC.Repositories
{
    public interface IUserGraphRepositories
    {
        public List<vm_UserTransactionGraph> getAllTransactionByUserId(int id);
        public List<vm_UserBarChart> getAllTransactionByMonth(int id);
    }
}