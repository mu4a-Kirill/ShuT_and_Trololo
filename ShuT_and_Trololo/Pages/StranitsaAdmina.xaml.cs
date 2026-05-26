using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ShuT_and_Trololo.Data;
using ShuT_and_Trololo.Models;

namespace ShuT_and_Trololo.Pages
{
    public partial class StranitsaAdmina : Page
    {
        public StranitsaAdmina()
        {
            InitializeComponent();
            ZagruzitVse();
        }

        private void ZagruzitVse()
        {
            ZagruzitZhaloby();
            ZagruzitRazmorozku();
            ZagruzitZayavkiAvtora();
            ZagruzitZamorozhennye();
            ZagruzitPolzovateley();
        }


        private void ZagruzitZhaloby()
        {
            panelZhaloby.Children.Clear();
            var zhaloby = ZhalobaDB.GetVseZhaloby();

            if (zhaloby.Count == 0)
            {
                panelZhaloby.Children.Add(PustoyTekst("Жалоб нет"));
                return;
            }

            foreach (var zh in zhaloby)
                panelZhaloby.Children.Add(SozdatStrokuZhaloby(zh));
        }

        private Border SozdatStrokuZhaloby(Zhaloba zh)
        {
            string tip = zh.BookId.HasValue ? "книгу"
                       : zh.ReviewId.HasValue ? "отзыв"
                       : zh.AuthorId.HasValue ? "автора"
                       : "неизвестно";

            string opisanie = $"Жалоба на {tip} от {zh.PolzovatelImya}\n" +
                              $"Причина: {zh.Reason ?? "не указана"}";

            return SozdatStrokuZayavki(opisanie,
                () => {
                    ZhalobaDB.PrinyatZhalobu(zh.ComplaintId, zh.BookId, zh.ReviewId, zh.AuthorId);
                    ZagruzitZhaloby();
                },
                () => {
                    ZhalobaDB.OtklonZhalobu(zh.ComplaintId);
                    ZagruzitZhaloby();
                });
        }


        private void ZagruzitRazmorozku()
        {
            panelRazmorozka.Children.Clear();
            var zayavki = ZayavkaDB.GetZayavkiNaRazmorozku();

            if (zayavki.Count == 0)
            {
                panelRazmorozka.Children.Add(PustoyTekst("Заявок на разморозку нет"));
                return;
            }

            foreach (var z in zayavki)
                panelRazmorozka.Children.Add(SozdatStrokuRazm(z));
        }

        private Border SozdatStrokuRazm(Zayavka z)
        {
            string chto = z.BookId.HasValue
                ? $"книгу «{z.KnigaTitle}»"
                : "аккаунт";

            string opisanie = $"Заявка на разморозку ({chto}) от {z.PolzovatelImya}\n" +
                              $"Причина: {z.Reason ?? "не указана"}";

            return SozdatStrokuZayavki(opisanie,
                () => {
                    ZayavkaDB.PrinyatRazmorozku(z.ApplicationId, z.UserId, z.BookId);
                    ZagruzitRazmorozku();
                },
                () => {
                    ZayavkaDB.OtklonRazmorozku(z.ApplicationId);
                    ZagruzitRazmorozku();
                });
        }

        private void ZagruzitZayavkiAvtora()
        {
            panelZayavki.Children.Clear();
            var zayavki = ZayavkaDB.GetZayavkiNaAvtora();

            if (zayavki.Count == 0)
            {
                panelZayavki.Children.Add(PustoyTekst("Заявок на роль Автора нет"));
                return;
            }

            foreach (var z in zayavki)
                panelZayavki.Children.Add(SozdatStrokuAvtorZayav(z));
        }

        private Border SozdatStrokuAvtorZayav(Zayavka z)
        {
            string opisanie = $"Заявка на роль Автора\nПользователь: {z.PolzovatelImya}\n" +
                              $"Дата: {z.CreatedAt:dd.MM.yyyy}";

            return SozdatStrokuZayavki(opisanie,
                () => {
                    ZayavkaDB.PrinyatZayavkuAvtora(z.ApplicationId, z.UserId);
                    ZagruzitZayavkiAvtora();
                },
                () => {
                    ZayavkaDB.OtklonZayavkuAvtora(z.ApplicationId);
                    ZagruzitZayavkiAvtora();
                });
        }

        private Border SozdatStrokuZayavki(string tekst,
            System.Action onPrinyat, System.Action onOtklon)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#2D2D3F")),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14, 12, 14, 12),
                Margin = new Thickness(0, 0, 0, 8)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition
            { Width = GridLength.Auto });

            var tb = new TextBlock
            {
                Text = tekst,
                Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#D1D5DB")),
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center
            };

            var btnPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };

            var btnPrin = new Button
            {
                Content = "✔ Принять",
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#059669")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(10, 5, 10, 5),
                FontSize = 12,
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 0, 8, 0)
            };
            btnPrin.Click += (s, e) =>
            {
                var res = MessageBox.Show("Принять?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes) onPrinyat();
            };

            var btnOtk = new Button
            {
                Content = "✖ Отклонить",
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#DC2626")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(10, 5, 10, 5),
                FontSize = 12,
                Cursor = System.Windows.Input.Cursors.Hand
            };
            btnOtk.Click += (s, e) =>
            {
                var res = MessageBox.Show("Отклонить?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes) onOtklon();
            };

            btnPanel.Children.Add(btnPrin);
            btnPanel.Children.Add(btnOtk);

            Grid.SetColumn(tb, 0);
            Grid.SetColumn(btnPanel, 1);
            grid.Children.Add(tb);
            grid.Children.Add(btnPanel);

            border.Child = grid;
            return border;
        }

        private void ZagruzitZamorozhennye()
        {
            panelZamKnigi.Children.Clear();
            var zamKnigi = AdminDB.GetZamorozhKnigi();

            if (zamKnigi.Count == 0)
                panelZamKnigi.Children.Add(PustoyTekst("Замороженных книг нет"));
            else
                foreach (var k in zamKnigi)
                    panelZamKnigi.Children.Add(SozdatStrokuZamKniga(k));

            panelZamPolz.Children.Clear();
            var vsePolz = AdminDB.GetVsehPolzovateley();
            bool estZamPolz = false;
            foreach (var p in vsePolz)
            {
                if (p.IsFrozen)
                {
                    panelZamPolz.Children.Add(SozdatStrokuZamPolz(p));
                    estZamPolz = true;
                }
            }
            if (!estZamPolz)
                panelZamPolz.Children.Add(PustoyTekst("Замороженных пользователей нет"));

            panelZamOtzyvy.Children.Clear();
            var zamOtzyvy = AdminDB.GetZamorozhOtzyvy();

            if (zamOtzyvy.Count == 0)
                panelZamOtzyvy.Children.Add(PustoyTekst("Замороженных отзывов нет"));
            else
                foreach (var o in zamOtzyvy)
                    panelZamOtzyvy.Children.Add(SozdatStrokuZamOtzyv(o));
        }

        private Border SozdatStrokuZamKniga(Kniga k)
        {
            return SozdatStrokuSRazmoroz(
                $"❄ {k.Title} (автор: {k.AutorImya})",
                () => { AdminDB.RazmorozitKnigu(k.BookId); ZagruzitZamorozhennye(); });
        }

        private Border SozdatStrokuZamPolz(Polzovatel p)
        {
            return SozdatStrokuSRazmoroz(
                $"❄ {p.DisplayName} [{p.RoleName}]\nПричина: {p.FreezeReason ?? "не указана"}",
                () => { AdminDB.RazmorozitPolzovatelya(p.UserId); ZagruzitZamorozhennye(); });
        }

        private Border SozdatStrokuZamOtzyv(Otzyv o)
        {
            return SozdatStrokuSRazmoroz(
                $"❄ {o.PolzovatelImya}: \"{o.ReviewText?.Substring(0, System.Math.Min(50, o.ReviewText.Length))}...\"",
                () => { AdminDB.RazmorozitOtzyv(o.ReviewId); ZagruzitZamorozhennye(); });
        }

        private Border SozdatStrokuSRazmoroz(string tekst, System.Action onRazmoroz)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#1E3A5F")),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14, 12, 14, 12),
                Margin = new Thickness(0, 0, 0, 8)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition
            { Width = GridLength.Auto });

            var tb = new TextBlock
            {
                Text = tekst,
                Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#93C5FD")),
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap
            };

            var btn = new Button
            {
                Content = "Разморозить",
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#1D4ED8")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(10, 5, 10, 5),
                FontSize = 12,
                Cursor = System.Windows.Input.Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Center
            };
            btn.Click += (s, e) =>
            {
                var res = MessageBox.Show("Разморозить?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes) onRazmoroz();
            };

            Grid.SetColumn(tb, 0);
            Grid.SetColumn(btn, 1);
            grid.Children.Add(tb);
            grid.Children.Add(btn);

            border.Child = grid;
            return border;
        }

        private void ZagruzitPolzovateley()
        {
            panelPolzovateli.Children.Clear();
            var polzovateli = AdminDB.GetVsehPolzovateley();

            foreach (var p in polzovateli)
                panelPolzovateli.Children.Add(SozdatStrokuPolzovatelya(p));
        }

        private Border SozdatStrokuPolzovatelya(Polzovatel p)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#2D2D3F")),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14, 12, 14, 12),
                Margin = new Thickness(0, 0, 0, 8)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition
            { Width = GridLength.Auto });

            var info = new StackPanel();
            info.Children.Add(new TextBlock
            {
                Text = p.DisplayName,
                Foreground = Brushes.White,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold
            });
            info.Children.Add(new TextBlock
            {
                Text = $"@{p.Login}  ·  {p.Email}  ·  {p.RoleName}" +
                       (p.IsFrozen ? "  ❄ Заморожен" : ""),
                Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#9CA3AF")),
                FontSize = 12
            });

            var btnPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };

            var btnRol = new Button
            {
                Content = "Роль",
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#7C3AED")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(10, 5, 10, 5),
                FontSize = 12,
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 0, 6, 0)
            };
            btnRol.Click += (s, e) => MenyatRol(p);

            var btnPar = new Button
            {
                Content = "Пароль",
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#3D3D5F")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(10, 5, 10, 5),
                FontSize = 12,
                Cursor = System.Windows.Input.Cursors.Hand
            };
            btnPar.Click += (s, e) => MenyatParol(p);

            btnPanel.Children.Add(btnRol);
            btnPanel.Children.Add(btnPar);

            Grid.SetColumn(info, 0);
            Grid.SetColumn(btnPanel, 1);
            grid.Children.Add(info);
            grid.Children.Add(btnPanel);

            border.Child = grid;
            return border;
        }

        private void MenyatRol(Polzovatel p)
        {
            var okno = new Windows.OknoVyboraRoli(p.UserId, p.RoleName);
            okno.ShowDialog();
            ZagruzitPolzovateley();
        }

        private void MenyatParol(Polzovatel p)
        {
            var okno = new Windows.OknoSmenyParola(p.UserId);
            okno.ShowDialog();
        }

        private TextBlock PustoyTekst(string tekst)
        {
            return new TextBlock
            {
                Text = tekst,
                Foreground = Brushes.Gray,
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 8)
            };
        }
    }
}