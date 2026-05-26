using System.Windows;
using ShuT_and_Trololo.Data;

namespace ShuT_and_Trololo.Windows
{
    public partial class OknoVhoda : Window
    {
        public OknoVhoda()
        {
            InitializeComponent();
        }

        private void btnVhod_Click(object sender, RoutedEventArgs e)
        {
            panelVhod.Visibility = Visibility.Visible;
            panelReg.Visibility = Visibility.Collapsed;
        }

        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            panelVhod.Visibility = Visibility.Collapsed;
            panelReg.Visibility = Visibility.Visible;
        }

        private void BtnVoyti_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string parol = pbParol.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(parol))
            {
                MessageBox.Show("Заполните все поля.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var polz = PolzovatelDB.Voiti(login, parol);
            if (polz == null)
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка входа",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Sessiya.TekushiyPolzovatel = polz;

            var glavOkno = new GlavnoeOkno();
            glavOkno.Show();
            this.Close();
        }

        private void BtnZaregistrirovat_Click(object sender, RoutedEventArgs e)
        {
            string login = txtRegLogin.Text.Trim();
            string imya = txtRegImya.Text.Trim();
            string email = txtRegEmail.Text.Trim();
            string parol = pbRegParol.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(imya) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(parol))
            {
                MessageBox.Show("Заполните все поля.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (login.Length < 3)
            {
                MessageBox.Show("Логин должен быть не менее 3 символов.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Введите корректный Email (должен содержать @ и .)",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (parol.Length < 4)
            {
                MessageBox.Show("Пароль должен быть не менее 4 символов.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PolzovatelDB.LoginZanyat(login))
            {
                MessageBox.Show("Этот логин уже занят.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PolzovatelDB.EmailZanyat(email))
            {
                MessageBox.Show("Этот Email уже зарегистрирован.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PolzovatelDB.Zaregistrirovat(login, parol, email, imya);
            MessageBox.Show("Регистрация успешна! Теперь войдите в аккаунт.",
                "Готово", MessageBoxButton.OK, MessageBoxImage.Information);

            btnVhod_Click(null, null);
            txtLogin.Text = login;
        }
    }
}