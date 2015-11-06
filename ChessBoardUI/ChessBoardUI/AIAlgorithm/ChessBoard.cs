using ChessBoardUI.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessBoardUI.AIAlgorithm
{
    public class ChessBoard
    {
        //public ulong WP = 0x000000000000ff00, WN = 0x0000000000000042, WB = 0x0000000000000024, WQ = 0x0000000000000008, WR = 0x0000000000000081, WK = 0x0000000000000010,
        //              BP = 0x00ff000000000000, BN = 0x4200000000000000, BB = 0x2400000000000000, BQ = 0x0800000000000000, BR = 0x8100000000000000, BK = 0x1000000000000000;

        public ulong WP , WN , WB , WQ, WR , WK ,
                      BP , BN , BB, BQ , BR , BK ;

        public ulong occupied, empty, whitePieces, enemyOrEmpty;

        public Move move; // the move taken to this board state 
       
        public ChessBoard bestState=null;
        public bool MKC = true;
        public bool MQC = true;
        public bool PKC = true;
        public bool PQC = true;
        //test
        public int eva;

        private static int[] PawnTable = new int[] {
                0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,
                7   ,   7   ,   13  ,   23  ,   26  ,   13  ,   7   ,   7   ,
                -2  ,   -2  ,   4   ,   12  ,   15  ,   4   ,   -2  ,   -2  ,
                -3  ,   -3  ,   2   ,   9   ,   11  ,   2   ,   -3  ,   -3  ,
                -4  ,   -4  ,   0   ,   6   ,   8   ,   0   ,   -4  ,   -4  ,
                -4  ,   -4  ,   0   ,   4   ,   6   ,   0   ,   -4  ,   -4  ,
                -1  ,   -1  ,   1   ,   5   ,   6   ,   1   ,   -1  ,   -1  ,
                0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0
                };

        private static int[] KnightTable = new int[] {
                -2  ,   2   ,   7   ,   9   ,   9   ,   7   ,   2   ,   -2  ,
                1   ,   4   ,   12  ,   13  ,   13  ,   12  ,   4   ,   1   ,
                5   ,   11  ,   18  ,   19  ,   19  ,   18  ,   11  ,   5   ,
                3   ,   10  ,   14  ,   14  ,   14  ,   14  ,   10  ,   3   ,
                0   ,   5   ,   8   ,   9   ,   9   ,   8   ,   5   ,   0   ,
                -3  ,   1   ,   3   ,   4   ,   4   ,   3   ,   1   ,   -3  ,
                -5  ,   -3  ,   -1  ,   0   ,   0   ,   -1  ,   -3  ,   -5  ,
                -7  ,   -5  ,   -4  ,   -2  ,   -2  ,   -4  ,   -5  ,   -7
                };

        private static int[] BishopTable = new int[] {
                2   ,   3   ,   4   ,   4   ,   4   ,   4   ,   3   ,   2   ,
                4   ,   7   ,   7   ,   7   ,   7   ,   7   ,   7   ,   4   ,
                3   ,   5   ,   6   ,   6   ,   6   ,   6   ,   5   ,   3   ,
                3   ,   5   ,   7   ,   7   ,   7   ,   7   ,   5   ,   3   ,
                4   ,   5   ,   6   ,   8   ,   8   ,   6   ,   5   ,   4   ,
                4   ,   5   ,   5   ,   -2  ,   -2  ,   5   ,   5   ,   4   ,
                5   ,   5   ,   5   ,   3   ,   3   ,   5   ,   5   ,   5   ,
                0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0
                };

        private static int[] RookTable = new int[] {
                9   ,   9   ,   11  ,   10  ,   11  ,   9   ,   9   ,   9   ,
                4   ,   6   ,   7   ,   9   ,   9   ,   7   ,   6   ,   4   ,
                9   ,   10  ,   10  ,   11  ,   11  ,   10  ,   10  ,   9   ,
                8   ,   8   ,   8   ,   9   ,   9   ,   8   ,   8   ,   8   ,
                6   ,   6   ,   5   ,   6   ,   6   ,   5   ,   6   ,   6   ,
                4   ,   5   ,   5   ,   5   ,   5   ,   5   ,   5   ,   4   ,
                3   ,   4   ,   4   ,   6   ,   6   ,   4   ,   4   ,   3  ,
                0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0
                };

        private static int[] KingTableO = new int[] {
                0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,
                0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,
                0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,
                0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,
                0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,
                0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,
                0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,
                0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,
                };

        private static int[] KingTableE = new int[] {
                -50 ,   -10 ,   0   ,   0   ,   0   ,   0   ,   -10 ,   -50 ,
                -10 ,    0  ,   10  ,   10  ,   10  ,   10  ,   0   ,   -10 ,
                0   ,   10  ,   20  ,   20  ,   20  ,   20  ,   10  ,   0   ,
                0   ,   10  ,   20  ,   40  ,   40  ,   20  ,   10  ,   0   ,
                0   ,   10  ,   20  ,   40  ,   40  ,   20  ,   10  ,   0   ,
                0   ,   10  ,   20  ,   20  ,   20  ,   20  ,   10  ,   0   ,
                -10,    0   ,   10  ,   10  ,   10  ,   10  ,   0   ,   -10 ,
                -50 ,   -10 ,   0   ,   0   ,   0   ,   0   ,   -10 ,   -50
                };

        private static int[] QueenTable = new int[] {
                2   ,   3   ,   4   ,   3   ,   4   ,   3   ,   3   ,   2   ,
                2   ,   3   ,   4   ,   4   ,   4   ,   4   ,   3   ,   2   ,
                3   ,   4   ,   4   ,   4   ,   4   ,   4   ,   4   ,   3   ,
                3   ,   3   ,   4   ,   4   ,   4   ,   4   ,   3   ,   3   ,
                2   ,   3   ,   3   ,   4   ,   4   ,   3   ,   3   ,   2   ,
                2   ,   2   ,   2   ,   3   ,   3   ,   2   ,   2   ,   2   ,
                2   ,   2   ,   2   ,   2   ,   2   ,   2   ,   2   ,   2   ,
                0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0
                };

        private static int[] Mirror64 = new int[] {
                56  ,   57  ,   58  ,   59  ,   60  ,   61  ,   62  ,   63  ,
                48  ,   49  ,   50  ,   51  ,   52  ,   53  ,   54  ,   55  ,
                40  ,   41  ,   42  ,   43  ,   44  ,   45  ,   46  ,   47  ,
                32  ,   33  ,   34  ,   35  ,   36  ,   37  ,   38  ,   39  ,
                24  ,   25  ,   26  ,   27  ,   28  ,   29  ,   30  ,   31  ,
                16  ,   17  ,   18  ,   19  ,   20  ,   21  ,   22  ,   23  ,
                8   ,   9   ,   10  ,   11  ,   12  ,   13  ,   14  ,   15  ,
                0   ,   1   ,   2   ,   3   ,   4   ,   5   ,   6   ,   7
                };


        public ChessBoard(ulong WP, ulong WN, ulong WB, ulong WQ, ulong WR, ulong WK, ulong BP, ulong BN, ulong BB, ulong BQ, ulong BR, ulong BK, Move move=null , bool MKC= true, bool MQC =true, bool PKC= true, bool PQC=true)
        {
            this.WP = WP;
            this.WN = WN;
            this.WB = WB;
            this.WQ = WQ;
            this.WR = WR;
            this.WK = WK;
            this.BP = BP;
            this.BN = BN;
            this.BB = BB;
            this.BQ = BQ;
            this.BR = BR;
            this.BK = BK;
            this.move = move;
            this.MKC = MKC;
            this.MQC = MQC;
            this.PKC = PKC;
            this.PQC = PQC;
            //this.player = currentPlayer;

            createUsefullBitboards();
        }



        private void createUsefullBitboards()
        {
            occupied = (WP | BP | WR | BR | WN | BN | WB | BB | WQ | BQ | WK | BK);
            empty = ~occupied;
            whitePieces = (WP | WR | WN | WB | WQ | WK);
            enemyOrEmpty = (empty | whitePieces);
        }

        //public void Copyboard(ulong WP, ulong WN, ulong WB, ulong WQ, ulong WR, ulong WK, ulong BP, ulong BN, ulong BB, ulong BQ, ulong BR, ulong BK)
        //{
        //    this.WP = WP;
        //    this.WN = WN;
        //    this.WB = WB;
        //    this.WQ = WQ;
        //    this.WR = WR;
        //    this.WK = WK;
        //    this.BP = BP;
        //    this.BN = BN;
        //    this.BB = BB;
        //    this.BQ = BQ;
        //    this.BR = BR;
        //    this.BK = BK;

        //    //createUsefullBitboards();
        //}

        // TO DO
        public int evaluateBoard(bool min_max, ChessBoard leaf_chessboard)
        {
            int Machine_Points = 0;
            int Player_Points = 0;

            // Evaluate pieces under threat
            MoveGenerator.setCurrentBitboards(leaf_chessboard.BP, leaf_chessboard.BR, leaf_chessboard.BN, leaf_chessboard.BB, leaf_chessboard.BQ, leaf_chessboard.BK, leaf_chessboard.WP, leaf_chessboard.WR, leaf_chessboard.WN, leaf_chessboard.WB, leaf_chessboard.WQ, leaf_chessboard.WK);
            MoveGenerator.setCurrentBitboardsHistoryMove(leaf_chessboard.move);
            MoveGenerator.setCurrentCastlingCondition(leaf_chessboard.MKC, leaf_chessboard.MQC, leaf_chessboard.PKC, leaf_chessboard.PQC);
            ArrayList moves;
            if (leaf_chessboard.move.PKC || leaf_chessboard.move.PQC)
            {
                Player_Points += 14;
            }
            if (leaf_chessboard.move.MKC || leaf_chessboard.move.MQC)
            {
                //Console.WriteLine("Hallo");
                Machine_Points += 14;
            }


            if (min_max)
            {
                moves = MoveGenerator.PossibleMovesMachine();
            }
            else
            {
                moves = MoveGenerator.PossibleMovesPlayer();
            }

            moves.Sort(new MoveCompare()); // sort the list so it get captured move first;
            Nullable<PieceType> piece_cap = null;
            foreach (Move leaf_move in moves)
            {

                //this is castling move bonus


                if (leaf_move.cap_type != piece_cap)
                {
                    if (min_max) //if it is a max leaf node, add value to machine's value;
                        Machine_Points += 7;
                    else
                        Player_Points += 7;
                }
                else
                {
                    break;
                }
            }


            for (int i = 0; i < 64; i++)
            {
                // Evaluate positions of each piece on the board
                if (((leaf_chessboard.occupied >> i) & 1) == 1)
                {
                    if (MoveGenerator.player_color) //if player plays white
                    {
                        if (((leaf_chessboard.WB >> i) & 1) == 1)
                        {
                            //Evaluation of White Bishop
                            Player_Points += (Constants.Constants.BISHOP_WEIGHT+ BishopTable[(8 * (7 - i / 8) + i % 8)]);

                        }
                        else if (((leaf_chessboard.WK >> i) & 1) == 1)
                        {
                            //Evaluation of White King
                            Player_Points += (Constants.Constants.KING_WEIGHT+ KingTableO[(8 * (7 - i / 8) + i % 8)]);

                        }
                        else if (((leaf_chessboard.WN >> i) & 1) == 1)
                        {
                            //Evaluation of White Knight 
                            Player_Points += (Constants.Constants.KNIGHT_WEIGHT+ KnightTable[(8 * (7 - i / 8) + i % 8)]);

                        }
                        else if (((leaf_chessboard.WP >> i) & 1) == 1)
                        {
                            //Evaluation of White Pawns 
                            Player_Points += (Constants.Constants.PAWN_WEIGHT+ PawnTable[(8 * (7 - i / 8) + i % 8)]);

                            //evaluate of dobble pawn weakness
                            for (int rank= (7- i/8); rank >=0 ; rank--)
                            if (((leaf_chessboard.WP >> (i+8*rank)) & 1) ==1)
                            {
                                Player_Points += -7;
                            }
                        }
                        else if (((leaf_chessboard.WQ >> i) & 1) == 1)
                        {
                            //Evaluation of White Queen 
                            Player_Points += (Constants.Constants.QUEEN_WEIGHT+ QueenTable[(8 * (7 - i / 8) + i % 8)]);

                        }
                        else if (((leaf_chessboard.WR >> i) & 1) == 1)
                        {
                            //Evaluation of White Rook 
                            Player_Points += (Constants.Constants.ROOK_WEIGHT+ RookTable[(8 * (7 - i / 8) + i % 8)]);
                        }
                        else if (((leaf_chessboard.BB >> i) & 1) == 1)
                        {
                            //Evaluation of Black Bishop 
                            Machine_Points += (Constants.Constants.BISHOP_WEIGHT+ BishopTable[Mirror64[(8 * (7 - i / 8) + i % 8)]]);

                        }
                        else if (((leaf_chessboard.BK >> i) & 1) == 1)
                        {
                            //Evaluation of Black King 
                            Machine_Points += (Constants.Constants.KING_WEIGHT + KingTableO[Mirror64[(8 * (7 - i / 8) + i % 8)]]);

                        }
                        else if (((leaf_chessboard.BN >> i) & 1) == 1)
                        {
                            //Evaluation of Black Knight 
                            Machine_Points += (Constants.Constants.KNIGHT_WEIGHT + KnightTable[Mirror64[(8 * (7 - i / 8) + i % 8)]]);

                        }
                        else if (((leaf_chessboard.BP >> i) & 1) == 1)
                        {
                            //Evaluation of Black Pawns 
                            Machine_Points += (Constants.Constants.PAWN_WEIGHT+ PawnTable[Mirror64[(8 * (7 - i / 8) + i % 8)]]);

                            //evaluate of dobble pawn weakness
                            for (int rank = (7 - i / 8); rank >= 0; rank--)
                                if (((leaf_chessboard.WP >> (i + 8 * rank)) & 1) == 1)
                                {
                                    Machine_Points += -7;
                                }
                        }
                        else if (((leaf_chessboard.BQ >> i) & 1) == 1)
                        {
                            //Evaluation of Black Queen 
                            Machine_Points += (Constants.Constants.QUEEN_WEIGHT+ QueenTable[Mirror64[(8 * (7 - i / 8) + i % 8)]]);

                        }
                        else if (((leaf_chessboard.BR >> i) & 1) == 1)
                        {
                            //Evaluation of Black Rook 
                            Machine_Points += (Constants.Constants.ROOK_WEIGHT + RookTable[Mirror64[(8 * (7 - i / 8) + i % 8)]]);
                        }
                    }


                    else //player plays black
                    {
                        if (((leaf_chessboard.WB >> i) & 1) == 1)
                        {
                            //Evaluation of White Bishop
                            Machine_Points += (Constants.Constants.BISHOP_WEIGHT+ BishopTable[Mirror64[(8 * (7 - i / 8) + i % 8)]]);

                        }
                        else if (((leaf_chessboard.WK >> i) & 1) == 1)
                        {
                            //Evaluation of White King
                            Machine_Points += (Constants.Constants.KING_WEIGHT+ KingTableO[Mirror64[(8 * (7 - i / 8) + i % 8)]]);

                        }
                        else if (((leaf_chessboard.WN >> i) & 1) == 1)
                        {
                            //Evaluation of White Knight 
                            Machine_Points += (Constants.Constants.KNIGHT_WEIGHT+ KnightTable[Mirror64[(8 * (7 - i / 8) + i % 8)]]);

                        }
                        else if (((leaf_chessboard.WP >> i) & 1) == 1)
                        {
                            //Evaluation of White Pawns 
                            Machine_Points += (Constants.Constants.PAWN_WEIGHT+ PawnTable[Mirror64[(8 * (7 - i / 8) + i % 8)]]);

                            //evaluate of dobble pawn weakness
                            for (int rank = (7 - i / 8); rank >= 0; rank--)
                                if (((leaf_chessboard.WP >> (i + 8 * rank)) & 1) == 1)
                                {
                                    Machine_Points += -7;
                                }
                        }
                        else if (((leaf_chessboard.WQ >> i) & 1) == 1)
                        {
                            //Evaluation of White Queen 
                            Machine_Points +=(Constants.Constants.QUEEN_WEIGHT+ QueenTable[Mirror64[(8 * (7 - i / 8) + i % 8)]]);

                        }
                        else if (((leaf_chessboard.WR >> i) & 1) == 1)
                        {
                            //Evaluation of White Rook 
                            Machine_Points += (Constants.Constants.ROOK_WEIGHT+ RookTable[Mirror64[(8 * (7 - i / 8) + i % 8)]]);
                        }
                        else if (((leaf_chessboard.BB >> i) & 1) == 1)
                        {
                            //Evaluation of Black Bishop 
                            Player_Points += (Constants.Constants.BISHOP_WEIGHT+ BishopTable[(8 * (7 - i / 8) + i % 8)]);

                        }
                        else if (((leaf_chessboard.BK >> i) & 1) == 1)
                        {
                            //Evaluation of Black King 
                            Player_Points += (Constants.Constants.KING_WEIGHT+ KingTableO[(8 * (7 - i / 8) + i % 8)]);

                        }
                        else if (((leaf_chessboard.BN >> i) & 1) == 1)
                        {
                            //Evaluation of Black Knight 
                            Player_Points += (Constants.Constants.KNIGHT_WEIGHT+ KnightTable[(8 * (7 - i / 8) + i % 8)]);

                        }
                        else if (((leaf_chessboard.BP >> i) & 1) == 1)
                        {
                            //Evaluation of Black Pawns 
                            Player_Points += (Constants.Constants.PAWN_WEIGHT+ PawnTable[(8 * (7 - i / 8) + i % 8)]);

                            //evaluate of dobble pawn weakness
                            for (int rank = (7 - i / 8); rank >= 0; rank--)
                                if (((leaf_chessboard.WP >> (i + 8 * rank)) & 1) == 1)
                                {
                                    Player_Points += -7;
                                }
                        }
                        else if (((leaf_chessboard.BQ >> i) & 1) == 1)
                        {
                            //Evaluation of Black Queen 
                            Player_Points +=(Constants.Constants.QUEEN_WEIGHT+ QueenTable[(8 * (7 - i / 8) + i % 8)]);

                        }
                        else if (((leaf_chessboard.BR >> i) & 1) == 1)
                        {
                            //Evaluation of Black Rook 
                            Player_Points += (Constants.Constants.ROOK_WEIGHT+ RookTable[(8 * (7 - i / 8) + i % 8)]);
                        }
                    }
                }

            }

            return Machine_Points - Player_Points;  //(machine point - player point)
        }

        public int AlphaBetaSearch(int alpha, int beta, int layer, bool min_max)
        {
           
            if (layer == 0)
            {
                return evaluateBoard(min_max, this);
            }
            else if (min_max)
            {
                List<ChessBoard> chessboards = MoveGenerator.generateChessBoards(min_max, BP, BR, BN, BB, BQ, BK, WP, WR, WN, WB, WQ, WK, this.move, this.MKC, this.MQC, this.PKC, this.PQC);
                
                foreach (ChessBoard CB in chessboards)
                {
                    if (DateTime.Compare(DateTime.Now, MoveGenerator.end_time) > 0)
                    { break; }
                    //Console.WriteLine("Chessboard item max " + Convert.ToString((long)CB.occupied, 2));
                    //Console.WriteLine("Chessboard eva " + evaluateBoard(!min_max, CB));
                    //evaluateBoard(min_max, CB);
                    //Console.WriteLine("Chessboard eva " + evaluateBoard(min_max, CB));
                    //evaluateBoard(min_max, CB);
                    int result = CB.AlphaBetaSearch(alpha, beta, layer - 1, !min_max);
                    if (result > alpha)
                    {
                        alpha = result;
                        bestState = CB;
                    }

                    if (alpha >= beta)
                    {
                        break;
                    }
                }
                return alpha;
            }
            else
            {
                List<ChessBoard> chessboards = MoveGenerator.generateChessBoards(min_max, BP, BR, BN, BB, BQ, BK, WP, WR, WN, WB, WQ, WK, this.move, this.MKC, this.MQC, this.PKC, this.PQC);
                foreach (ChessBoard CB in chessboards)
                {
                    if (DateTime.Compare(DateTime.Now, MoveGenerator.end_time) > 0)
                    { break; }
                    //  Console.WriteLine("This is min
                    // Console.WriteLine("Chessboard item min " + Convert.ToString((long)CB.occupied, 2));
                    //Console.WriteLine("Chessboard Bish min " + Convert.ToString((long)CB.BB, 2));
                    //Console.WriteLine("Chessboard knig min " + Convert.ToString((long)CB.WN, 2));
                    //Console.WriteLine("Chessboard eva " + evaluateBoard(!min_max, CB));
                    //evaluateBoard(min_max, CB);
                    int result = CB.AlphaBetaSearch(alpha, beta, layer - 1, !min_max);
                    if (result < beta)
                    {
                        beta = result;
                        bestState = CB;
                    }

                    if (alpha >= beta)
                    {
                        break;
                    }
                }
                return beta;
            }
        }
    }
}
