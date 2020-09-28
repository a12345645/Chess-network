using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        Socket socket;
        string sendbuffer;
        System.Windows.Forms.Timer timer  = new System.Windows.Forms.Timer();
        ListBox chatlistbox = new ListBox();
        TextBox chattextbox = new TextBox();
        Button chatbutton = new Button();
        Chatroom chatroom;
        //------------------------------------------- gameroom
        Button[] JoinRoom = new Button[6];
        Button NewRoom = new Button();
        Button CloseRoom = new Button();
        Button NextPage = new Button();
        Button RefreshPage = new Button();
        System.Windows.Forms.Timer roomtimer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer buttonTimer = new System.Windows.Forms.Timer();
        int selectedpage = 0;
        Thread getinfo;
        //-------------------------------------------------
        String UserID;

        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        int screenHeight = Screen.PrimaryScreen.Bounds.Height;

        public Form1()
        {
            Button button = new Button();
            Form.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Size = new Size(screenWidth * 2 / 3, screenHeight * 2 / 3);
            this.BackgroundImage = Properties.Resources.log_in;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button6.Enabled = false;
            button1.Location = new Point(screenWidth * 2 / 3 - 30 * Global.unit,
                screenHeight * 2 / 3 - 10 * Global.unit);
            label1.Top = -10 * Global.unit;
            Start_Connection();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)    //嚕
        {
            int WM_SYSCOMMAND = 0x0112;

            int SC_MOVE = 0xF010;
            int HTCAPTION = 0x0002;

            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void button1_Click(object sender, EventArgs e)   //開遊戲
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void button2_Click(object sender, EventArgs e) //登入
        {
            Byte[] buffer = new Byte[64];
            NetworkStream stream = new NetworkStream(socket);
            StreamReader sr = new StreamReader(stream);
            StreamWriter sw = new StreamWriter(stream);
            string s;


            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("帳號或密碼不可空白");
                return;
            }

            sendbuffer = "Login*";
            Thread thread = new Thread(new ThreadStart(Send_Data_to_Server));         //寄送login請求
            thread.Start();
            thread.Join();


            socket.Receive(buffer);
            s = Encoding.Default.GetString(buffer);
            int position = s.IndexOf("*");
            s = s.Substring(0, position);
            Console.WriteLine(s);

            string a = textBox1.Text;
            string b = textBox2.Text;
            sendbuffer = a + "$" + b + "*";

            thread = new Thread(new ThreadStart(Send_Data_to_Server));               //寄送帳號密碼 格式 帳號$密碼*
            thread.Start();
            thread.Join();

            socket.Receive(buffer);
            s = Encoding.Default.GetString(buffer);
            position = s.IndexOf("*");
            s = s.Substring(0, position);
            Console.WriteLine(s);

            if (s == "error")
            {
                MessageBox.Show("帳號或密碼錯誤");
                textBox1.Text = "";
                textBox2.Text = "";
                return;
            }
            else if (s == "LLogin")
            {
                UserID = a;
                LLogin();
                Global.UserID = UserID;

            }
            else if(s == "Logined")
            {
                MessageBox.Show("此帳號已在線上");
                textBox1.Text = "";
                textBox2.Text = "";
                return;
            }
        }

        private void LLogin()
        {

            button1.Top = -20 * Global.unit; button2.Top = -20 * Global.unit;
            button6.Top = -20 * Global.unit; button4.Top = -20 * Global.unit;
            textBox1.Top = -20 * Global.unit; textBox2.Top = -20 * Global.unit; 
            this.BackgroundImage = Properties.Resources.loading;
            timer.Interval = 1200;
            timer.Tick += new EventHandler(wallpaperchange);
            timer.Start();
        }


        private void wallpaperchange(object sender, EventArgs e)///螢幕切換
        {
            this.BackgroundImage = Properties.Resources.wallpaper;
            chatlistbox.Size = new Size(30 * Global.unit, 20 * Global.unit);
            chatlistbox.Location = new Point(10 * Global.unit, 45 * Global.unit);
            chatlistbox.BorderStyle = BorderStyle.FixedSingle;
            chattextbox.Width = 20 * Global.unit;
            chattextbox.Location = new Point(10 * Global.unit, 65 * Global.unit);
            chatbutton.Size = new Size(9 * Global.unit, (int)(2.5 * Global.unit));
            chatbutton.Location = new Point(31 * Global.unit, 65 * Global.unit);
            chatbutton.Text = "發送";
            label1.Location = new Point(10 * Global.unit, 5 * Global.unit);
            label1.Text = "帳號:" + UserID;

            this.Controls.Add(chatlistbox);
            this.Controls.Add(chattextbox);
            this.Controls.Add(chatbutton);chatbutton.BringToFront();

            chatroom = new Chatroom(chatlistbox, chattextbox, chatbutton);

            timer.Stop();

            GetRoomData();
            Room_Load(); //房間列表讀取
            roomtimer.Interval = 10000;
            roomtimer.Tick += new EventHandler(RoomData_Update);
            //roomtimer.Start();
        }
        

        private void RoomData_Update(object sender, EventArgs e)
        {
    

            GetRoomData();
            ShowRoom(selectedpage);
        }

        private void GetRoomData()
        {
            Byte[] buffer = new Byte[64];
            sendbuffer = "roomData*";
            Thread thread = new Thread(new ThreadStart(Send_Data_to_Server));
            thread.Start();
            thread.Join();
            int n;
            string temp;

            n = socket.Receive(buffer);
            if (n > 0)
            {
                
                temp = Encoding.Default.GetString(buffer);

                Console.WriteLine(temp);
                int position = temp.IndexOf("*");
                temp = temp.Substring(0, position);
                

                string[] data = temp.Split(',');
                int i = 0;
                foreach (string roomname in data)
                {
                    Global.rooms[i] = roomname;
                    i+=1;
                }
                Global.roomnumbers = i-1;
                Console.WriteLine(Global.roomnumbers.ToString());
            }

            if (selectedpage > Global.roomnumbers / 6)
                selectedpage = Global.roomnumbers / 6;



        }
        private void button3_Click(object sender, EventArgs e)     //disconnect
        {
            Byte[] buffer = new Byte[64];
            try
            {
                if (socket == null)
                {

                    Application.Exit();
                    return;
                }
                sendbuffer = "disconnect*";
                Thread thread = new Thread(new ThreadStart(Send_Data_to_Server));        
                thread.Start();
                thread.Join();

                socket.Close();
                //chatroom.myThread.Abort();

                Application.Exit();
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void button4_Click(object sender, EventArgs e)     // new game
        {
            Global.selfplayer = -1;
            GameNet gamenet = new GameNet();
            Thread thread = new Thread(new ThreadStart(gamenet.ServerMain));
            thread.Start();
            Form2 form2 = new Form2();
            form2.ShowDialog();

        }

        public delegate void ButtonEnabled();
        public ButtonEnabled buttonenabled;

        private void connectserver()
        {
            try
            {
                buttonenabled = new ButtonEnabled(buttoncanclick);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(Global.severip, 12345);
                this.Invoke(buttonenabled);
            }
            catch (SocketException ex)
            {
                //MessageBox.Show(ex.Message);
                MessageBox.Show("無法連線至伺服器");
            }
        }

        private void buttoncanclick()
        {
            button2.Enabled = true;
            button4.Enabled = false;
            button6.Enabled = true;
        }



        private void Send_Data_to_Server()
        {
            if (socket != null)
            {
                try
                {
                    socket.Send(Encoding.Default.GetBytes(sendbuffer));
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void Start_Connection()
        {
            //建立一個執行緒，其建構式為一個委派方法物件，Signature(無傳入參數 無返回參數)
            Thread oThread = new Thread(new ThreadStart(connectserver));

            //執行該方法StartConnection
            oThread.Start();
        }

       
        private void button6_Click(object sender, EventArgs e)   //註冊
        {
            Byte[] buffer = new Byte[64];
            NetworkStream stream = new NetworkStream(socket);
            StreamReader sr = new StreamReader(stream);
            StreamWriter sw = new StreamWriter(stream);




            sendbuffer = "register*";
            Thread thread = new Thread(new ThreadStart(Send_Data_to_Server));         //寄送註冊請求
            thread.Start();
            thread.Join();

            Point i = new Point(screenWidth * 2 / 6 - 17 * Global.unit,
                screenHeight * 2 / 6 - 20 * Global.unit);
            i = PointToScreen(i);
            
            Form3 form3 = new Form3();
            form3.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            form3.Location = new System.Drawing.Point(i.X, i.Y);

            form3.ShowDialog();


            sendbuffer = Global.restemp;
            thread = new Thread(new ThreadStart(Send_Data_to_Server));         //寄送註冊資料
            thread.Start();
            thread.Join();


        }

        private void Room_Load()
        {
           
            

            NewRoom = new Button
            {
                Text = "建立房間",
                Visible = true,
                Size = new Size(10 * Global.unit, (int)(5 * Global.unit)),
                Location = new Point(100 * Global.unit, 50 * Global.unit)
            };
            NewRoom.Click += new EventHandler(NewRoom_Click);

            CloseRoom = new Button
            {
                Text = "關閉房間",
                Visible = false,
                Size = new Size(10 * Global.unit, (int)(5 * Global.unit)),
                Location = new Point(100 * Global.unit, 57 * Global.unit)
            };
            CloseRoom.Click += new EventHandler(CloseRoom_Click);

            NextPage = new Button
            {
                Text = "下一頁",
                Visible = true,
                Size = new Size(10 * Global.unit, (int)(5 * Global.unit)),
                Location = new Point(100 * Global.unit, 65 * Global.unit)
            };

            RefreshPage = new Button
            {
                Text = "重新整理",
                Visible = true,
                Size = new Size(10 * Global.unit, (int)(5 * Global.unit)),
                Location = new Point(100 * Global.unit, 43 * Global.unit)
            };
            RefreshPage.Click += new EventHandler(RefreshPage_Click);
            this.Controls.Add(NewRoom);
            this.Controls.Add(CloseRoom);
          //  this.Controls.Add(NextPage);
            this.Controls.Add(RefreshPage);

            int roomPage_num = (Global.roomnumbers/6);

            ShowRoom(selectedpage);
        }


        private void ShowRoom(int page)
        {
            

            
            for(int i=0;i<6;i++)
            {
                this.Controls.Remove(JoinRoom[i]);
            }

            int butidx = 0;
            for(int i=page*6; i<(page*6)+6 && i < Global.roomnumbers; i++)
            {
                Console.WriteLine("1234page");
                JoinRoom[butidx] = new Button
                { 
                    Visible = true,
                    Size = new Size(20 * Global.unit, (int)(10 * Global.unit)),
                    Location = new Point(70 * Global.unit, (butidx + 1) * 100)
                };
                if (Global.rooms[i] == UserID)
                {
                    JoinRoom[butidx].Text = UserID + "(自己)"+"的房間";
                    JoinRoom[butidx].Enabled = false;
                }
                else
                    JoinRoom[butidx].Text = Global.rooms[i]+"*的房間";
                
                this.Controls.Add(JoinRoom[butidx]);
                JoinRoom[butidx].Click += new EventHandler(JoinRoom_Click);
                butidx +=1;
            }
            
            Console.WriteLine("END");
        }



        private void NewRoom_Click(object sender, EventArgs e)
        {
            Byte[] buffer = new Byte[64];
            NetworkStream stream = new NetworkStream(socket);
            StreamReader sr = new StreamReader(stream);
            StreamWriter sw = new StreamWriter(stream);

            this.NewRoom.Visible = false;
            this.CloseRoom.Visible = true;
            this.RefreshPage.Enabled = false;
           


            
            
            sendbuffer = "Newroom*";
            Thread thread = new Thread(new ThreadStart(Send_Data_to_Server));         //寄送開房
            thread.Start();
            thread.Join();
         
            GetRoomData();
            ShowRoom(selectedpage);
            
            getinfo = new Thread(new ThreadStart(Getstart));
            getinfo.IsBackground = true;
            getinfo.Start();

            
        }

        private void Getstart()
        {

            Byte[] buffer = new Byte[64];

             int n = socket.Receive(buffer);

            if (n > 0)
            {
                string temp, data;
                temp = Encoding.Default.GetString(buffer);
                int position = temp.IndexOf("*");
                temp = temp.Substring(0, position);

                if (temp == "GO")
                {
                    Global.selfplayer = -1;
                    GameNet gamenet = new GameNet();
                    Thread thread = new Thread(new ThreadStart(gamenet.ServerMain));
                    thread.Start();
                    Form2 form2 = new Form2();
                    form2.ShowDialog();
                    this.CloseRoom.Visible = false;
                    this.NewRoom.Visible = true;
                    this.RefreshPage.Enabled = true;

                    sendbuffer = "CloseRoom*";
                    
                    thread = new Thread(new ThreadStart(Send_Data_to_Server));         //寄送開房
                    thread.Start();
                    thread.Join();
                    GetRoomData();
                    ShowRoom(selectedpage);
                    
                    
                }
                else
                {
                    return;
                }
            }



        }

        private void RefreshPage_Click(object sender, EventArgs e)
        {
            
           this.RefreshPage.Enabled = false;
            buttonTimer.Interval = 5000;
            buttonTimer.Tick += new EventHandler(buttonEnble);
            buttonTimer.Start();
            GetRoomData();
            ShowRoom(selectedpage);
        }
        private void buttonEnble(object sender, EventArgs e)
        {
            this.RefreshPage.Enabled = true;
        }

        
        private void CloseRoom_Click(object sender, EventArgs e)
        {
            Byte[] buffer = new Byte[64];
            sendbuffer = "CloseRoom*";
            this.CloseRoom.Visible = false;
            this.NewRoom.Visible = true;
            
            Thread thread = new Thread(new ThreadStart(Send_Data_to_Server));         //寄送開房
            thread.Start();
            thread.Join();
            GetRoomData();
            ShowRoom(selectedpage);
            this.RefreshPage.Enabled = true;
        }
        private void JoinRoom_Click(object sender, EventArgs e)
        {
            Button thisroom = (Button)sender;
            Byte[] buffer = new Byte[64];

            sendbuffer = "JoinRoom*";
            Thread thread = new Thread(new ThreadStart(Send_Data_to_Server));         //寄送加入房間
            thread.Start();
            thread.Join();

            sendbuffer = thisroom.Text;
            thread = new Thread(new ThreadStart(Send_Data_to_Server));         //寄送加入房間請求
            thread.Start();
            thread.Join();


            int n = socket.Receive(buffer);
            string ip = Encoding.Default.GetString(buffer);

            Console.WriteLine(ip);
            int position = ip.IndexOf("*");
            ip = ip.Substring(0, position);

            Global.selfplayer = 1;
            GameNet gamenet = new GameNet();
            gamenet.SetIP(ip);
            thread = new Thread(new ThreadStart(gamenet.ClientMain));
            thread.Start();
            Form2 form2 = new Form2();
            form2.ShowDialog();

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            Start_Connection();
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
    }
}
