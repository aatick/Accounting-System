using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountingSystem.Models
{
    public class Company
    {
        public int Id { get; set; }
        [DisplayName("Company Name")]
        //[Remote("CheckCompanyName","Company",ErrorMessage = "This Company already exist")]
        //[Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        //[Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        //[Required(ErrorMessage = "City is required")]
        public string City { get; set; }
        //[Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }
        //[Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Fax { get; set; }
        //[Required(ErrorMessage = "Contact Person is required")]
        public string ContactPerson { get; set; }
        public string Designation { get; set; }
        public double Balance { get; set; }
        public bool BlackListed { get; set; }
        [DisplayName("Online Id")]
        //[Required(AllowEmptyStrings = true)]
        [DataType(DataType.Text)]
        public int CP_Id { get; set; }
        [DisplayName("Acc. Person")]
        public string AccContactName { get; set; }
        public string VatRegNo { get; set; }
        public string VatRegAdd { get; set; }

        public string AccCreatedDate { get; set; }

    }
}