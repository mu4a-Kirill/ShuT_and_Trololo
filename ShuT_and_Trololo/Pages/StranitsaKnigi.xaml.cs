using System.Windows.Controls;

namespace ShuT_and_Trololo.Pages
{
    public partial class StranitsaKnigi : Page
    {
        private int knigaId;

        public StranitsaKnigi(int bookId)
        {
            InitializeComponent();
            knigaId = bookId;
        }
    }
}