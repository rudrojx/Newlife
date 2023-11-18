using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Newlife.Models
{
    public class NewlifeDBContext : DbContext
    {
        public NewlifeDBContext()
            : base("name=NewlifeConnection")
        { 
    }

        public DbSet<DoctorDetails> DocDetail { get; set; }
        public DbSet<patient_profile> Userinfo { get; set; }
        public DbSet<Doctor_Profile> DocProfile { get; set; }
        public DbSet<Clinic_Details> Clinicinfo { get; set; }
        public DbSet<AdminDetails> Admininfo { get; set; }
        public DbSet<AppointmentSlots> Appslotinfo { get; set; }
        public DbSet<AppointmentDetails> Appointmenlists { get; set; }
        public DbSet<Review_Details> reviewdetails { get; set; }
    }
}