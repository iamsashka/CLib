using CLib;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace CLib
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow && window != this)
                {
                    window.Activate();
                    return;
                }
            }

            MainWindow newWindow = new MainWindow();
            newWindow.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show("Логин не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пароль не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (login.Length < 3)
            {
                MessageBox.Show("Логин должен содержать хотя бы 3 символа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать хотя бы 6 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string role = AuthenticateUser(login, password);

            if (!string.IsNullOrEmpty(role))
            {
                MessageBox.Show($"Вы успешно вошли в систему как {role}!", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);

                Window nextWindow = null;
                switch (role.ToLower())
                {
                    case "администратор":
                        nextWindow = new Glavnaya();
                        break;
                    case "продавец":
                        nextWindow = new Prodavec();
                        break;
                    case "руководитель":
                        nextWindow = new Otchetixaml();
                        break;
                    default:
                        MessageBox.Show("Роль не распознана. Обратитесь к администратору.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                nextWindow?.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string AuthenticateUser(string login, string password)
        {
            try
            {
                using (var context = new BookstoreDBEntities2())
                {
                    string hashedPassword = HashPassword(password);

                    var user = context.UserAccounts
                        .FirstOrDefault(u => u.Login == login && u.Password == hashedPassword);

                    if (user != null)
                    {
                        var role = context.Roles.FirstOrDefault(r => r.ID_Roles == user.Roles_ID);
                        return role?.RoleName;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подключении к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
