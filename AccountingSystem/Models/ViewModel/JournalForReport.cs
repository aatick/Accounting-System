using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models.ViewModel
{
    public class JournalForReport
    {
        public int id{ get; set; }
        public int lid { get; set; }
        public string sbname { get; set; }
        public string Description { get; set; }
        public double Debt { get; set; }
        public double Credit { get; set; }
        public DateTime jDate { get; set; }
        public int jid { get; set; }
        public string Approval { get; set; }
        public string Users { get; set; }
        public DateTime PostDate { get; set; }
    }
}