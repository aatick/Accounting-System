using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class Remark
    {
        public int Id { get; set; }
        public string RemarkDate { get; set; }
        public string Remarks { get; set; }
        public int InvoiceId { get; set; }

    }
}