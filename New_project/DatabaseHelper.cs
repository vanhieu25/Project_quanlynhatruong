using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace New_project
{
    public class DatabaseHelper
    {
        private readonly string connectionString = @"Data Source=LAPTOP-LJP88J8V;Initial Catalog=QLHoctap;Integrated Security=True;TrustServerCertificate=True";

        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
 {
            DataTable dataTable = new DataTable();
  try
  {
     using (SqlConnection connection = new SqlConnection(connectionString))
 using (SqlCommand command = new SqlCommand(query, connection))
    {
           if (parameters != null)
         command.Parameters.AddRange(parameters);
        
             connection.Open();
          using (SqlDataAdapter adapter = new SqlDataAdapter(command))
         {
          adapter.Fill(dataTable);
         }
     }
  }
 catch (Exception ex)
            {
  MessageBox.Show($"L?i k?t n?i c? s? d? li?u: {ex.Message}", "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
  return dataTable;
        }

        public DataTable ExecuteStoredProcedure(string procedureName, SqlParameter[] parameters = null)
        {
            DataTable dataTable = new DataTable();
         try
            {
    using (SqlConnection connection = new SqlConnection(connectionString))
   using (SqlCommand command = new SqlCommand(procedureName, connection))
         {
   command.CommandType = CommandType.StoredProcedure;
      if (parameters != null)
  command.Parameters.AddRange(parameters);
               
            connection.Open();
     using (SqlDataAdapter adapter = new SqlDataAdapter(command))
    {
         adapter.Fill(dataTable);
      }
      }
            }
            catch (Exception ex)
 {
          MessageBox.Show($"L?i th?c thi stored procedure: {ex.Message}", "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
   }
 return dataTable;
  }

   public bool ExecuteNonQuery(string procedureName, SqlParameter[] parameters = null)
        {
            try
            {
       using (SqlConnection connection = new SqlConnection(connectionString))
    using (SqlCommand command = new SqlCommand(procedureName, connection))
     {
               command.CommandType = CommandType.StoredProcedure;
   if (parameters != null)
  command.Parameters.AddRange(parameters);
           
        connection.Open();
           int rowsAffected = command.ExecuteNonQuery();
         return rowsAffected > 0;
        }
   }
          catch (Exception ex)
            {
         MessageBox.Show($"L?i th?c thi l?nh: {ex.Message}", "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
      return false;
    }
        }

    public object ExecuteScalar(string procedureName, SqlParameter[] parameters = null)
        {
        try
        {
      using (SqlConnection connection = new SqlConnection(connectionString))
             using (SqlCommand command = new SqlCommand(procedureName, connection))
  {
            command.CommandType = CommandType.StoredProcedure;
      if (parameters != null)
        command.Parameters.AddRange(parameters);
    
     connection.Open();
    return command.ExecuteScalar();
     }
 }
   catch (Exception ex)
   {
        MessageBox.Show($"L?i th?c thi l?nh: {ex.Message}", "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
       return null;
       }
        }
    }
}
