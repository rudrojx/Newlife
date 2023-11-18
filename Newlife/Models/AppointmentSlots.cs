using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Newlife.Models
{
    public class AppointmentSlots
    {
        [Key]
        public int Id { get; set; }
        public string DoctorID { get; set; }
        public string DoctorName { get; set; }
        public string ClinicName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public int AvailableSlots { get; set; }
        public string IsAvailable { get; set; }
    }
}