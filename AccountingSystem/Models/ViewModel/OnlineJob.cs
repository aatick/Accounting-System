using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models.ViewModel
{
    public class OnlineJob
    {
        public int Serial { get; set; }
        public int CpId { get; set; }
        public int AccId { get; set; }
        public string Company { get; set; }
        public int JobPosted { get; set; }
        public int TotalJobPosted { get; set; }
        public int OpId { get; set; }
        public int AddType { get; set; }
        public string CompanyStatus { get; set; }
        public string Date { get; set; }
    }
}