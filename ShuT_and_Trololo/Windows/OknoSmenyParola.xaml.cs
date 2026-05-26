using System.Windows;
using System.Windows.Controls;
using ShuT_and_Trololo.Data;

namespace ShuT_and_Trololo.Windows
{
    public partial class OknoSmenyParola : Window
    {
        private int polzId;

        public OknoSmenyParola(int userId)
        {
            InitializeComponent();
            polzId = userId;
            Width = 340; Height = 220;
            Title = "Сменить пароль";
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
                Text = "Новый пароль:",
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 8)
            });

            var pbNov = new PasswordBox
            {
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#2D2D3F")),
                Foreground = System.Windows.Media.Brushes.White,
                BorderBrush = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#7C3AED")),
                FontSize = 13,
                Padding = new Thickness(8, 6, 8, 6),
                Margin = new Thickness(0, 0, 0, 14)
            };

            var btnSohr = new Button
            {
                Content = "Сохранить",
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#7C3AED")),
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(12, 8, 12, 8),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            btnSohr.Click += (s, e) =>
            {
                string novPar = pbNov.Password.Trim();
                if (novPar.Length < 4)
                {
                    MessageBox.Show("Пароль должен содержать минимум 4 символа.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                AdminDB.SmParol(polzId, novPar);
                MessageBox.Show("Пароль изменён.", "Готово",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            };

            stack.Children.Add(pbNov);
            stack.Children.Add(btnSohr);
            Content = stack;
        }
    }
}