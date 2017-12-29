using System;

namespace AccountingSystem.Models
{
    public class Journal
    {
        public int Id { get; set; }
        public int JId { get; set; }
        public int SId { get; set; }
        public string Description { get; set; }
        public double Debt { get; set; }
        public double Credit { get; set; }
        public string AccType { get; set; }
        public DateTime JDate { get; set; }
        public int Tno { get; set; }
        public string Notify { get; set; }
        public DateTime PostDate { get; set; }
        public bool Lock { get; set; }
        public int UserId { get; set; }
        public int ApprovedBy { get; set; }
        public DateTime ApprovalDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public int TotalRecord { get; set; }
        public string Journaldate { get; set; }

        public string AccName { get; set; }
        public string Group { set; get; }
        public int Background { get; set; }
    }
}