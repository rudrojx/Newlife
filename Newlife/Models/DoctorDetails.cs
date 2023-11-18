using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Newlife.Models
{
    public class DoctorDetails
    {
        [Key]
        public int Id { get; set; }
        public string DoctorID { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Experience { get; set; }
        public int Clinic_Charge { get; set; }
        public string Education { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
        public string ProfilePicture { get; set; }
        public string Active { get; set; }
    }
}