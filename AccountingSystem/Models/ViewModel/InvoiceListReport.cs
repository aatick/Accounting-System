using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models.ViewModel
{
    public class InvoiceListReport
    {
        public int id { get; set; }
        public string invoice_no { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string phone { get; set; }
        public double tamount { get; set; }
        public DateTime invsendDt { get; set; }
        public string AccContactName { get; set; }

    }
}