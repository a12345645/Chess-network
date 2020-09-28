using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class Black : Piece
    {
        public delegate void PieceChangeNet();
        public PieceChangeNet piecechangenet;

        public Black(int x, int y, PieceType piecetype, Point point , int num) : base(x, y, point, piecetype, -num)
        {
            
            if (piecetype == PieceType.King)
                this.BackgroundImage = Properties.Resources.BKing;
            else if (piecetype == PieceType.Queen)
                this.BackgroundImage = Properties.Resources.BQueen;
            else if (piecetype == PieceType.Bishop)
                this.BackgroundImage = Properties.Resources.BBishop;
            else if (piecetype == PieceType.Knight)
                this.BackgroundImage = Properties.Resources.BKnight;
            else if (piecetype == PieceType.Rook)
                this.BackgroundImage = Properties.Resources.BRook;
            else if (piecetype == PieceType.Pawn)
                this.BackgroundImage = Properties.Resources.BPawn;

            piecechangenet = new PieceChangeNet(piecetypechange);
        }

        public void piecetypechange(PieceType p) // 升變
        {
            piecetype = p;
            if (piecetype == PieceType.Queen)
                this.BackgroundImage = Properties.Resources.BQueen;
            else if (piecetype == PieceType.Bishop)
                this.BackgroundImage = Properties.Resources.BBishop;
            else if (piecetype == PieceType.Knight)
                this.BackgroundImage = Properties.Resources.BKnight;
            else if (piecetype == PieceType.Rook)
                this.BackgroundImage = Properties.Resources.BRook;
        }

        public void piecetypechangeset(PieceType p)//升變 網路
        {
            piecetype = p;
        }
        public void piecetypechange()//升變 網路
        {
            if (piecetype == PieceType.Queen)
                this.BackgroundImage = Properties.Resources.BQueen;
            else if (piecetype == PieceType.Bishop)
                this.BackgroundImage = Properties.Resources.BBishop;
            else if (piecetype == PieceType.Knight)
                this.BackgroundImage = Properties.Resources.BKnight;
            else if (piecetype == PieceType.Rook)
                this.BackgroundImage = Properties.Resources.BRook;
            if (Global.game.trun < 0)
                Global.game.trun = 1;
            else
                Global.game.trun = -1;
        }
    }
}
