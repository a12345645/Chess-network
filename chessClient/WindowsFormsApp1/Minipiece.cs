using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace WindowsFormsApp1
{
    class Minipiece : PictureBox // 升變用棋
    {
        public int number;
        public Minipiece(int n)
        {
            this.BackColor = Color.Transparent;
            this.BackgroundImageLayout = ImageLayout.Zoom;
            this.Size = new Size(80, 80);
            this.Top = -100;

            number = n;
            this.MouseClick += new MouseEventHandler(minipiecemoveClick);
        }
        void minipiecemoveClick(object sender, EventArgs e)
        {
            Debug.WriteLine(Global.game.trun);
            if (Global.game.trun<0)
            {
                switch (number)
                {
                    case 0:
                        Global.game.black[-Global.game.boardplace-1].piecetypechange(PieceType.Queen);
                        break;
                    case 1:
                        Global.game.black[-Global.game.boardplace-1].piecetypechange(PieceType.Bishop);
                        break;
                    case 2:
                        Global.game.black[-Global.game.boardplace-1].piecetypechange(PieceType.Knight);
                        break;
                    case 3:
                        Global.game.black[-Global.game.boardplace-1].piecetypechange(PieceType.Rook);
                        break;
                }               
            }
            else
            {
                switch (number)
                {
                    case 0:
                        Global.game.white[Global.game.boardplace-1].piecetypechange(PieceType.Queen);
                        break;
                    case 1:
                        Global.game.white[Global.game.boardplace-1].piecetypechange(PieceType.Bishop);
                        break;
                    case 2:
                        Global.game.white[Global.game.boardplace-1].piecetypechange(PieceType.Knight);
                        break;
                    case 3:
                        Global.game.white[Global.game.boardplace-1].piecetypechange(PieceType.Rook);
                        break;
                }
            }
            if (Global.selfplayer != 0)
            {
                Global.stemp = "piecetypechange pnum= " + (Global.game.boardplace).ToString() + " " + number.ToString();
                Global.issend = true;
                Console.WriteLine(Global.stemp);
            }
            Global.game.minipieceboxmove();
            if (Global.game.trun < 0)
                Global.game.trun = 1;
            else
                Global.game.trun = -1;
            
        }
    }
}
