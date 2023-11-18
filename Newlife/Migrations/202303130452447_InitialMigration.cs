namespace Newlife.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoctorDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DoctorID = c.String(),
                        DoctorName = c.String(),
                        Specialization = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        Experience = c.Int(nullable: false),
                        Clinic_Charge = c.Int(nullable: false),
                        Education = c.String(),
                        Bio = c.String(),
                        ProfilePicture = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.patient_profile",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        First_Name = c.String(),
                        Last_Name = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        DateofBirth = c.String(),
                        PictureUrl = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.patient_profile");
            DropTable("dbo.DoctorDetails");
        }
    }
}
