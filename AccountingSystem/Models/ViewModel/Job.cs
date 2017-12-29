using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models.ViewModel
{
    public class Job
    {
        public int JpId { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string PostingDate { get; set; }
        public string ValidDate { get; set; }
        public int AddType { get; set; }
    }
}