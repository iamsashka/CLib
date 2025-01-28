using System;
using System.Windows;

namespace CLib
{
    public partial class AddSupplierDialog : Window
    {
        private readonly BookstoreDBEntities2 _context;
        /// <summary>
        /// Конструктор класса, инициализирует окно и создаёт контекст базы данных (BookstoreDBEntities2), 
        /// через который будет происходить взаимодействие с базой данных для добавления поставщика.
        /// </summary>
        public AddSupplierDialog()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
        }
        /// <summary>
        /// Метод выполняет следующие действия:
        /// Проверяет введенные данные в текстовых полях(название компании, контактное лицо, телефон и адрес) на корректность с помощью регулярных выражений.
        /// Если данные некорректны, выводится сообщение об ошибке, и добавление поставщика отменяется.
        /// Если все поля корректны, создается новый объект Suppliers, который добавляется в контекст базы данных, и изменения сохраняются.Выводится сообщение об успешном добавлении поставщика.
        /// Устанавливается DialogResult = true, что означает успешное завершение действия.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CompanyNameTextBox.Text) ||
                !System.Text.RegularExpressions.Regex.IsMatch(CompanyNameTextBox.Text.Trim(), @"^[a-zA-Zа-яА-Я\s""]+$"))
            {
                MessageBox.Show("Название компании может содержать только буквы и пробелы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ContactPersonTextBox.Text) ||
                !System.Text.RegularExpressions.Regex.IsMatch(ContactPersonTextBox.Text.Trim(), @"^[a-zA-Zа-яА-Я\s]+$"))
            {
                MessageBox.Show("Контактное лицо может содержать только буквы и пробелы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string phonePattern = @"^(?:\+7|8)\d{10}$";
            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text) ||
                !System.Text.RegularExpressions.Regex.IsMatch(PhoneTextBox.Text.Trim(), phonePattern))
            {
                MessageBox.Show("Введите корректный номер телефона (например, +7XXXXXXXXXX или 8XXXXXXXXXX).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(AddressTextBox.Text) ||
                !System.Text.RegularExpressions.Regex.IsMatch(AddressTextBox.Text.Trim(), @"^[a-zA-Zа-яА-Я0-9\s,./]+$"))
            {
                MessageBox.Show("Адрес может содержать только буквы, цифры, пробелы, точки, запятые и слэши.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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
        /// <summary>
        /// Метод, который закрывает окно добавления поставщика без сохранения изменений, устанавливая DialogResult = false.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
