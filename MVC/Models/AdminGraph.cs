namespace MVC.Models;

 public class CategoryExpense
    {
        public string Category { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class MonthlyExpense
    {
        public string Month { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class PaymentModeExpense
    {
        public string PaymentMode { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class UserExpense
    {
        public string Name { get; set; }
        public decimal TotalSpent { get; set; }
    }