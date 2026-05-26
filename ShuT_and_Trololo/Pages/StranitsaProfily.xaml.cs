using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using ShuT_and_Trololo.Data;
using ShuT_and_Trololo.Models;
using IOPath = System.IO.Path;

namespace ShuT_and_Trololo.Pages
{
    public partial class StranitsaProfily : Page
    {
        public StranitsaProfily()
        {
            InitializeComponent();
            ZagruzitProfil();
        }

        private void ZagruzitProfil()
        {
            var polz = Sessiya.TekushiyPolzovatel;

            txtImya.Text = polz.DisplayName;
            txtLogin.Text = "Логин: " + polz.Login;
            txtEmail.Text = "Email: " + polz.Email;
            txtRol.Text = "Роль: " + polz.RoleName;

            ZagruzitAvatar(polz.AvatarPath);

            if (polz.IsFrozen)
            {
                panelZamorozka.Visibility = Visibility.Visible;
                txtPrichinaZam.Text = "Причина: " +
                    (string.IsNullOrWhiteSpace(polz.FreezeReason)
                        ? "не указана"
                        : polz.FreezeReason);
            }

            if (polz.RoleName == "Читатель")
            {
                bool estZayavka = ZayavkaDB.EstZayavkaNaAvtora(polz.UserId);
                if (!estZayavka)
                    btnZayavkaAvtor.Visibility = Visibility.Visible;
            }

            ZagruzitOtzyvy();
        }

        private void ZagruzitAvatar(string avatarPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(avatarPath))
                {
                    string put = IOPath.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Resources", "Images",
                        IOPath.GetFileName(avatarPath));

                    if (File.Exists(put))
                    {
                        imgAvatar.Source = new BitmapImage(new Uri(put));
                        return;
                    }
                }

            }
            catch { }
        }

        private void ZagruzitOtzyvy()
        {
            panelOtzyvy.Children.Clear();
            var otzyvy = OtzyvDB.GetOtzyvyPolzovatelya(
                Sessiya.TekushiyPolzovatel.UserId);

            if (otzyvy.Count == 0)
            {
                panelOtzyvy.Children.Add(new TextBlock
                {
                    Text = "Вы ещё не оставляли отзывов",
                    Foreground = Brushes.Gray,
                    FontSize = 14
                });
                return;
            }

            foreach (var otzyv in otzyvy)
            {
                var card = new Border
                {
                    Background = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#2D2D3F")),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(16),
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var stack = new StackPanel();

                string knigaNazv = KnigaDB.GetNazvanieKnigi(otzyv.BookId);

                stack.Children.Add(new TextBlock
                {
                    Text = "📖 " + knigaNazv,
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#A78BFA")),
                    FontSize = 13,
                    FontWeight = FontWeights.SemiBold,
                    Margin = new Thickness(0, 0, 0, 4)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"★ {otzyv.Rating}/10  ·  {otzyv.CreatedAt:dd.MM.yyyy}",
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#FCD34D")),
                    FontSize = 12,
                    Margin = new Thickness(0, 0, 0, 6)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = otzyv.ReviewText,
                    Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#D1D5DB")),
                    FontSize = 13,
                    TextWrapping = TextWrapping.Wrap
                });

                card.Child = stack;
                panelOtzyvy.Children.Add(card);
            }
        }

        private void btnZayavkaAvtor_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show(
                "Подать заявку на роль Автора?\n\nАдминистратор рассмотрит её в ближайшее время.",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                ZayavkaDB.PodatZayavkuNaAvtora(Sessiya.TekushiyPolzovatel.UserId);
                MessageBox.Show("Заявка отправлена! Ожидайте решения.",
                    "Готово", MessageBoxButton.OK, MessageBoxImage.Information);

                btnZayavkaAvtor.Visibility = Visibility.Collapsed;
            }
        }

        private void btnOsporit_Click(object sender, RoutedEventArgs e)
        {
            var okno = new Windows.OknoZhaloby("заморозку аккаунта", (reason) =>
            {
                ZayavkaDB.PodatZayavkuNaRazmorozku(
                    Sessiya.TekushiyPolzovatel.UserId, reason);
            });
            okno.ShowDialog();
        }
    }
}