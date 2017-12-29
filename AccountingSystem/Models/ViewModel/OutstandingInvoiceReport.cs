using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models.ViewModel
{
    public class OutstandingInvoiceReport
    {
        public int id { get; set; }
        public string invoice_no { get; set; }
        public string sbname { get; set; }
        public double Amount { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string phone { get; set; }
        public string Email { get; set; }
    }
}