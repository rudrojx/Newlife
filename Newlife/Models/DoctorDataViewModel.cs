using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Newlife.Models
{
    public class DoctorDataViewModel
    {
        public DoctorDetails Doctorinfo { get; set; }
        public List<DoctorDetails> docdetails { get; set; }
        public List<Clinic_Details> cliniclist { get; set; }
        public Clinic_Details clinicinformation { get; set; }
        public AppointmentSlots appslots { get; set; }
        public List<AppointmentSlots> appslotslist { get; set; }
        public List<Review_Details> reviewlists { get; set; }
        public List<patient_profile> userDetails { get; set; }
    }
}