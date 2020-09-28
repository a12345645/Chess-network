using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            Global.restemp = "";
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("不得空白");
            }
            else if(textBox2.Text != textBox3.Text)
            {
             
                MessageBox.Show("第二次密碼錯誤");
            }
            else
            {
                Global.restemp = textBox1.Text + "$" + textBox2.Text + "*";
                MessageBox.Show("已傳送註冊資料");
                this.Close();
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)48 || e.KeyChar == (Char)49 ||
              e.KeyChar == (Char)50 || e.KeyChar == (Char)51 ||
              e.KeyChar == (Char)52 || e.KeyChar == (Char)53 ||
              e.KeyChar == (Char)54 || e.KeyChar == (Char)55 ||
              e.KeyChar == (Char)56 || e.KeyChar == (Char)57 ||
              e.KeyChar == (Char)13 || e.KeyChar == (Char)8 ||
              (e.KeyChar >= 'a' && e.KeyChar <= 'z') || (e.KeyChar >= 'A' && e.KeyChar <= 'Z'))

            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)48 || e.KeyChar == (Char)49 ||
               e.KeyChar == (Char)50 || e.KeyChar == (Char)51 ||
               e.KeyChar == (Char)52 || e.KeyChar == (Char)53 ||
               e.KeyChar == (Char)54 || e.KeyChar == (Char)55 ||
               e.KeyChar == (Char)56 || e.KeyChar == (Char)57 ||
               e.KeyChar == (Char)13 || e.KeyChar == (Char)8 ||
               (e.KeyChar >= 'a' && e.KeyChar <= 'z') || (e.KeyChar >= 'A' && e.KeyChar <= 'Z'))

            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }



        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)48 || e.KeyChar == (Char)49 ||
               e.KeyChar == (Char)50 || e.KeyChar == (Char)51 ||
               e.KeyChar == (Char)52 || e.KeyChar == (Char)53 ||
               e.KeyChar == (Char)54 || e.KeyChar == (Char)55 ||
               e.KeyChar == (Char)56 || e.KeyChar == (Char)57 ||
               e.KeyChar == (Char)13 || e.KeyChar == (Char)8 ||
               (e.KeyChar >= 'a' && e.KeyChar <= 'z') || (e.KeyChar >= 'A' && e.KeyChar <= 'Z'))

            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }



        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(Global.restemp == "")
                Global.restemp = "break*";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dispose(true);
            GC.Collect();
            this.Close();
        }
    }
}
