using System.Windows;

namespace ShuT_and_Trololo.Windows
{
    public partial class OknoVyboraSpiskaWindow : Window
    {
        private int knigaId;

        public OknoVyboraSpiskaWindow(int bookId)
        {
            InitializeComponent();
            knigaId = bookId;
        }
    }
}