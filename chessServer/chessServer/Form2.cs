using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace chessServer
{
    public partial class Form2 : Form
    {
        private DataSet ds = new DataSet();
        private BindingSource bindingSource1 = new BindingSource();
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Refresh();
            string cn = @"Data Source=(LocalDB)\MSSQLLocalDB;" +
                    "AttachDBFilename=|DataDirectory|UserData.mdf;" +
                    "Integrated Security=True";
            SqlConnection db = new SqlConnection(cn);
            //db.ConnectionString = cn;
            db.Open();
            SqlDataAdapter ap = new SqlDataAdapter
                ("SELECT * FROM AccountData", db);
            //DataSet ds = new DataSet();
            ap.Fill(ds, "AccountData");
            bindingSource1.DataSource = ds;
            bindingSource1.DataMember = "AccountData";
            dataGridView1.DataSource = bindingSource1;
        }

        private void button1_Click(object sender, EventArgs e)
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
                cmd.Parameters.AddWithValue("@ac", textBox1.Text);
                cmd.Parameters.AddWithValue("@pa", textBox2.Text);
                cmd.ExecuteNonQuery();
                db.Close();
                Form2_Load(sender, e);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void button2_Click(object sender, EventArgs e)
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
                cmd.CommandText = textBox3.Text;

                cmd.ExecuteNonQuery();
                db.Close();
                Form2_Load(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
