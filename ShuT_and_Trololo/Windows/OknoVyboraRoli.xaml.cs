using System.Windows;
using System.Windows.Controls;
using ShuT_and_Trololo.Data;

namespace ShuT_and_Trololo.Windows
{
    public partial class OknoVyboraRoli : Window
    {
        private int polzId;

        public OknoVyboraRoli(int userId, string tekRol)
        {
            InitializeComponent();
            polzId = userId;
            Width = 300; Height = 260;
            Title = "Назначить роль";
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;
            Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#1E1E2E"));
            SozdatUI(tekRol);
        }

        private void SozdatUI(string tekRol)
        {
            var stack = new StackPanel { Margin = new Thickness(20) };

            stack.Children.Add(new TextBlock
            {
                Text = $"Текущая роль: {tekRol}",
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 16)
            });

            var roli = new[] {
                (1, "Читатель"),
                (2, "Автор"),
                (3, "Администратор")
            };

            foreach (var (rid, rname) in roli)
            {
                int localRid = rid;
                var btn = new Button
                {
                    Content = rname,
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
                    AdminDB.IzmRol(polzId, localRid);
                    MessageBox.Show($"Роль изменена на {rname}.", "Готово",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                };
                stack.Children.Add(btn);
            }

            Content = stack;
        }
    }
}