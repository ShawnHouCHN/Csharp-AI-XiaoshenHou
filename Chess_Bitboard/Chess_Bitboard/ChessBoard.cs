using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Bitboard
{
    public class ChessBoard
    {
        public static void initiateBoard()
        {
            long WP = 0L, WN = 0L, WB = 0L, WQ = 0L, WR = 0L, WK = 0L,
                BP = 0L, BN = 0L, BB = 0L, BQ = 0L, BR = 0L, BK = 0L;
            string[,] chessboard = new string[8, 8] {
            {"r","n","b","q","k","b","n","r"},  //black
            {"p","p","p","p","p","p","p","p"},
            {" "," "," "," "," "," "," "," "},
            {" "," "," "," "," "," "," "," "},
            {" "," "," "," "," "," "," "," "},
            {" "," "," "," "," "," "," "," "},
            {"P","P","P","P","P","P","P","P"},   //white
            {"R","N","B","Q","K","B","N","R"}
            };
            arrayToBitBoard(chessboard, WP, WN, WB, WQ, WR, WK, BP, BN, BB, BQ, BR, BK);
        }

        public static void arrayToBitBoard(string[,] chessboard, long WP, long WN, long WB, long WQ, long WR, long WK, long BP, long BN, long BB, long BQ, long BR, long BK)
        {
            string binary;     //64-bit string
            for (int i = 0; i < 64; i++)
            {

                binary = "0000000000000000000000000000000000000000000000000000000000000000";
                binary = binary.Substring(i + 1) + "1" + binary.Substring(0, i);
                switch (chessboard[i / 8, i % 8])  //12 bitboard of each piece 
                {
                    
                    
                    case "P": WP += convertStringToBitboard(binary);
                        break;
                    case "R": WR += convertStringToBitboard(binary);
                        break;
                    case "N": WN += convertStringToBitboard(binary);
                        break;
                    case "B": WB += convertStringToBitboard(binary);
                        break;
                    case "Q": WQ += convertStringToBitboard(binary);
                        break;
                    case "K": WK += convertStringToBitboard(binary);
                        break;
                    case "p": BP += convertStringToBitboard(binary);
                      break;
                    case "r": BR += convertStringToBitboard(binary);
                        break;
                    case "n": BN += convertStringToBitboard(binary);
                        break;
                    case "b": BB += convertStringToBitboard(binary);
                        break;
                    case "q": BQ += convertStringToBitboard(binary);
                        break;
                    case "k": BK += convertStringToBitboard(binary);
                        break;
                    case " " :
                        break;
                        
                }
            }

           //// Console.Write(Convert.ToString(WR+WK, 2)[3]);
           // Console.WriteLine();
           // Console.Write(WN);
           // Console.Write(Convert.ToString(WK, 2));
           // Console.WriteLine();
            
           // Console.Write(Convert.ToString(BQ, 2));
            drawArray(WP, WN, WB, WQ, WR, WK, BP, BN, BB, BQ, BR, BK);
            Console.WriteLine();
            Console.Write(Convert.ToString(WR+WK, 2));
            Console.ReadLine();
        }

        public static long convertStringToBitboard(string binary)
        {

            if (binary[0].Equals('0'))
            {
                return Convert.ToInt64(binary, 2);
            }
            else
            {
                return Convert.ToInt64("1" + binary.Substring(2), 2) * 2;
            }

        }

        public static void drawWhiteQueenArray(long WQ)
        {
            string[,] chessboard_revert = new string[8, 8];
            for (int i = 0; i < 64; i++)
            {
                if (((WQ >> i) & 1) == 1) { chessboard_revert[i / 8, i % 8] = "Q"; }
            }
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (chessboard_revert[row, col] == null || chessboard_revert[row, col] == " ")
                        Console.Write("*");
                    else
                        Console.Write(chessboard_revert[row, col]);
                }
                Console.WriteLine();
            }

        }

        public static void drawArray(long WP, long WN, long WB, long WQ, long WR, long WK, long BP, long BN, long BB, long BQ, long BR, long BK)
        {
           
            string[,] chessboard_revert = new string[8, 8];
            for (int i = 0; i < 64; i++)
            {
                if (((WP >> i) & 1) == 1) { chessboard_revert[i / 8, i % 8] = "P"; }
                if (((WN >> i) & 1) == 1) { chessboard_revert[i / 8, i % 8] = "N"; }
                if (((WB >> i) & 1) == 1) { chessboard_revert[i / 8, i % 8] = "B"; }
                if (((WQ >> i) & 1) == 1) { chessboard_revert[i / 8, i % 8] = "Q"; }
                if (((WK >> i) & 1) == 1) { chessboard_revert[i / 8, i % 8] = "K"; }
                if (((WR >> i) & 1) == 1) { chessboard_revert[i / 8, i % 8] = "R"; }
                if (((BP >> i) & 1) == 1) { chessboard_revert[i / 8, i % 8] = "p"; }
                if (((BN >> i) & 1) == 1) { chessboard_revert[i / 8, i % 8] = "n"; }
                if (((BB >> i) & 1) == 1) { chessboard_revert[i / 8, i % 8] = "b"; }
                if (((BQ >> i) & 1) == 1) { chessboard_revert[i / 8, i % 8] = "q"; }
                if (((BK >> i) & 1) == 1) { chessboard_revert[i / 8, i % 8] = "k"; }
                if (((BR >> i) & 1) == 1) { chessboard_revert[i / 8, i % 8] = "r"; }
            }


            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (chessboard_revert[row, col] == null|| chessboard_revert[row, col] == " ")
                        Console.Write("*");
                    else
                        Console.Write(chessboard_revert[row, col]);
                }
                Console.WriteLine();
            }
        }
    }
}