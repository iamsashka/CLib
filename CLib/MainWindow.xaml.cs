using CLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace CLib
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, открыто ли уже окно MainWindow
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow && window != this)
                {
                    // Если уже есть другое окно MainWindow, фокусируем его
                    window.Activate();
                    return;
                }
            }

            // Если окна MainWindow нет, создаем и открываем его
            MainWindow newWindow = new MainWindow();
            newWindow.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text; // Логин из TextBox
            string password = PasswordBox.Password; // Пароль из PasswordBox

            // Проверка на пустоту логина и пароля
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

            // Проверка минимальной длины логина и пароля
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

            // Проверка учетных данных и получение роли
            string role = AuthenticateUser(login, password);

            if (!string.IsNullOrEmpty(role))
            {
                MessageBox.Show($"Вы успешно вошли в систему как {role}!", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);

                // Открытие соответствующего окна в зависимости от роли
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

                // Открытие нового окна
                nextWindow?.Show();

                // Закрытие текущего окна
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод для аутентификации пользователя
        private string AuthenticateUser(string login, string password)
        {
            try
            {
                using (var context = new BookstoreDBEntities2())
                {
                    // Хеширование пароля
                    string hashedPassword = HashPassword(password);

                    // Поиск пользователя в базе данных по логину и паролю
                    var user = context.UserAccounts
                        .FirstOrDefault(u => u.Login == login && u.Password == hashedPassword);

                    if (user != null)
                    {
                        // Возврат названия роли пользователя
                        var role = context.Roles.FirstOrDefault(r => r.ID_Roles == user.Roles_ID);
                        return role?.RoleName;
                    }
                    else
                    {
                        return null; // Если пользователь не найден
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подключении к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        // Метод для хеширования пароля
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);

                // Преобразуем байты в строку Base64 для сравнения с базой данных
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
