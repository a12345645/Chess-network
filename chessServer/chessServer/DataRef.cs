using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace chessServer
{
    class DataRef
    {
        public void DeleteDataSpace()
        {
            string cn = @"Data Source=(LocalDB)\MSSQLLocalDB;" +
                   "AttachDBFilename=|DataDirectory|UserData.mdf;" +
                   "Integrated Security=True";
            SqlConnection db = new SqlConnection(cn);
            db.Open();
            string SQL = "SELECT REPLACE(Account,' ','') FROM AccountData";

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            cmd.CommandText = SQL;
            cmd.ExecuteNonQuery();
        }
        public void NewUser(string ac, string pa)
        {

            try
            {
                SqlConnection db = new SqlConnection();
                db.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;" +
                        "AttachDBFilename=|DataDirectory|UserData.mdf;" +
                        "Integrated Security=True";
                db.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = db;
                cmd.CommandText = "INSERT INTO AccountData(Account,password)" + "VALUES(@ac,@pa)";
                cmd.Parameters.AddWithValue("@ac", ac);
                cmd.Parameters.AddWithValue("@pa", pa);
                cmd.ExecuteNonQuery();
                db.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("此帳號已被使用");
            }
        }
        public bool LoginCheck(string a,string p)
        {
            string cn = @"Data Source=(LocalDB)\MSSQLLocalDB;" +
                   "AttachDBFilename=|DataDirectory|UserData.mdf;" +
                   "Integrated Security=True";
            SqlConnection db = new SqlConnection(cn);
            db.Open();
            string SQL = "SELECT * FROM AccountData WHERE Account=@ac AND password=@pa";

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            cmd.CommandText = SQL;
            while (a.Length < 20)
                a += ' ';

            while (p.Length < 20)
                p += ' ';

            cmd.Parameters.AddWithValue("@ac", a);
            cmd.Parameters.AddWithValue("@pa", p);
            SqlDataReader dataReader = cmd.ExecuteReader();
            try
            {
                dataReader.Read();
                string t1 = dataReader["Account"].ToString();
                string t2 = dataReader["password"].ToString();
                if (t1 == a && t2 == p)
                    return true;

                else
                    return false;
            }
            catch (Exception e)
            {
                
                Console.WriteLine(e.Message);
                return false;
            }
            

         
        }
    }
}
