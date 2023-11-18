using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Newlife.Models
{
    public class Doctor_Profile
    {
        [Key]
        public int Id { get; set; }
        public string DoctorID { get; set; }
        public string DoctorName { get; set; }        
        public string DoctorPassword { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string LanguageKnown { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
    }
}