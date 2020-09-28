using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Data.SqlClient;

namespace chessServer
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        Thread t1,t2;
        
        public Form1()
        {

            this.DoubleBuffered = true;
            // Closing += new CancelEventHandler(Form1_Closing);
            InitializeComponent();
            
           

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            t1 = new Thread(new ThreadStart(re));
            t1.IsBackground = true;
            t1.Start();

            t2 = new Thread(new ThreadStart(Listener.StartListening));
            t2.IsBackground = true;
            t2.Start();
            GlobalVar.usernumber = 0;
            GlobalVar.dataRef.DeleteDataSpace();
            timer1.Start();
            timer1.Interval = 1000;

        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)    //嚕
        {
            int WM_SYSCOMMAND = 0x0112;

            int SC_MOVE = 0xF010;
            int HTCAPTION = 0x0002;

            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        public void re()
        {
            while(true)
            {
                Thread.Sleep(500);
                string s = "";
                for(int i=0;i<GlobalVar.arr.Count;i++)
                {
                    Socket so = (Socket)GlobalVar.arr[i];
                    IPEndPoint point = so.RemoteEndPoint as IPEndPoint;
                    string ip = point.Address.ToString();
                    string us = "";
                    if(true == (GlobalVar.loginUser.ContainsKey(so)))
                        us = GlobalVar.loginUser[so];
                 //   string port = point.Port.ToString(); 

                    s += ip + " "+ "Login:" + us;
                    s += '\n' ;
                }
                //s += "<< ALL SOCKET";
                GlobalVar.temp = s;
                //this.Refresh();
            }
            
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = GlobalVar.temp;
            label3.Text = "Now Room:" + GlobalVar.roomData; 
            this.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(this, "確定退出？", "離開通知" + "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
                Application.Exit();

        }

      /*  public void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult dr = MessageBox.Show(this, "確定退出？", "退出視窗通知", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr != DialogResult.Yes)
            {
                
                e.Cancel = true;
            }
            else
            {

                
                
                t1.Abort();
                t1.Join(); 
                //  Thread.Sleep(100);
                GlobalVar.t2.Abort();
                GlobalVar.t2.Join();
                // Thread.Sleep(100);
                if (t2.IsAlive == true)
                    Console.WriteLine("123");
                t2.Abort();
                t2.Join(); Console.WriteLine("789");

           
                Application.Exit();
             
                
            }
        }*/
    }
}
