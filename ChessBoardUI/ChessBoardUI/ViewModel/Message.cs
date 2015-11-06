using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using ChessBoardUI.ViewTreeHelper;
using ChessBoardUI.Constants;

namespace ChessBoardUI.ViewModel
{
    class HumanMoveMessage
    {
        private Point from_point;
        private Point to_point;
        private PieceType type;
        private bool castling;
        private bool an_passent;
        private bool promote;
        private bool turn;

        public bool Turn
        {
            get { return promote; }
            set { promote = value; }
        }

        public bool Promotion
        {
            get { return promote; }
            set { promote = value; }
        }

        public bool Castling
        {
            get { return castling; }
            set { castling = value; }
        }

        public bool AnPassent
        {
            get { return an_passent; }
            set { an_passent = value; }
        }

        public Point FromPoint
        {
            get { return from_point; }
            set { from_point = value; }
        }

        public Point ToPoint
        {
            get { return to_point; }
            set { to_point = value; }
        }

        public PieceType Type
        {
            get { return type; }
            set { type = value; }
        }

        }

    class MachineMoveMessage
        {
        private bool turn;
        private int from_rank;
        private int from_file;
        private int to_rank;
        private int to_file;
        private bool M_K_C;
        private bool M_Q_C;

        public bool MKC
        {
            get { return M_K_C; }
            set { M_K_C = value; }
        }

        public bool MQC
        {
            get { return M_Q_C; }
            set { M_Q_C = value; }
        }

        public bool Turn
            {
                get { return turn; }
                set { turn = value; }
            }

        public int From_Rank
        {
            get { return from_rank; }
            set { from_rank = value; }
        }
        public int To_Rank
        {
            get { return to_rank; }
            set { to_rank= value; }
        }
        public int From_File
        {
            get { return from_file; }
            set { from_file = value; }
        }
        public int To_File
        {
            get { return to_file; }
            set { to_file = value; }
        }
    }
    }




