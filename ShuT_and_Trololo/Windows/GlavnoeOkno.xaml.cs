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
using ShuT_and_Trololo.Data;
using ShuT_and_Trololo.Pages;

namespace ShuT_and_Trololo.Windows
{
    public partial class GlavnoeOkno : Window
    {
        public GlavnoeOkno()
        {
            InitializeComponent();
            NastroyitSidebar();
            mainFrame.Navigate(new StranitsaKataloga());
        }

        private void NastroyitSidebar()
        {
            if (Sessiya.EtoAdmin)
                btnSideAdmin.Visibility = Visibility.Visible;

            if (Sessiya.EtoAvtor)
                btnSideAvtor.Visibility = Visibility.Visible;

            if (Sessiya.AkkZamorozhen)
                btnSideZamorozka.Visibility = Visibility.Visible;
        }

        private void btnSideKatalog_Click(object sender, RoutedEventArgs e)
            => mainFrame.Navigate(new StranitsaKataloga());

        private void btnSideSpisok_Click(object sender, RoutedEventArgs e)
            => mainFrame.Navigate(new StranitsaSpiskov());

        private void btnSideAvtor_Click(object sender, RoutedEventArgs e)
            => mainFrame.Navigate(new StranitsaAvtora());

        private void btnSideAdmin_Click(object sender, RoutedEventArgs e)
            => mainFrame.Navigate(new StranitsaAdmina());

        private void btnSideZamorozka_Click(object sender, RoutedEventArgs e)
            => mainFrame.Navigate(new StranitsaProfily());

        private void btnSideProfil_Click(object sender, RoutedEventArgs e)
            => mainFrame.Navigate(new StranitsaProfily());
    }
}
