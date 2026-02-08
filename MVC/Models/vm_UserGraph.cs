using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class vm_UserGraph
    {
        public int TransID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }

        public DateTime TransDate { get; set; }

        public string Category { get; set; }

        public float Amount { get; set; }

        public string PaymentMode { get; set; }
    }
}