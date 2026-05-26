using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ShuT_and_Trololo.Data;
using ShuT_and_Trololo.Models;
using ShuT_and_Trololo.Windows;
using IOPath = System.IO.Path;

namespace ShuT_and_Trololo.Pages
{
    public partial class StranitsaKnigi : Page
    {
        private int knigaId;
        private Kniga tekKniga;

        public StranitsaKnigi(int bookId)
        {
            InitializeComponent();
            knigaId = bookId;
            ZagruzitVse();
        }

        private void ZagruzitVse()
        {
            tekKniga = KnigaDB.GetKnigaPoId(knigaId);
            if (tekKniga == null)
            {
                MessageBox.Show("Книга не найдена.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            txtNazv.Text = tekKniga.Title;
            txtAvtor.Text = "Автор: " + tekKniga.AutorImya;
            txtOtsenka.Text = tekKniga.SrednyayaOtsenka > 0
                ? $"★ {tekKniga.SrednyayaOtsenka:F1} / 10"
                : "★ Нет оценок";
            txtOpis.Text = tekKniga.Description ?? "Описание отсутствует";

            ZagruzitOblozhku(tekKniga.CoverPath);

            ZagruzitZhanry();

            if (Sessiya.EtoAdmin)
                btnZamorozitKnigu.Visibility = Visibility.Visible;

            if (OtzyvDB.UjeEstOtzyv(Sessiya.TekushiyPolzovatel.UserId, knigaId))
                panelFormOtzyv.Visibility = Visibility.Collapsed;

            for (int i = 1; i <= 10; i++)
                cbOtsenka.Items.Add(i);
            cbOtsenka.SelectedIndex = 9; 

                 ZagruzitOtzyvy();
        }

        private void ZagruzitOblozhku(string coverPath)
        {
            try
            {
                string put = IOPath.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Resources", "Covers",
                    IOPath.GetFileName(coverPath ?? ""));

                if (File.Exists(put))
                    imgOblozhka.Source = new BitmapImage(new Uri(put));
                else
                    imgOblozhka.Source = new BitmapImage(new Uri(
                        "pack://application:,,,/Resources/Images/no_cover.png"));
            }
            catch { }
        }

        private void ZagruzitZhanry()
        {
            panelZhanry.Children.Clear();
            var zhanry = KnigaDB.GetZhanryKnigi(knigaId);

            foreach (var z in zhanry)
            {
                var tag = new Border
                {
                    Background = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#4C1D95")),
                    CornerRadius = new CornerRadius(12),
                    Padding = new Thickness(10, 4, 10, 4),
                    Margin = new Thickness(0, 0, 6, 6),
                    Child = new TextBlock
                    {
                        Text = z.GenreName,
                        Foreground = Brushes.White,
                        FontSize = 12
                    }
                };
                panelZhanry.Children.Add(tag);
            }
        }

        private void ZagruzitOtzyvy()
        {
            panelOtzyvy.Children.Clear();
            var otzyvy = OtzyvDB.GetOtzyvy(knigaId);

            if (otzyvy.Count == 0)
            {
                panelOtzyvy.Children.Add(new TextBlock
                {
                    Text = "Отзывов пока нет. Будьте первым!",
                    Foreground = Brushes.Gray,
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 16)
                });
                return;
            }

            foreach (var o in otzyvy)
            {
                panelOtzyvy.Children.Add(SozdatKartochkuOtzyva(o));
            }
        }

        private Border SozdatKartochkuOtzyva(Otzyv otzyv)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#2D2D3F")),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16),
                Margin = new Thickness(0, 0, 0, 12)
            };

            if (otzyv.IsFrozen)
            {
                border.Child = new TextBlock
                {
                    Text = "❄ Этот отзыв заморожен администратором",
                    Foreground = Brushes.LightBlue,
                    FontStyle = FontStyles.Italic
                };
                return border;
            }

            var stack = new StackPanel();

            var shapka = new Grid();
            shapka.ColumnDefinitions.Add(new ColumnDefinition());
            shapka.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var imyaData = new StackPanel { Orientation = Orientation.Horizontal };
            imyaData.Children.Add(new TextBlock
            {
                Text = otzyv.PolzovatelImya,
                Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#A78BFA")),
                FontWeight = FontWeights.SemiBold,
                FontSize = 14,
                Margin = new Thickness(0, 0, 12, 0)
            });
            imyaData.Children.Add(new TextBlock
            {
                Text = otzyv.CreatedAt.ToString("dd.MM.yyyy"),
                Foreground = Brushes.Gray,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            });

            var ocenkaBlok = new TextBlock
            {
                Text = $"★ {otzyv.Rating}/10",
                Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#FCD34D")),
                FontSize = 14,
                FontWeight = FontWeights.Bold
            };

            Grid.SetColumn(imyaData, 0);
            Grid.SetColumn(ocenkaBlok, 1);
            shapka.Children.Add(imyaData);
            shapka.Children.Add(ocenkaBlok);

            var tekstBlok = new TextBlock
            {
                Text = otzyv.ReviewText ?? "",
                Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#D1D5DB")),
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 8, 0, 8)
            };

            var btnPanel = new WrapPanel();

            var btnZhaloba = new Button
            {
                Content = "⚠ Пожаловаться",
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#3D3D5F")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(10, 5, 10, 5),
                FontSize = 12,
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 0, 8, 0)
            };
            btnZhaloba.Click += (s, e) => ZhalobaNaOtzyv(otzyv.ReviewId);

            btnPanel.Children.Add(btnZhaloba);

            if (Sessiya.EtoAdmin)
            {
                var btnZamor = new Button
                {
                    Content = "❄ Заморозить отзыв",
                    Background = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#1D4ED8")),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(10, 5, 10, 5),
                    FontSize = 12,
                    Cursor = System.Windows.Input.Cursors.Hand
                };
                btnZamor.Click += (s, e) => ZamorozitOtzyv(otzyv.ReviewId);
                btnPanel.Children.Add(btnZamor);
            }

            stack.Children.Add(shapka);
            stack.Children.Add(tekstBlok);
            stack.Children.Add(btnPanel);
            border.Child = stack;
            return border;
        }

        private void btnNazad_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void btnChitat_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tekKniga?.Content))
            {
                MessageBox.Show("Текст книги недоступен.", "Читать",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var oknoChteniya = new OknoChtenia(tekKniga.Title, tekKniga.Content);
            oknoChteniya.ShowDialog();
        }

        private void btnVSpisok_Click(object sender, RoutedEventArgs e)
        {
            var okno = new OknoVyboraSpiskaWindow(knigaId);
            okno.ShowDialog();
        }

        private void btnZhalobaKniga_Click(object sender, RoutedEventArgs e)
        {
            var okno = new OknoZhaloby("книгу", (prichina) =>
            {
                ZhalobaDB.ZhalobaNaKnigu(
                    Sessiya.TekushiyPolzovatel.UserId, knigaId, prichina);
            });
            okno.ShowDialog();
        }

        private void btnZhalobaAvtor_Click(object sender, RoutedEventArgs e)
        {
            var okno = new OknoZhaloby("автора", (prichina) =>
            {
                ZhalobaDB.ZhalobaNaAvtora(
                    Sessiya.TekushiyPolzovatel.UserId, tekKniga.AuthorId, prichina);
            });
            okno.ShowDialog();
        }

        private void ZhalobaNaOtzyv(int reviewId)
        {
            var okno = new OknoZhaloby("отзыв", (prichina) =>
            {
                ZhalobaDB.ZhalobaNaOtzyv(
                    Sessiya.TekushiyPolzovatel.UserId, reviewId, prichina);
            });
            okno.ShowDialog();
        }

        private void btnZamorozitKnigu_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show(
                $"Заморозить книгу \"{tekKniga.Title}\"?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (res == MessageBoxResult.Yes)
            {
                KnigaDB.ZamorozitKnigu(knigaId);
                MessageBox.Show("Книга заморожена.", "Готово",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new StranitsaKataloga());
            }
        }

        private void ZamorozitOtzyv(int reviewId)
        {
            var res = MessageBox.Show(
                "Заморозить этот отзыв?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (res == MessageBoxResult.Yes)
            {
                OtzyvDB.ZamorozitOtzyv(reviewId);
                ZagruzitOtzyvy(); 
            }
        }

        private void btnOtpravitOtzyv_Click(object sender, RoutedEventArgs e)
        {
            string tekst = txtOtzyvTekst.Text.Trim();

            if (string.IsNullOrEmpty(tekst) ||
                tekst == "Напишите ваш отзыв...")
            {
                MessageBox.Show("Напишите текст отзыва.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (tekst.Length < 10)
            {
                MessageBox.Show("Отзыв слишком короткий (минимум 10 символов).",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int otsenka = (int)cbOtsenka.SelectedItem;

            OtzyvDB.DobavitOtzyv(
                Sessiya.TekushiyPolzovatel.UserId, knigaId, tekst, otsenka);

            MessageBox.Show("Отзыв добавлен!", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);

            panelFormOtzyv.Visibility = Visibility.Collapsed;
            ZagruzitOtzyvy();
        }
    }
}