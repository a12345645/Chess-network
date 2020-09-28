using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
//using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        int screenHeight = Screen.PrimaryScreen.Bounds.Height;
        PictureBox pictureBox1 = new PictureBox();

        public Form2()
        {
            Global.isend = false;
            InitializeComponent();
            this.DoubleBuffered = true;

            Global.texture = new Texture();
            Global.game.start();


            for (int i = 0; i < 16; i++)
            {
                this.Controls.Add(Global.game.black[i]);
                
                this.Controls.Add(Global.game.white[i]);
            }
            for (int i = 0; i < 4; i++)
            {
                this.Controls.Add(Global.game.minipiece[i]);
            }
            this.Controls.Add(Global.game.picturebox);
            this.Controls.Add(Global.texture.texturepiture);
            this.Controls.Add(Global.texture.texturepiture2);

            for (int i = 0; i < 9; i++)
                this.Controls.Add(Global.texture.texturename[i]);
            this.Controls.Add(Global.texture.button);

            this.Controls.Add(Global.game.blacklabel);
            this.Controls.Add(Global.game.whitelabel);

            this.Size = new Size(screenWidth * 2 / 3, screenHeight * 2 / 3);
            this.BackgroundImage = Properties.Resources.broad;

            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Location = new Point(screenWidth * 2 / 3 - screenWidth / 20, screenHeight / 30);
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.BackgroundImage = Properties.Resources.X;
            this.Controls.Add(pictureBox1);

            pictureBox1.Click += new EventHandler(pictureBox1_Click);

            int a = screenWidth / 20;

            
        }

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            int WM_SYSCOMMAND = 0x0112;

            int SC_MOVE = 0xF010;
            int HTCAPTION = 0x0002;

            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);

        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Global.texture.timer1.Stop();
            Global.texture = null;
            for (int i = 0; i < 16; i++)
            {
                Global.game.black[i] = null;
                Global.game.white[i] = null;
            }
            for (int i = 0; i < 4; i++)
                Global.game.minipiece[i] = null;
            Global.game.picturebox = null;
            Global.selfplayer = 0;
            Global.isend = true;
            Dispose(true);
            GC.Collect();
            this.Close();
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            pictureBox1.Size = new Size(Global.unit * 5, Global.unit * 5);
            
            if (Global.selfplayer != 0)
            {
                Global.stemp = "color= " + Global.selfplayer.ToString() + " " + Global.UserID;
                Global.issend = true;

                if (Global.selfplayer == 1)
                    Global.game.whitelabel.Text = "白:" + Global.UserID;
                else if (Global.selfplayer == -1)
                    Global.game.blacklabel.Text = "黑:" + Global.UserID;
            }
            /**/
        }
    }
}
