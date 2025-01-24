using System;
using System.Windows;

namespace CLib
{
    public partial class AddSupplierDialog : Window
    {
        private readonly BookstoreDBEntities2 _context;

        public AddSupplierDialog()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CompanyNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(ContactPersonTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneTextBox.Text) ||
                string.IsNullOrWhiteSpace(AddressTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

           
            string phonePattern = @"^(?:\+7|8)\d{10}$"; 
            if (!System.Text.RegularExpressions.Regex.IsMatch(PhoneTextBox.Text, phonePattern))
            {
                MessageBox.Show("Введите корректный номер телефона (например, +7XXXXXXXXXX или 8XXXXXXXXXX.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var supplier = new Suppliers
                {
                    CompanyName = CompanyNameTextBox.Text,
                    ContactPerson = ContactPersonTextBox.Text,
                    Phone = PhoneTextBox.Text,
                    Address = AddressTextBox.Text
                };

                _context.Suppliers.Add(supplier);
                _context.SaveChanges();
                MessageBox.Show("Поставщик добавлен успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении поставщика: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
