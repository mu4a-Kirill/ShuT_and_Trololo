using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using ShuT_and_Trololo.Data;
using ShuT_and_Trololo.Models;

namespace ShuT_and_Trololo.Windows
{
    public partial class OknoRedaktirovaniyaKnigi : Window
    {
        private Kniga tekKniga;       
        private string vybranCover;    
        private List<Zhanr> vseZhanry;

        public OknoRedaktirovaniyaKnigi(Kniga kniga)
        {
            InitializeComponent();
            tekKniga = kniga;
            ZagruzitFormu();
        }

        private void ZagruzitFormu()
        {
            vseZhanry = AvtorDB.GetVseZhanry();

            if (tekKniga != null)
            {
                Title = "Редактировать книгу";
                txtZagolovok.Text = "Редактировать книгу";
                txtTitle.Text = tekKniga.Title;
                txtDesc.Text = tekKniga.Description ?? "";
                txtCoverPath.Text = tekKniga.CoverPath ?? "Файл не выбран";
                txtContent.Text = tekKniga.Content ?? "";
                vybranCover = tekKniga.CoverPath;
            }

            List<int> tekZhanryIds = tekKniga != null
                ? GetZhanryIdsKnigi(tekKniga.BookId)
                : new List<int>();

            foreach (var z in vseZhanry)
            {
                var cb = new System.Windows.Controls.CheckBox
                {
                    Content = z.GenreName,
                    Tag = z.GenreId,
                    Foreground = System.Windows.Media.Brushes.White,
                    FontSize = 13,
                    Margin = new Thickness(0, 0, 16, 8),
                    IsChecked = tekZhanryIds.Contains(z.GenreId)
                };
                panelZhanry.Children.Add(cb);
            }
        }

        private List<int> GetZhanryIdsKnigi(int bookId)
        {
            var ids = new List<int>();
            var zhanry = KnigaDB.GetZhanryKnigi(bookId);
            foreach (var z in zhanry)
                ids.Add(z.GenreId);
            return ids;
        }

        private void btnVybratOblozhku_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите обложку",
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (dialog.ShowDialog() == true)
            {
                vybranCover = dialog.FileName;
                txtCoverPath.Text = System.IO.Path.GetFileName(vybranCover);
            }
        }

        private void btnSokhranit_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string desc = txtDesc.Text.Trim();
            string content = txtContent.Text.Trim();

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Введите название книги.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (title.Length < 2)
            {
                MessageBox.Show("Название слишком короткое.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var vybranZhanryIds = new List<int>();
            foreach (System.Windows.Controls.CheckBox cb in panelZhanry.Children)
            {
                if (cb.IsChecked == true && cb.Tag is int zid)
                    vybranZhanryIds.Add(zid);
            }

            string coverFileName = "";
            if (!string.IsNullOrEmpty(vybranCover) && File.Exists(vybranCover))
            {
                string papka = System.IO.Path.Combine(
                    System.AppDomain.CurrentDomain.BaseDirectory,
                    "Resources", "Covers");

                if (!Directory.Exists(papka))
                    Directory.CreateDirectory(papka);

                coverFileName = System.IO.Path.GetFileName(vybranCover);
                string dest = System.IO.Path.Combine(papka, coverFileName);

                if (!File.Exists(dest))
                    File.Copy(vybranCover, dest);
            }
            else if (tekKniga != null)
            {
                coverFileName = tekKniga.CoverPath ?? "";
            }

            if (tekKniga == null)
            {
                int newId = AvtorDB.DobavitKnigu(
                    title, desc, coverFileName, content,
                    Sessiya.TekushiyPolzovatel.UserId);

                AvtorDB.ObnvitZhanry(newId, vybranZhanryIds);
                MessageBox.Show("Книга добавлена!", "Готово",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                AvtorDB.ObnovitKnigu(tekKniga.BookId, title, desc, coverFileName, content);
                AvtorDB.ObnvitZhanry(tekKniga.BookId, vybranZhanryIds);
                MessageBox.Show("Книга сохранена!", "Готово",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            Close();
        }

        private void btnOtmena_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}