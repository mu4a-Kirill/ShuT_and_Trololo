using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using ShuT_and_Trololo.Data;
using ShuT_and_Trololo.Models;
using IOPath = System.IO.Path;

namespace ShuT_and_Trololo.Pages
{
    public partial class StranitsaSpiskov : Page
    {
        private List<Kniga> tekushieKnigi = new List<Kniga>();
        private bool poiskPodkazka = true;

        private Dictionary<int, WrapPanel> paneli;

        public StranitsaSpiskov()
        {
            InitializeComponent();

            paneli = new Dictionary<int, WrapPanel>
            {
                { 1, panelZabrosil },
                { 2, panelVPlanah },
                { 3, panelChtenie },
                { 4, panelProchitano }
            };

            ZagruzitZhanry();
            ZagruzitAktivnuyuVkladku();
        }

        private void ZagruzitZhanry()
        {
            cbZhanr.Items.Clear();
            cbZhanr.Items.Add(new ComboBoxItem { Content = "Все жанры", Tag = 0 });

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
                private void ZagruzitAktivnuyuVkladku()
        {
            if (tabSpiskov.SelectedItem is TabItem tab && tab.Tag is string tagStr)
            {
                int sectionId = int.Parse(tagStr);
                tekushieKnigi = SpisokDB.GetKnigiPolzovatelya(
                    Sessiya.TekushiyPolzovatel.UserId, sectionId);
                OtobrazitKnigi(tekushieKnigi, sectionId);
            }
        }

        private void OtobrazitKnigi(List<Kniga> knigi, int sectionId)
        {
            if (!paneli.ContainsKey(sectionId)) return;

            var panel = paneli[sectionId];
            panel.Children.Clear();

            if (knigi.Count == 0)
            {
                panel.Children.Add(new TextBlock
                {
                    Text = "В этом списке пока нет книг",
                    Foreground = Brushes.Gray,
                    FontSize = 15,
                    Margin = new Thickness(20)
                });
                return;
            }

            foreach (var kniga in knigi)
            {
                panel.Children.Add(SozdatKartochku(kniga, sectionId));
            }
        }

        private Border SozdatKartochku(Kniga kniga, int tekSectionId)
        {
            var border = new Border
            {
                Width = 155,
                Height = 270,
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
            {
                if (e.OriginalSource is Button) return;
                NavigationService.Navigate(new StranitsaKnigi(kniga.BookId));
            };

            var stack = new StackPanel();

            var img = new Image { Height = 130, Stretch = Stretch.UniformToFill };
            try
            {
                string put = IOPath.Combine(
                    System.AppDomain.CurrentDomain.BaseDirectory,
                    "Resources", "Covers",
                    IOPath.GetFileName(kniga.CoverPath ?? ""));

                if (File.Exists(put))
                    img.Source = new BitmapImage(new System.Uri(put));
                else
                    img.Source = new BitmapImage(new System.Uri(
                        "pack://application:,,,/Resources/Images/no_cover.png"));
            }
            catch { }

            var nazv = new TextBlock
            {
                Text = kniga.Title,
                Foreground = Brushes.White,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(8, 6, 8, 2),
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

            var btnPerem = new Button
            {
                Content = "Переместить...",
                Margin = new Thickness(8, 0, 8, 8),
                Padding = new Thickness(4, 4, 4, 4),
                FontSize = 11,
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#3D3D5F")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };

            btnPerem.Click += (s, e) => OtkrytMenuPeremescheniya(kniga, tekSectionId);

            stack.Children.Add(img);
            stack.Children.Add(nazv);
            stack.Children.Add(avtor);
            stack.Children.Add(otsenka);
            stack.Children.Add(btnPerem);

            border.Child = stack;
            return border;
        }

        private void OtkrytMenuPeremescheniya(Kniga kniga, int tekSectionId)
        {
            var okno = new Windows.OknoVyboraSpiskaWindow(kniga.BookId);
            okno.ShowDialog();
            ZagruzitAktivnuyuVkladku();
        }

        private void Primenit()
        {
            if (tabSpiskov.SelectedItem is TabItem tab && tab.Tag is string tagStr)
            {
                int sectionId = int.Parse(tagStr);
                var rezultat = new List<Kniga>(tekushieKnigi);

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
                    var ids = new System.Collections.Generic.HashSet<int>(
                        poZhanru.Select(k => k.BookId));
                    rezultat = rezultat.Where(k => ids.Contains(k.BookId)).ToList();
                }

                if (cbSortir.SelectedItem is ComboBoxItem sortItem)
                {
                    switch (sortItem.Tag?.ToString())
                    {
                        case "name_asc":
                            rezultat = rezultat.OrderBy(k => k.Title).ToList(); break;
                        case "name_desc":
                            rezultat = rezultat.OrderByDescending(k => k.Title).ToList(); break;
                        case "rate_desc":
                            rezultat = rezultat.OrderByDescending(k => k.SrednyayaOtsenka).ToList(); break;
                        case "rate_asc":
                            rezultat = rezultat.OrderBy(k => k.SrednyayaOtsenka).ToList(); break;
                    }
                }

                OtobrazitKnigi(rezultat, sectionId);
            }
        }

        private void tabSpiskov_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (poiskPodkazka == false)
            {
                txtPoisk.Text = "Поиск по названию или автору...";
                txtPoisk.Foreground = new SolidColorBrush(Colors.Gray);
                poiskPodkazka = true;
            }
            ZagruzitAktivnuyuVkladku();
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

        private void cbZhanr_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => Primenit();

        private void cbSortir_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => Primenit();

        private void btnSbros_Click(object sender, RoutedEventArgs e)
        {
            cbZhanr.SelectedIndex = 0;
            cbSortir.SelectedIndex = -1;
            txtPoisk.Text = "Поиск по названию или автору...";
            txtPoisk.Foreground = new SolidColorBrush(Colors.Gray);
            poiskPodkazka = true;
            OtobrazitKnigi(tekushieKnigi,
                int.Parse(((TabItem)tabSpiskov.SelectedItem).Tag.ToString()));
        }
    }
}