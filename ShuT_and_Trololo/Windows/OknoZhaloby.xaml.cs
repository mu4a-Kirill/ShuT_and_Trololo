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
    public partial class OknoZhaloby : Window
    {
        private Action<string> onPodtverdit;

        public OknoZhaloby(string naChto, Action<string> callback)
        {
            InitializeComponent();
            Title = $"Жалоба на {naChto}";
            onPodtverdit = callback;
            SozdatUI(naChto);
        }

        private void SozdatUI(string naChto)
        {
            Width = 400; Height = 280;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;
            Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#1E1E2E"));

            var stack = new StackPanel { Margin = new Thickness(20) };

            stack.Children.Add(new TextBlock
            {
                Text = $"Укажите причину жалобы на {naChto}:",
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 10)
            });

            var txtPrichina = new TextBox
            {
                Height = 100,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#2D2D3F")),
                Foreground = System.Windows.Media.Brushes.White,
                BorderBrush = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#7C3AED")),
                Padding = new Thickness(8),
                Margin = new Thickness(0, 0, 0, 16)
            };

            var btnOtpr = new Button
            {
                Content = "Отправить жалобу",
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                        .ConvertFromString("#DC2626")),
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(12, 8, 12, 8),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            btnOtpr.Click += (s, e) =>
            {
                string prichina = txtPrichina.Text.Trim();
                if (prichina.Length < 5)
                {
                    MessageBox.Show("Опишите причину подробнее (мин. 5 символов).",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                onPodtverdit(prichina);
                MessageBox.Show("Жалоба отправлена. Спасибо!",
                    "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            };

            stack.Children.Add(txtPrichina);
            stack.Children.Add(btnOtpr);
            Content = stack;
        }
    }
}
