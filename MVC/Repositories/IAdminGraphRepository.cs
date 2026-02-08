using MVC.Models;
namespace MVC.interfaces;

public interface IAdminGraphRepository
{

    List<CategoryExpense> GetCategoryWiseExpense();

    List<MonthlyExpense> GetMonthlyExpenseTrend();

    List<PaymentModeExpense> GetPaymentModeSummary();

    List<UserExpense> GetTopSpendingUsers();

    int GetActiveUserCount();
    int GetInactiveUserCount();
    int GetTotalUserCount();


}