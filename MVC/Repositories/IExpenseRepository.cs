using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVC.Models;

namespace MVC.implement
{
    public interface IExpenseRepository
    {
        public Task<List<t_transaction>> getAllExpense(int userid);

        public Task<int> addExpense(t_transaction transaction);
        public Task<t_transaction> getExpenseById(int id);
        public Task<int> updateExpense(t_transaction transaction);
        public Task<int> deleteExpense(int id);

        public Task<int> totalExpense(int id);
    }
}