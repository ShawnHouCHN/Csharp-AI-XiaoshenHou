using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessBoardUI.ViewTreeHelper
{
    class UIHelper
    {

        public static bool RestrictImageMove(double x, double y)
        {
            if (x >= Constants.Constants.IMAGE_CENTER_TO_LEFTTOP && x <= (8 * Constants.Constants.CELL_EDGE_LENGTH- Constants.Constants.IMAGE_CENTER_TO_LEFTTOP) && y <= ( 8 * Constants.Constants.CELL_EDGE_LENGTH- Constants.Constants.IMAGE_CENTER_TO_LEFTTOP) && y >= Constants.Constants.IMAGE_CENTER_TO_LEFTTOP)
                return true;
            else
                return false;
        }


    }
}
