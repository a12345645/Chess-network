using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class Game : Control
    {
        public Black[] black = new Black[16];
        public White[] white = new White[16];
        public int trun; 
        private readonly int Lattice_size = (int)(Screen.PrimaryScreen.Bounds.Height * 0.063);
        private readonly int Border_distance = (int)(Screen.PrimaryScreen.Bounds.Width * 0.3578125);
        private readonly int Top_distance = (int)(Screen.PrimaryScreen.Bounds.Height * 0.1269);

        public PictureBox picturebox ; 
        private Timer timer = new Timer(); // 一秒後移走pucturebox
        public Label blacklabel, whitelabel;

        public delegate void changeBlabel(string s);
        public changeBlabel changeblabel;

        public delegate void changeWlabel(string s);
        public changeWlabel changewlabel;

        public Game ()
        {
            Global.board = new int[8][];
            for (int i = 0; i < 8; i++)
                Global.board[i] = new int[8];

            Console.WriteLine(Screen.PrimaryScreen.Bounds.Height.ToString());
            Console.WriteLine(Screen.PrimaryScreen.Bounds.Width.ToString());
        }

        public void start ()
        {
            changeblabel = new changeBlabel(changeblacklabel);
            changewlabel = new changeWlabel(changewhitelabel);

            picturebox = new PictureBox();

            blacklabel = new Label();
            whitelabel = new Label();
            if (Global.selfplayer == 0)
            {
                blacklabel.Text = "玩家1";
                whitelabel.Text = "玩家2";
            }

            blacklabel.Font = new Font("Times New Roman", 20, FontStyle.Regular);
            whitelabel.Font = new Font("Times New Roman", 20, FontStyle.Regular);

            blacklabel.AutoSize = false;
            whitelabel.AutoSize = false;

            blacklabel.Width = 330;
            blacklabel.Height = 40;
            whitelabel.Width = 330;
            whitelabel.Height = 40;

            blacklabel.BackColor = Color.Transparent;
            whitelabel.BackColor = Color.Transparent;

            blacklabel.Location = new Point(Global.unit * 45, Global.unit * 50);
            whitelabel.Location = new Point(Global.unit * 45, Global.unit * 20);

            picturebox.BackgroundImageLayout = ImageLayout.Zoom;

            for (int i = 0; i < 4; i++)
            {
                minipiece[i] = new Minipiece(i); 
            }
            picturebox.Top = -200;

            timer.Tick += new EventHandler(pictureboxmove);
            

            trun = -1;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    Global.board[i][j] = 0;
            for (int i = 0; i < 8; i++)
            {
                Global.board[0][i] = i + 1;
                Global.board[7][i] = -i - 1;
            }
            for (int i = 8; i < 16; i++)
            {
                Global.board[1][i - 8] = i + 1;
                Global.board[6][i - 8] = -i - 1;
            }
            Point point = new Point();
            for(int i = 0; i < 8; i++)
            {
                point.X = i;
                point.Y = 7;
                if (i == 0 || i == 7)
                    black[i] = new Black(Border_distance + Lattice_size * i,
                        Top_distance + Lattice_size * 7, PieceType.Rook, point, i + 1);
                else if (i == 1 || i == 6)
                    black[i] = new Black(Border_distance + Lattice_size * i,
                        Top_distance + Lattice_size * 7, PieceType.Knight, point, i + 1);
                else if (i == 2 || i == 5)
                    black[i] = new Black(Border_distance + Lattice_size * i,
                        Top_distance + Lattice_size * 7, PieceType.Bishop, point, i + 1);
                else if (i == 3)
                    black[i] = new Black(Border_distance + Lattice_size * i,
                        Top_distance + Lattice_size * 7, PieceType.Queen, point, i + 1);
                else if (i == 4)
                    black[i] = new Black(Border_distance + Lattice_size * i,
                        Top_distance + Lattice_size * 7, PieceType.King, point, i + 1);
            }
            for (int i = 8; i < 16; i++)
            {
                point.X = i-8;
                point.Y = 6;
                black[i] = new Black(Border_distance + Lattice_size * (i - 8),
                        Top_distance + Lattice_size * 6, PieceType.Pawn, point, i + 1);
            }
            for (int i = 0; i < 8; i++)
            {
                point.X = i;
                point.Y = 0;
                if (i == 0 || i == 7)
                    white[i] = new White(Border_distance + Lattice_size * i,
                        Top_distance, PieceType.Rook, point, i + 1);
                else if (i == 1 || i == 6)
                    white[i] = new White(Border_distance + Lattice_size * i,
                        Top_distance, PieceType.Knight, point, i + 1);
                else if (i == 2 || i == 5)
                    white[i] = new White(Border_distance + Lattice_size * i,
                        Top_distance, PieceType.Bishop, point, i + 1);
                else if (i == 3)
                    white[i] = new White(Border_distance + Lattice_size * i,
                        Top_distance, PieceType.Queen, point, i + 1);
                else if (i == 4)
                    white[i] = new White(Border_distance + Lattice_size * i,
                        Top_distance, PieceType.King, point, i + 1);

            }
            for (int i = 8; i < 16; i++)
            {
                point.X = i-8;
                point.Y = 1;
                white[i] = new White(Border_distance + Lattice_size * (i - 8),
                        Top_distance + Lattice_size, PieceType.Pawn, point, i + 1);
            }
        }

        private void changeblacklabel(string s)
        {
            blacklabel.Text = "黑:" + s;
        }

        private void changewhitelabel(string s)
        {
            whitelabel.Text = "白:" + s;
        }
        public void eat(int x,int y)
        {
            int num = Global.board[y][x];
            if (num == 5)
            {
                picturebox.Location = new Point(42 * Global.unit, 30 * Global.unit);
                picturebox.Size = new Size(60 * Global.unit, 20 * Global.unit);
                picturebox.BackgroundImage = Properties.Resources.black_win;
                picturebox.BackgroundImageLayout = ImageLayout.Zoom;
                picturebox.BringToFront();
                trun = 3;
                Global.isend = true;
            }
            if (num == -5)
            {
                picturebox.Location = new Point(42 * Global.unit, 30 * Global.unit);
                picturebox.Size = new Size(60 * Global.unit, 20 * Global.unit);
                picturebox.BackgroundImage = Properties.Resources.white_win;
                picturebox.BackgroundImageLayout = ImageLayout.Zoom;
                picturebox.BringToFront();
                Global.isend = true;
                trun = 3;
            }
            if (num<0)
            {
                black[-num - 1].Top = -80;
                black[-num - 1].boardplace.X = -1;
            }
            else
            {
                white[num - 1].Top = -80;
                white[num - 1].boardplace.X = -1;
            }
                
        }

        ///////////////////////////////////////////////////////////////
        ///
        public Minipiece[] minipiece = new Minipiece[4];////

        public void minipieceboxmove() //移走minipiece
        {
            for (int i = 0; i < 4; i++)
                minipiece[i].Top = -100;    
        }

        public int boardplace;

        public void promotion(Point boardplace) // 升變
        {
            trun = trun * 2;
            int piece = Global.board[boardplace.Y][boardplace.X];
            this.boardplace = piece;
            for (int i =0;i<4;i++)
            {
                minipiece[i].Top = 380;
                minipiece[i].Left = i * 130 + 50 + Border_distance;
                minipiece[i].BringToFront();
            }
            if (piece<0)
            {
                minipiece[0].BackgroundImage = Properties.Resources.BQueen;
                minipiece[1].BackgroundImage = Properties.Resources.BBishop;
                minipiece[2].BackgroundImage = Properties.Resources.BKnight;
                minipiece[3].BackgroundImage = Properties.Resources.BRook;
            }
            else
            {
                minipiece[0].BackgroundImage = Properties.Resources.WQueen;
                minipiece[1].BackgroundImage = Properties.Resources.WBishop;
                minipiece[2].BackgroundImage = Properties.Resources.WKnight;
                minipiece[3].BackgroundImage = Properties.Resources.WRook;
            }
        }

        

        public void checkmate(PieceType piecetype, Point boardplace)//確認將軍
        {
            int step = Global.rule.move(piecetype, boardplace);
            int x, y;
            for (int i = 0; i < step; i++) 
            {
                x = (Global.move[i].X - Global.rule.Border_distance) / Lattice_size;
                y = (Global.move[i].Y - Global.rule.Top_distance + 34) / Lattice_size;
                if (Global.board[y][x]+1 == -4 || Global.board[y][x]-1 == 4)
                {
                    checkmateTrue();
                }
            }
        }

        

        private void checkmateTrue()//真的將軍
        {

            if (checkmateout())
            {
                picturebox.Location = new Point(42 * Global.unit, 30 * Global.unit);
                picturebox.Size = new Size(60 * Global.unit, 20 * Global.unit);
                picturebox.BackgroundImage = Properties.Resources.checkmateout;
                picturebox.BackgroundImageLayout = ImageLayout.Zoom;
                picturebox.BringToFront();
                trun *= 2;
            }
            else
            {

                /*if (trun == 1)
                {
                    Global.piececheckmate.n = true;
                    Global.piececheckmate.who = -1;
                }
                else
                {
                    Global.piececheckmate.n = true;
                    Global.piececheckmate.who = 1;
                }*/

                picturebox.Location = new Point(42 * Global.unit, 30 * Global.unit);
                picturebox.Size = new Size(60 * Global.unit, 20 * Global.unit);
                picturebox.BackgroundImage = Properties.Resources.checkmate;
                picturebox.BackgroundImageLayout = ImageLayout.Zoom;
                picturebox.BringToFront();


                timer.Interval = 1000;
                timer.Start();
                trun *= 2;

                //System.Threading.Thread.Sleep(1000);

            }
        }
        

        private void pictureboxmove(object sender , EventArgs e)
        {
            picturebox.Top = -200;

            if (trun < 0)
                trun = -1;
            else
                trun = 1;

            timer.Stop();
        }

        private bool checkmateout() //將死
        {
            if (trun < 0)
            {
                int step = Global.rule.move(PieceType.King, white[4].boardplace);

                if (step == 0)
                    return true;
                Point[] kingmove = new Point[step];
                for(int i=0;i<step;i++)
                    kingmove[i] = Global.move[i];
                int n = step;
                for (int i = 0; i < 8; i++)
                {
                    if (i == 4 || black[i].boardplace.X == -1)
                        continue;
                    step = Global.rule.move(black[i].piecetype, black[i].boardplace);
                    for (int j = 0; j < n; j++)
                        for (int k = 0; k < step; k++)
                            if (kingmove[j] == Global.move[k])
                                kingmove[j].X = -1;
                }
                for (int i = 8; i < 16; i++)
                {
                    if (Math.Abs(black[i].boardplace.X - white[4].boardplace.X) > 1 || black[i].boardplace.X == -1)
                        continue;
                    step = Global.rule.move(black[i].piecetype, black[i].boardplace);
                    for (int j = 0; j < n; j++)
                        for (int k = 0; k < step; k++)
                            if (kingmove[j] == Global.move[k])
                                kingmove[j].X = -1;
                }
                for (int j = 0; j < n; j++)
                {
                    if (kingmove[j].X != -1)
                        return false;
                }
                return true;
            }
            else
            {
                int step = Global.rule.move(PieceType.King, black[4].boardplace);
                if (step == 0)
                    return true;
                Point[] kingmove = new Point[step];
                for (int i = 0; i < step; i++)
                    kingmove[i] = Global.move[i];
                int n = step;
                for (int i = 0; i < 8; i++)
                {
                    if (i == 4 || black[i].boardplace.X == -1)
                        continue;
                    step = Global.rule.move(white[i].piecetype, white[i].boardplace);
                    for (int j = 0; j < n; j++)
                        for (int k = 0; k < step; k++)
                            if (kingmove[j] == Global.move[k])
                                kingmove[j].X = -1;
                }
                for (int i = 8; i < 16; i++)
                {
                    if (Math.Abs(white[i].boardplace.X - black[4].boardplace.X) > 1 || black[i].boardplace.X == -1)
                        continue;
                    step = Global.rule.move(white[i].piecetype, white[i].boardplace);
                    for (int j = 0; j < n; j++)
                        for (int k = 0; k < step; k++)
                            if (kingmove[j] == Global.move[k])
                                kingmove[j].X = -1;
                }
                for (int j = 0; j < n; j++)
                {
                    if (kingmove[j].X != -1)
                        return false;
                }
                return true;
            }
        }
    }
}

/*public void incheckmate ()////判斷是否解除將軍
        {
            int step;
            Point kingplace;
            bool n = false;

            if (Global.piececheckmate.who == 1)
            {
                kingplace = white[4].boardplace;
                for (int i = 0; i < 8; i++)
                {
                    if (i == 4 || black[i].boardplace.X == -1)
                        continue;
                    step = Global.rule.move(black[i].piecetype, black[i].boardplace);
                    for (int j = 0; j < step; j++)
                        if (kingplace == Global.move[j])
                        {
                            n = true;
                            break;
                        }
                }
                //Debug.WriteLine(kingplace.X.ToString(), kingplace.Y.ToString());
                if (!n)
                    for (int i = 8; i < 16; i++)
                    {
                        if (Math.Abs(black[i].boardplace.X - kingplace.X) > 1 || black[i].boardplace.X == -1)
                            continue;
                        step = Global.rule.move(black[i].piecetype, black[i].boardplace);
                        
                        for (int j = 0; j < step; j++)
                        {
                            Global.move[j].X = (Global.move[j].X - Border_distance + 33) / Lattice_size;
                            Global.move[j].Y = (Global.move[j].Y - Top_distance + 33) / Lattice_size;
                            //Debug.WriteLine(Global.move[j].X.ToString(), Global.move[j].Y.ToString());
                            if (kingplace == Global.move[j])
                            {
                                n = true;
                                break;
                            }
                        }
                            
                    }
                if(!n)
                {
                    Global.piececheckmate.n = false;
                    Global.piececheckmate.who = 0;
                }
            }
            else
            {
                kingplace = black[4].boardplace;
                for (int i = 0; i < 8; i++)
                {
                    if (i == 4 || black[i].boardplace.X == -1)
                        continue;
                    step = Global.rule.move(white[i].piecetype, white[i].boardplace);
                    for (int j = 0; j < step; j++)
                        if (kingplace == Global.move[j])
                        {
                            n = true;
                            break;
                        }
                }
                if (!n)
                    for (int i = 8; i < 16; i++)
                    {
                        if (Math.Abs(white[i].boardplace.X - kingplace.X) > 1 || black[i].boardplace.X == -1)
                            continue;
                        step = Global.rule.move(white[i].piecetype, white[i].boardplace);

                        for (int j = 0; j < step; j++)
                        {
                            Global.move[j].X = (Global.move[j].X - Border_distance + 33) / Lattice_size;
                            Global.move[j].Y = (Global.move[j].Y - Top_distance + 33) / Lattice_size;
                            if (kingplace == Global.move[j])
                            {
                                n = true;
                                break;
                            }
                        }
                            
                    }
                if (!n)
                {
                    Global.piececheckmate.n = false;
                    Global.piececheckmate.who = 0;
                }
            }
        }*/
