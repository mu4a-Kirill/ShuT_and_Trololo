using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ShuT_and_Trololo.Data;
using ShuT_and_Trololo.Models;

namespace ShuT_and_Trololo.Pages
{
    public partial class StranitsaAvtora : Page
    {
        public StranitsaAvtora()
        {
            InitializeComponent();
            ZagruzitKnigi();
        }

        private void ZagruzitKnigi()
        {
            panelKnigiAvtora.Children.Clear();
            panelZamorozhen.Children.Clear();

            var vsekNigi = AvtorDB.GetKnigiAvtora(
                Sessiya.TekushiyPolzovatel.UserId);

            bool estObichnye = false;
            bool estZamor = false;

            foreach (var kniga in vsekNigi)
            {
                if (kniga.IsFrozen)
                {
                    panelZamorozhen.Children.Add(SozdatStrokiZamor(kniga));
                    estZamor = true;
                }
                else
                {
                    panelKnigiAvtora.Children.Add(SozdatStrokiKnigi(kniga));
                    estObichnye = true;
                }
            }

            if (!estObichnye)
            {
                panelKnigiAvtora.Children.Add(new TextBlock
                {
                    Text = "У вас пока нет опубликованных книг",
                    Foreground = Brushes.Gray,
                    FontSize = 14
                });
            }

            if (!estZamor)
            {
                panelZamorozhen.Children.Add(new TextBlock
                {
                    Text = "Замороженных книг нет",
                    Foreground = Brushes.Gray,
                    FontSize = 14
                });
            }
        }

        // Строка с опубликованной книгой: название + кнопка редактировать
        private Border SozdatStrokiKnigi(Kniga kniga)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#2D2D3F")),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16, 12, 16, 12),
                Margin = new Thickness(0, 0, 0, 8)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition
            { Width = System.Windows.GridLength.Auto });

            var info = new StackPanel();
            info.Children.Add(new TextBlock
            {
                Text = kniga.Title,
                Foreground = Brushes.White,
                FontSize = 15,
                FontWeight = FontWeights.SemiBold
            });
            info.Children.Add(new TextBlock
            {
                Text = kniga.SrednyayaOtsenka > 0
                    ? $"★ {kniga.SrednyayaOtsenka:F1}"
                    : "★ нет оценок",
                Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#FCD34D")),
                FontSize = 12,
                Margin = new Thickness(0, 4, 0, 0)
            });

            var btnEdit = new Button
            {
                Content = "✏ Редактировать",
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#3D3D5F")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(12, 6, 12, 6),
                FontSize = 12,
                Cursor = System.Windows.Input.Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Center
            };
            btnEdit.MouseEnter += (s, e) => btnEdit.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4D4D6F"));
            btnEdit.MouseLeave += (s, e) => btnEdit.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3D3D5F"));
            btnEdit.Click += (s, e) => RedaktirovatKnigu(kniga);

            System.Windows.Controls.Grid.SetColumn(info, 0);
            System.Windows.Controls.Grid.SetColumn(btnEdit, 1);
            grid.Children.Add(info);
            grid.Children.Add(btnEdit);

            border.Child = grid;
            return border;
        }

        // Строка с замороженной книгой: название + кнопка оспорить
        private Border SozdatStrokiZamor(Kniga kniga)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#1E3A5F")),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16, 12, 16, 12),
                Margin = new Thickness(0, 0, 0, 8)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition
            { Width = System.Windows.GridLength.Auto });

            var info = new StackPanel();
            info.Children.Add(new TextBlock
            {
                Text = "❄ " + kniga.Title,
                Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#93C5FD")),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold
            });

            var btnOspor = new Button
            {
                Content = "Оспорить заморозку",
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#1D4ED8")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(12, 6, 12, 6),
                FontSize = 12,
                Cursor = System.Windows.Input.Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Center
            };
            btnOspor.Click += (s, e) => OsporeniyeZamorozki(kniga);

            System.Windows.Controls.Grid.SetColumn(info, 0);
            System.Windows.Controls.Grid.SetColumn(btnOspor, 1);
            grid.Children.Add(info);
            grid.Children.Add(btnOspor);

            border.Child = grid;
            return border;
        }

        private void btnDobavit_Click(object sender, RoutedEventArgs e)
        {
            var okno = new Windows.OknoRedaktirovaniyaKnigi(null);
            okno.ShowDialog();
            ZagruzitKnigi(); // обновляем список
        }

        private void RedaktirovatKnigu(Kniga kniga)
        {
            var okno = new Windows.OknoRedaktirovaniyaKnigi(kniga);
            okno.ShowDialog();
            ZagruzitKnigi();
        }

        private void OsporeniyeZamorozki(Kniga kniga)
        {
            var okno = new Windows.OknoZhaloby("заморозку книги", (reason) =>
            {
                ZayavkaDB.PodatZayavkuNaRazmorzKnigu(
                    Sessiya.TekushiyPolzovatel.UserId, kniga.BookId, reason);
            });
            okno.ShowDialog();
        }
    }
}