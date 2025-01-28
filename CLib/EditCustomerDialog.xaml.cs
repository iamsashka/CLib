using System;
using System.Linq;
using System.Windows;

namespace CLib
{
    public partial class EditCustomerDialog : Window
    {
        private Customers _customer;
        /// <summary>
        /// Конструктор класса EditCustomerDialog, который инициализирует компоненты интерфейса и 
        /// заполняет текстовые поля данными выбранного клиента для редактирования.
        /// </summary>
        /// <param name="customer"></param>
        public EditCustomerDialog(Customers customer)
        {
            InitializeComponent();
            _customer = customer;
            FirstNameTextBox.Text = customer.FirstName;
            LastNameTextBox.Text = customer.LastName;
            PhoneTextBox.Text = customer.Phone;
            DiscountCardNumberTextBox.Text = customer.DiscountCardNumber.ToString();
            DiscountRateTextBox.Text = customer.DiscountRate.ToString();
        }
        /// <summary>
        /// Метод, который срабатывает при нажатии кнопки "Сохранить". 
        /// Он сохраняет изменения, сделанные пользователем в полях редактирования, и обновляет данные клиента в базе данных. 
        /// Если клиент найден, данные обновляются, и окно закрывается. В случае ошибок появляется сообщение с описанием ошибки.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _customer.FirstName = FirstNameTextBox.Text;
                _customer.LastName = LastNameTextBox.Text;
                _customer.Phone = PhoneTextBox.Text;
                _customer.DiscountCardNumber = int.Parse(DiscountCardNumberTextBox.Text);
                _customer.DiscountRate = double.Parse(DiscountRateTextBox.Text);

                using (var context = new BookstoreDBEntities2())
                {
                    var existingCustomer = context.Customers.Find(_customer.ID_Customer);

                    if (existingCustomer != null)
                    {
                        context.Entry(existingCustomer).CurrentValues.SetValues(_customer);
                        context.SaveChanges();
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
        /// <summary>
        /// Метод, который срабатывает при нажатии кнопки "Отмена". Он отменяет редактирование и закрывает окно без сохранения изменений.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
