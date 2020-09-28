using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class Global
    {
        public static int[][] board;
        public static Point[] move = new Point[30];
        public static Rule rule = new Rule();
        public static Game game = new Game();
        public static Texture texture = new Texture();
        public static readonly int unit = Screen.PrimaryScreen.Bounds.Height / 100; // 視窗事件移動的最小單位
        public static int selfplayer = 0;//自己是甚麼顏色
        //public static PieceCheckmate piececheckmate = new PieceCheckmate();
        //------------
        public static string stemp;
        public static string rtemp = "none";
        public static bool issend = false;
        public static string restemp;
        public static int roomnumbers;
        public static string[] rooms = new string[30];
        public static bool isend;
        public static string UserID;
        public static string severip = "61.64.111.114";
    }
}
