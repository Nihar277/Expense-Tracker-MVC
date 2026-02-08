using System.ComponentModel.DataAnnotations;

namespace MVC.Models;

public class t_transaction
{
    [Key]
    public int TransID { get; set; }
    public int UserID { get; set; }

    public DateTime TransDate { get; set; }

    public string Category { get; set; }

    public float Amount { get; set; }

    public string PaymentMode { get; set; }

    public string? ReceiptImage { get; set; }

    public IFormFile? transaction_ReceiptImage { get; set; }

    public string? Description { get; set; }

}