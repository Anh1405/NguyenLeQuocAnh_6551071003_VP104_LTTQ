using System;
using System.Data;
using System.Data.SqlClient; 

namespace NguyenLeQuocAnh_6551071003_VP104_LTTQ
{
    public class DatabaseConnect
    {
        
        private string strConn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DuLieu_NhanSu;Integrated Security=True;TrustServerCertificate=True";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(strConn);
        }

       
        public DataTable GetData(string sql)
        {
            using (SqlConnection conn = GetConnection())
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        
        public int ExecuteNonQuery(string sql, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteNonQuery();
            }
        }

        // 3. Hàm lấy 1 giá trị đơn lẻ (dùng để kiểm tra trùng mã)
        public object ExecuteScalar(string sql, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteScalar();
            }
        }
    }
}