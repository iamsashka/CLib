using System;
using System.Linq;
using System.Windows;

namespace CLib
{
    public partial class EditCustomerDialog : Window
    {
        private Customers _customer;

        // Конструктор, принимающий клиента, который будет редактироваться
        public EditCustomerDialog(Customers customer)
        {
            InitializeComponent();
            _customer = customer;

            // Инициализируем поля данными из клиента
            FirstNameTextBox.Text = customer.FirstName;
            LastNameTextBox.Text = customer.LastName;
            PhoneTextBox.Text = customer.Phone;
            DiscountCardNumberTextBox.Text = customer.DiscountCardNumber.ToString();
            DiscountRateTextBox.Text = customer.DiscountRate.ToString();
        }

        // Событие для кнопки Сохранить
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Обновляем данные клиента на основе введенных значений
                _customer.FirstName = FirstNameTextBox.Text;
                _customer.LastName = LastNameTextBox.Text;
                _customer.Phone = PhoneTextBox.Text;
                _customer.DiscountCardNumber = int.Parse(DiscountCardNumberTextBox.Text);
                _customer.DiscountRate = double.Parse(DiscountRateTextBox.Text);

                // Используем новый контекст для сохранения изменений
                using (var context = new BookstoreDBEntities2())
                {
                    // Прикрепляем объект клиента к новому контексту
                    var existingCustomer = context.Customers.Find(_customer.ID_Customer);

                    if (existingCustomer != null)
                    {
                        // Обновляем данные в базе
                        context.Entry(existingCustomer).CurrentValues.SetValues(_customer);
                        context.SaveChanges();

                        // Закрываем окно диалога с результатом успеха
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Клиент не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Событие для кнопки Отмена
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Просто закрываем окно без изменений
            DialogResult = false;
            Close();
        }
    }
}
