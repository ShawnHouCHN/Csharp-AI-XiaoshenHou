using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChessBoardUI.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using ChessBoardUI.Players;

namespace ChessBoardUI
{
    public partial class MainWindow : Window
    {

        
        //HMPlayer human_player;
        //MCPlayer machine_player;
        Dictionary<int, ChessPiece> board_layout; //generic hashtable
        MainControl board;



        public MainWindow()
        {
            InitializeComponent();

        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {

            board_layout = new Dictionary<int, ChessPiece>();

            if ((String)((ComboBoxItem)ChooseColor.SelectedItem).Content == "Black")
                board = new MainControl(false, (String)((ComboBoxItem)ChooseLevel.SelectedItem).Content);
            else
                board = new MainControl(true, (String)((ComboBoxItem)ChooseLevel.SelectedItem).Content);


            //Console.WriteLine("{0},{1}", ((ComboBoxItem)ChooseColor.SelectedItem).Content, ((ComboBoxItem)ChooseLevel.SelectedItem).Content);
            StartButton.IsEnabled = false;
            ChooseLevel.IsEnabled = false;
            ChooseColor.IsEnabled = false;


            player_timer.DataContext = board.HumanPlayer.HumanTimer;
            pc_timer.DataContext = board.MachinePlayer.MachineTimer;


            board.HumanPlayer.HumanTimer.startClock();
     
            PlayerCapStack.ItemsSource = board.HumanPlayer.HumanCaptureStack.CapturedPiecesCollection;
            MachineCapStack.ItemsSource = board.MachinePlayer.MachineCaptureStack.CapturedPiecesCollection;


            this.ChessBoard.ItemsSource = board.BoardCollection;


        }
    }
}
