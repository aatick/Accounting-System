using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public string Tno { get; set; }
        public string SbName { get; set; }
        public double Amount { get; set; }
        public string Comment { get; set; }

        public string CompanyName { get; set; }
        public string SDate { get; set; }
        public int TotalRecords { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public double AccReceivale { get; set; }
        public bool IsPosted { get; set; }
        public int Duration { get; set; }
        public double Tax { get; set; }
        public int BillingContactId { get; set; }
        public string RefNo { get; set; }
        public int TaxId { get; set; }
    }
}