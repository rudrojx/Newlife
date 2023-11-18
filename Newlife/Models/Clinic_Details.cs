using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Newlife.Models
{
    public class Clinic_Details
    {
        [Key]
        public int Id { get; set; }
        public string DoctorID { get; set; }
        public string DoctorName { get; set; }
        public string ClinicName { get; set; }
        public string DoctorSpeciality { get; set; }
        public string Location { get; set; }
        public string Clinic_Address { get; set; }
        public int ClinicFee { get; set; }
        public string Clinic_Timing { get; set; }
        public string Doctor_Availability { get; set; }
    }
}