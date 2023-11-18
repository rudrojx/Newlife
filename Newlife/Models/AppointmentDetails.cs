using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Newlife.Models
{
    public class AppointmentDetails
    {
        [Key]
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string Email { get; set; }
        public string DoctorName { get; set; }
        public string ClinicName { get; set; }
        public string ClinicLocation { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string BookingStatus { get; set; }
        public string ConfirmationCode { get; set; }
    }
}