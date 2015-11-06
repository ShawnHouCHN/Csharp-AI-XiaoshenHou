using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using GalaSoft.MvvmLight.Messaging;
using ChessBoardUI.Players;
using ChessBoardUI.AIAlgorithm;

namespace ChessBoardUI.ViewModel
{
    class MainControl
    {
        private ObservableCollection<ChessPiece> pieces_collection;
        HMPlayer human_player;
        AIPlayer machine_player;
        private Dictionary<int, ChessPiece> pieces_dict;
        


        public MainControl(bool color, string difficulty)
        {
            this.pieces_dict = new Dictionary<int, ChessPiece>();
            ChessPiece player_pawn0, player_pawn1, player_pawn2, player_pawn3, player_pawn4, player_pawn5, player_pawn6, player_pawn7, player_rook0, player_knight0, player_bishop0, player_king, player_queen, player_bishop1, player_knight1, player_rook1,
                       machine_pawn0, machine_pawn1, machine_pawn2, machine_pawn3, machine_pawn4, machine_pawn5, machine_pawn6, machine_pawn7, machine_rook0, machine_knight0, machine_bishop0, machine_king, machine_queen, machine_bishop1, machine_knight1, machine_rook1;

            if (color)
            {
                player_pawn0 = new ChessPiece { Pos = new Point(0, 6), Type = PieceType.Pawn, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                player_pawn1 = new ChessPiece { Pos = new Point(1, 6), Type = PieceType.Pawn, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                player_pawn2 = new ChessPiece { Pos = new Point(2, 6), Type = PieceType.Pawn, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                player_pawn3 = new ChessPiece { Pos = new Point(3, 6), Type = PieceType.Pawn, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                player_pawn4 = new ChessPiece { Pos = new Point(4, 6), Type = PieceType.Pawn, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                player_pawn5 = new ChessPiece { Pos = new Point(5, 6), Type = PieceType.Pawn, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                player_pawn6 = new ChessPiece { Pos = new Point(6, 6), Type = PieceType.Pawn, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                player_pawn7 = new ChessPiece { Pos = new Point(7, 6), Type = PieceType.Pawn, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                player_rook0 = new ChessPiece { Pos = new Point(0, 7), Type = PieceType.Rook, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                player_knight0 = new ChessPiece { Pos = new Point(1, 7), Type = PieceType.Knight, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                player_bishop0 = new ChessPiece { Pos = new Point(2, 7), Type = PieceType.Bishop, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                player_king = new ChessPiece { Pos = new Point(4, 7), Type = PieceType.King, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                player_queen = new ChessPiece { Pos = new Point(3, 7), Type = PieceType.Queen, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                player_bishop1 = new ChessPiece { Pos = new Point(5, 7), Type = PieceType.Bishop, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                player_knight1 = new ChessPiece { Pos = new Point(6, 7), Type = PieceType.Knight, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                player_rook1 = new ChessPiece { Pos = new Point(7, 7), Type = PieceType.Rook, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_pawn0 = new ChessPiece { Pos = new Point(0, 1), Type = PieceType.Pawn, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_pawn1 = new ChessPiece { Pos = new Point(1, 1), Type = PieceType.Pawn, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_pawn2 = new ChessPiece { Pos = new Point(2, 1), Type = PieceType.Pawn, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_pawn3 = new ChessPiece { Pos = new Point(3, 1), Type = PieceType.Pawn, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_pawn4 = new ChessPiece { Pos = new Point(4, 1), Type = PieceType.Pawn, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_pawn5 = new ChessPiece { Pos = new Point(5, 1), Type = PieceType.Pawn, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_pawn6 = new ChessPiece { Pos = new Point(6, 1), Type = PieceType.Pawn, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_pawn7 = new ChessPiece { Pos = new Point(7, 1), Type = PieceType.Pawn, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_rook0 = new ChessPiece { Pos = new Point(0, 0), Type = PieceType.Rook, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_knight0 = new ChessPiece { Pos = new Point(1, 0), Type = PieceType.Knight, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_bishop0 = new ChessPiece { Pos = new Point(2, 0), Type = PieceType.Bishop, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_king = new ChessPiece { Pos = new Point(4, 0), Type = PieceType.King, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_queen = new ChessPiece { Pos = new Point(3, 0), Type = PieceType.Queen, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_bishop1 = new ChessPiece { Pos = new Point(5, 0), Type = PieceType.Bishop, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_knight1 = new ChessPiece { Pos = new Point(6, 0), Type = PieceType.Knight, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_rook1 = new ChessPiece { Pos = new Point(7, 0), Type = PieceType.Rook, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };

                MoveGenerator.setInitBitboards(color, 0x00ff000000000000, 0x8100000000000000, 0x4200000000000000, 0x2400000000000000, 0x0800000000000000, 0x1000000000000000, 0x000000000000ff00, 0x000000000000081, 0x000000000000042, 0x000000000000024, 0x0000000000000008, 0x0000000000000010);
            }
            else
            {
                machine_pawn0 = new ChessPiece { Pos = new Point(0, 1), Type = PieceType.Pawn, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_pawn1 = new ChessPiece { Pos = new Point(1, 1), Type = PieceType.Pawn, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_pawn2 = new ChessPiece { Pos = new Point(2, 1), Type = PieceType.Pawn, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_pawn3 = new ChessPiece { Pos = new Point(3, 1), Type = PieceType.Pawn, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_pawn4 = new ChessPiece { Pos = new Point(4, 1), Type = PieceType.Pawn, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_pawn5 = new ChessPiece { Pos = new Point(5, 1), Type = PieceType.Pawn, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_pawn6 = new ChessPiece { Pos = new Point(6, 1), Type = PieceType.Pawn, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_pawn7 = new ChessPiece { Pos = new Point(7, 1), Type = PieceType.Pawn, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_rook0 = new ChessPiece { Pos = new Point(0, 0), Type = PieceType.Rook, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_knight0 = new ChessPiece { Pos = new Point(1, 0), Type = PieceType.Knight, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_bishop0 = new ChessPiece { Pos = new Point(2, 0), Type = PieceType.Bishop, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_king = new ChessPiece { Pos = new Point(3, 0), Type = PieceType.King, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_queen = new ChessPiece { Pos = new Point(4, 0), Type = PieceType.Queen, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_bishop1 = new ChessPiece { Pos = new Point(5, 0), Type = PieceType.Bishop, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_knight1 = new ChessPiece { Pos = new Point(6, 0), Type = PieceType.Knight, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                machine_rook1 = new ChessPiece { Pos = new Point(7, 0), Type = PieceType.Rook, Player = Player.White, Ownership = color, PieceClickCommand = null, PieceMoveCommand = null };
                player_pawn0 = new ChessPiece { Pos = new Point(0, 6), Type = PieceType.Pawn, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                player_pawn1 = new ChessPiece { Pos = new Point(1, 6), Type = PieceType.Pawn, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                player_pawn2 = new ChessPiece { Pos = new Point(2, 6), Type = PieceType.Pawn, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                player_pawn3 = new ChessPiece { Pos = new Point(3, 6), Type = PieceType.Pawn, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                player_pawn4 = new ChessPiece { Pos = new Point(4, 6), Type = PieceType.Pawn, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                player_pawn5 = new ChessPiece { Pos = new Point(5, 6), Type = PieceType.Pawn, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                player_pawn6 = new ChessPiece { Pos = new Point(6, 6), Type = PieceType.Pawn, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                player_pawn7 = new ChessPiece { Pos = new Point(7, 6), Type = PieceType.Pawn, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                player_rook0 = new ChessPiece { Pos = new Point(0, 7), Type = PieceType.Rook, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                player_knight0 = new ChessPiece { Pos = new Point(1, 7), Type = PieceType.Knight, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                player_bishop0 = new ChessPiece { Pos = new Point(2, 7), Type = PieceType.Bishop, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                player_king = new ChessPiece { Pos = new Point(3, 7), Type = PieceType.King, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                player_queen = new ChessPiece { Pos = new Point(4, 7), Type = PieceType.Queen, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                player_bishop1 = new ChessPiece { Pos = new Point(5, 7), Type = PieceType.Bishop, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                player_knight1 = new ChessPiece { Pos = new Point(6, 7), Type = PieceType.Knight, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };
                player_rook1 = new ChessPiece { Pos = new Point(7, 7), Type = PieceType.Rook, Player = Player.Black, Ownership = !color, PieceClickCommand = null, PieceMoveCommand = null };

                MoveGenerator.setRevertInitBitboards(color, 0x000000000000ff00, 0x0000000000000081, 0x0000000000000042, 0x0000000000000024, 0x0000000000000010, 0x0000000000000008, 0x00ff000000000000, 0x8100000000000000, 0x4200000000000000, 0x2400000000000000, 0x1000000000000000, 0x0800000000000000);

            }

            this.pieces_collection = new ObservableCollection<ChessPiece>
            {
                player_rook0,player_knight0,player_bishop0, player_king,player_queen,player_bishop1,player_knight1,player_rook1,
                player_pawn0,player_pawn1,player_pawn2,player_pawn3,player_pawn4,player_pawn5,player_pawn6,player_pawn7,
                machine_rook0,machine_knight0,machine_bishop0, machine_king,machine_queen,machine_bishop1,machine_knight1,machine_rook1,
                machine_pawn0,machine_pawn1,machine_pawn2,machine_pawn3,machine_pawn4,machine_pawn5,machine_pawn6,machine_pawn7
            };

            this.pieces_dict.Add(player_pawn0.Coor_X * 10 + player_pawn0.Coor_Y, player_pawn0);             this.pieces_dict.Add(machine_pawn0.Coor_X * 10 + machine_pawn0.Coor_Y, machine_pawn0);
            this.pieces_dict.Add(player_pawn1.Coor_X * 10 + player_pawn1.Coor_Y, player_pawn1);             this.pieces_dict.Add(machine_pawn1.Coor_X * 10 + machine_pawn1.Coor_Y, machine_pawn1);
            this.pieces_dict.Add(player_pawn2.Coor_X * 10 + player_pawn2.Coor_Y, player_pawn2);             this.pieces_dict.Add(machine_pawn2.Coor_X * 10 + machine_pawn2.Coor_Y, machine_pawn2);
            this.pieces_dict.Add(player_pawn3.Coor_X * 10 + player_pawn3.Coor_Y, player_pawn3);             this.pieces_dict.Add(machine_pawn3.Coor_X * 10 + machine_pawn3.Coor_Y, machine_pawn3);
            this.pieces_dict.Add(player_pawn4.Coor_X * 10 + player_pawn4.Coor_Y, player_pawn4);             this.pieces_dict.Add(machine_pawn4.Coor_X * 10 + machine_pawn4.Coor_Y, machine_pawn4);
            this.pieces_dict.Add(player_pawn5.Coor_X * 10 + player_pawn5.Coor_Y, player_pawn5);             this.pieces_dict.Add(machine_pawn5.Coor_X * 10 + machine_pawn5.Coor_Y, machine_pawn5);
            this.pieces_dict.Add(player_pawn6.Coor_X * 10 + player_pawn6.Coor_Y, player_pawn6);             this.pieces_dict.Add(machine_pawn6.Coor_X * 10 + machine_pawn6.Coor_Y, machine_pawn6);
            this.pieces_dict.Add(player_pawn7.Coor_X * 10 + player_pawn7.Coor_Y, player_pawn7);             this.pieces_dict.Add(machine_pawn7.Coor_X * 10 + machine_pawn7.Coor_Y, machine_pawn7);
            this.pieces_dict.Add(player_rook0.Coor_X * 10 + player_rook0.Coor_Y, player_rook0);             this.pieces_dict.Add(machine_rook0.Coor_X * 10 + machine_rook0.Coor_Y, machine_rook0);
            this.pieces_dict.Add(player_rook1.Coor_X * 10 + player_rook1.Coor_Y, player_rook1);             this.pieces_dict.Add(machine_rook1.Coor_X * 10 + machine_rook1.Coor_Y, machine_rook1);
            this.pieces_dict.Add(player_knight0.Coor_X * 10 + player_knight0.Coor_Y, player_knight0);       this.pieces_dict.Add(machine_knight0.Coor_X * 10 + machine_knight0.Coor_Y, machine_knight0);
            this.pieces_dict.Add(player_knight1.Coor_X * 10 + player_knight1.Coor_Y, player_knight1);       this.pieces_dict.Add(machine_knight1.Coor_X * 10 + machine_knight1.Coor_Y, machine_knight1);
            this.pieces_dict.Add(player_bishop0.Coor_X * 10 + player_bishop0.Coor_Y, player_bishop0);       this.pieces_dict.Add(machine_bishop0.Coor_X * 10 + machine_bishop0.Coor_Y, machine_bishop0);
            this.pieces_dict.Add(player_bishop1.Coor_X * 10 + player_bishop1.Coor_Y, player_bishop1);       this.pieces_dict.Add(machine_bishop1.Coor_X * 10 + machine_bishop1.Coor_Y, machine_bishop1);
            this.pieces_dict.Add(player_queen.Coor_X * 10 + player_queen.Coor_Y, player_queen);             this.pieces_dict.Add(machine_queen.Coor_X * 10 + machine_queen.Coor_Y, machine_queen);
            this.pieces_dict.Add(player_king.Coor_X * 10 + player_king.Coor_Y, player_king);                this.pieces_dict.Add(machine_king.Coor_X * 10 + machine_king.Coor_Y, machine_king);

            this.human_player = new HMPlayer(this.pieces_collection, this.pieces_dict);
            this.machine_player = new AIPlayer(this.pieces_collection, this.pieces_dict);

            //add difficulty
            if (difficulty.Equals("Easy"))
                this.machine_player.Interval = 10;
            else if(difficulty.Equals("Normal"))
                this.machine_player.Interval = 15;
            else if (difficulty.Equals("Hard"))
                this.machine_player.Interval = 25;

        }


        public ObservableCollection<ChessPiece> BoardCollection
        {
            get { return this.pieces_collection; }
           
        }
       

        public HMPlayer HumanPlayer
        {
            get {return this.human_player; }
        }

        public AIPlayer MachinePlayer
        {
            get { return this.machine_player; }
        }







    }
}
