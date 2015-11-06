using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ChessBoardUI.ViewModel
{
    class SPCapturedViewModel:ViewModelBase
    {
        private ObservableCollection<BitmapImage> _cap_piece_collection;
        public ObservableCollection<BitmapImage> CapturedPiecesCollection
        {
            get
            {
                return _cap_piece_collection;
            }
            set
            {
                _cap_piece_collection = value;
                RaisePropertyChanged(() => this.CapturedPiecesCollection);
            }
        }
    }
}
