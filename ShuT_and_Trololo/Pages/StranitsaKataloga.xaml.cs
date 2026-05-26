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
using System.IO;
using ShuT_and_Trololo.Data;
using ShuT_and_Trololo.Models;
using IOPath = System.IO.Path; 
using System.IO;

namespace ShuT_and_Trololo.Pages
{
    public partial class StranitsaKataloga : Page
    {
        private List<Kniga> vsekKnigi = new List<Kniga>();
        private bool poiskPodkazka = true; 

        public StranitsaKataloga()
        {
            InitializeComponent();
            ZagruzitZhanry();
            ZagruzitKnigi();
        }

        private void ZagruzitZhanry()
        {
            cbZhanr.Items.Clear();
            cbZhanr.Items.Add(new ComboBoxItem
            {
                Content = "Все жанры",
                Tag = 0
            });

            var zhanry = KnigaDB.GetVseZhanry();
            foreach (var z in zhanry)
            {
                cbZhanr.Items.Add(new ComboBoxItem
                {
                    Content = z.GenreName,
                    Tag = z.GenreId
                });
            }

            cbZhanr.SelectedIndex = 0;
        }

        private void ZagruzitKnigi()
        {
            vsekKnigi = KnigaDB.GetVseKnigi();
            OtobrazitKnigi(vsekKnigi);
        }

        private void OtobrazitKnigi(List<Kniga> knigi)
        {
            panelKnigi.Children.Clear();

            if (knigi.Count == 0)
            {
                var tekst = new TextBlock
                {
                    Text = "Книги не найдены",
                    Foreground = Brushes.Gray,
                    FontSize = 16,
                    Margin = new Thickness(20)
                };
                panelKnigi.Children.Add(tekst);
                return;
            }

            foreach (var kniga in knigi)
            {
                panelKnigi.Children.Add(SozdatKartochku(kniga));
            }
        }

        private Border SozdatKartochku(Kniga kniga)
        {
            var border = new Border
            {
                Width = 155,
                Height = 250,
                Margin = new Thickness(8),
                CornerRadius = new CornerRadius(8),
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#2D2D3F")),
                Cursor = Cursors.Hand
            };

            border.MouseEnter += (s, e) =>
                border.Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#3D3D5F"));
            border.MouseLeave += (s, e) =>
                border.Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#2D2D3F"));

            border.MouseLeftButtonUp += (s, e) =>
                NavigationService.Navigate(
                    new StranitsaKnigi(kniga.BookId));

            var stack = new StackPanel();

            var img = new Image
            {
                Height = 140,
                Stretch = Stretch.UniformToFill,
                Margin = new Thickness(0, 0, 0, 8)
            };

            try
            {
                string put = IOPath.Combine(
                    System.AppDomain.CurrentDomain.BaseDirectory,
                    "Resources", "Covers",
                    IOPath.GetFileName(kniga.CoverPath ?? ""));

                if (File.Exists(put))
                {
                    img.Source = new BitmapImage(new System.Uri(put));
                }
                else
                {
                    img.Source = new BitmapImage(new System.Uri(
                        "pack://application:,,,/Resources/Images/no_cover.png"));
                }
            }
            catch
            {
             
            }

            var nazv = new TextBlock
            {
                Text = kniga.Title,
                Foreground = Brushes.White,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(8, 0, 8, 4),
                MaxHeight = 36
            };

            var avtor = new TextBlock
            {
                Text = kniga.AutorImya,
                Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#A78BFA")),
                FontSize = 11,
                Margin = new Thickness(8, 0, 8, 4),
                TextTrimming = TextTrimming.CharacterEllipsis
            };

                        var otsenka = new TextBlock
            {
                Text = kniga.SrednyayaOtsenka > 0
                    ? $"★ {kniga.SrednyayaOtsenka:F1}"
                    : "★ нет оценок",
                Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#FCD34D")),
                FontSize = 11,
                Margin = new Thickness(8, 0, 8, 6)
            };

            var btnSpisok = new Button
            {
                Content = "+ В список",
                Margin = new Thickness(8, 0, 8, 8),
                Padding = new Thickness(4, 4, 4, 4),
                FontSize = 11,
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#7C3AED")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };

            btnSpisok.PreviewMouseLeftButtonUp += (s, e) => e.Handled = true;
            btnSpisok.Click += (s, e) => DobavitVSpisok(kniga);

            stack.Children.Add(img);
            stack.Children.Add(nazv);
            stack.Children.Add(avtor);
            stack.Children.Add(otsenka);
            stack.Children.Add(btnSpisok);

            border.Child = stack;
            return border;
        }

        private void Primenit()
        {
            var rezultat = new List<Kniga>(vsekKnigi);

            if (!poiskPodkazka && !string.IsNullOrWhiteSpace(txtPoisk.Text))
            {
                string q = txtPoisk.Text.ToLower();
                rezultat = rezultat.Where(k =>
                    k.Title.ToLower().Contains(q) ||
                    k.AutorImya.ToLower().Contains(q)).ToList();
            }

            if (cbZhanr.SelectedItem is ComboBoxItem cbItem &&
                cbItem.Tag is int zhanrId && zhanrId > 0)
            {
                var poZhanru = KnigaDB.PoZhanru(zhanrId);
                var idsZhanra = new HashSet<int>(poZhanru.Select(k => k.BookId));
                rezultat = rezultat.Where(k => idsZhanra.Contains(k.BookId)).ToList();
            }

            if (cbSortir.SelectedItem is ComboBoxItem sortItem)
            {
                switch (sortItem.Tag?.ToString())
                {
                    case "name_asc":
                        rezultat = rezultat.OrderBy(k => k.Title).ToList();
                        break;
                    case "name_desc":
                        rezultat = rezultat.OrderByDescending(k => k.Title).ToList();
                        break;
                    case "rate_desc":
                        rezultat = rezultat.OrderByDescending(
                            k => k.SrednyayaOtsenka).ToList();
                        break;
                    case "rate_asc":
                        rezultat = rezultat.OrderBy(
                            k => k.SrednyayaOtsenka).ToList();
                        break;
                }
            }

            OtobrazitKnigi(rezultat);
        }

        private void DobavitVSpisok(Kniga kniga)
        {
            var okno = new Windows.OknoVyboraSpiskaWindow(kniga.BookId);
            okno.ShowDialog();
        }

        private void txtPoisk_GotFocus(object sender, RoutedEventArgs e)
        {
            if (poiskPodkazka)
            {
                txtPoisk.Text = "";
                txtPoisk.Foreground = Brushes.White;
                poiskPodkazka = false;
            }
        }

        private void txtPoisk_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPoisk.Text))
            {
                txtPoisk.Text = "Поиск по названию или автору...";
                txtPoisk.Foreground = new SolidColorBrush(Colors.Gray);
                poiskPodkazka = true;
            }
        }

        private void txtPoisk_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!poiskPodkazka) Primenit();
        }

        private void cbZhanr_SelectionChanged(object sender,
            SelectionChangedEventArgs e) => Primenit();

        private void cbSortir_SelectionChanged(object sender,
            SelectionChangedEventArgs e) => Primenit();

        private void btnSbros_Click(object sender, RoutedEventArgs e)
        {
            cbZhanr.SelectedIndex = 0;
            cbSortir.SelectedIndex = -1;
            txtPoisk.Text = "Поиск по названию или автору...";
            txtPoisk.Foreground = new SolidColorBrush(Colors.Gray);
            poiskPodkazka = true;
            OtobrazitKnigi(vsekKnigi);
        }
    }
}
