using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class Ledger
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Group Name is required")]
        public string GroupName { get; set; }
        public string Under { get; set; }
        [Required(ErrorMessage = "Main Group is required")]
        public string MaingroupName { get; set; }
        public int LevelNo { get; set; }
        public double OpeningBalance { get; set; }
        public DateTime OpeningDate { get; set; }
        public bool IsLedgerAccount { get; set; }
        public double Balance { get; set; }
        public string Account { get; set; }

        public string sbname { get; set; }
        public string mgroup { get; set; }
        public double Total { get; set; }

    }
}