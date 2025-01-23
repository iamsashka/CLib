using System.Windows;

namespace CLib
{
    public partial class AddCustomerDialog : Window
    {
        // Определяем свойство Customer, которое будет содержать данные нового клиента
        public Customers Customer { get; set; }

        // Конструктор, инициализируем Customer
        public AddCustomerDialog()
        {
            InitializeComponent();
            Customer = new Customers();  // Инициализируем новый объект клиента
            this.DataContext = Customer;  // Привязываем его к DataContext окна
        }

        // Событие для сохранения данных и закрытия окна
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;  // Устанавливаем результат в true, чтобы окно вернуло данные
            this.Close();  // Закрываем окно
        }

        // Событие для отмены добавления клиента
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;  // Устанавливаем результат в false, если отмена
            this.Close();  // Закрываем окно
        }
    }
}
