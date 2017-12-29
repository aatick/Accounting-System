using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models.ViewModel
{
    public class ProductForInvoice
    {
        public int Id { get; set; }
        public string ListItem { get; set; }
        public string Product { get; set; }
        public int LedgerId { get; set; }
        public string SbName { get; set; }
        public string SDate { get; set; }
        public string EDate { get; set; }
        public string Comments { get; set; }
        public double Amount { get; set; }
        public int TNO { get; set; }

        public string Company { get; set; }
        public double TD { get; set; }
        public DateTime sdate { set; get; }
    }
}