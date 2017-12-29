using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AccountingSystem.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        [Required(ErrorMessage = "User Name is required")]
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string AccessRight { get; set; }
        public bool CanApprove { get; set; }
        public string ApproveRight { get; set; }
        public bool CanModifyAdmin { get; set; }
        public string AccessReports { get; set; }
        public bool ValidUser { get; set; }
        public bool AccountDep { get; set; }

    }
}