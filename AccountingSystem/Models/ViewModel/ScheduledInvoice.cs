using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models.ViewModel
{
    public class ScheduledInvoice
    {
        public int CId { get; set; }
        public string Name { get; set; }
        public string SbName { get; set; }
        public double SalesPrice { get; set; }
        public int Id { get; set; }
        public int InvshdlNo { get; set; }
        public string ScheduleDate { get; set; }
        public double Amount { get; set; }
    }
}