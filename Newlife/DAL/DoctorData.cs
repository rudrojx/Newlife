using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Newlife.DAL
{
    public class DoctorData
    {
        private string connectionString;

        public DoctorData()
        {
            // Get the connection string from your web.config file
            connectionString = ConfigurationManager.ConnectionStrings["NewlifeConnection"].ConnectionString;
        }

        //public List<>

        public void AddRecord(string firstName, string lastName, string email)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("AddRecord", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Email", email);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}