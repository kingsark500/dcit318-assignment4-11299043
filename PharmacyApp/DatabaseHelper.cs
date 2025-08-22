using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PharmacyApp
{
    public partial class DatabaseHelper
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["PharmacyDB"].ConnectionString;

        public void AddMedicine(string name, string category, decimal price, int quantity)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("AddMedicine", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", name);

                cmd.Parameters.AddWithValue("@Category", category);
                cmd.Parameters.AddWithValue("@Price", price);

                cmd.Parameters.AddWithValue("@Quantity", quantity);

                con.Open();

                cmd.ExecuteNonQuery();

                MessageBox.Show("Medicine Added Successfully!");
            }
        }

        public DataTable SearchMedicine(string searchTerm)
        {
            using (SqlConnection con = new SqlConnection(connectionString))

            using (SqlCommand cmd = new SqlCommand("SearchMedicine", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SearchTerm", searchTerm);

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        public void UpdateStock(int medicineID, int quantity)
        {
            using (SqlConnection con = new SqlConnection(connectionString))

            using (SqlCommand cmd = new SqlCommand("UpdateStock", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MedicineID", medicineID);

                cmd.Parameters.AddWithValue("@Quantity", quantity);

                con.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Stock Updated Successfully!");
            }
        }

        public void RecordSale(int medicineID, int quantitySold)
        {
            using (SqlConnection con = new SqlConnection(connectionString))

            using (SqlCommand cmd = new SqlCommand("RecordSale", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MedicineID", medicineID);

                cmd.Parameters.AddWithValue("@QuantitySold", quantitySold);

                con.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Sale Recorded Successfully!");
            }
        }

        public DataTable GetAllMedicines()
        {
            using (SqlConnection con = new SqlConnection(connectionString))

            using (SqlCommand cmd = new SqlCommand("GetAllMedicines", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }
    }
}
