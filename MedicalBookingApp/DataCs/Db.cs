using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalBookingApp.DataCs
{
    public static class Db
    {
        private static string ConnStr => ConfigurationManager.ConnectionStrings["MedicalDb"].ConnectionString;

        public static SqlConnection GetOpenConnection()
        {
            var conn = new SqlConnection(ConnStr);
            conn.Open();
            return conn;
        }

        public static SqlCommand CreateCommand(SqlConnection conn, string sql, CommandType type = CommandType.Text)
        {
            var cmd = new SqlCommand(sql, conn) { CommandType = type };
            return cmd;
        }

        public static SqlParameter In(string name, SqlDbType type, object value)
        {
            return new SqlParameter(name, type)
            {
                Direction = ParameterDirection.Input,
                Value = value ?? DBNull.Value
            };
        }
    }
}
