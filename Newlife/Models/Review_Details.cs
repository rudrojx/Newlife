using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Newlife.Models
{
    public class Review_Details
    {
        [Key]
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string PatientID { get; set; }
        public string Health_Cause { get; set; }
        public string Clinic_Location { get; set; }
        public string Doctor_Review { get; set; }
        public string DoctorName { get; set; }
        public string DoctorID { get; set; }
    }
}