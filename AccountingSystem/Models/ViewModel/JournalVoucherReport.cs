using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models.ViewModel
{
    public class JournalVoucherReport
    {
        public string sbname { get; set; }
        public string description { get; set; }
        public double debt { get; set; }
        public double credit { get; set; }
        public DateTime jdate { get; set; }
        public string Postedby { get; set; }
        public string des1 { get; set; }
        public DateTime PostDate { get; set; }
        public string ApprBy { get; set; }
        public string des2 { get; set; }
        public DateTime ApprovalDate { get; set; }
        public string VoucherNo { get; set; }
    }
}