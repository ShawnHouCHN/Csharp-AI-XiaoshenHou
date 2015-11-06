using ChessBoardUI.AIAlgorithm;
using ChessBoardUI.ViewModel;
using ChessBoardUI.ViewTreeHelper;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
//using ChessBoardUI.AIAlgorithm;

namespace ChessBoardUI.Players
{
   
    class AIPlayer
    {
        private TimerViewModel machine_timer;
        private SPCapturedViewModel machine_stack;
        private Dictionary<int, ChessPiece> pieces_dict;
        private ObservableCollection<ChessPiece> pieces_collection;
        private bool turn = false;
        Thread algo_thread;
        MoveGenerator move_generator;
        private Image cap_piece_image;
        private static int time_interval;

       
        //Player ai_color;

        public AIPlayer(ObservableCollection<ChessPiece> pieces_collection, Dictionary<int, ChessPiece> pieces_dict)
        {
            move_generator = new MoveGenerator();

            //ai color to use;
            //this.ai_color = ai_color;

            //machine clock instantiation
            machine_timer = new TimerViewModel
            {
                Participant = Participant.PC,
                TimeSpan = TimeSpan.FromMinutes(30),
                TimerDispatcher = new DispatcherTimer(),
                Display = "00:30:00"
            };

            //human move messenger registration
            Messenger.Default.Register<HumanMoveMessage>(this, (action) => HumanPiecePositionChangeHandler(action));

            //captured stack instantiation 
            machine_stack = new SPCapturedViewModel { CapturedPiecesCollection = new ObservableCollection<BitmapImage>() };

            //collection and dictionary referencing
            this.pieces_collection = pieces_collection;
            this.pieces_dict = pieces_dict;


            //algorithm thread instantiation
            algo_thread = new Thread(new ThreadStart(AlgorithmThread));
            algo_thread.IsBackground = true; //terminate thread when window is closed 
            algo_thread.Start();
        }

        public int Interval
        {
            get { return time_interval; }
            set { time_interval = value; }
        }

        public SPCapturedViewModel MachineCaptureStack  //stack of pieces captured by human player(collection of images)
        {
            get { return machine_stack; }
            set { machine_stack = value; }
        }

        public TimerViewModel MachineTimer
        {
            get { return machine_timer; }
            set { machine_timer = value; }
        }

        public Dictionary<int, ChessPiece> PieceDictionary
        {
            get { return pieces_dict; }
            set { pieces_dict = value; }
        }

        public Image Cap_Image
        {
            get { return cap_piece_image; }
            set { cap_piece_image = value; }
        }

        public ObservableCollection<ChessPiece> PieceCollection
        {
            get { return pieces_collection; }
            set { pieces_collection = value; }
        }

        public void HumanPiecePositionChangeHandler(HumanMoveMessage action)
        {
            Console.WriteLine("Player moved From {0}  to {1}", action.FromPoint, action.ToPoint);
            ChessPiece moved=this.pieces_dict[(int)action.FromPoint.X * 10 + (int)action.FromPoint.Y];
            
            int to_location = (int)action.ToPoint.X * 10 + (int)action.ToPoint.Y;

            //player castling move
            if (action.Castling)
            {
                int from_dic_index = (int)action.FromPoint.X * 10 + (int)action.FromPoint.Y;
                int to_dic_index = (int)action.ToPoint.X * 10 + (int)action.ToPoint.Y;
                //Console.WriteLine("from value is " + from_dic_index);
                ChessPiece king = this.pieces_dict[from_dic_index];
                ChessPiece rook = this.pieces_dict[to_dic_index];
               
                if (rook.Pri_Coor_X > king.Pri_Coor_X)
                {
                    king.Pos_X = king.Pri_Coor_X + 2;
                    king.Pos_Y = 7;
                    rook.Pos_X = king.Pri_Coor_X + 1;
                    rook.Pos_Y = 7;
                    MoveGenerator.updateCastlingMovedBitboard(king.Pri_Coor_X + 1, true); //true =right side rook
                }
                else
                {
                    king.Pos_X = king.Pri_Coor_X - 2;
                    king.Pos_Y = 7;
                    rook.Pos_X = king.Pri_Coor_X - 1;
                    rook.Pos_Y = 7;
                    MoveGenerator.updateCastlingMovedBitboard(king.Pri_Coor_X - 1, false); //false=left side rook
                }
                Console.WriteLine("Player made an castling");
                this.pieces_dict.Remove(to_dic_index);
                this.pieces_dict.Remove(from_dic_index);
                
                this.pieces_dict.Add(king.Coor_X * 10 + king.Coor_Y, king);
               
                this.pieces_dict.Add(rook.Coor_X * 10 + rook.Coor_Y, rook);
                MoveGenerator.PKC = false;
                MoveGenerator.PQC = false;

            }



            // en passent move
            else if (action.AnPassent)
            {
                Console.WriteLine("Player made an passent capture");
                int en_passent_cap_index = (int)action.ToPoint.X * 10 + (int)action.ToPoint.Y + 1;
                ChessPiece to_piece_location = this.pieces_dict[en_passent_cap_index];
                int cap_index = (7 - (int)action.ToPoint.Y - 1) * 8 + (int)action.ToPoint.X;
                ulong passent_cap_place = 0x0000000000000001;
                passent_cap_place = (passent_cap_place << (cap_index));


                this.pieces_collection.Remove(to_piece_location);
                this.pieces_dict.Remove(to_location);

                int from_index = (7 - (int)action.FromPoint.Y) * 8 + (int)action.FromPoint.X;
                int to_index = (7 - (int)action.ToPoint.Y) * 8 + (int)action.ToPoint.X;
                ulong moved_place = 0x0000000000000001;
                ulong new_place = 0x0000000000000001;
                moved_place = ~(moved_place << (from_index)); //can be optimised
                new_place = (new_place << (to_index));

                MoveGenerator.UpdateAnyMovedBitboard(PieceType.Pawn, moved_place, new_place);
                MoveGenerator.UpdateAnyCapturedBitboard(PieceType.Pawn, passent_cap_place);


                Application.Current.Dispatcher.Invoke((Action)(() => {
                    String cap_piece_img = "/PieceImg/chess_piece_" + to_piece_location.Player.ToString() + "_" + to_piece_location.Type.ToString() + ".png";
                    Uri uri_cap_piece_img = new Uri(cap_piece_img, UriKind.Relative);
                    BitmapImage hm_cap_img = new BitmapImage();
                    hm_cap_img.BeginInit();
                    hm_cap_img.UriSource = uri_cap_piece_img;
                    hm_cap_img.DecodePixelHeight = 70;
                    hm_cap_img.DecodePixelWidth = 70;
                    hm_cap_img.EndInit();
                    machine_stack.CapturedPiecesCollection.Add(hm_cap_img);
                }));
                this.pieces_dict.Add((moved.Coor_X * 10 + moved.Coor_Y), moved);
                this.pieces_dict.Remove((int)action.FromPoint.X * 10 + (int)action.FromPoint.Y);


            }

            //normal move
            else 
            {
                int from_index = (7 - (int)action.FromPoint.Y) * 8 + (int)action.FromPoint.X;
                int to_index = (7 - (int)action.ToPoint.Y) * 8 + (int)action.ToPoint.X;


                ulong moved_place = 0x0000000000000001;
                ulong new_place = 0x0000000000000001;
                moved_place = MoveGenerator.full_occupied & ~(moved_place << (from_index)); //can be optimised
                new_place = (new_place << (to_index));
                MoveGenerator.UpdateAnyMovedBitboard(action.Type, moved_place, new_place);
                MoveGenerator.setCurrentBitboardsHistoryMove(new Move((7 - (int)action.FromPoint.Y), (int)action.FromPoint.X, (7 - (int)action.ToPoint.Y), (int)action.ToPoint.X, action.Type, false));

                if (this.pieces_dict.ContainsKey(to_location))
                {
                    // if two piece is same color,then it is a castling. 
                    Console.WriteLine("Player made a capture move");
                    ChessPiece to_piece_location = this.pieces_dict[to_location];

                    ulong removed_place = 0x0000000000000001;
                    removed_place = (removed_place << (to_index));
                    this.pieces_collection.Remove(to_piece_location);
                    this.pieces_dict.Remove(to_location);
                    MoveGenerator.UpdateAnyCapturedBitboard(to_piece_location.Type, removed_place);


                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        String cap_piece_img = "/PieceImg/chess_piece_" + to_piece_location.Player.ToString() + "_" + to_piece_location.Type.ToString() + ".png";
                        Uri uri_cap_piece_img = new Uri(cap_piece_img, UriKind.Relative);
                        BitmapImage hm_cap_img = new BitmapImage();
                        hm_cap_img.BeginInit();
                        hm_cap_img.UriSource = uri_cap_piece_img;
                        hm_cap_img.DecodePixelHeight = 70;
                        hm_cap_img.DecodePixelWidth = 70;
                        hm_cap_img.EndInit();
                        machine_stack.CapturedPiecesCollection.Add(hm_cap_img);
                    }));
                   
                }

                else
                {
                    Console.WriteLine("Player made a normal move");
               

                }
                this.pieces_dict.Add(to_location, moved);
                this.pieces_dict.Remove((int)action.FromPoint.X * 10 + (int)action.FromPoint.Y);
                //Console.WriteLine("Replace "+((int)action.FromPoint.X * 10 + (int)action.FromPoint.Y) + " with "+ to_location);
            }

            
            


            //code belwo is for castling move
            if (action.Type == PieceType.King)
            {
                MoveGenerator.PKC = false;
                MoveGenerator.PQC = false;
            }
            if (action.Type == PieceType.Rook && action.FromPoint.X == 0 && MoveGenerator.player_color)
                MoveGenerator.PQC = false;
            if (action.Type == PieceType.Rook && action.FromPoint.X == 7 && MoveGenerator.player_color)
                MoveGenerator.PKC = false;
            if (action.Type == PieceType.Rook && action.FromPoint.X == 0 && !MoveGenerator.player_color)
                MoveGenerator.PKC = false;
            if (action.Type == PieceType.Rook && action.FromPoint.X == 7 && !MoveGenerator.player_color)
                MoveGenerator.PQC = false;


            Console.WriteLine("Moved piece is a {0} {1} placed at {2}", this.pieces_dict[(moved.Coor_X * 10 + moved.Coor_Y)].Player, this.pieces_dict[(moved.Coor_X * 10 + moved.Coor_Y)].Type, this.pieces_dict[(moved.Coor_X * 10 + moved.Coor_Y)].Pos);

            Console.WriteLine(" board state is " + Convert.ToString((long)MoveGenerator.pieces_occupied, 2));  //!!!!!!!
            this.turn = action.Turn;
           
        }


        public void AlgorithmThread()
        {
            try
            {
                while (true)
                {
                    if (turn)
                    {
                        this.MachineTimer.startClock();

                        // this is the current chess board state
                        ChessBoard curr_board_state = new ChessBoard(MoveGenerator.white_pawns, MoveGenerator.white_knights, MoveGenerator.white_bishops, MoveGenerator.white_queens, MoveGenerator.white_rooks, MoveGenerator.white_king, MoveGenerator.black_pawns, MoveGenerator.black_knights, MoveGenerator.black_bishops, MoveGenerator.black_queens, MoveGenerator.black_rooks, MoveGenerator.black_king, MoveGenerator.history_move, MoveGenerator.MKC, MoveGenerator.MQC, MoveGenerator.PKC, MoveGenerator.PQC);

  

                        //code below is for updating frontend
                        Move ai_move = getNextMove(curr_board_state);

                        if (ai_move.MKC || ai_move.MQC)
                        {
                            Console.WriteLine("AI moves Castling");
                            Messenger.Default.Send(new MachineMoveMessage { Turn = this.turn, From_Rank = ai_move.from_rank, From_File = ai_move.from_file, MKC = ai_move.MKC, MQC = ai_move.MQC });
                            this.MachineTimer.stopClock();
                            this.turn = false;
                        }

                        else
                        {

                            //Console.WriteLine("AI Move " + ai_move.moved_type + " from " + ai_move.from_rank + " " + ai_move.from_file + " to " + ai_move.to_rank + " " + ai_move.to_file + " Cap " + ai_move.cap_type);
                            Messenger.Default.Send(new MachineMoveMessage { Turn = this.turn, From_Rank = ai_move.from_rank, From_File = ai_move.from_file, To_Rank = ai_move.to_rank, To_File = ai_move.to_file });
                            this.MachineTimer.stopClock();
                            this.turn = false;
                        }
                        Console.WriteLine(" board state is " + Convert.ToString((long)MoveGenerator.pieces_occupied, 2));  //!!!!!!!
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Something wrong with the algorithm. check the code!!!");
            }
            
        }

        public Move getNextMove(ChessBoard curr_board_state)
        {
            startIterativeSearch(curr_board_state);
            MoveGenerator.setCurrentBitboards(curr_board_state.bestState.BP, curr_board_state.bestState.BR, curr_board_state.bestState.BN, curr_board_state.bestState.BB, curr_board_state.bestState.BQ, curr_board_state.bestState.BK, curr_board_state.bestState.WP, curr_board_state.bestState.WR, curr_board_state.bestState.WN, curr_board_state.bestState.WB, curr_board_state.bestState.WQ, curr_board_state.bestState.WK);
            MoveGenerator.setCurrentBitboardsHistoryMove(curr_board_state.bestState.move);            
            MoveGenerator.setCurrentCastlingCondition(curr_board_state.bestState.MKC, curr_board_state.bestState.MQC, curr_board_state.bestState.PKC, curr_board_state.bestState.PQC);
            return curr_board_state.bestState.move;

        }

        private void startIterativeSearch(ChessBoard init)
        {
            DateTime start_time = DateTime.Now;
            MoveGenerator.end_time = start_time.AddSeconds(this.Interval); //set iterative deepning end time;
            int i = 1;
            while (DateTime.Compare(DateTime.Now , MoveGenerator.end_time)<=0)
            {
                Console.WriteLine("Depth is "+i);
                init.AlphaBetaSearch(int.MinValue, int.MaxValue, i, true);
                ChessBoard bestState = init.bestState;
                MoveGenerator.best_move_queue.Clear();
                while (bestState != null)
                {                
                   MoveGenerator.addAIBestMoveQueue(bestState.move);
                   bestState = bestState.bestState;
                 }

                //foreach (Move move in MoveGenerator.best_move_queue)
                //{
                //    Console.WriteLine("Best move is " + move.from_rank + move.from_file + move.to_rank + move.to_file);
                //}
                i++;
            }
            return;

        }



    }

   


}
