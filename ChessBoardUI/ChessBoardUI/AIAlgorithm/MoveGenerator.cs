using ChessBoardUI.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessBoardUI.AIAlgorithm
{
    public class Move
    {
        public int from_rank;
        public int from_file;
        public int to_rank;
        public int to_file;
        public PieceType moved_type;
        public PieceType? cap_type=null;
        
        public bool promote = false;
        public bool MKC = false;
        public bool MQC = false;
        public bool PKC = false;
        public bool PQC = false;


        public Move(int from_x, int from_y, int to_x, int to_y)
        {
            this.from_rank = from_x;
            this.from_file = from_y;
            this.to_rank = to_x;
            this.to_file = to_y;
        }

        public Move(int from_x, int from_y, bool MKC, bool MQC , bool PKC, bool PQC)
        {
            this.from_rank = from_x;
            this.from_file = from_y;
            this.MKC = MKC;
            this.MQC = MQC;
            this.PKC = PKC;
            this.PQC = PQC;
        }

        public Move(int from_x, int from_y, int to_x, int to_y, PieceType moved_type, bool promote)
        {
            this.from_rank = from_x;
            this.from_file = from_y;
            this.to_rank = to_x;
            this.to_file = to_y;
            this.promote = promote;
            this.moved_type = moved_type;
        }

        public Move(int from_x, int from_y, int to_x, int to_y, PieceType moved_type, PieceType cap_type, bool promote)
        {
            this.from_rank = from_x;
            this.from_file = from_y;
            this.to_rank = to_x;
            this.to_file = to_y;
            this.promote = promote;
            this.moved_type = moved_type;
            this.cap_type = cap_type;
        }
    }

    class MoveCompare: IComparer
    {
        Move last_move;
        public MoveCompare()
        {
            this.last_move = null;
        }

        public MoveCompare(Move last_move=null)
        {
            this.last_move = last_move;
        }

        public int Compare(Object x, Object y)
        {
            if (x == null)
                return (y == null) ? 0 : 1;

            if (y == null)
                return -1;

            Move item_one = x as Move;
            Move item_two = y as Move;
            Nullable<PieceType> piece_cap = null;

            if (last_move == null)
            {
                if (item_one.cap_type != piece_cap && item_two.cap_type == piece_cap)
                {
                    return -1;
                }
                else if (item_one.cap_type == piece_cap && item_two.cap_type != piece_cap)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {

                if (item_one.from_rank == last_move.from_rank && item_one.from_file == last_move.from_file&& item_one.to_rank==last_move.to_rank&& item_one.to_file==last_move.to_file)
                {
                    return -1;
                }
                else if (item_two.from_rank == last_move.from_rank && item_two.from_file == last_move.from_file&& item_two.to_rank == last_move.to_rank && item_two.to_file == last_move.to_file)
                {
                    return 1;
                }
                else if (item_one.cap_type != piece_cap && item_two.cap_type == piece_cap)
                {
                    return -1;
                }
                else if (item_one.cap_type == piece_cap && item_two.cap_type != piece_cap)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    class MoveGenerator
    {

        //init position
        public static ulong white_pawns ;
        public static ulong black_pawns ;
        public static ulong white_knights ;
        public static ulong black_knights ;
        public static ulong white_bishops ;
        public static ulong black_bishops ;
        public static ulong white_rooks ;
        public static ulong black_rooks ;
        public static ulong white_queens ;
        public static ulong black_queens ;
        public static ulong white_king ;
        public static ulong black_king ;
        public static bool  player_color;  // deefualt is white
        public static bool MQC=true, MKC=true, PQC=true, PKC=true; // machine queen castling, macine king castling, player queen castling, player king castling
        
        //full occupied bitboard
        public static ulong full_occupied = 0xffffffffffffffff;
        public static ulong white_occupied = white_pawns | white_knights | white_bishops | white_rooks | white_queens | white_king;
        public static ulong black_occupied = black_pawns | black_knights | black_bishops | black_rooks | black_queens | black_king;
        public static ulong empty = ~(white_pawns | white_knights | white_bishops | white_rooks | white_queens | white_king | black_pawns | black_knights | black_bishops | black_rooks | black_queens | black_king);
        public static ulong pieces_occupied = ~ empty;

        //history move(en passent)
        public static Move history_move;
        public static Move history_ai_move;

        public static DateTime process_time;
        public static DateTime end_time;

        //these are for pawn moves
        static ulong rank7 = 0xff00000000000000;
        static ulong rank0 = 0x00000000000000ff;
        static ulong rank3 = 0x00000000ff000000;
        static ulong rank4 = 0x000000ff00000000;
        static ulong file0 = 0x0101010101010101;
        static ulong file7 = 0x8080808080808080;

        static ulong not_black_occupied;

        
        static ulong black_occupied_noking;
        //static ulong empty;
        //static ulong rook_moves, rook_cap_moves;
        //static ulong bishop_moves, bishop_cap_moves;
        

        static public ulong[] rook_right_board_set = {
            0x00000000000000fe,
            0x00000000000000fc,
            0x00000000000000f8,
            0x00000000000000f0,
            0x00000000000000e0,
            0x00000000000000c0,
            0x0000000000000080,
            0x0000000000000000,
            0x000000000000fe00,
            0x000000000000fc00,
            0x000000000000f800,
            0x000000000000f000,
            0x000000000000e000,
            0x000000000000c000,
            0x0000000000008000,
            0x0000000000000000,
            0x0000000000fe0000,
            0x0000000000fc0000,
            0x0000000000f80000,
            0x0000000000f00000,
            0x0000000000e00000,
            0x0000000000c00000,
            0x0000000000800000,
            0x0000000000000000,
            0x00000000fe000000,
            0x00000000fc000000,
            0x00000000f8000000,
            0x00000000f0000000,
            0x00000000e0000000,
            0x00000000c0000000,
            0x0000000080000000,
            0x0000000000000000,
            0x000000fe00000000,
            0x000000fc00000000,
            0x000000f800000000,
            0x000000f000000000,
            0x000000e000000000,
            0x000000c000000000,
            0x0000008000000000,
            0x0000000000000000,
            0x0000fe0000000000,
            0x0000fc0000000000,
            0x0000f80000000000,
            0x0000f00000000000,
            0x0000e00000000000,
            0x0000c00000000000,
            0x0000800000000000,
            0x0000000000000000,
            0x00fe000000000000,
            0x00fc000000000000,
            0x00f8000000000000,
            0x00f0000000000000,
            0x00e0000000000000,
            0x00c0000000000000,
            0x0080000000000000,
            0x0000000000000000,
            0xfe00000000000000,
            0xfc00000000000000,
            0xf800000000000000,
            0xf000000000000000,
            0xe000000000000000,
            0xc000000000000000,
            0x8000000000000000,
            0x0000000000000000,
        };

        static public ulong[] rook_left_board_set = {
            0x0000000000000000,
            0x0000000000000001,
            0x0000000000000003,
            0x0000000000000007,
            0x000000000000000f,
            0x000000000000001f,
            0x000000000000003f,
            0x000000000000007f,
            0x0000000000000000,
            0x0000000000000100,
            0x0000000000000300,
            0x0000000000000700,
            0x0000000000000f00,
            0x0000000000001f00,
            0x0000000000003f00,
            0x0000000000007f00,
            0x0000000000000000,
            0x0000000000010000,
            0x0000000000030000,
            0x0000000000070000,
            0x00000000000f0000,
            0x00000000001f0000,
            0x00000000003f0000,
            0x00000000007f0000,
            0x0000000000000000,
            0x0000000001000000,
            0x0000000003000000,
            0x0000000007000000,
            0x000000000f000000,
            0x000000001f000000,
            0x000000003f000000,
            0x000000007f000000,
            0x0000000000000000,
            0x0000000100000000,
            0x0000000300000000,
            0x0000000700000000,
            0x0000000f00000000,
            0x0000001f00000000,
            0x0000003f00000000,
            0x0000007f00000000,
            0x0000000000000000,
            0x0000010000000000,
            0x0000030000000000,
            0x0000070000000000,
            0x00000f0000000000,
            0x00001f0000000000,
            0x00003f0000000000,
            0x00007f0000000000,
            0x0000000000000000,
            0x0001000000000000,
            0x0003000000000000,
            0x0007000000000000,
            0x000f000000000000,
            0x001f000000000000,
            0x003f000000000000,
            0x007f000000000000,
            0x0000000000000000,
            0x0100000000000000,
            0x0300000000000000,
            0x0700000000000000,
            0x0f00000000000000,
            0x1f00000000000000,
            0x3f00000000000000,
            0x7f00000000000000,
        };

        static public ulong[] rook_up_board_set = {
            0x0101010101010100,
            0x0202020202020200,
            0x0404040404040400,
            0x0808080808080800,
            0x1010101010101000,
            0x2020202020202000,
            0x4040404040404000,
            0x8080808080808000,
            0x0101010101010000,
            0x0202020202020000,
            0x0404040404040000,
            0x0808080808080000,
            0x1010101010100000,
            0x2020202020200000,
            0x4040404040400000,
            0x8080808080800000,
            0x0101010101000000,
            0x0202020202000000,
            0x0404040404000000,
            0x0808080808000000,
            0x1010101010000000,
            0x2020202020000000,
            0x4040404040000000,
            0x8080808080000000,
            0x0101010100000000,
            0x0202020200000000,
            0x0404040400000000,
            0x0808080800000000,
            0x1010101000000000,
            0x2020202000000000,
            0x4040404000000000,
            0x8080808000000000,
            0x0101010000000000,
            0x0202020000000000,
            0x0404040000000000,
            0x0808080000000000,
            0x1010100000000000,
            0x2020200000000000,
            0x4040400000000000,
            0x8080800000000000,
            0x0101000000000000,
            0x0202000000000000,
            0x0404000000000000,
            0x0808000000000000,
            0x1010000000000000,
            0x2020000000000000,
            0x4040000000000000,
            0x8080000000000000,
            0x0100000000000000,
            0x0200000000000000,
            0x0400000000000000,
            0x0800000000000000,
            0x1000000000000000,
            0x2000000000000000,
            0x4000000000000000,
            0x8000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
        };


        static public ulong[] rook_down_board_set = {
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000001,
            0x0000000000000002,
            0x0000000000000004,
            0x0000000000000008,
            0x0000000000000010,
            0x0000000000000020,
            0x0000000000000040,
            0x0000000000000080,
            0x0000000000000101,
            0x0000000000000202,
            0x0000000000000404,
            0x0000000000000808,
            0x0000000000001010,
            0x0000000000002020,
            0x0000000000004040,
            0x0000000000008080,
            0x0000000000010101,
            0x0000000000020202,
            0x0000000000040404,
            0x0000000000080808,
            0x0000000000101010,
            0x0000000000202020,
            0x0000000000404040,
            0x0000000000808080,
            0x0000000001010101,
            0x0000000002020202,
            0x0000000004040404,
            0x0000000008080808,
            0x0000000010101010,
            0x0000000020202020,
            0x0000000040404040,
            0x0000000080808080,
            0x0000000101010101,
            0x0000000202020202,
            0x0000000404040404,
            0x0000000808080808,
            0x0000001010101010,
            0x0000002020202020,
            0x0000004040404040,
            0x0000008080808080,
            0x0000010101010101,
            0x0000020202020202,
            0x0000040404040404,
            0x0000080808080808,
            0x0000101010101010,
            0x0000202020202020,
            0x0000404040404040,
            0x0000808080808080,
            0x0001010101010101,
            0x0002020202020202,
            0x0004040404040404,
            0x0008080808080808,
            0x0010101010101010,
            0x0020202020202020,
            0x0040404040404040,
            0x0080808080808080,
        };

        static public ulong[] bishop_45_board_set =
        {
            0x8040201008040200,
            0x0080402010080400,
            0x0000804020100800,
            0x0000008040201000,
            0x0000000080402000,
            0x0000000000804000,
            0x0000000000008000,
            0x0000000000000000, //rank 1
            0x4020100804020000,
            0x8040201008040000,
            0x0080402010080000,
            0x0000804020100000,
            0x0000008040200000,
            0x0000000080400000,
            0x0000000000800000,
            0x0000000000000000, //rank 2
            0x2010080402000000,
            0x4020100804000000,
            0x8040201008000000,
            0x0080402010000000,
            0x0000804020000000,
            0x0000008040000000,
            0x0000000080000000,
            0x0000000000000000, //rank 3
            0x1008040200000000,
            0x2010080400000000,
            0x4020300800000000,
            0x8040201000000000,
            0x0080402000000000,
            0x0000804000000000,
            0x0000008000000000,
            0x0000000000000000, //rank 4
            0x0804020000000000,
            0x1008040000000000,
            0x2010080000000000,
            0x4020100000000000,
            0x8040200000000000,
            0x0080400000000000,
            0x0000800000000000,
            0x0000000000000000, // rank 5
            0x0402000000000000,
            0x0804000000000000,
            0x1008000000000000,
            0x2010000000000000,
            0x4020000000000000,
            0x8040000000000000,
            0x0080000000000000,
            0x0000000000000000, //rank 6
            0x0200000000000000,
            0x0400000000000000,
            0x0800000000000000,
            0x1000000000000000,
            0x2000000000000000,
            0x4000000000000000,
            0x8000000000000000,
            0x0000000000000000, //rank 7
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000, //rank 8
        };

        static public ulong[] bishop_135_board_set =
        {
            0x0000000000000000,
            0x0000000000000100,
            0x0000000000010200,
            0x0000000001020400,
            0x0000000102040800,
            0x0000010204081000,
            0x0001020408102000,
            0x0102040810204000, // rank 1
            0x0000000000000000,
            0x0000000000010000,
            0x0000000001020000,
            0x0000000102040000,
            0x0000010204080000,
            0x0001020408100000,
            0x0102040810200000,
            0x0204081020400000, // rank 2
            0x0000000000000000,
            0x0000000001000000,
            0x0000000102000000,
            0x0000010204000000,
            0x0001020408000000,
            0x0102040810000000,
            0x0204081020000000,
            0x0408102040000000, // rank 3
            0x0000000000000000,
            0x0000000100000000,
            0x0000010200000000,
            0x0001020400000000,
            0x0102040800000000,
            0x0204081000000000,
            0x0408102000000000,
            0x0810204000000000, // rank 4
            0x0000000000000000,
            0x0000010000000000,
            0x0001020000000000,
            0x0102040000000000,
            0x0204080000000000,
            0x0408100000000000,
            0x0810200000000000,
            0x1020400000000000, // rank 5
            0x0000000000000000,
            0x0001000000000000,
            0x0102000000000000,
            0x0204000000000000,
            0x0408000000000000,
            0x0810000000000000,
            0x1020000000000000,
            0x2040000000000000,  //rank 6
            0x0000000000000000,
            0x0100000000000000,
            0x0200000000000000,
            0x0400000000000000,
            0x0800000000000000,
            0x1000000000000000,
            0x2000000000000000,
            0x4000000000000000, // rank 7
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,

        };

        static public ulong[] bishop_225_board_set =
        {
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,  // rank 1
            0x0000000000000000,
            0x0000000000000001,
            0x0000000000000002,
            0x0000000000000004,
            0x0000000000000008,
            0x0000000000000010,
            0x0000000000000020,
            0x0000000000000040,   // rank 2
            0x0000000000000000,
            0x0000000000000100,
            0x0000000000000201,
            0x0000000000000402,
            0x0000000000000804,
            0x0000000000001008,
            0x0000000000002010,
            0x0000000000004020,   // rank 3
            0x0000000000000000,
            0x0000000000010000,
            0x0000000000020100,
            0x0000000000040201,
            0x0000000000080402,
            0x0000000000100804,
            0x0000000000201008,
            0x0000000000402010,    // rank 4
            0x0000000000000000,
            0x0000000001000000,
            0x0000000002010000,
            0x0000000004020100,
            0x0000000008040201,
            0x0000000010080402,
            0x0000000020100804,
            0x0000000040201008,     // rank 5
            0x0000000000000000,
            0x0000000100000000,
            0x0000000201000000,
            0x0000000402010000,
            0x0000000804020100,
            0x0000001008040201,
            0x0000002010080402,
            0x0000004020100804,  // rank 6
            0x0000000000000000,
            0x0000010000000000,
            0x0000020100000000,
            0x0000040201000000,
            0x0000080402010000,
            0x0000100804020100,
            0x0000201008040201,
            0x0000402010080402,   // rank 7
            0x0000000000000000,
            0x0001000000000000,
            0x0002010000000000,
            0x0004020100000000,
            0x0008040201000000,
            0x0010080402010000,
            0x0020100804020100,
            0x0040201008040201, // rank 8
        };


        static public ulong[] bishop_315_board_set =
        {
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,
            0x0000000000000000,  // rank 1
            0x0000000000000002,
            0x0000000000000004,
            0x0000000000000008,
            0x0000000000000010,
            0x0000000000000020,
            0x0000000000000040,
            0x0000000000000080,
            0x0000000000000000,  // rank 2
            0x0000000000000204,
            0x0000000000000408,
            0x0000000000000810,
            0x0000000000001020,
            0x0000000000002040,
            0x0000000000004080,
            0x0000000000008000,
            0x0000000000000000, // rank 3
            0x0000000000020408,
            0x0000000000040810,
            0x0000000000081020,
            0x0000000000102040,
            0x0000000000204080,
            0x0000000000408000,
            0x0000000000800000,
            0x0000000000000000,   // rank 4
            0x0000000002040810,
            0x0000000004081020,
            0x0000000008102040,
            0x0000000010204080,
            0x0000000020408000,
            0x0000000040800000,
            0x0000000080000000,
            0x0000000000000000,   // rank 5
            0x0000000204081020,
            0x0000000408102040,
            0x0000000810204080,
            0x0000001020408000,
            0x0000002040800000,
            0x0000004080000000,
            0x0000008000000000,
            0x0000000000000000,     // rank 6
            0x0000020408102040,
            0x0000040810204080,
            0x0000081020408000,
            0x0000102040800000,
            0x0000204080000000,
            0x0000408000000000,
            0x0000800000000000,
            0x0000000000000000,     // rank 7
            0x0002040810204080,
            0x0004081020408000,
            0x0008102040800000,
            0x0010204080000000,
            0x0020408000000000,
            0x0040800000000000,
            0x0080000000000000,
            0x0000000000000000,     // rank 8


        };

        static public ulong[] knight_board_set =
        {
            0x0000000000020400,
            0x0000000000050800,
            0x00000000000A1100,
            0x0000000000142200,
            0x0000000000284400,
            0x0000000000508800,
            0x0000000000A01000,
            0x0000000000402000,      // rank 1
            0x0000000002040004,
            0x0000000005080008,
            0x000000000A110011,
            0x0000000014220022,
            0x0000000028440044,
            0x0000000050880088,
            0x00000000A0100010,
            0x0000000040200020,     // rank 2
            0x0000000204000402,
            0x0000000508000804,
            0x0000000A1100110A,
            0x0000001422002214,
            0x0000002844004428,
            0x0000005088008850,
            0x000000A0100010A0,
            0x0000004020002040,     // rank 3
            0x0000020400040200,
            0x0000050800080500,
            0x00000A1100110A00,
            0x0000142200221400,
            0x0000284400442800,
            0x0000508800885000,
            0x0000A0100010A000,
            0x0000402000204000,     // rank 4
            0x0002040004020000,
            0x0005080008050000,
            0x000A1100110A0000,
            0x0014220022140000,
            0x0028440044280000,
            0x0050880088500000,
            0x00A0100010A00000,
            0x0040200020400000,     // rank 5
            0x0204000402000000,
            0x0508000805000000,
            0x0A1100110A000000,
            0x1422002214000000,
            0x2844004428000000,
            0x5088008850000000,
            0xA0100010A0000000,
            0x4020002040000000,     // rank 6
            0x0400040200000000,
            0x0800080500000000,
            0x1100110A00000000,
            0x2200221400000000,
            0x4400442800000000,
            0x8800885000000000,
            0x100010A000000000,
            0x2000204000000000,     // rank 7
            0x0004020000000000,
            0x0008050000000000,
            0x00110A0000000000,
            0x0022140000000000,
            0x0044280000000000,
            0x0088500000000000,
            0x0010A00000000000,
            0x0020400000000000,     // rank 8
        };

        static public ulong[] king_board_set =
        {
            0x0000000000000302,
            0x0000000000000705,
            0x0000000000000E0A,
            0x0000000000001C14,
            0x0000000000003828,
            0x0000000000007050,
            0x000000000000E0A0,
            0x000000000000C040,     // rank 1
            0x0000000000030203,
            0x0000000000070507,
            0x00000000000E0A0E,
            0x00000000001C141C,
            0x0000000000382838,
            0x0000000000705070,
            0x0000000000E0A0E0,
            0x0000000000C040C0,     // rank 2
            0x0000000003020300,
            0x0000000007050700,
            0x000000000E0A0E00,
            0x000000001C141C00,
            0x0000000038283800,
            0x0000000070507000,
            0x00000000E0A0E000,
            0x00000000C040C000,      // rank 3
            0x0000000302030000,
            0x0000000705070000,
            0x0000000E0A0E0000,
            0x0000001C141C0000,
            0x0000003828380000,
            0x0000007050700000,
            0x000000E0A0E00000,
            0x000000C040C00000,      // rank 4
            0x0000030203000000,
            0x0000070507000000,
            0x00000E0A0E000000,
            0x00001C141C000000,
            0x0000382838000000,
            0x0000705070000000,
            0x0000E0A0E0000000,
            0x0000C040C0000000,      // rank 5
            0x0003020300000000,
            0x0007050700000000,
            0x000E0A0E00000000,
            0x001C141C00000000,
            0x0038283800000000,
            0x0070507000000000,
            0x00E0A0E000000000,
            0x00C040C000000000,      // rank 6
            0x0302030000000000,
            0x0705070000000000,
            0x0E0A0E0000000000,
            0x1C141C0000000000,
            0x3828380000000000,
            0x7050700000000000,
            0xE0A0E00000000000,
            0xC040C00000000000,      // rank 7
            0x0203000000000000,
            0x0507000000000000,
            0x0A0E000000000000,
            0x141C000000000000,
            0x2838000000000000,
            0x5070000000000000,
            0xA0E0000000000000,
            0x40C0000000000000,      // rank 8
        };

        static public ulong[] file_mask =
        {
            0x0101010101010101,
            0x0202020202020202,
            0x0404040404040404,
            0x0808080808080808,
            0x1010101010101010,
            0x2020202020202020,
            0x4040404040404040,
            0x8080808080808080,
        };

        static ArrayList bp_move_list;
        static ArrayList r_move_list; //rook list
        static ArrayList b_move_list; //bishop list
        static ArrayList n_move_list; //knight list
        static ArrayList q_move_list; //queen list
        static ArrayList k_move_list;
        static ArrayList wp_move_list;
        static ArrayList machine_castling_move_list;
        static ArrayList player_castling_move_list;
        public static Queue<Move> best_move_queue;

        public MoveGenerator()
        {
            //white_occupied = white_pawns | white_knights | white_bishops | white_rooks | white_queens | white_king;
            empty = ~(white_pawns | white_knights | white_bishops | white_rooks | white_queens | white_king | black_pawns | black_knights | black_bishops | black_rooks | black_queens | black_king);
            black_occupied = (black_pawns | black_knights | black_bishops | black_rooks | black_queens | black_king);
            pieces_occupied = ~empty;
            //pieces_occupied = (white_pawns | white_knights | white_bishops | white_rooks | white_queens | white_king | black_pawns | black_knights | black_bishops | black_rooks | black_queens | black_king);
            bp_move_list = new ArrayList();
            r_move_list = new ArrayList();
            b_move_list = new ArrayList();
            n_move_list = new ArrayList();
            q_move_list = new ArrayList();
            k_move_list = new ArrayList();
            wp_move_list = new ArrayList();
            machine_castling_move_list = new ArrayList();
            player_castling_move_list = new ArrayList();
            best_move_queue = new Queue<Move>();
        }


        public void debugDrawFunction(long left, long right, long up, long down, int index)
        {
            string[,] chessboard_revert = new string[8, 8];
            chessboard_revert[7 - (index / 8), index % 8] = "S";
            for (int i = 0; i < 64; i++)
            {
                if (((left >> i) & 1) == 1 || ((right >> i) & 1) == 1 || ((up >> i) & 1) == 1 || ((down >> i) & 1) == 1) { chessboard_revert[7 - (i / 8), (i % 8)] = "*"; }

            }

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (chessboard_revert[row, col] == null || chessboard_revert[row, col] == " ")
                        Console.Write("-");
                    else
                        Console.Write(chessboard_revert[row, col]);
                }
                Console.WriteLine();
            }
        }


        public void debugDrawFunction(long jump_piece, int index)
        {
            string[,] chessboard_revert = new string[8, 8];
            chessboard_revert[7 - (index / 8), index % 8] = "S";
            for (int i = 0; i < 64; i++)
            {
                if (((jump_piece >> i) & 1) == 1) { chessboard_revert[7 - (i / 8), (i % 8)] = "*"; }

            }

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (chessboard_revert[row, col] == null || chessboard_revert[row, col] == " ")
                        Console.Write("-");
                    else
                        Console.Write(chessboard_revert[row, col]);
                }
                Console.WriteLine();
            }
        }

        // if player uses white
        public static void setInitBitboards(bool playercolor, ulong BP, ulong BR, ulong BN, ulong BB, ulong BQ, ulong BK, ulong WP, ulong WR, ulong WN, ulong WB, ulong WQ, ulong WK)
        {
            player_color = playercolor;
            black_pawns = BP;
            black_rooks = BR;
            black_knights = BN;
            black_bishops = BB;
            black_queens = BQ;
            black_king = BK;
            white_pawns = WP;
            white_rooks = WR;
            white_knights = WN;
            white_queens = WQ;
            white_king = WK;
            white_bishops = WB;
            empty = ~(WP | WR | WN | WB | WQ | WK | BK | BP | BR | BN | BB | BQ);
            white_occupied = (WP | WR | WN | WB | WQ | WK);
            black_occupied = (BK | BP | BR | BN | BB | BQ);
            pieces_occupied = (WP | WR | WN | WB | WQ | WK | BK | BP | BR | BN | BB | BQ);
        }

        //if player uses black
        public static void setRevertInitBitboards(bool playercolor, ulong BP, ulong BR, ulong BN, ulong BB, ulong BQ, ulong BK, ulong WP, ulong WR, ulong WN, ulong WB, ulong WQ, ulong WK)
        {
            player_color = playercolor;
            black_pawns = BP;
            black_rooks = BR;
            black_knights = BN;
            black_bishops = BB;
            black_queens = BQ;
            black_king = BK;
            white_pawns = WP;
            white_rooks = WR;
            white_knights = WN;
            white_queens = WQ;
            white_king = WK;
            white_bishops = WB;
            empty = ~(WP | WR | WN | WB | WQ | WK | BK | BP | BR | BN | BB | BQ);
            white_occupied = (WP | WR | WN | WB | WQ | WK);
            black_occupied = (BK | BP | BR | BN | BB | BQ);
            pieces_occupied = (WP | WR | WN | WB | WQ | WK | BK | BP | BR | BN | BB | BQ);
        }



        public static void setCurrentBitboards(ulong BP, ulong BR, ulong BN, ulong BB, ulong BQ, ulong BK, ulong WP, ulong WR, ulong WN, ulong WB, ulong WQ, ulong WK)
        {
            black_pawns = BP;
            black_rooks = BR;
            black_knights = BN;
            black_bishops = BB;
            black_queens = BQ;
            black_king = BK;
            white_pawns = WP;
            white_rooks = WR;
            white_knights = WN;
            white_queens = WQ;
            white_king = WK;
            white_bishops = WB;
            empty = ~(WP | WR | WN | WB | WQ | WK | BK | BP | BR | BN | BB | BQ);
            white_occupied = (WP | WR | WN | WB | WQ | WK);
            black_occupied = (BK | BP | BR | BN | BB | BQ);
            pieces_occupied = (WP | WR | WN | WB | WQ | WK | BK | BP | BR | BN | BB | BQ);
        }



        public static void setCurrentBitboardsHistoryMove(Move historymove=null)
        {
            history_move = historymove;
        }

        public static void addAIBestMoveQueue(Move historymove = null)
        {
            best_move_queue.Enqueue(historymove);

        }

        public static void setCurrentCastlingCondition(bool _MKC=true, bool _MQC=true, bool _PKC=true, bool _PQC=true)
        {
            MKC = _MKC;
            MQC = _MQC;
            PKC = _PKC;
            PQC = _PQC;
        }

        public static Move getCurrentBitboardsHistoryMove()
        {
            return history_move;
        }
        
        //castling is not made yet, but bool is kept in the parameters.
        public static ArrayList PossibleMovesMachine(string history="")
        {

            not_black_occupied = ~(white_pawns | white_knights | white_bishops | white_rooks | white_queens | white_king | black_king);
            black_occupied_noking = (black_pawns | black_rooks | black_knights | black_bishops | black_queens);
            black_occupied = (black_pawns | black_rooks | black_knights | black_bishops | black_queens| black_king);
            white_occupied = (white_pawns | white_knights | white_bishops | white_rooks | white_queens | white_king);
            ArrayList bp_list;
  
            if (player_color)
            {
                bp_list = PossibleKing(black_king, white_occupied);
                bp_list.AddRange(PossibleQueen(black_queens, white_occupied));
                bp_list.AddRange(PossibleKnight(black_knights, white_occupied));
                bp_list.AddRange(PossibleBishop(black_bishops, white_occupied));
                bp_list.AddRange(PossibleRook(black_rooks, white_occupied));
                bp_list.AddRange(PossiblePawnMachine(history, black_pawns));   //here comes the new code for castling
                bp_list.AddRange(PossibleCastlingMachine());
            }
            else
            {
                bp_list = PossibleKing(white_king, black_occupied);
                bp_list.AddRange(PossibleQueen(white_queens, black_occupied));
                bp_list.AddRange(PossibleKnight(white_knights, black_occupied));
                bp_list.AddRange(PossibleBishop(white_bishops, black_occupied));
                bp_list.AddRange(PossibleRook(white_rooks, black_occupied));
                bp_list.AddRange(PossiblePawnMachine(history, white_pawns));  // here comes the new code for castling
                bp_list.AddRange(PossibleCastlingMachine());
            }


            //can also be kept in hashtable for further usage. type oriented 
            return bp_list;
        }


        public static ArrayList PossibleMovesPlayer(string history="")
        {

            not_black_occupied = ~(white_pawns | white_knights | white_bishops | white_rooks | white_queens | white_king | black_king);
            black_occupied_noking = (black_pawns | black_rooks | black_knights | black_bishops | black_queens);
            black_occupied = (black_pawns | black_rooks | black_knights | black_bishops | black_queens | black_king);
            white_occupied = (white_pawns | white_knights | white_bishops | white_rooks | white_queens | white_king);
            ArrayList wp_list;

            if (player_color)
            {
                wp_list = PossibleKing(white_king, black_occupied);
                wp_list.AddRange(PossibleQueen(white_queens, black_occupied));
                wp_list.AddRange(PossibleKnight(white_knights, black_occupied));
                wp_list.AddRange(PossibleBishop(white_bishops, black_occupied));
                wp_list.AddRange(PossibleRook(white_rooks, black_occupied));
                wp_list.AddRange(PossiblePawnPlayer(history, white_pawns));
                wp_list.AddRange(PossibleCastlingPlayer());
            }
            else
            {
                wp_list = PossibleKing(black_king, white_occupied);
                wp_list.AddRange(PossibleQueen(black_queens, white_occupied));
                wp_list.AddRange(PossibleKnight(black_knights, white_occupied));
                wp_list.AddRange(PossibleBishop(black_bishops, white_occupied));
                wp_list.AddRange(PossibleRook(black_rooks, white_occupied));
                wp_list.AddRange(PossiblePawnPlayer(history, black_pawns));
                wp_list.AddRange(PossibleCastlingPlayer());
            }
            


            return wp_list; // test

        }

        public static ArrayList PossiblePawnMachine(string history, ulong machine_pawns)   //return list of moves black pawn can take. four characters in the list is a move, source rank, source file, desti rank, desti file.
        {

            bp_move_list.Clear();
            ulong enemy_pawns, enemy_rooks, enemy_knights, enemy_bishops, enemy_queens, enemy_king, enemy_occupied;

            //check the player color here
            if (machine_pawns == black_pawns)
            {
                enemy_pawns = white_pawns; enemy_bishops = white_bishops; enemy_knights = white_knights; enemy_rooks = white_rooks; enemy_king = white_king; enemy_queens = white_queens;
                enemy_occupied = white_occupied;
            }
            else
            {
                enemy_pawns = black_pawns; enemy_bishops = black_bishops; enemy_knights = black_knights; enemy_rooks = black_rooks; enemy_king = black_king; enemy_queens = black_queens;
                enemy_occupied = black_occupied;
            }

            ulong bp_cap_right_moves = (machine_pawns >> 7) & enemy_occupied & ~rank0 & ~file0;

            //Console.WriteLine(Convert.ToString((long)bp_cap_right_moves, 2));

            for (int i = 0; i < 64; i++)
            {
                if (((bp_cap_right_moves >> i) & 1) == 1)
                {
                    if(((enemy_pawns >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Pawn, false));
                    else if(((enemy_knights >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Knight, false));
                    else if(((enemy_bishops >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Bishop, false));
                    else if(((enemy_rooks >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Rook, false));
                    else if(((enemy_queens >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Queen, false));
                    else if(((enemy_king >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.King, false));
                }

            }

            ulong bp_cap_left_moves = (machine_pawns >> 9) & enemy_occupied & ~rank0 & ~file7;
            //Console.WriteLine(Convert.ToString((long)bp_cap_left_moves, 2));
            for (int i = 0; i < 64; i++)
            {
                if (((bp_cap_left_moves >> i) & 1) == 1)
                {
                    if (((enemy_pawns >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Pawn, false));
                    else if (((enemy_knights >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Knight, false));
                    else if (((enemy_bishops >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Bishop, false));
                    else if (((enemy_rooks >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Rook, false));
                    else if (((enemy_queens >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Queen, false));
                    else if (((enemy_king >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.King, false));
                }

            }

            ulong bp_1_downward_moves = (ulong)(machine_pawns >> 8) & empty & ~rank0;
            for (int i = 0; i < 64; i++)
            {
                if (((bp_1_downward_moves >> i) & 1) == 1)
                {
                   bp_move_list.Add(new Move((i / 8 + 1), (i % 8), (i / 8), (i % 8), PieceType.Pawn,false));
                }

            }

           // Console.WriteLine(Convert.ToString((long)bp_1_downward_moves, 2));

            ulong bp_2_downward_moves = (ulong)(machine_pawns >> 16) & empty & (empty >> 8) & rank4;
            for (int i = 0; i < 64; i++)
            {
                if (((bp_2_downward_moves >> i) & 1) == 1)
                {
                    bp_move_list.Add(new Move((i / 8 + 2), (i % 8), (i / 8), (i % 8), PieceType.Pawn, false));
                }

            }

            //Console.WriteLine(Convert.ToString((long)bp_2_downward_moves, 2));

            //promotion move needs to be add in later..........
            ulong bp_promote_right_cap_moves = (machine_pawns >> 7) & enemy_occupied & rank0 & ~file0;        //promote by right capture;
            for (int i = 0; i < 64; i++)
            {
                if (((bp_promote_right_cap_moves >> i) & 1) == 1)
                {
                    if (((enemy_pawns >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Pawn, true));
                    else if (((enemy_knights >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Knight, true));
                    else if (((enemy_bishops >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Bishop, true));
                    else if (((enemy_rooks >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Rook, true));
                    else if (((enemy_queens >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Queen, true));
                    else if (((enemy_king >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.King, true));
                }
            }
           // Console.WriteLine(Convert.ToString((long)bp_promote_right_cap_moves, 2));

            ulong bp_promote_left_cap_moves = (machine_pawns >> 9) & enemy_occupied & rank0 & ~file7;        //promote by left capture;
            for (int i = 0; i < 64; i++)
            {
                if (((bp_promote_left_cap_moves >> i) & 1) == 1)
                {
                    if (((enemy_pawns >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Pawn, true));
                    else if (((enemy_knights >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Knight, true));
                    else if (((enemy_bishops >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Bishop, true));
                    else if (((enemy_rooks >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Rook, true));
                    else if (((enemy_queens >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Queen, true));
                    else if (((enemy_king >> i) & 1) == 1)
                        bp_move_list.Add(new Move((i / 8 + 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.King, true));
                }
            }
            //Console.WriteLine(Convert.ToString((long)bp_promote_left_cap_moves, 2));

            ulong bp_promote_downward_moves = (machine_pawns >> 8) & rank0 & empty;        //promote by right capture;
            for (int i = 0; i < 64; i++)
            {
                if (((bp_promote_downward_moves >> i) & 1) == 1)
                {
                    bp_move_list.Add(new Move((i / 8 + 1), (i % 8), (i / 8), (i % 8), PieceType.Pawn, true));
                }
            }
            // Console.WriteLine(Convert.ToString((long)bp_promote_downward_moves, 2));

            //en_passent move here
            if (history_move.moved_type == PieceType.Pawn)
            {
                if (history_move.from_rank == 1 && history_move.to_rank == 3 && history_move.moved_type == PieceType.Pawn && history_move.from_file == history_move.to_file)
                {
                   
                    ulong passent_right = (machine_pawns << 1) & enemy_pawns & rank3 & ~file0 & file_mask[history_move.from_file];  //right en passent capture
                    //Console.WriteLine("Hallo!! "+Convert.ToString((long)passent_right,2));
                    if (passent_right != 0)
                    {
                        
                        for (int i = 24; i < 32; i++)
                        {
                            if (((passent_right >> i) & 1) == 1)
                            {
                                //Console.WriteLine("Hallo!! {0} {1} {2} {3}", (i / 8), (i % 8 - 1), (i / 8 - 1), (i % 8));
                                bp_move_list.Add(new Move((i / 8), (i % 8 - 1), (i / 8 - 1), (i % 8), PieceType.Pawn, PieceType.Pawn, false));

                            }
                        }
                    }
                    ulong passent_left = (machine_pawns >> 1) & enemy_pawns & rank3 & ~file7 & file_mask[history_move.from_file];  //right en passent capture  
                    if (passent_left != 0)
                    {
                        for (int i = 24; i < 32; i++)
                        {
                            if (((passent_left >> i) & 1) == 1)
                            {
                                bp_move_list.Add(new Move((i / 8), (i % 8 + 1), (i / 8 - 1), (i % 8), PieceType.Pawn, PieceType.Pawn, false));

                            }
                        }
                    }
                }
            }



            return bp_move_list;
        }

        public static ArrayList PossiblePawnPlayer(string history, ulong player_pawns)   //return list of moves black pawn can take. four characters in the list is a move, source rank, source file, desti rank, desti file.
        {

            wp_move_list.Clear();
            ulong enemy_pawns, enemy_rooks, enemy_knights, enemy_bishops, enemy_queens, enemy_king, enemy_occupied;
            //check the player color here
            if (player_pawns == black_pawns)
            {
                enemy_pawns = white_pawns; enemy_bishops = white_bishops; enemy_knights = white_knights; enemy_rooks = white_rooks; enemy_king = white_king; enemy_queens = white_queens;
                
                enemy_occupied = white_occupied;
            }
            else
            {
                enemy_pawns = black_pawns; enemy_bishops = black_bishops; enemy_knights = black_knights; enemy_rooks = black_rooks; enemy_king = black_king; enemy_queens = black_queens;
                enemy_occupied = black_occupied;
            }

            ulong wp_cap_right_moves = (player_pawns << 9) & enemy_occupied & ~rank7 & ~file0;

            //Console.WriteLine(Convert.ToString((long)wp_cap_right_moves, 2));

            for (int i = 0; i < 64; i++)
            {
                if (((wp_cap_right_moves >> i) & 1) == 1)
                {
                    if (((enemy_pawns >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Pawn, false));
                    else if (((enemy_knights >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Knight, false));
                    else if (((enemy_bishops >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Bishop, false));
                    else if (((enemy_rooks >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Rook, false));
                    else if (((enemy_queens >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Queen, false));
                    else if (((enemy_king >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.King, false));
                }

            }

            ulong wp_cap_left_moves = (player_pawns << 7) & enemy_occupied & ~rank7 & ~file7;
           // Console.WriteLine(Convert.ToString((long)wp_cap_left_moves, 2));
            for (int i = 0; i < 64; i++)
            {
                if (((wp_cap_left_moves >> i) & 1) == 1)
                {
                    if (((enemy_pawns >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Pawn, false));
                    else if (((enemy_knights >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Knight, false));
                    else if (((enemy_bishops >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Bishop, false));
                    else if (((enemy_rooks >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Rook, false));
                    else if (((enemy_queens >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Queen, false));
                    else if (((enemy_king >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.King, false));
                }

            }

            ulong wp_1_upward_moves = (ulong)(player_pawns << 8) & empty & ~rank7;
            for (int i = 0; i < 64; i++)
            {
                if (((wp_1_upward_moves >> i) & 1) == 1)
                {
                    wp_move_list.Add(new Move((i / 8 - 1), (i % 8), (i / 8), (i % 8), PieceType.Pawn, false));
                }

            }

            //Console.WriteLine(Convert.ToString((long)wp_1_upward_moves, 2));


            ulong wp_2_upward_moves = (ulong)(player_pawns << 16) & empty & (empty >> 8) & rank3;
            for (int i = 0; i < 64; i++)
            {
                if (((wp_2_upward_moves >> i) & 1) == 1)
                {
                    wp_move_list.Add(new Move((i / 8 - 2), (i % 8), (i / 8), (i % 8), PieceType.Pawn, false));
                }

            }

           // Console.WriteLine(Convert.ToString((long)wp_2_upward_moves, 2));

            //promotion move needs to be add in later..........
            ulong wp_promote_right_cap_moves = (player_pawns << 9) & enemy_occupied & rank7 & ~file0;        //promote by right capture;
            for (int i = 0; i < 64; i++)
            {
                if (((wp_promote_right_cap_moves >> i) & 1) == 1)
                {
                    if (((enemy_pawns >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Pawn, true));
                    else if (((enemy_knights >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Knight, true));
                    else if (((enemy_bishops >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Bishop, true));
                    else if (((enemy_rooks >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Rook, true));
                    else if (((enemy_queens >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Queen, true));
                    else if (((enemy_queens >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 - 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.King, true));
                }
            }
            //Console.WriteLine(Convert.ToString((long)wp_promote_right_cap_moves, 2));

            ulong wp_promote_left_cap_moves = (player_pawns << 7) & enemy_occupied & rank7 & ~file7;        //promote by right capture;
            for (int i = 0; i < 64; i++)
            {
                if (((wp_promote_left_cap_moves >> i) & 1) == 1)
                {
                    if (((enemy_pawns >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Pawn, true));
                    else if (((enemy_knights >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Knight, true));
                    else if (((enemy_bishops >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Bishop, true));
                    else if (((enemy_rooks >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Rook, true));
                    else if (((enemy_queens >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.Queen, true));
                    else if (((enemy_king >> i) & 1) == 1)
                        wp_move_list.Add(new Move((i / 8 - 1), (i % 8 + 1), (i / 8), (i % 8), PieceType.Pawn, PieceType.King, true));
                }

            }
            //Console.WriteLine(Convert.ToString((long)wp_promote_left_cap_moves, 2));

            ulong wp_promote_upward_moves = (player_pawns >> 8) & rank7 & empty;        //promote by upward 1 move;
            for (int i = 0; i < 64; i++)
            {
                if (((wp_promote_upward_moves >> i) & 1) == 1)
                {
                    wp_move_list.Add(new Move((i / 8 - 1), (i % 8), (i / 8), (i % 8), PieceType.Pawn, true));
                }
            }
            //Console.WriteLine(Convert.ToString((long)wp_promote_upward_moves, 2));

            //en passent move for player
            if (history_move != null)
            {
                if (history_move.from_rank == 6 && history_move.to_rank == 4 && history_move.moved_type == PieceType.Pawn && history_move.from_file == history_move.to_file)
                {
                    ulong passent_right = (player_pawns << 1) & enemy_pawns & rank4 & ~file0 & file_mask[history_move.from_file];  //right en passent capture  
                    if (passent_right != 0)
                    {
                        for (int i = 32; i < 40; i++)
                        {
                            if (((passent_right >> i) & 1) == 1)
                            {
                                wp_move_list.Add(new Move((i / 8), (i % 8 - 1), (i / 8 + 1), (i % 8), PieceType.Pawn, PieceType.Pawn, false));
                            }
                        }
                    }

                    ulong passent_left = (player_pawns >> 1) & enemy_pawns & rank4 & ~file7 & file_mask[history_move.from_file];  //right en passent capture  
                    if (passent_left != 0)
                    {
                        for (int i = 32; i < 40; i++)
                        {
                            if (((passent_left >> i) & 1) == 1)
                            {
                                wp_move_list.Add(new Move((i / 8), (i % 8 + 1), (i / 8 + 1), (i % 8), PieceType.Pawn, PieceType.Pawn, false));
                            }
                        }
                    }
                }
            }
            return wp_move_list;
        }

        public static ArrayList PossibleRook(ulong Rook, ulong enemy_occupied)  //possible rook moves

        {
            r_move_list.Clear();

            int rook_index;
            ulong rook_right, rook_right_moves;
            ulong rook_left, rook_left_moves;
            ulong rook_up, rook_up_moves;
            ulong rook_down, rook_down_moves;
            ulong rook_moves, rook_cap_moves;


            for (int i = 0; i < 64; i++)
            {
                if (((Rook >> i) & 1) == 1)
                {
                    rook_index = i;
                    rook_right = rook_right_board_set[rook_index];  //pesudo move on 4 dimentions
                    rook_left = rook_left_board_set[rook_index];
                    rook_up = rook_up_board_set[rook_index];
                    rook_down = rook_down_board_set[rook_index];

                    rook_right_moves = rook_right & pieces_occupied;
                    rook_left_moves = rook_left & pieces_occupied;
                    rook_up_moves = rook_up & pieces_occupied;
                    rook_down_moves = rook_down & pieces_occupied;

                    rook_right_moves = (rook_right_moves << 1) | (rook_right_moves << 2) | (rook_right_moves << 3) | (rook_right_moves << 4) | (rook_right_moves << 5) | (rook_right_moves << 6);
                    rook_left_moves = (rook_left_moves >> 1) | (rook_left_moves >> 2) | (rook_left_moves >> 3) | (rook_left_moves >> 4) | (rook_left_moves >> 5) | (rook_left_moves >> 6);
                    rook_up_moves = (rook_up_moves << 8) | (rook_up_moves << 16) | (rook_up_moves << 24) | (rook_up_moves << 32) | (rook_up_moves << 40) | (rook_up_moves << 48);
                    rook_down_moves = (rook_down_moves >> 8) | (rook_down_moves >> 16) | (rook_down_moves >> 24) | (rook_down_moves >> 32) | (rook_down_moves >> 40) | (rook_down_moves >> 48);

                    rook_right_moves = rook_right_moves & rook_right;
                    rook_left_moves = rook_left_moves & rook_left;
                    rook_up_moves = rook_up_moves & rook_up;
                    rook_down_moves = rook_down_moves & rook_down;


                    rook_right_moves = (rook_right_moves ^ rook_right) & (empty | enemy_occupied);
                    rook_left_moves = (rook_left_moves ^ rook_left) & (empty | enemy_occupied);
                    rook_up_moves = (rook_up_moves ^ rook_up) & (empty | enemy_occupied);
                    rook_down_moves = (rook_down_moves ^ rook_down) & (empty | enemy_occupied);

                    rook_moves = rook_right_moves | rook_left_moves | rook_up_moves | rook_down_moves;
                    rook_cap_moves = rook_moves & enemy_occupied;
                    //Console.WriteLine("rook index :" + rook_index);
                    //Console.WriteLine("rook : " + Convert.ToString((long)rook_moves, 2));
                    if (enemy_occupied == white_occupied)
                    {
                        for (int j = 0; j < 64; j++)
                        {
                            if (((rook_moves >> j) & 1) == 1)
                            {
                                if (((white_pawns >> j) & 1) == 1)
                                    r_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Rook, PieceType.Pawn, false)); //from rank, file - to rank, file
                                else if (((white_knights >> j) & 1) == 1)
                                    r_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Rook, PieceType.Knight, false));
                                else if (((white_bishops >> j) & 1) == 1)
                                    r_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Rook, PieceType.Bishop, false));
                                else if (((white_rooks >> j) & 1) == 1)
                                    r_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Rook, PieceType.Rook, false));
                                else if (((white_queens >> j) & 1) == 1)
                                    r_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Rook, PieceType.Queen, false));
                                else if (((white_king >> j) & 1) == 1)
                                    r_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Rook, PieceType.King, false));
                                else
                                    r_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Rook, false));
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 64; j++)
                        {
                            if (((rook_moves >> j) & 1) == 1)
                            {
                                if (((black_pawns >> j) & 1) == 1)
                                    r_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Rook, PieceType.Pawn, false)); //from rank, file - to rank, file
                                else if (((black_knights >> j) & 1) == 1)
                                    r_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Rook, PieceType.Knight, false));
                                else if (((black_bishops >> j) & 1) == 1)
                                    r_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Rook, PieceType.Bishop, false));
                                else if (((black_rooks >> j) & 1) == 1)
                                    r_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Rook, PieceType.Rook, false));
                                else if (((black_queens >> j) & 1) == 1)
                                    r_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Rook, PieceType.Queen, false));
                                else if (((black_king >> j) & 1) == 1)
                                    r_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Rook, PieceType.King, false));
                                else
                                    r_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Rook, false));
                            }
                        }
                    }
                }

            }
            return r_move_list;
        }

        public static ArrayList PossibleBishop(ulong Bishop, ulong enemy_occupied)
        {
            b_move_list.Clear();
            int bishop_index;
            ulong bishop_45, bishop_45_moves;
            ulong bishop_135, bishop_135_moves;
            ulong bishop_225, bishop_225_moves;
            ulong bishop_315, bishop_315_moves;
            ulong bishop_moves, bishop_cap_moves;

            for (int i = 0; i < 64; i++)
            {
                if (((Bishop >> i) & 1) == 1)
                {
                    bishop_index = i;
                    bishop_45 = bishop_45_board_set[bishop_index];
                    bishop_135 = bishop_135_board_set[bishop_index];
                    bishop_225 = bishop_225_board_set[bishop_index];
                    bishop_315 = bishop_315_board_set[bishop_index];

                    bishop_45_moves = bishop_45 & pieces_occupied;
                    bishop_135_moves = bishop_135 & pieces_occupied;
                    bishop_225_moves = bishop_225 & pieces_occupied;
                    bishop_315_moves = bishop_315 & pieces_occupied;

                    bishop_45_moves = (bishop_45_moves << 9) | (bishop_45_moves << 18) | (bishop_45_moves << 27) | (bishop_45_moves << 36) | (bishop_45_moves << 45) | (bishop_45_moves << 54);
                    bishop_135_moves = (bishop_135_moves << 7) | (bishop_135_moves << 14) | (bishop_135_moves << 21) | (bishop_135_moves << 28) | (bishop_135_moves << 35) | (bishop_135_moves << 42);
                    bishop_225_moves = (bishop_225_moves >> 9) | (bishop_225_moves >> 18) | (bishop_225_moves >> 27) | (bishop_225_moves >> 36) | (bishop_225_moves >> 45) | (bishop_225_moves >> 54);
                    bishop_315_moves = (bishop_315_moves >> 7) | (bishop_315_moves >> 14) | (bishop_315_moves >> 21) | (bishop_315_moves >> 28) | (bishop_315_moves >> 35) | (bishop_315_moves >> 42);

                    bishop_45_moves = bishop_45_moves & bishop_45;
                    bishop_135_moves = bishop_135_moves & bishop_135;
                    bishop_225_moves = bishop_225_moves & bishop_225;
                    bishop_315_moves = bishop_315_moves & bishop_315;

                    bishop_45_moves = (bishop_45_moves ^ bishop_45) & (empty | enemy_occupied);
                    bishop_135_moves = (bishop_135_moves ^ bishop_135) & (empty | enemy_occupied);
                    bishop_225_moves = (bishop_225_moves ^ bishop_225) & (empty | enemy_occupied);
                    bishop_315_moves = (bishop_315_moves ^ bishop_315) & (empty | enemy_occupied);

                    bishop_moves = bishop_45_moves | bishop_135_moves | bishop_225_moves | bishop_315_moves;
                    bishop_cap_moves = bishop_moves & enemy_occupied;

                    //Console.WriteLine("bishop index :" + bishop_index);
                    //Console.WriteLine("bishop : " + Convert.ToString((long)bishop_moves, 2));
                    if (enemy_occupied == white_occupied)
                    {
                        for (int j = 0; j < 64; j++)
                        {
                            if (((bishop_moves >> j) & 1) == 1)
                            {
                                if (((white_pawns >> j) & 1) == 1)
                                    b_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Bishop, PieceType.Pawn, false)); //from rank, file - to rank, file
                                else if (((white_knights >> j) & 1) == 1)
                                    b_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Bishop, PieceType.Knight, false));
                                else if (((white_bishops >> j) & 1) == 1)
                                    b_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Bishop, PieceType.Bishop, false));
                                else if (((white_rooks >> j) & 1) == 1)
                                    b_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Bishop, PieceType.Rook, false));
                                else if (((white_queens >> j) & 1) == 1)
                                    b_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Bishop, PieceType.Queen, false));
                                else if (((white_king >> j) & 1) == 1)
                                    b_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Bishop, PieceType.King, false));
                                else
                                    b_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Bishop, false));
                                //b_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8)));  //from rank, file - to rank, file
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 64; j++)
                        {
                            if (((bishop_moves >> j) & 1) == 1)
                            {
                                if (((black_pawns >> j) & 1) == 1)
                                    b_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Bishop, PieceType.Pawn, false)); //from rank, file - to rank, file
                                else if (((black_knights >> j) & 1) == 1)
                                    b_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Bishop, PieceType.Knight, false));
                                else if (((black_bishops >> j) & 1) == 1)
                                    b_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Bishop, PieceType.Bishop, false));
                                else if (((black_rooks >> j) & 1) == 1)
                                    b_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Bishop, PieceType.Rook, false));
                                else if (((black_queens >> j) & 1) == 1)
                                    b_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Bishop, PieceType.Queen, false));
                                else if (((black_king >> j) & 1) == 1)
                                    b_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Bishop, PieceType.King, false));
                                else
                                    b_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Bishop, false));
                                //b_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8)));  //from rank, file - to rank, file
                            }
                        }
                    }

                }
            }


            return b_move_list;

        }

        public static ArrayList PossibleKnight(ulong knight, ulong enemy_occupied)
        {
            n_move_list.Clear();
            int knight_index;
            ulong knight_bitboard, knight_moves;
            for (int i = 0; i < 64; i++)
            {
                if (((knight >> i) & 1) == 1)
                {
                    knight_index = i;
                    knight_bitboard = knight_board_set[i];
                    knight_moves = knight_board_set[i] & (empty | enemy_occupied);
                    //Console.WriteLine("knight index :" + knight_index);
                    //Console.WriteLine("knight : " + Convert.ToString((long)knight_moves, 2));
                    if (enemy_occupied == white_occupied)
                    {
                        for (int j = 0; j < 64; j++)
                        {
                            if (((knight_moves >> j) & 1) == 1)
                            {
                                if (((white_pawns >> j) & 1) == 1)
                                    n_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Knight, PieceType.Pawn, false)); //from rank, file - to rank, file
                                else if (((white_knights >> j) & 1) == 1)
                                    n_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Knight, PieceType.Knight, false));
                                else if (((white_bishops >> j) & 1) == 1)
                                    n_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Knight, PieceType.Bishop, false));
                                else if (((white_rooks >> j) & 1) == 1)
                                    n_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Knight, PieceType.Rook, false));
                                else if (((white_queens >> j) & 1) == 1)
                                    n_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Knight, PieceType.Queen, false));
                                else if (((white_king >> j) & 1) == 1)
                                    n_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Knight, PieceType.King, false));
                                else
                                    n_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Knight, false));
                                //n_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8)));  //from rank, file - to rank, file
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 64; j++)
                        {
                            if (((knight_moves >> j) & 1) == 1)
                            {
                                if (((black_pawns >> j) & 1) == 1)
                                    n_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Knight, PieceType.Pawn, false)); //from rank, file - to rank, file
                                else if (((black_knights >> j) & 1) == 1)
                                    n_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Knight, PieceType.Knight, false));
                                else if (((black_bishops >> j) & 1) == 1)
                                    n_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Knight, PieceType.Bishop, false));
                                else if (((black_rooks >> j) & 1) == 1)
                                    n_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Knight, PieceType.Rook, false));
                                else if (((black_queens >> j) & 1) == 1)
                                    n_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Knight, PieceType.Queen, false));
                                else if (((black_king >> j) & 1) == 1)
                                    n_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Knight, PieceType.King, false));
                                else
                                    n_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Knight, false));
                                //n_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8)));  //from rank, file - to rank, file
                            }
                        }
                    }
                }

            }

            return n_move_list;
        }

        public static ArrayList PossibleQueen(ulong queen, ulong enemy_occupied)
        {
            q_move_list.Clear();
            int queen_index;
            ulong queen_right, queen_right_moves;
            ulong queen_left, queen_left_moves;
            ulong queen_up, queen_up_moves;
            ulong queen_down, queen_down_moves;
            ulong queen_45, queen_45_moves;
            ulong queen_135, queen_135_moves;
            ulong queen_225, queen_225_moves;
            ulong queen_315, queen_315_moves;
            ulong queen_moves, queen_cap_moves;

            for (int i = 0; i < 64; i++)
            {
                if (((queen >> i) & 1) == 1)
                {
                    queen_index = i;
                    queen_45 = bishop_45_board_set[queen_index];
                    queen_135 = bishop_135_board_set[queen_index];
                    queen_225 = bishop_225_board_set[queen_index];
                    queen_315 = bishop_315_board_set[queen_index];
                    queen_right = rook_right_board_set[queen_index];  //pesudo move on 4 dimentions
                    queen_left = rook_left_board_set[queen_index];
                    queen_up = rook_up_board_set[queen_index];
                    queen_down = rook_down_board_set[queen_index];

                    queen_right_moves = queen_right & pieces_occupied;
                    queen_left_moves = queen_left & pieces_occupied;
                    queen_up_moves = queen_up & pieces_occupied;
                    queen_down_moves = queen_down & pieces_occupied;
                    queen_45_moves = queen_45 & pieces_occupied;
                    queen_135_moves = queen_135 & pieces_occupied;
                    queen_225_moves = queen_225 & pieces_occupied;
                    queen_315_moves = queen_315 & pieces_occupied;

                    queen_right_moves = (queen_right_moves << 1) | (queen_right_moves << 2) | (queen_right_moves << 3) | (queen_right_moves << 4) | (queen_right_moves << 5) | (queen_right_moves << 6);
                    queen_left_moves = (queen_left_moves >> 1) | (queen_left_moves >> 2) | (queen_left_moves >> 3) | (queen_left_moves >> 4) | (queen_left_moves >> 5) | (queen_left_moves >> 6);
                    queen_up_moves = (queen_up_moves << 8) | (queen_up_moves << 16) | (queen_up_moves << 24) | (queen_up_moves << 32) | (queen_up_moves << 40) | (queen_up_moves << 48);
                    queen_down_moves = (queen_down_moves >> 8) | (queen_down_moves >> 16) | (queen_down_moves >> 24) | (queen_down_moves >> 32) | (queen_down_moves >> 40) | (queen_down_moves >> 48);
                    queen_45_moves = (queen_45_moves << 9) | (queen_45_moves << 18) | (queen_45_moves << 27) | (queen_45_moves << 36) | (queen_45_moves << 45) | (queen_45_moves << 54);
                    queen_135_moves = (queen_135_moves << 7) | (queen_135_moves << 14) | (queen_135_moves << 21) | (queen_135_moves << 28) | (queen_135_moves << 35) | (queen_135_moves << 42);
                    queen_225_moves = (queen_225_moves >> 9) | (queen_225_moves >> 18) | (queen_225_moves >> 27) | (queen_225_moves >> 36) | (queen_225_moves >> 45) | (queen_225_moves >> 54);
                    queen_315_moves = (queen_315_moves >> 7) | (queen_315_moves >> 14) | (queen_315_moves >> 21) | (queen_315_moves >> 28) | (queen_315_moves >> 35) | (queen_315_moves >> 42);


                    queen_right_moves = queen_right_moves & queen_right;
                    queen_left_moves = queen_left_moves & queen_left;
                    queen_up_moves = queen_up_moves & queen_up;
                    queen_down_moves = queen_down_moves & queen_down;
                    queen_45_moves = queen_45_moves & queen_45;
                    queen_135_moves = queen_135_moves & queen_135;
                    queen_225_moves = queen_225_moves & queen_225;
                    queen_315_moves = queen_315_moves & queen_315;

                    queen_right_moves = (queen_right_moves ^ queen_right) & (empty | enemy_occupied);
                    queen_left_moves = (queen_left_moves ^ queen_left) & (empty | enemy_occupied);
                    queen_up_moves = (queen_up_moves ^ queen_up) & (empty | enemy_occupied);
                    queen_down_moves = (queen_down_moves ^ queen_down) & (empty | enemy_occupied);
                    queen_45_moves = (queen_45_moves ^ queen_45) & (empty | enemy_occupied);
                    queen_135_moves = (queen_135_moves ^ queen_135) & (empty | enemy_occupied);
                    queen_225_moves = (queen_225_moves ^ queen_225) & (empty | enemy_occupied);
                    queen_315_moves = (queen_315_moves ^ queen_315) & (empty | enemy_occupied);

                    queen_moves = queen_right_moves | queen_left_moves | queen_up_moves | queen_down_moves | queen_45_moves | queen_135_moves | queen_225_moves | queen_315_moves;
                    queen_cap_moves = queen_moves & enemy_occupied;

                    //Console.WriteLine("queen index :" + queen_index);
                    //Console.WriteLine("queen : " + Convert.ToString((long)queen_moves, 2));
                    if (enemy_occupied == white_occupied)
                    {
                        for (int j = 0; j < 64; j++)
                        {
                            if (((queen_moves >> j) & 1) == 1)
                            {
                                if (((white_pawns >> j) & 1) == 1)
                                    q_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Queen, PieceType.Pawn, false)); //from rank, file - to rank, file
                                else if (((white_knights >> j) & 1) == 1)
                                    q_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Queen, PieceType.Knight, false));
                                else if (((white_bishops >> j) & 1) == 1)
                                    q_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Queen, PieceType.Bishop, false));
                                else if (((white_rooks >> j) & 1) == 1)
                                    q_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Queen, PieceType.Rook, false));
                                else if (((white_queens >> j) & 1) == 1)
                                    q_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Queen, PieceType.Queen, false));
                                else if (((white_king >> j) & 1) == 1)
                                    q_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Queen, PieceType.King, false));
                                else
                                    q_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Queen, false));
                                //q_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8)));  //from rank, file - to rank, file
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 64; j++)
                        {
                            if (((queen_moves >> j) & 1) == 1)
                            {
                                if (((black_pawns >> j) & 1) == 1)
                                    q_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Queen, PieceType.Pawn, false)); //from rank, file - to rank, file
                                else if (((black_knights >> j) & 1) == 1)
                                    q_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Queen, PieceType.Knight, false));
                                else if (((black_bishops >> j) & 1) == 1)
                                    q_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Queen, PieceType.Bishop, false));
                                else if (((black_rooks >> j) & 1) == 1)
                                    q_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Queen, PieceType.Rook, false));
                                else if (((black_queens >> j) & 1) == 1)
                                    q_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Queen, PieceType.Queen, false));
                                else if (((black_king >> j) & 1) == 1)
                                    q_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Queen, PieceType.King, false));
                                else
                                    q_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.Queen, false));
                                //q_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8)));  //from rank, file - to rank, file
                            }
                        }
                    }
                }
            }
            return q_move_list;
        }
     
        public static ArrayList PossibleKing(ulong BK, ulong enemy_occupied)
        {
            k_move_list.Clear();
            int king_index;
            ulong king_bitboard, king_moves;
            for (int i = 0; i < 64; i++)
            {
                if (((BK >> i) & 1) == 1)
                {
                    king_index = i;
                    king_bitboard = king_board_set[i];
                    king_moves = king_board_set[i] & (empty | enemy_occupied);


                    //Console.WriteLine("king index :" + king_index);
                    //Console.WriteLine("king : " + Convert.ToString((long)king_moves, 2));

                    if (enemy_occupied == white_occupied)
                    {
                        for (int j = 0; j < 64; j++)
                        {
                            if (((king_moves >> j) & 1) == 1)
                            {
                                if (((white_pawns >> j) & 1) == 1)
                                    k_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.King, PieceType.Pawn, false)); //from rank, file - to rank, file
                                else if (((white_knights >> j) & 1) == 1)
                                    k_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.King, PieceType.Knight, false));
                                else if (((white_bishops >> j) & 1) == 1)
                                    k_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.King, PieceType.Bishop, false));
                                else if (((white_rooks >> j) & 1) == 1)
                                    k_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.King, PieceType.Rook, false));
                                else if (((white_queens >> j) & 1) == 1)
                                    k_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.King, PieceType.Queen, false));
                                else if (((white_king >> j) & 1) == 1)
                                    k_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.King, PieceType.King, false));
                                else
                                    k_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.King, false));
                                //k_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8)));  //from rank, file - to rank, file
                            }
                        }


                    }
                    else
                    {
                        for (int j = 0; j < 64; j++)
                        {
                            if (((king_moves >> j) & 1) == 1)
                            {
                                if (((black_pawns >> j) & 1) == 1)
                                    k_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.King, PieceType.Pawn, false)); //from rank, file - to rank, file
                                else if (((black_knights >> j) & 1) == 1)
                                    k_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.King, PieceType.Knight, false));
                                else if (((black_bishops >> j) & 1) == 1)
                                    k_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.King, PieceType.Bishop, false));
                                else if (((black_rooks >> j) & 1) == 1)
                                    k_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.King, PieceType.Rook, false));
                                else if (((black_queens >> j) & 1) == 1)
                                    k_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.King, PieceType.Queen, false));
                                else if (((black_king >> j) & 1) == 1)
                                    k_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.King, PieceType.King, false));
                                else
                                    k_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8), PieceType.King, false));
                                //k_move_list.Add(new Move((i / 8), (i % 8), (j / 8), (j % 8)));  //from rank, file - to rank, file
                            }
                        }
                    }
                    break;
                }

            }

            return k_move_list;
        }

        public static ulong PossiblePawnAttackMachine(ulong machine_pawns)
        {

            ulong bp_promote_right_cap_moves = (machine_pawns >> 7) & ~file0;
            ulong bp_promote_left_cap_moves = (machine_pawns >> 9) & ~file7;

            return (bp_promote_right_cap_moves | bp_promote_left_cap_moves);
        }

        public static ulong PossiblePawnAttackPlayer(ulong player_pawns)
        {
            ulong bp_promote_right_cap_moves = (player_pawns << 9) & ~file0;
            ulong bp_promote_left_cap_moves = (player_pawns << 7) & ~file7;

            return (bp_promote_right_cap_moves | bp_promote_left_cap_moves);
        }

        public static ulong PossibleKnightAttack(ulong knight, ulong enemy_occupied)
        {
            ulong knight_moves = 0L, knight_bitboard = 0L, all_moves = 0L;
            int knight_index;

            for (int i = 0; i < 64; i++)
            {
                if (((knight >> i) & 1) == 1)
                {
                    knight_index = i;
                    knight_bitboard = knight_board_set[i];
                    knight_moves = knight_board_set[i] & (empty | enemy_occupied);
                }
                all_moves |= knight_moves;
            }

            return all_moves;
        }

        public static ulong PossibleRookAttack(ulong rook, ulong enemy_occupied)
        {
            int rook_index;
            ulong rook_right, rook_right_moves;
            ulong rook_left, rook_left_moves;
            ulong rook_up, rook_up_moves;
            ulong rook_down, rook_down_moves;
            ulong rook_moves = 0L, rook_cap_moves;
            ulong all_moves = 0L;


            for (int i = 0; i < 64; i++)
            {
                if (((rook >> i) & 1) == 1)
                {
                    rook_index = i;
                    rook_right = rook_right_board_set[rook_index];  //pesudo move on 4 dimentions
                    rook_left = rook_left_board_set[rook_index];
                    rook_up = rook_up_board_set[rook_index];
                    rook_down = rook_down_board_set[rook_index];

                    rook_right_moves = rook_right & pieces_occupied;
                    rook_left_moves = rook_left & pieces_occupied;
                    rook_up_moves = rook_up & pieces_occupied;
                    rook_down_moves = rook_down & pieces_occupied;

                    rook_right_moves = (rook_right_moves << 1) | (rook_right_moves << 2) | (rook_right_moves << 3) | (rook_right_moves << 4) | (rook_right_moves << 5) | (rook_right_moves << 6);
                    rook_left_moves = (rook_left_moves >> 1) | (rook_left_moves >> 2) | (rook_left_moves >> 3) | (rook_left_moves >> 4) | (rook_left_moves >> 5) | (rook_left_moves >> 6);
                    rook_up_moves = (rook_up_moves << 8) | (rook_up_moves << 16) | (rook_up_moves << 24) | (rook_up_moves << 32) | (rook_up_moves << 40) | (rook_up_moves << 48);
                    rook_down_moves = (rook_down_moves >> 8) | (rook_down_moves >> 16) | (rook_down_moves >> 24) | (rook_down_moves >> 32) | (rook_down_moves >> 40) | (rook_down_moves >> 48);

                    rook_right_moves = rook_right_moves & rook_right;
                    rook_left_moves = rook_left_moves & rook_left;
                    rook_up_moves = rook_up_moves & rook_up;
                    rook_down_moves = rook_down_moves & rook_down;


                    rook_right_moves = (rook_right_moves ^ rook_right) & (empty | enemy_occupied);
                    rook_left_moves = (rook_left_moves ^ rook_left) & (empty | enemy_occupied);
                    rook_up_moves = (rook_up_moves ^ rook_up) & (empty | enemy_occupied);
                    rook_down_moves = (rook_down_moves ^ rook_down) & (empty | enemy_occupied);

                    rook_moves = rook_right_moves | rook_left_moves | rook_up_moves | rook_down_moves;
                }
                all_moves |= rook_moves;

            }
            return all_moves;
        }

        public static ulong PossibleBishopAttack(ulong bishop, ulong enemy_occupied)
        {
            int bishop_index;
            ulong bishop_45, bishop_45_moves;
            ulong bishop_135, bishop_135_moves;
            ulong bishop_225, bishop_225_moves;
            ulong bishop_315, bishop_315_moves;
            ulong bishop_moves = 0L, bishop_cap_moves;
            ulong all_moves = 0L;
            for (int i = 0; i < 64; i++)
            {
                if (((bishop >> i) & 1) == 1)
                {
                    bishop_index = i;
                    bishop_45 = bishop_45_board_set[bishop_index];
                    bishop_135 = bishop_135_board_set[bishop_index];
                    bishop_225 = bishop_225_board_set[bishop_index];
                    bishop_315 = bishop_315_board_set[bishop_index];

                    bishop_45_moves = bishop_45 & pieces_occupied;
                    bishop_135_moves = bishop_135 & pieces_occupied;
                    bishop_225_moves = bishop_225 & pieces_occupied;
                    bishop_315_moves = bishop_315 & pieces_occupied;

                    bishop_45_moves = (bishop_45_moves << 9) | (bishop_45_moves << 18) | (bishop_45_moves << 27) | (bishop_45_moves << 36) | (bishop_45_moves << 45) | (bishop_45_moves << 54);
                    bishop_135_moves = (bishop_135_moves << 7) | (bishop_135_moves << 14) | (bishop_135_moves << 21) | (bishop_135_moves << 28) | (bishop_135_moves << 35) | (bishop_135_moves << 42);
                    bishop_225_moves = (bishop_225_moves >> 9) | (bishop_225_moves >> 18) | (bishop_225_moves >> 27) | (bishop_225_moves >> 36) | (bishop_225_moves >> 45) | (bishop_225_moves >> 54);
                    bishop_315_moves = (bishop_315_moves >> 7) | (bishop_315_moves >> 14) | (bishop_315_moves >> 21) | (bishop_315_moves >> 28) | (bishop_315_moves >> 35) | (bishop_315_moves >> 42);

                    bishop_45_moves = bishop_45_moves & bishop_45;
                    bishop_135_moves = bishop_135_moves & bishop_135;
                    bishop_225_moves = bishop_225_moves & bishop_225;
                    bishop_315_moves = bishop_315_moves & bishop_315;

                    bishop_45_moves = (bishop_45_moves ^ bishop_45) & (empty | enemy_occupied);
                    bishop_135_moves = (bishop_135_moves ^ bishop_135) & (empty | enemy_occupied);
                    bishop_225_moves = (bishop_225_moves ^ bishop_225) & (empty | enemy_occupied);
                    bishop_315_moves = (bishop_315_moves ^ bishop_315) & (empty | enemy_occupied);

                    bishop_moves = bishop_45_moves | bishop_135_moves | bishop_225_moves | bishop_315_moves;
                    bishop_cap_moves = bishop_moves & enemy_occupied;
                }
                all_moves |= bishop_moves;
            }
            return all_moves;
        }

        public static ulong PossibleQueenAttack(ulong queen, ulong enemy_occupied)
        {
            int queen_index;
            ulong queen_right, queen_right_moves;
            ulong queen_left, queen_left_moves;
            ulong queen_up, queen_up_moves;
            ulong queen_down, queen_down_moves;
            ulong queen_45, queen_45_moves;
            ulong queen_135, queen_135_moves;
            ulong queen_225, queen_225_moves;
            ulong queen_315, queen_315_moves;
            ulong queen_moves = 0L, queen_cap_moves;
            ulong all_moves = 0L;
            for (int i = 0; i < 64; i++)
            {
                if (((queen >> i) & 1) == 1)
                {
                    queen_index = i;
                    queen_45 = bishop_45_board_set[queen_index];
                    queen_135 = bishop_135_board_set[queen_index];
                    queen_225 = bishop_225_board_set[queen_index];
                    queen_315 = bishop_315_board_set[queen_index];
                    queen_right = rook_right_board_set[queen_index];  //pesudo move on 4 dimentions
                    queen_left = rook_left_board_set[queen_index];
                    queen_up = rook_up_board_set[queen_index];
                    queen_down = rook_down_board_set[queen_index];

                    queen_right_moves = queen_right & pieces_occupied;
                    queen_left_moves = queen_left & pieces_occupied;
                    queen_up_moves = queen_up & pieces_occupied;
                    queen_down_moves = queen_down & pieces_occupied;
                    queen_45_moves = queen_45 & pieces_occupied;
                    queen_135_moves = queen_135 & pieces_occupied;
                    queen_225_moves = queen_225 & pieces_occupied;
                    queen_315_moves = queen_315 & pieces_occupied;

                    queen_right_moves = (queen_right_moves << 1) | (queen_right_moves << 2) | (queen_right_moves << 3) | (queen_right_moves << 4) | (queen_right_moves << 5) | (queen_right_moves << 6);
                    queen_left_moves = (queen_left_moves >> 1) | (queen_left_moves >> 2) | (queen_left_moves >> 3) | (queen_left_moves >> 4) | (queen_left_moves >> 5) | (queen_left_moves >> 6);
                    queen_up_moves = (queen_up_moves << 8) | (queen_up_moves << 16) | (queen_up_moves << 24) | (queen_up_moves << 32) | (queen_up_moves << 40) | (queen_up_moves << 48);
                    queen_down_moves = (queen_down_moves >> 8) | (queen_down_moves >> 16) | (queen_down_moves >> 24) | (queen_down_moves >> 32) | (queen_down_moves >> 40) | (queen_down_moves >> 48);
                    queen_45_moves = (queen_45_moves << 9) | (queen_45_moves << 18) | (queen_45_moves << 27) | (queen_45_moves << 36) | (queen_45_moves << 45) | (queen_45_moves << 54);
                    queen_135_moves = (queen_135_moves << 7) | (queen_135_moves << 14) | (queen_135_moves << 21) | (queen_135_moves << 28) | (queen_135_moves << 35) | (queen_135_moves << 42);
                    queen_225_moves = (queen_225_moves >> 9) | (queen_225_moves >> 18) | (queen_225_moves >> 27) | (queen_225_moves >> 36) | (queen_225_moves >> 45) | (queen_225_moves >> 54);
                    queen_315_moves = (queen_315_moves >> 7) | (queen_315_moves >> 14) | (queen_315_moves >> 21) | (queen_315_moves >> 28) | (queen_315_moves >> 35) | (queen_315_moves >> 42);


                    queen_right_moves = queen_right_moves & queen_right;
                    queen_left_moves = queen_left_moves & queen_left;
                    queen_up_moves = queen_up_moves & queen_up;
                    queen_down_moves = queen_down_moves & queen_down;
                    queen_45_moves = queen_45_moves & queen_45;
                    queen_135_moves = queen_135_moves & queen_135;
                    queen_225_moves = queen_225_moves & queen_225;
                    queen_315_moves = queen_315_moves & queen_315;

                    queen_right_moves = (queen_right_moves ^ queen_right) & (empty | enemy_occupied);
                    queen_left_moves = (queen_left_moves ^ queen_left) & (empty | enemy_occupied);
                    queen_up_moves = (queen_up_moves ^ queen_up) & (empty | enemy_occupied);
                    queen_down_moves = (queen_down_moves ^ queen_down) & (empty | enemy_occupied);
                    queen_45_moves = (queen_45_moves ^ queen_45) & (empty | enemy_occupied);
                    queen_135_moves = (queen_135_moves ^ queen_135) & (empty | enemy_occupied);
                    queen_225_moves = (queen_225_moves ^ queen_225) & (empty | enemy_occupied);
                    queen_315_moves = (queen_315_moves ^ queen_315) & (empty | enemy_occupied);

                    queen_moves = queen_right_moves | queen_left_moves | queen_up_moves | queen_down_moves | queen_45_moves | queen_135_moves | queen_225_moves | queen_315_moves;
                    queen_cap_moves = queen_moves & enemy_occupied;
                }
                all_moves |= queen_moves;
            }
            return all_moves;
        }


        public static ulong PossibleKingAttack(ulong king, ulong enemy_occupied)
        {
            int king_index;
            ulong king_bitboard, king_moves = 0L;
            for (int i = 0; i < 64; i++)
            {
                if (((king >> i) & 1) == 1)
                {
                    king_index = i;
                    king_bitboard = king_board_set[i];
                    king_moves = king_board_set[i] & (empty | enemy_occupied);
                    break;
                }
            }
            return king_moves;
        }

        public static ArrayList PossibleCastlingPlayer()
        {
            player_castling_move_list.Clear();
            if(PKC && PQC)
            {
                ulong lowest_rank_occupied = pieces_occupied & rank0;
                ulong ply_white_safe_zone = 0x000000000000007c;
                ulong ply_black_safe_zone = 0x000000000000003e;
                ulong unsafe_lowest_rank = UnsafePlayerKingMove();
                //player queen side castling
                if (player_color && ((lowest_rank_occupied >> 1) & 1) != 1 && ((lowest_rank_occupied >> 2) & 1) != 1 && ((lowest_rank_occupied >> 3) & 1) != 1 && (white_rooks & 1) ==1)
                {
                    if((unsafe_lowest_rank & ply_white_safe_zone)==0)
                        machine_castling_move_list.Add(new Move(0, 4, false, false, false, true));
                }
                //player king side castlig
                else if (player_color && ((lowest_rank_occupied >> 5) & 1) != 1 && ((lowest_rank_occupied >> 6) & 1) != 1 && ((white_rooks >> 7) & 1) == 1)
                {
                    if ((unsafe_lowest_rank & ply_white_safe_zone) == 0)
                        machine_castling_move_list.Add(new Move(0, 4, false, false, true, false));
                }

                //player king side castling
                else if (!player_color && ((lowest_rank_occupied >> 1) & 1) != 1 && ((lowest_rank_occupied >> 2) & 1) != 1 && (black_rooks & 1)==1)
                {
                    if ((unsafe_lowest_rank & ply_black_safe_zone) == 0)
                        machine_castling_move_list.Add(new Move(0, 3, false, false, true, false));
                }

                //player queen side castling
                else if (!player_color && ((lowest_rank_occupied >> 4) & 1) != 1 && ((lowest_rank_occupied >> 5) & 1) != 1 && ((lowest_rank_occupied >> 6) & 1) != 1 && ((black_rooks >> 7) & 1) == 1)
                {
                    if ((unsafe_lowest_rank & ply_black_safe_zone) == 0)
                        machine_castling_move_list.Add(new Move(0, 3, false, false, false, true));
                }
            }
            return player_castling_move_list;
        }

        public static ArrayList PossibleCastlingMachine()
        {
            machine_castling_move_list.Clear();
            if (MKC && MQC)
            {
                ulong mac_white_safe_zone = 0x3e00000000000000;
                ulong mac_black_safe_zone = 0x7c00000000000000;
                ulong highest_rank_occupied = pieces_occupied & rank7;
                ulong unsafe_highest_rank = UnsafeMachineKingMove();
                
                if (player_color && ((highest_rank_occupied >> 57) & 1) != 1 && ((highest_rank_occupied >> 58) & 1) != 1 && ((highest_rank_occupied >> 59) & 1) != 1 && ((black_rooks >>56)&1)==1)
                {
                    
                    if ((mac_black_safe_zone & unsafe_highest_rank) ==0)                
                        machine_castling_move_list.Add(new Move(7, 4, false, true, false, false));
                }
                else if (player_color && ((highest_rank_occupied >> 61) & 1) != 1 && ((highest_rank_occupied >> 62) & 1) != 1 && ((black_rooks >> 63) & 1) == 1)
                {
                    if ((mac_black_safe_zone & unsafe_highest_rank) == 0)
                        machine_castling_move_list.Add(new Move(7, 4, true, false, false, false));
                }
                else if (!player_color && ((highest_rank_occupied >> 57) & 1) != 1 && ((highest_rank_occupied >> 58) & 1) != 1 && ((white_rooks >> 56) & 1) == 1)
                {
                    if ((mac_white_safe_zone & unsafe_highest_rank) == 0)
                        machine_castling_move_list.Add(new Move(7, 3, true, false, false, false));
                }
                else if (!player_color && ((highest_rank_occupied >> 60) & 1) != 1 && ((highest_rank_occupied >> 61) & 1) != 1 && ((highest_rank_occupied >> 62) & 1) != 1 && ((white_rooks >> 63) & 1) == 1)
                {
                    if ((mac_white_safe_zone & unsafe_highest_rank) == 0)
                        machine_castling_move_list.Add(new Move(7, 3, false, true, false, false));
                }
            }
            return machine_castling_move_list;
        }

        public static bool LegalRegularMove(int x, int y, int new_x, int new_y, PieceType piece_type)
        {
            ulong white_occupied, black_occupied, enemy_occupied, my_pawns, my_knights, my_bishops, my_king, my_queens, my_rooks;
            white_occupied = (white_pawns | white_knights | white_bishops | white_king | white_queens | white_rooks);
            black_occupied = (black_pawns | black_knights | black_bishops | black_king | black_queens | black_rooks);
            if(player_color)
            {
                my_pawns = white_pawns; my_knights = white_knights; my_bishops = white_bishops; my_king = white_king; my_queens = white_queens; my_rooks = white_rooks;
                enemy_occupied = black_occupied;
            }
            else
            {
                my_pawns = black_pawns; my_knights = black_knights; my_bishops = black_bishops; my_king = black_king; my_queens = black_queens; my_rooks = black_rooks;
                enemy_occupied = white_occupied;
            }


            switch (piece_type)
            {
                case PieceType.Pawn:
                    return LegalSinglePawnMove(white_occupied, black_occupied,  x,  y,  new_x,  new_y);
                    
                case PieceType.Knight:
                    return LegalSingleKnightMove(my_knights, enemy_occupied, x, y, new_x, new_y);

                case PieceType.Queen:
                    return LegalSingleQueenMove(my_queens, enemy_occupied, x, y, new_x, new_y);

                case PieceType.King:
                    return LegalSingleKingMove(my_king, enemy_occupied, x, y, new_x, new_y);

                case PieceType.Bishop:
                    return LegalSingleBishopMove(my_bishops, enemy_occupied, x, y, new_x, new_y);

                case PieceType.Rook:
                    return LegalSingleRookMove(my_rooks, enemy_occupied, x, y, new_x, new_y);

            }
            return false;
        }

        public static bool LegalSinglePawnMove(ulong white_occupied, ulong black_occupied, int x, int y, int new_x, int new_y)
        {
            int pawn_index = (7 - y) * 8 + x;
            Console.WriteLine("index "+ pawn_index);
            //one step forward
            if ((((~(white_occupied | black_occupied)) >> (pawn_index+8)) & 1) == 1 && new_x==x && new_y == y-1)
            {
                Console.WriteLine("One step forward");
                return true;
            }

            else if((((~(white_occupied | black_occupied)) >> (pawn_index + 8)) & 1) == 1 && (((~(white_occupied | black_occupied)) >> (pawn_index + 16)) & 1) == 1 && y==6 && new_x==x && new_y==4)
            {
                Console.WriteLine("two step forward");
                return true;
            }
            if (player_color)
            {
                if (((black_occupied >> (pawn_index + 7)) & 1) == 1 && x == new_x + 1 && y == new_y + 1)  // not on left most file, but left capture
                {
                    Console.WriteLine("left capture");
                    return true;
                }
                if (((black_occupied >> (pawn_index + 9)) & 1) == 1 && x == new_x - 1 && y == new_y + 1)  // not on left most file, but left capture
                {
                    Console.WriteLine("right capture");
                    return true;
                }
            }
            else
            {
                if (((white_occupied >> (pawn_index + 7)) & 1) == 1 && x == new_x + 1 && y == new_y + 1)  // not on left most file, but left capture
                {
                    Console.WriteLine("left capture");
                    return true;
                }
                if (((white_occupied >> (pawn_index + 9)) & 1) == 1 && x == new_x - 1 && y == new_y + 1)  // not on left most file, but left capture
                {
                    Console.WriteLine("right capture");
                    return true;
                }
            }
            return false;

        }

        public static bool LegalSingleKnightMove(ulong white_knight, ulong black_occupied, int x, int y, int new_x, int new_y)
        {
            int knight_index = (7 - y) * 8 + x;
            int new_knight_index = (7 - new_y) * 8 + new_x;
            Move player_move = new Move((7 - y),x , (7 - new_y), new_x);
            //Console.WriteLine("index " + knight_index);

            if(((white_knight >> knight_index) & 1) == 1)
            {
                ArrayList pos_moves = PossibleKnight(white_knight, black_occupied);
                foreach(Move move in pos_moves)
                {
                    if (move.from_rank == (7 - y) && move.from_file == x && move.to_rank == (7 - new_y) && move.to_file == new_x)
                    {
                        return true;
                    }
                    
                }
                return false;
            }
            else
            {
                return false;
            }
                
        }

        public static bool LegalSingleQueenMove(ulong white_queen, ulong black_occupied, int x, int y, int new_x, int new_y)
        {
            int queen_index = (7 - y) * 8 + x;
            int new_queen_index = (7 - new_y) * 8 + new_x;
            Move player_move = new Move((7 - y), x, (7 - new_y), new_x);
            if (((white_queen >> queen_index) & 1) == 1)
            {
                ArrayList pos_moves = PossibleQueen(white_queen, black_occupied);
                foreach (Move move in pos_moves)
                {
                    if (move.from_rank == (7 - y) && move.from_file == x && move.to_rank == (7 - new_y) && move.to_file == new_x)
                    {
                        return true;
                    }

                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public static bool LegalSingleBishopMove(ulong white_bishop, ulong black_occupied, int x, int y, int new_x, int new_y)
        {
            int bishop_index = (7 - y) * 8 + x;
            int new_bishop_index = (7 - new_y) * 8 + new_x;
            Move player_move = new Move((7 - y), x, (7 - new_y), new_x);
            if (((white_bishop >> bishop_index) & 1) == 1)
            {
                ArrayList pos_moves = PossibleBishop(white_bishop, black_occupied);
                foreach (Move move in pos_moves)
                {
                    if (move.from_rank == (7 - y) && move.from_file == x && move.to_rank == (7 - new_y) && move.to_file == new_x)
                    {
                        return true;
                    }

                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public static bool LegalSingleRookMove(ulong white_rook, ulong black_occupied, int x, int y, int new_x, int new_y)
        {
            int rook_index = (7 - y) * 8 + x;
            int new_rook_index = (7 - new_y) * 8 + new_x;
            Move player_move = new Move((7 - y), x, (7 - new_y), new_x);
            if (((white_rook >> rook_index) & 1) == 1)
            {
                ArrayList pos_moves = PossibleRook(white_rook, black_occupied);
                foreach (Move move in pos_moves)
                {
                    if (move.from_rank == (7 - y) && move.from_file == x && move.to_rank == (7 - new_y) && move.to_file == new_x)
                    {
                        return true;
                    }

                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public static bool LegalSingleKingMove(ulong white_king, ulong black_occupied, int x, int y, int new_x, int new_y)
        {
            int king_index = (7 - y) * 8 + x;
            int new_king_index = (7 - new_y) * 8 + new_x;
            Move player_move = new Move((7 - y), x, (7 - new_y), new_x);
            //Console.WriteLine("index " + knight_index);

            if (((white_king >> king_index) & 1) == 1)
            {
                ArrayList pos_moves = PossibleKing(white_king, black_occupied);
                foreach (Move move in pos_moves)
                {
                    if (move.from_rank == (7 - y) && move.from_file == x && move.to_rank == (7 - new_y) && move.to_file == new_x)
                    {
                        return true;
                    }

                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public static bool LegalEnPassentPawnMove(int x, int y, int new_x, int new_y, PieceType moved_type)
        {
            if (moved_type == PieceType.Pawn && history_move!=null)
            {
                if (history_move.moved_type == PieceType.Pawn && history_move.to_file == history_move.from_file && history_move.from_rank == 6 && history_move.to_rank == 4 && (7 - y) == 4)
                {
                    if (history_move.to_file == new_x && history_move.to_rank == (7 - y) && Math.Abs(history_move.to_file - x) == 1 && history_move.to_rank == (6 - new_y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static ulong UnsafeMachineKingMove()
        {
            ulong _unsafe, _unsafe_highest_rank;
            ulong enemy_pawn, enemy_rook, enemy_knight, enemy_bishop, enemy_queen, enemy_king;
            ulong fridl_pawn, fridl_rook, fridl_knight, fridl_bishop, fridl_queen, fridl_king;
            ulong enemy_pieces, fridl_pieces;

            if (!player_color)
            {
                enemy_pawn= black_pawns; enemy_rook = black_rooks; enemy_knight = black_knights; enemy_bishop = black_bishops; enemy_queen = black_queens; enemy_king = black_king;
                fridl_pawn = white_pawns; fridl_rook = white_rooks; fridl_knight = white_knights; fridl_bishop = white_bishops; fridl_queen = white_queens; fridl_king = white_king;
            }
            else
            {
                enemy_pawn = white_pawns; enemy_rook = white_rooks; enemy_knight = white_knights; enemy_bishop = white_bishops; enemy_queen = white_queens; enemy_king = white_king;
                fridl_pawn = black_pawns; fridl_rook = black_rooks; fridl_knight = black_knights; fridl_bishop = black_bishops; fridl_queen = black_queens; fridl_king = black_king;
            }
            enemy_pieces = enemy_pawn | enemy_rook | enemy_knight | enemy_bishop | enemy_queen | enemy_king;
            fridl_pieces = fridl_pawn | fridl_rook | fridl_knight | fridl_bishop | fridl_queen | fridl_king;

            _unsafe = PossiblePawnAttackPlayer(enemy_pawn) | PossibleKnightAttack(enemy_knight, fridl_pieces) | PossibleRookAttack(enemy_rook, fridl_pieces) | PossibleBishopAttack(enemy_bishop, fridl_pieces) | PossibleQueenAttack(enemy_queen, fridl_pieces) | PossibleKingAttack(enemy_king, fridl_pieces);
            _unsafe_highest_rank = _unsafe & rank7;
            return _unsafe_highest_rank;
        }

        public static ulong UnsafePlayerKingMove()
        {
            ulong _unsafe, _unsafe_lowest_rank;
            ulong enemy_pawn, enemy_rook, enemy_knight, enemy_bishop, enemy_queen, enemy_king;
            ulong fridl_pawn, fridl_rook, fridl_knight, fridl_bishop, fridl_queen, fridl_king;
            ulong enemy_pieces, fridl_pieces;

            if (player_color)
            {
                enemy_pawn = black_pawns; enemy_rook = black_rooks; enemy_knight = black_knights; enemy_bishop = black_bishops; enemy_queen = black_queens; enemy_king = black_king;
                fridl_pawn = white_pawns; fridl_rook = white_rooks; fridl_knight = white_knights; fridl_bishop = white_bishops; fridl_queen = white_queens; fridl_king = white_king;
            }
            else
            {
                enemy_pawn = white_pawns; enemy_rook = white_rooks; enemy_knight = white_knights; enemy_bishop = white_bishops; enemy_queen = white_queens; enemy_king = white_king;
                fridl_pawn = black_pawns; fridl_rook = black_rooks; fridl_knight = black_knights; fridl_bishop = black_bishops; fridl_queen = black_queens; fridl_king = black_king;
            }
            enemy_pieces = enemy_pawn | enemy_rook | enemy_knight | enemy_bishop | enemy_queen | enemy_king;
            fridl_pieces = fridl_pawn | fridl_rook | fridl_knight | fridl_bishop | fridl_queen | fridl_king;

            _unsafe = PossiblePawnAttackMachine(enemy_pawn) | PossibleKnightAttack(enemy_knight, fridl_pieces) | PossibleRookAttack(enemy_rook, fridl_pieces) | PossibleBishopAttack(enemy_bishop, fridl_pieces) | PossibleQueenAttack(enemy_queen, fridl_pieces) | PossibleKingAttack(enemy_king, fridl_pieces);
            
            _unsafe_lowest_rank = _unsafe & rank0;

            return _unsafe_lowest_rank;
        }

        public static bool LegalCastlingMove(int x, int y, int new_x, int new_y, PieceType type)
        {

            int king_index = (7 - y) * 8 + x;
            int new_king_index = (7 - new_y) * 8 + new_x;
            //Console.WriteLine(" black is before check " + Convert.ToString((long)black_occupied, 2));  //!!!!!!!
            ulong _unsafe_lowest_rank = UnsafePlayerKingMove();
            ulong player_king_castle_zone, player_king, player_rooks;
   
            if (type != PieceType.King)
                return false;
            if (Math.Abs(king_index - new_king_index) != 3 && Math.Abs(king_index - new_king_index) != 4)
                return false;


            if (player_color)
            {
                player_king_castle_zone = 0x000000000000007c;
                player_king = white_king;
                player_rooks = white_rooks;


                if(Math.Abs(king_index-new_king_index)==3) // player plays white & king side castling
                {
                    if(((white_occupied >> (king_index+1)) & 1) == 1 || ((white_occupied >> (king_index + 2)) & 1) == 1)
                    {
                        return false;
                    }
                    if (!PKC)
                    {
                        return false;
                    }
                }
                if(Math.Abs(king_index - new_king_index) == 4) // player plays white & queen side castling
                {
                    if (((white_occupied >> (king_index - 1)) & 1) == 1 || ((white_occupied >> (king_index - 2)) & 1) == 1 && ((white_occupied >> (king_index - 3)) & 1) == 1)
                    {
                        return false;
                    }
                    if (!PQC)
                    {
                        return false;
                    }
                }
                if ((_unsafe_lowest_rank & player_king_castle_zone) != 0)
                    return false;

            }
            else
            {
                
                player_king_castle_zone = 0x000000000000003E;
                player_king = black_king;
                player_rooks = black_rooks;
                if (Math.Abs(king_index - new_king_index) == 3) // player plays black & king side castling
                {
                    
                    if (((black_occupied >> (king_index - 1)) & 1) == 1 || ((black_occupied >> (king_index - 2)) & 1) == 1)
                    {
                        return false;
                    }
                    if (!PKC)
                    {
                        return false;
                    }
                }
                if (Math.Abs(king_index - new_king_index) == 4) // player plays black & queen side castling
                {
                    if (((black_occupied >> (king_index + 1)) & 1) == 1 || ((black_occupied >> (king_index + 2)) & 1) == 1 && ((black_occupied >> (king_index + 3)) & 1) == 1)
                    {
                        return false;
                    }
                    if (!PQC)
                    {
                        return false;
                    }
                }
                if ((_unsafe_lowest_rank & player_king_castle_zone) != 0)
                    return false;

            }
            return true;
        }

        public static void UpdateAnyMovedBitboard(PieceType piece_type, ulong moved_place, ulong new_place)
        {
            //check if it is promotion or castling in the first place;
            if (player_color)
            {
                switch (piece_type)
                {
                    case PieceType.Pawn:
                        white_pawns &= moved_place;
                        white_pawns |= new_place;
                        break;
                    case PieceType.Knight:
                        white_knights &= moved_place;
                        white_knights |= new_place;
                        break;
                    case PieceType.Queen:
                        white_queens &= moved_place;
                        white_queens |= new_place;
                        break;
                    case PieceType.King:
                        white_king &= moved_place;
                        white_king |= new_place;
                        break;
                    case PieceType.Bishop:
                        white_bishops &= moved_place;
                        white_bishops |= new_place;
                        break;
                    case PieceType.Rook:
                        white_rooks &= moved_place;
                        white_rooks |= new_place;
                        break;
                    default:  //reserved for castling
                        break;
                }
                white_occupied = white_pawns | white_bishops | white_rooks | white_queens | white_knights | white_king;
            }
            else
            {
                switch (piece_type)
                {
                    case PieceType.Pawn:
                        black_pawns &= moved_place;
                        black_pawns |= new_place;
                        break;
                    case PieceType.Knight:
                        black_knights &= moved_place;
                        black_knights |= new_place;
                        break;
                    case PieceType.Queen:
                        black_queens &= moved_place;
                        black_queens |= new_place;
                        break;
                    case PieceType.King:
                        black_king &= moved_place;
                        black_king |= new_place;
                        break;
                    case PieceType.Bishop:
                        black_bishops &= moved_place;
                        black_bishops |= new_place;
                        
                        break;
                    case PieceType.Rook:
                        black_rooks &= moved_place;
                        black_rooks |= new_place;
                        break;
                    default:  //reserved for castling
                        break;
                }
                
                black_occupied = black_pawns | black_bishops | black_rooks | black_queens | black_knights | black_king;
               

            }
            pieces_occupied = white_occupied | black_occupied;
            empty = ~ pieces_occupied;
        }

        public static void updateCastlingMovedBitboard(int rook_new_index, bool side)
        {
            if (player_color)
            {
                if (side) //right side rook
                {
                    white_king = white_king << 2;
                    white_rooks &= 0xffffffffffffff7f;
                    white_rooks |= ((ulong)1 << rook_new_index);
                   
                }
                else // left side rook
                {
                    white_king = white_king >> 2;
                    white_rooks &= 0xfffffffffffffffe;
                    white_rooks |= ((ulong)1 << rook_new_index);
                }
                white_occupied = white_pawns | white_bishops | white_rooks | white_queens | white_knights | white_king;
            }
            else
            {
                if (side) //right side rook
                {
                    black_king = black_king << 2;
                    black_rooks &= 0xffffffffffffff7f;
                    black_rooks |= ((ulong)1 << rook_new_index);
                    
                }
                else // left side rook
                {
                    black_king = black_king >> 2;
                    black_rooks &= 0xfffffffffffffffe;
                    black_rooks |= ((ulong)1 << rook_new_index);
                }
                black_occupied = black_pawns | black_bishops | black_rooks | black_queens | black_knights | black_king;
            }
            pieces_occupied = white_pawns | white_knights | white_bishops | white_rooks | white_queens | white_king | black_pawns | black_knights | black_bishops | black_rooks | black_queens | black_king;
            empty = ~pieces_occupied;
            PKC = false;
            PQC = false;
        }
        //this is for captured
        public static void UpdateAnyCapturedBitboard(PieceType piece_type, ulong moved_place)
        {
            if (player_color)
            {
                switch (piece_type)
                {
                    case PieceType.Pawn:
                        black_pawns &= (~moved_place);
                        break;
                    case PieceType.Knight:
                        black_knights &= (~moved_place);
                        break;
                    case PieceType.Queen:
                        black_queens &= (~moved_place);
                        break;
                    case PieceType.King:
                        black_king &= (~moved_place);
                        break;
                    case PieceType.Bishop:
                        black_bishops &= (~moved_place);
                        break;
                    case PieceType.Rook:
                        black_rooks &= (~moved_place);
                        break;
                    default:  //reserved for castling
                        break;
                }
                black_occupied = black_pawns | black_bishops | black_rooks | black_queens | black_knights | black_king;
            }
            else
            {
                switch (piece_type)
                {
                    case PieceType.Pawn:
                        white_pawns &= (~moved_place);                      
                        break;
                    case PieceType.Knight:
                        white_knights &= (~moved_place);
                        break;
                    case PieceType.Queen:
                        white_queens &= (~moved_place);
                        break;
                    case PieceType.King:
                        white_king &= (~moved_place);
                        break;
                    case PieceType.Bishop:
                        white_bishops &= (~moved_place);
                        break;
                    case PieceType.Rook:
                        white_rooks &= (~moved_place);
                        break;
                    default:  //reserved for castling
                        break;
                }
                white_occupied = white_pawns | white_bishops | white_rooks | white_queens | white_knights | white_king;
            }
            pieces_occupied = white_occupied | black_occupied;
            empty = ~ pieces_occupied;
        }

        public static List<ChessBoard> generateChessBoards(bool min_max, ulong B_P, ulong B_R, ulong B_N, ulong B_B, ulong B_Q, ulong B_K, ulong W_P, ulong W_R, ulong W_N, ulong W_B, ulong W_Q, ulong W_K, Move history_move=null, bool MKC=true, bool MQC=true, bool PKC=true, bool PQC=true)
        {
            List<ChessBoard> theList = new List<ChessBoard>();

            

            if (min_max)  // if it is an ai max node
            {
                //Console.WriteLine("---------------");
                setCurrentBitboards(B_P, B_R, B_N, B_B, B_Q, B_K, W_P, W_R, W_N, W_B, W_Q, W_K);
                setCurrentBitboardsHistoryMove(history_move);
                setCurrentCastlingCondition(MKC, MQC, PKC, PQC);        
                ArrayList moves = PossibleMovesMachine();
                Move last_time_best=null;
                if (best_move_queue.Count!=0)
                {
                    last_time_best = best_move_queue.Dequeue();
                    //Console.WriteLine("Best Move is " + last_time_best.from_rank + " " + last_time_best.from_file + " " + last_time_best.to_rank + " " + last_time_best.to_file);
                }                   
                //Console.WriteLine("Best move is " + last_time_best.from_rank + last_time_best.from_file + last_time_best.to_rank + last_time_best.to_file);
                moves.Sort(new MoveCompare(last_time_best));

                // else
                // {
                //Move last_best = best_move_queue.Dequeue();
                // moves.Sort(new MoveCompare()); //move ordering
                //moves.Sort(new MoveCompare(last_best));
                //}
                
                //foreach (Move move in moves)
                //{
                //    Console.WriteLine("Move is " + move.from_rank + " " + move.from_file + " " + move.to_rank + " " + move.to_file);
                //}


                foreach (Move move in moves)
                {
                    ulong BP = B_P;         ulong WP = W_P;
                    ulong BR = B_R;         ulong WR = W_R;
                    ulong BN = B_N;         ulong WN = W_N;
                    ulong BQ = B_Q;         ulong WQ = W_Q;
                    ulong BB = B_B;         ulong WB = W_B;
                    ulong BK = B_K;         ulong WK = W_K;
                    bool M_K_C = MKC;
                    bool M_Q_C = MQC;
                    bool P_K_C = PKC;
                    bool P_Q_C = PQC;

                    //process_time = DateTime.Now;
                    //if (DateTime.Compare(process_time, end_time) > 0)
                    //    break;

                    //castling move.

                    if (move.MQC) // if move is a machine queen side castling
                    {
                        
                        int king_index = move.from_rank * 8 + move.from_file;
                        if(player_color)
                        {
                            BK = (BK >> 2);
                            BR &= ~((ulong)0x0100000000000000);
                            BR |= (ulong)0x0800000000000000;
                            //Console.WriteLine("BK is " + Convert.ToString((long)(BR+BK), 2));
                        }
                        else
                        {
                            WK = (WK << 2);
                            WR &= ~((ulong)0x8000000000000000);
                            WR |= (ulong)0x0100000000000000;
                        }
                        M_Q_C = false; M_K_C = false;
                        ChessBoard castling_b = new ChessBoard(WP, WN, WB, WQ, WR, WK, BP, BN, BB, BQ, BR, BK, move, M_K_C, M_Q_C, P_K_C, P_Q_C);
                        theList.Add(castling_b);
                        continue;
                    }

                    if (move.MKC) // if the move is a machine king side castling
                    {
                        
                        int king_index = move.from_rank * 8 + move.from_file;
                        if (player_color)
                        {
                            BK = (BK << 2);
                            BR &= ~((ulong)0x8000000000000000);
                            BR |= (ulong)0x2000000000000000;
                        }
                        else
                        {
                            WK = (WK >> 2);
                            WR &= ~((ulong)0x0100000000000000);
                            WR |= (ulong)0x0400000000000000;
                        }
                        M_Q_C = false; M_K_C = false;
                        ChessBoard castling_b = new ChessBoard(WP, WN, WB, WQ, WR, WK, BP, BN, BB, BQ, BR, BK, move, M_K_C, M_Q_C, P_K_C, P_Q_C);
                        theList.Add(castling_b);
                        continue;
                    }



                    //normal moves are following 


                    switch (move.cap_type)
                    {
                        case PieceType.King:
                            if (player_color) //if player play black then ai updates the white piece                             
                                WK &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            else
                                BK &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            break;
                        case PieceType.Queen:
                            if (player_color)
                                WQ &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            else
                                BQ &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            break;
                        case PieceType.Rook:
                            if (player_color)
                                WR &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            else
                                BR &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            break;
                        case PieceType.Knight:
                            if (player_color)
                                WN &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            else
                                BN &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));                           
                            break;
                        case PieceType.Bishop:
                            if (player_color)
                                WB &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            else
                                BB &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            break;
                        case PieceType.Pawn:
                            if (player_color)
                                WP &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            else
                                BP &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            break;
                        default:
                            break;
                    }
                    switch (move.moved_type)
                    {
                        case PieceType.King:
                            if (player_color)
                            {
                                BK |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                BK &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }
                            else
                            {
                                WK |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                WK &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }
                            M_K_C = false;
                            M_Q_C = false;
                            break;
                        case PieceType.Queen:
                            if (player_color)
                            {
                                BQ |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                BQ &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }
                            else
                            {
                                WQ |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                WQ &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }
                            break;
                        case PieceType.Rook:
                            if (player_color)
                            {
                                BR |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                BR &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                                if (move.from_file == 0)
                                    M_Q_C = false;
                                if (move.from_file == 7)
                                    M_K_C = false;
                            }
                            else
                            {
                                WR |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                WR &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                                if (move.from_file == 0)
                                    M_K_C = false;
                                if (move.from_file == 7)
                                    M_Q_C = false;
                            }
                            
                            break;
                        case PieceType.Knight:
                            if (player_color)
                            { 
                                BN |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                BN &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }
                            else
                            {
                                WN |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                WN &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }
                            break;
                        case PieceType.Bishop:
                            if (player_color)
                            {
                                BB |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                BB &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }
                            else
                            {
                                WB |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                WB &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }
                            break;
                        case PieceType.Pawn:
                            if (player_color)
                            {
                                BP |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                BP &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }
                            else
                            {
                                WP |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                WP &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }
                            break;
                        default:
                            break;

                    }
                    // Switch case for special events! promotion, enpassant etc.
                    ChessBoard cb = new ChessBoard(WP, WN, WB, WQ, WR, WK, BP, BN, BB, BQ, BR, BK, move, M_K_C, M_Q_C, P_K_C, P_Q_C);
                    theList.Add(cb);
                }
            }

            // if it is a player min node 
            else
            {
                setCurrentBitboards(B_P, B_R, B_N, B_B, B_Q, B_K, W_P, W_R, W_N, W_B, W_Q, W_K);
                setCurrentBitboardsHistoryMove(history_move);
                setCurrentCastlingCondition(MKC, MQC, PKC, PQC);
                ArrayList moves = PossibleMovesPlayer();
                Move last_time_best = null;
                if (best_move_queue.Count != 0)
                {
                    last_time_best = best_move_queue.Dequeue();
                }
                //Console.WriteLine("Best move is " + last_time_best.from_rank + last_time_best.from_file + last_time_best.to_rank + last_time_best.to_file);
                moves.Sort(new MoveCompare(last_time_best));

                foreach (Move move in moves)
                {
                    ulong BP = B_P; ulong WP = W_P;
                    ulong BR = B_R; ulong WR = W_R;
                    ulong BN = B_N; ulong WN = W_N;
                    ulong BQ = B_Q; ulong WQ = W_Q;
                    ulong BB = B_B; ulong WB = W_B;
                    ulong BK = B_K; ulong WK = W_K;
                    bool M_K_C = MKC;
                    bool M_Q_C = MQC;
                    bool P_K_C = PKC;
                    bool P_Q_C = PQC;
                    //process_time = DateTime.Now;
                    //if (DateTime.Compare(process_time, end_time) > 0)
                    //    break;
                    //process_time = DateTime.Now;
                    //if (DateTime.Compare(process_time, end_time) >= 0)
                    //    break;

                    if (move.PQC) // if move is a player queen side castling
                    {
                        if (player_color)
                        {
                            WK = (WK >> 2);
                            WR &= ~((ulong)0x0000000000000001);
                            WR |= (ulong)0x0000000000000008;
                        }
                        else
                        {
                            BK = (BK << 2);
                            BR &= ~((ulong)0x0000000000000080);
                            BR |= (ulong)0x0000000000000010;
                        }
                        P_Q_C = false; P_K_C = false;
                        ChessBoard castling_b = new ChessBoard(WP, WN, WB, WQ, WR, WK, BP, BN, BB, BQ, BR, BK, move, M_K_C, M_Q_C, P_K_C, P_Q_C);
                        theList.Add(castling_b);
                        continue;
                    }

                    if (move.PKC) // if the move is a player king side castling
                    {
                        if (player_color)
                        {
                            WK = (WK << 2);
                            WR &= ~((ulong)0x0000000000000080);
                            WR |= (ulong)0x0000000000000020;
                        }
                        else
                        {
                            BK = (BK >> 2);
                            BR &= ~((ulong)0x0000000000000001);
                            BR |= (ulong)0x0000000000000004;
                        }
                        P_Q_C = false; P_K_C = false;
                        ChessBoard castling_b = new ChessBoard(WP, WN, WB, WQ, WR, WK, BP, BN, BB, BQ, BR, BK, move, M_K_C, M_Q_C, P_K_C, P_Q_C);
                        theList.Add(castling_b);
                        continue;
                    }



                    switch (move.cap_type)
                    {
                        case PieceType.King:
                            if(player_color) // if player use black
                                BK &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            else
                                WK &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            break;
                        case PieceType.Queen:
                            if (player_color)
                                BQ &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            else
                                WQ &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            break;
                        case PieceType.Rook:
                            if (player_color)
                                BR &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            else
                                WR &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            break;
                        case PieceType.Knight:
                            if (player_color)
                                BN &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            else
                                WN &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            break;
                        case PieceType.Bishop:
                            if (player_color)
                                BB &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            else
                                WB &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            break;
                        case PieceType.Pawn:
                            if (player_color)
                                BP &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            else
                                WP &= ~((ulong)1 << (move.to_rank * 8 + move.to_file));
                            break;
                        default:
                            break;
                    }
                    switch (move.moved_type)
                    {
                        case PieceType.King:
                            if (player_color)
                            {
                                WK |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                WK &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }
                            else
                            {
                                BK |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                BK &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }
                            PKC = false;
                            PQC = false;
                            break;
                        case PieceType.Queen:
                            if (player_color)
                            {
                                WQ |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                WQ &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }
                            else
                            {
                                BQ |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                BQ &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }
                            break;
                        case PieceType.Rook:
                            if (player_color)
                            {
                                WR |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                WR &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                                if (move.from_file == 0)
                                    P_Q_C = false;
                                if (move.from_file == 7)
                                    P_K_C = false;
                            }
                            else
                            {
                                BR |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                BR &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                                if (move.from_file == 0)
                                    P_K_C = false;
                                if (move.from_file == 7)
                                    P_Q_C = false;
                            }

                            break;
                        case PieceType.Knight:
                            if (player_color)
                            {
                                WN |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                WN &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }
                            else
                            {
                                BN |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                BN &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }

                            break;
                        case PieceType.Bishop:
                            if (player_color)
                            {
                                WB |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                WB &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }
                            else
                            {
                                BB |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                BB &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                            }

                            break;
                        case PieceType.Pawn:
                            if (player_color)
                            {
                                WP &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                                if(move.promote) //if the pawn got promoted, then update a new piece in queen bitboard
                                    WQ |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                else
                                    WP |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                               
                            }
                            else
                            {
                                BP &= ~((ulong)1 << (move.from_rank * 8 + move.from_file));
                                if (move.promote) //if the pawn got promoted, then update a new piece in queen bitboard
                                    BQ |= ((ulong)1 << (move.to_rank * 8 + move.to_file));
                                else
                                    BP |= ((ulong)1 << (move.to_rank * 8 + move.to_file));

                            }

                            break;
                        default:
                            break;

                    }
                    // switch for special events
                    ChessBoard cb = new ChessBoard(WP, WN, WB, WQ, WR, WK, BP, BN, BB, BQ, BR, BK, move, M_K_C, M_Q_C, P_K_C, P_Q_C);
                    theList.Add(cb);
                }
            }
            return theList;
        }


    }
}
