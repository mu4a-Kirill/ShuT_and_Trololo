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
using System.Windows.Shapes;

namespace ShuT_and_Trololo.Windows
{
    public partial class OknoChtenia : Window
    {
        public OknoChtenia(string nazvanie, string tekst)
        {
            InitializeComponent();
            Title = nazvanie;
            var tb = new System.Windows.Controls.TextBlock
            {
                Text = tekst,
                Foreground = System.Windows.Media.Brushes.White,
                TextWrapping = System.Windows.TextWrapping.Wrap,
                Margin = new Thickness(20),
                FontSize = 14
            };
            var scroll = new System.Windows.Controls.ScrollViewer();
            scroll.Content = tb;
            Content = scroll;
            Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#1E1E2E"));
        }
    }
}
