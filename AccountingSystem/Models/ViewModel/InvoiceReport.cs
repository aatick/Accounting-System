using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models.ViewModel
{
    public class InvoiceReport
    {
        public string Invoice_No { get; set; }
        public DateTime InvSendDt { get; set; }
        public string CName { get; set; }
        public string sbname { get; set; }
        public double amount { get; set; }
        public string comments { get; set; }
        public string bname { get; set; }
        public string designation { get; set; }
        public string RefNo { get; set; }
        public string VATRegNo { get; set; }
    }
}