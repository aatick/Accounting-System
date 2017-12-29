using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class ContactPerson
    {
        public int Id { get; set; }
        public int CId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string PType { get; set; }
    }
}