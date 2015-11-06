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

namespace Hashtabletest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Dictionary<Point, String> test = new Dictionary<Point, string>();
            test.Add(new Point(0, 1), "first point");
            test.Add(new Point(0, 2), "Second point");
            test.Add(new Point(0, 1), "third point");
            Console.WriteLine(test[new Point(0, 1)]);

        }
    }
}
