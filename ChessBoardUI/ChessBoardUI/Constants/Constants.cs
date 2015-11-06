using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessBoardUI.Constants
{
    public static class Constants
    {
        internal const int CELL_EDGE_LENGTH = 70;
        internal const int CANVAS_MARGIN_LEFT = 35;  //to pinpoint the cursor position in the canvas 
        internal const int CANVAS_MARGIN_TOP = 45;   //to pinpoint the cursor position in the canvas 
        internal const int IMAGE_CENTER_TO_LEFTTOP = 30; //center of image to left-top corner
        const ulong fullboard = 0xffffffffffffffff ;
        public const int PAWN_WEIGHT = 100;
        public const int KNIGHT_WEIGHT = 300;
        public const int BISHOP_WEIGHT = 300;
        public const int ROOK_WEIGHT = 500;
        public const int QUEEN_WEIGHT = 900;
        public const int KING_WEIGHT = 10000;
    }


}
