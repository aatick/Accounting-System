using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models.ViewModel
{
    public class InvoiceRemarkReport
    {
        public int id { get; set; }
        public string invoice_no { get; set; }
        public double TAmount { get; set; }
        public DateTime RemarkDate { get; set; }
        public string Remarks { get; set; }
        public string UName { get; set; }
        public string CName { get; set; }
        public string BCName { get; set; }
        public string designation { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string Email { get; set; }

    }
}