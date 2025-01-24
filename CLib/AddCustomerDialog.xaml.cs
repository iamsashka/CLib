using System.Text.RegularExpressions;
using System.Windows;

namespace CLib
{
    public partial class AddCustomerDialog : Window
    {
        public Customers Customer { get; set; }

        public AddCustomerDialog()
        {
            InitializeComponent();
            Customer = new Customers();
            this.DataContext = Customer;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsPhoneValid(Customer.Phone))
            {
                MessageBox.Show("Неверный формат номера телефона. Укажите номер в формате +7XXXXXXXXXX или 8XXXXXXXXXX.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private bool IsPhoneValid(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            
            var phonePattern = @"^(?:\+7|8)\d{10}$";
            return Regex.IsMatch(phone, phonePattern);
        }
    }
}
