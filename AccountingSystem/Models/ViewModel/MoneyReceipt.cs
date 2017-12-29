using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models.ViewModel
{
    public class MoneyReceipt
    {
        public string Invoice_No { get; set; }
        public string TAmount { get; set; }
        public string Name { get; set; }
        public string sbname { set; get; }
        public string MoneyReceiptNo { set; get; }
    }
}