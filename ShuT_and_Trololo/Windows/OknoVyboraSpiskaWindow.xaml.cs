using System.Windows;
using System.Windows.Controls;
using ShuT_and_Trololo.Data;

namespace ShuT_and_Trololo.Windows
{
    public partial class OknoVyboraSpiskaWindow : Window
    {
        private int knigaId;

        public OknoVyboraSpiskaWindow(int bookId)
        {
            InitializeComponent();
            knigaId = bookId;
            Width = 300; Height = 260;
            Title = "Добавить в список";
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#1E1E2E"));
            SozdatUI();
        }

        private void SozdatUI()
        {
            var stack = new StackPanel { Margin = new Thickness(20) };

            stack.Children.Add(new TextBlock
            {
                Text = "Выберите список:",
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 15,
                Margin = new Thickness(0, 0, 0, 16)
            });

            var sektsii = SpisokDB.GetSektsii();

            foreach (var (id, nazvanie) in sektsii)
            {
                int sid = id;
                var btn = new Button
                {
                    Content = nazvanie,
                    Margin = new Thickness(0, 0, 0, 8),
                    Padding = new Thickness(12, 8, 12, 8),
                    Background = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                            .ConvertFromString("#7C3AED")),
                    Foreground = System.Windows.Media.Brushes.White,
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                btn.Click += (s, e) =>
                {
                    SpisokDB.DobavitIliPerenestiKnigu(
                        Sessiya.TekushiyPolzovatel.UserId, knigaId, sid);
                    MessageBox.Show($"Книга добавлена в список \"{nazvanie}\"!",
                        "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                };

                stack.Children.Add(btn);
            }

            Content = stack;
        }
    }
}