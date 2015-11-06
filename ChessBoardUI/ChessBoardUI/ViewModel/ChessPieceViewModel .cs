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
using ChessBoardUI.AIAlgorithm;

namespace ChessBoardUI.ViewModel
{
    public enum PieceType
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }

    public enum Player
    {
        White,
        Black
    }

    class ChessPiece : ViewModelBase
    {
        private Point _Pos;
        private PieceType _Type;
        private Player _Player;
        private bool _Ownership;
        private RelayCommand _PieceClickCommand;
        private RelayCommand _PieceMoveCommand;
        private bool _chose = false;
        private int priv_coor_x;
        private int priv_coor_y;


        public int Pri_Coor_X
        {
            get { return priv_coor_x; }
        }

        public int Pri_Coor_Y
        {
            get { return priv_coor_y; }
        }

        public int Coor_X
        {
            get { return ((int)this._Pos.X / Constants.Constants.CELL_EDGE_LENGTH); }
        }

        public int Coor_Y
        {
            get { return ((int)this._Pos.Y / Constants.Constants.CELL_EDGE_LENGTH); }
        }

        public Point Pos
        {
            get { return this._Pos; }
            set {
                this.priv_coor_x =(int) value.X; this.priv_coor_y = (int)value.Y; value.X = value.X * Constants.Constants.CELL_EDGE_LENGTH; value.Y=value.Y* Constants.Constants.CELL_EDGE_LENGTH; this._Pos = value;  RaisePropertyChanged(() => this.Pos);
            }
        }

        public double Pos_X
        {
            get { return this._Pos.X; }
            set { this._Pos.X = value* Constants.Constants.CELL_EDGE_LENGTH; RaisePropertyChanged(() => this.Pos); }
        }

        public double Pos_Y
        {
            get { return this._Pos.Y; }
            set { this._Pos.Y = value * Constants.Constants.CELL_EDGE_LENGTH; RaisePropertyChanged(() => this.Pos); }
        }

        public PieceType Type
        {
            get { return this._Type; }
            set { this._Type = value; RaisePropertyChanged(() => this.Type); RaisePropertyChanged(() => this.Type); }  //RaisePropertyChanged Called from a property setter to notify the framework that an Entity member has changed.
        }

        public Player Player
        {
            get { return this._Player; }
            set { this._Player = value; RaisePropertyChanged(() => this.Player); }
        }

        public bool Ownership
        {
            get { return this._Ownership; }
            set { this._Ownership = value; RaisePropertyChanged(() => this.Ownership); }
        }

        public bool Chose
        {
            get { return this._chose; }
            set {this._chose = value; RaisePropertyChanged(() => this.Chose); }
        }

        public RelayCommand PieceClickCommand
        {
            get { return this._PieceClickCommand; }
            set { this._PieceClickCommand = new RelayCommand(this.selectPiece) ;}
        }

        public RelayCommand PieceMoveCommand
        {
            get { return this._PieceMoveCommand; }
            set { this._PieceMoveCommand = new RelayCommand(this.dragPiece); }
        }


        public void selectPiece()
        {
            if (this._Ownership)
            {
                if (!this.Chose)
                {
                    this.Chose = true;
                    this.Pos_X = ((int)Mouse.GetPosition(null).X - Constants.Constants.CANVAS_MARGIN_LEFT) / Constants.Constants.CELL_EDGE_LENGTH;
                    this.Pos_Y = ((int)Mouse.GetPosition(null).Y - Constants.Constants.CANVAS_MARGIN_TOP) / Constants.Constants.CELL_EDGE_LENGTH;
                    this.priv_coor_x = this.Coor_X;
                    this.priv_coor_y = this.Coor_Y;
                }
                else
                {
                    //targetx&y is used to check if it is legal move later
                    int target_x = ((int)Mouse.GetPosition(null).X - Constants.Constants.CANVAS_MARGIN_LEFT) / Constants.Constants.CELL_EDGE_LENGTH;
                    int target_y = ((int)Mouse.GetPosition(null).Y - Constants.Constants.CANVAS_MARGIN_TOP) / Constants.Constants.CELL_EDGE_LENGTH;

                    //if player click on the same place. do nothing
                    if (target_x == this.priv_coor_x && target_y== this.priv_coor_y)
                    {
                        this.Pos_X = ((int)Mouse.GetPosition(null).X - Constants.Constants.CANVAS_MARGIN_LEFT) / Constants.Constants.CELL_EDGE_LENGTH;
                        this.Pos_Y = ((int)Mouse.GetPosition(null).Y - Constants.Constants.CANVAS_MARGIN_TOP) / Constants.Constants.CELL_EDGE_LENGTH;
                        this.Chose = false;
                        return ;
                    }

                    //if player makes a casstling move
                    if(MoveGenerator.LegalCastlingMove(this.priv_coor_x, this.priv_coor_y, target_x, target_y, this.Type))
                    {
                        //this.Pos_X = ((int)Mouse.GetPosition(null).X - Constants.Constants.CANVAS_MARGIN_LEFT) / Constants.Constants.CELL_EDGE_LENGTH;
                        //this.Pos_Y = ((int)Mouse.GetPosition(null).Y - Constants.Constants.CANVAS_MARGIN_TOP) / Constants.Constants.CELL_EDGE_LENGTH;
                        //Console.WriteLine("Coor for king in vm is " + this.Coor_X);
                        this.Chose = false;
                        Messenger.Default.Send(new HumanMoveMessage { FromPoint = new Point(this.priv_coor_x, this.priv_coor_y), ToPoint = new Point(target_x, target_y), Type = this.Type, Castling = true, AnPassent = false, Promotion = false, Turn = true });
                        return ;
                    }


                    //need player make en passent move
                    if(MoveGenerator.LegalEnPassentPawnMove(this.priv_coor_x, this.priv_coor_y, target_x, target_y, this.Type))
                    {
                        this.Pos_X = ((int)Mouse.GetPosition(null).X - Constants.Constants.CANVAS_MARGIN_LEFT) / Constants.Constants.CELL_EDGE_LENGTH;
                        this.Pos_Y = ((int)Mouse.GetPosition(null).Y - Constants.Constants.CANVAS_MARGIN_TOP) / Constants.Constants.CELL_EDGE_LENGTH;
                        this.Chose = false;
                        Messenger.Default.Send(new HumanMoveMessage { FromPoint = new Point(this.priv_coor_x, this.priv_coor_y), ToPoint = new Point(this.Coor_X, this.Coor_Y), Type = this.Type, Castling = false, AnPassent = true, Promotion = false, Turn = true });
                        return ;
                    }



                    if (MoveGenerator.LegalRegularMove(this.priv_coor_x, this.priv_coor_y, target_x, target_y, this.Type))
                    {
                        //promote humans pawn
                        if (target_y == 0 && this.Type == PieceType.Pawn && this.Player==Player.White)
                        {
                            this.Type = PieceType.Queen;
                        }

                        this.Pos_X = ((int)Mouse.GetPosition(null).X - Constants.Constants.CANVAS_MARGIN_LEFT) / Constants.Constants.CELL_EDGE_LENGTH;
                        this.Pos_Y = ((int)Mouse.GetPosition(null).Y - Constants.Constants.CANVAS_MARGIN_TOP) / Constants.Constants.CELL_EDGE_LENGTH;
                        this.Chose = false;

                        Messenger.Default.Send(new HumanMoveMessage { FromPoint = new Point(this.priv_coor_x, this.priv_coor_y), ToPoint = new Point(this.Coor_X, this.Coor_Y), Type = this.Type, Castling = false, AnPassent = false, Promotion=false, Turn=true});
                    }

                }

            }
        }

        public void dragPiece()
        {
            
           
            double real_x = Mouse.GetPosition(null).X - Constants.Constants.CANVAS_MARGIN_LEFT;
            double real_y = Mouse.GetPosition(null).Y - Constants.Constants.CANVAS_MARGIN_TOP;


            if (this.Chose&&UIHelper.RestrictImageMove(real_x, real_y))
            {
                this.Pos_X = (Mouse.GetPosition(null).X - Constants.Constants.CANVAS_MARGIN_LEFT - Constants.Constants.IMAGE_CENTER_TO_LEFTTOP) / Constants.Constants.CELL_EDGE_LENGTH;
                this.Pos_Y = (Mouse.GetPosition(null).Y - Constants.Constants.CANVAS_MARGIN_TOP - Constants.Constants.IMAGE_CENTER_TO_LEFTTOP) / Constants.Constants.CELL_EDGE_LENGTH;
                RaisePropertyChanged(() => this.Pos);
            }
        }

    }
}
