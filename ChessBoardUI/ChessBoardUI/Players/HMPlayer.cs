
using ChessBoardUI.AIAlgorithm;
using ChessBoardUI.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ChessBoardUI.Players
{
    class HMPlayer
    {
        private TimerViewModel human_timer;
        private SPCapturedViewModel human_capture;
        private Dictionary<int, ChessPiece> pieces_dict;
        private ObservableCollection<ChessPiece> pieces_collection;
        //private Image cap_piece_image;

        //servableCollection<ChessPiece> all_pieces;
        //ctionary<int, ChessPiece> board_layout;

        public HMPlayer(ObservableCollection<ChessPiece> pieces_collection, Dictionary<int, ChessPiece> pieces_dict)
        {
            human_timer = new TimerViewModel
            {
                Participant = Participant.Player,
                TimeSpan = TimeSpan.FromMinutes(30),
                TimerDispatcher = new DispatcherTimer(),
                Display = "00:30:00"
            };

            Messenger.Default.Register<HumanMoveMessage>(this, (action) => HumanPiecePositionChangeHandler(action));
            Messenger.Default.Register<MachineMoveMessage>(this, (action) => MachinePiecePositionChangeHandler(action));
            human_capture = new SPCapturedViewModel { CapturedPiecesCollection = new ObservableCollection<BitmapImage>() };
            this.pieces_collection = pieces_collection;
            this.pieces_dict = pieces_dict;

        }

        public Dictionary<int, ChessPiece> PieceDictionary
        {
            get { return pieces_dict; }
            set { pieces_dict = value; }
        }


        public ObservableCollection<ChessPiece> PieceCollection
        {
            get { return pieces_collection; }
            set { pieces_collection = value; }
        }

        public SPCapturedViewModel HumanCaptureStack  //stack of pieces captured by human player(collection of images)
        {
            get { return human_capture; }
            set { human_capture = value; }
        }

        public TimerViewModel HumanTimer
        {
            get { return human_timer; }
            set { human_timer = value; }
        }


        public void HumanPiecePositionChangeHandler(HumanMoveMessage action)
        {
            this.HumanTimer.stopClock();



            //lock the entire board so that use cannot click on piece when it is machine's turn.
            foreach (KeyValuePair<int, ChessPiece> item in this.pieces_dict)
            {
                if (item.Value.Player == Player.White && MoveGenerator.player_color)
                {
                    item.Value.Ownership = false;
                }
                else if (item.Value.Player == Player.Black && !MoveGenerator.player_color)
                {
                    item.Value.Ownership = false;
                }
            }


            // some bit operations to get the bit
         

        }

        public void MachinePiecePositionChangeHandler(MachineMoveMessage action)
        {
            int from_loca_index = action.From_File * 10 + (7 - action.From_Rank);
            int to_loca_index = action.To_File * 10 + (7 - action.To_Rank);

            ChessPiece moved = this.pieces_dict[from_loca_index];


            //this is all for castling move updating on frontend
            if (action.MKC)
            {
                if(action.From_File==4) //means machine use black
                {
                    ChessPiece rook_king_side = this.pieces_dict[70];
                    moved.Pos_X = 6;
                    rook_king_side.Pos_X = 5;
                    this.pieces_dict.Remove(from_loca_index);
                    this.pieces_dict.Remove(70);
                    this.pieces_dict.Add(60, moved);
                    this.pieces_dict.Add(50, rook_king_side);
                }
                else  //means machine use white
                {
                    ChessPiece rook_king_side = this.pieces_dict[0];
                    moved.Pos_X = 1;
                    rook_king_side.Pos_X = 2;
                    this.pieces_dict.Remove(from_loca_index);
                    this.pieces_dict.Remove(0);
                    this.pieces_dict.Add(10, moved);
                    this.pieces_dict.Add(20, rook_king_side);
                }
                return ;
            }
            if (action.MQC)
            {
                if (action.From_File == 4) //means machine use black
                {
                    ChessPiece rook_king_side = this.pieces_dict[0];
                    moved.Pos_X = 2;
                    rook_king_side.Pos_X = 3;
                    this.pieces_dict.Remove(from_loca_index);
                    this.pieces_dict.Remove(0);
                    this.pieces_dict.Add(20, moved);
                    this.pieces_dict.Add(30, rook_king_side);
                }
                else  //means machine use white
                {
                    ChessPiece rook_king_side = this.pieces_dict[70];
                    moved.Pos_X = 5;
                    rook_king_side.Pos_X = 4;
                    this.pieces_dict.Remove(from_loca_index);
                    this.pieces_dict.Remove(70);
                    this.pieces_dict.Add(50, moved);
                    this.pieces_dict.Add(40, rook_king_side);
                }
                return;
            }


            if (this.pieces_dict.ContainsKey(to_loca_index))
            {
                ChessPiece to_piece_location = this.pieces_dict[to_loca_index];
                Application.Current.Dispatcher.Invoke((Action)(() => this.pieces_collection.Remove(to_piece_location)));
                this.pieces_dict.Remove(to_loca_index);

                Application.Current.Dispatcher.Invoke((Action)(() => {
                    String cap_piece_img = "/PieceImg/chess_piece_" + to_piece_location.Player.ToString() + "_" + to_piece_location.Type.ToString()+".png";
                    //Console.WriteLine(cap_piece_img);
                    Uri uri_cap_piece_img = new Uri(cap_piece_img, UriKind.Relative);
                    BitmapImage hm_cap_img = new BitmapImage();
                    // BitmapImage resized_img = new BitmapImage();
                    hm_cap_img.BeginInit();
                    hm_cap_img.UriSource = uri_cap_piece_img;
                    hm_cap_img.DecodePixelHeight = 70;
                    hm_cap_img.DecodePixelWidth = 70;
                    hm_cap_img.EndInit();
                    human_capture.CapturedPiecesCollection.Add(hm_cap_img);
                }));

            }
            moved.Pos_X = action.To_File;
            moved.Pos_Y = 7 - action.To_Rank;

            this.pieces_dict.Remove(from_loca_index);
            this.pieces_dict.Add(to_loca_index, moved);

            //unlock human player pieces so he can go on
            foreach (KeyValuePair<int, ChessPiece> item in this.pieces_dict)
            {
                if (item.Value.Player == Player.White && MoveGenerator.player_color)
                {
                    item.Value.Ownership = action.Turn;
                }
                else if (item.Value.Player == Player.Black && !MoveGenerator.player_color)
                {
                    item.Value.Ownership = action.Turn;
                }
            }


            this.HumanTimer.startClock();

        }



    }
}
