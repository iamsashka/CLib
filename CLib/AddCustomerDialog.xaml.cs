using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace CLib
{
    public partial class AddCustomerDialog : Window
    {
        public Customers Customer { get; set; }
        /// <summary>
        /// Конструктор класса, который инициализирует компоненты окна и создает новый объект Customer типа Customers. 
        /// Также устанавливает контекст данных окна, связывая его с объектом Customer.
        /// </summary>
        public AddCustomerDialog()
        {
            InitializeComponent();
            Customer = new Customers();
            this.DataContext = Customer;
        }
        /// <summary>
        /// Метод, который выполняется при нажатии кнопки "Сохранить". Он проверяет корректность формата телефонного номера с помощью метода IsPhoneValid.
        /// Если номер некорректен, появляется сообщение об ошибке. Если номер корректен, устанавливается результат диалога в true, и окно закрывается.
        /// sender: объект, который вызывает этот метод (в данном случае кнопка, на которую нажали).
        /// e: данные события, содержащие информацию о событии(например, параметры события нажатия кнопки).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static List<int> ExistingCardNumbers = new List<int>(); // список существующих номеров карт

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsNameValid(Customer.FirstName))
                {
                    ShowError("Имя может содержать только русские буквы и пробелы.");
                    return;
                }

                if (!IsNameValid(Customer.LastName))
                {
                    ShowError("Фамилия может содержать только русские буквы и пробелы.");
                    return;
                }

                if (!IsPhoneValid(Customer.Phone))
                {
                    ShowError("Неверный формат номера телефона. Укажите номер в формате +7XXXXXXXXXX или 8XXXXXXXXXX.");
                    return;
                }

                if (!IsDiscountValid(Customer.DiscountRate.ToString()))
                {
                    ShowError("Скидка должна быть числом от 0 до 1 (например, 0.15 для 15% скидки).");
                    return;
                }

                if (!IsDiscountCardNumberValid(Customer.DiscountCardNumber))
                {
                    ShowError("Номер дисконтной карты должен быть числом от 1000 до 5000.");
                    return;
                }

                if (ExistingCardNumbers.Contains(Customer.DiscountCardNumber))
                {
                    ShowError("Данный номер дисконтной карты уже используется.");
                    return;
                }

                // Добавляем номер карты в список
                ExistingCardNumbers.Add(Customer.DiscountCardNumber);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при добавлении клиента: {ex.Message}");
            }
        }
        private bool TryParsePositiveDecimal(string input, out decimal result)
        {
            return decimal.TryParse(input, out result) && result > 0;
        }

        private bool TryParsePositiveInt(string input, out int result)
        {
            return int.TryParse(input, out result) && result > 0;
        }

        /// <summary>
        /// Проверяет, что строка состоит только из русских букв и пробелов.
        /// </summary>
        private bool IsNameValid(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var namePattern = @"^[а-яА-ЯёЁ\s]+$";
            return Regex.IsMatch(name, namePattern);
        }
        /// <summary>
        /// Проверяет, что скидка является числом в диапазоне от 0 до 1.
        /// </summary>
        private bool IsDiscountValid(string discountRate)
        {
            return double.TryParse(discountRate, out double discount) && discount >= 0 && discount <= 1;
        }
        /// <summary>
        /// Проверяет, что номер дисконтной карты является числом в диапазоне от 1000 до 5000.
        /// </summary>
        private bool IsDiscountCardNumberValid(int cardNumber)
        {
            // Проверка диапазона чисел от 1000 до 5000
            return cardNumber >= 1000 && cardNumber <= 5000;
        }


        /// <summary>
        /// Показывает сообщение об ошибке пользователю.
        /// </summary>
        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        /// <summary>
        /// Метод, который выполняется при нажатии кнопки "Отмена". Он устанавливает результат диалога в false и закрывает окно.
        /// sender: объект, который вызывает этот метод (кнопка "Отмена").
        /// e: данные события, содержащие информацию о событии(например, параметры события нажатия кнопки).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        /// <summary>
        /// Метод, который проверяет корректность формата телефонного номера. 
        /// Он принимает строку и возвращает true, если номер соответствует шаблону формата "+7XXXXXXXXXX" или "8XXXXXXXXXX". 
        /// В противном случае возвращает false.
        /// phone: строка, представляющая телефонный номер, который требуется проверить.
        /// </summary>
        /// <param name="phone"></param>
        /// <returns>Метод возвращает true, если номер телефона соответствует формату, и false в противном случае.</returns>
        private bool IsPhoneValid(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;
            
            var phonePattern = @"^(?:\+7|8)\d{10}$";
            return Regex.IsMatch(phone, phonePattern);
        }
    }
}
