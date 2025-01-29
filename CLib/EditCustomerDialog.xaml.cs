using System;
using System.Linq;
using System.Windows;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace CLib
{
    public partial class EditCustomerDialog : Window
    {
        private Customers _customer;
        private static readonly List<int> ExistingCardNumbers = new List<int>();

        public EditCustomerDialog(Customers customer)
        {
            InitializeComponent();
            _customer = customer;

            // Заполняем поля для редактирования
            FirstNameTextBox.Text = customer.FirstName;
            LastNameTextBox.Text = customer.LastName;
            PhoneTextBox.Text = customer.Phone;
            DiscountCardNumberTextBox.Text = customer.DiscountCardNumber.ToString();
            DiscountRateTextBox.Text = customer.DiscountRate.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string firstName = FirstNameTextBox.Text.Trim();
                string lastName = LastNameTextBox.Text.Trim();
                string phone = PhoneTextBox.Text.Trim();
                string cardNumberText = DiscountCardNumberTextBox.Text.Trim();
                string discountRateText = DiscountRateTextBox.Text.Trim();

                // Валидация полей
                if (!IsNameValid(firstName))
                {
                    ShowError("Имя может содержать только русские буквы и пробелы.");
                    return;
                }

                if (!IsNameValid(lastName))
                {
                    ShowError("Фамилия может содержать только русские буквы и пробелы.");
                    return;
                }

                if (!IsPhoneValid(phone))
                {
                    ShowError("Неверный формат номера телефона. Укажите номер в формате +7XXXXXXXXXX или 8XXXXXXXXXX.");
                    return;
                }

                if (!int.TryParse(cardNumberText, out int cardNumber) || !IsDiscountCardNumberValid(cardNumber))
                {
                    ShowError("Номер дисконтной карты должен быть числом от 1005 до 5000.");
                    return;
                }

                if (!double.TryParse(discountRateText, out double discountRate) || !IsDiscountValid(discountRate))
                {
                    ShowError("Скидка должна быть числом от 0 до 1 (например, 0.15 для 15% скидки).");
                    return;
                }

                if (ExistingCardNumbers.Contains(cardNumber) && cardNumber != _customer.DiscountCardNumber)
                {
                    ShowError("Данный номер дисконтной карты уже используется.");
                    return;
                }

                // Применяем изменения
                _customer.FirstName = firstName;
                _customer.LastName = lastName;
                _customer.Phone = phone;
                _customer.DiscountCardNumber = cardNumber;
                _customer.DiscountRate = discountRate;

                using (var context = new BookstoreDBEntities2())
                {
                    var existingCustomer = context.Customers.Find(_customer.ID_Customer);

                    if (existingCustomer != null)
                    {
                        context.Entry(existingCustomer).CurrentValues.SetValues(_customer);
                        context.SaveChanges();
                        ExistingCardNumbers.Add(cardNumber); // Добавляем новый номер карты
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        ShowError("Клиент не найден в базе данных.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool IsNameValid(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var namePattern = @"^[а-яА-ЯёЁ\s]+$";
            return Regex.IsMatch(name, namePattern);
        }

        private bool IsPhoneValid(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            var phonePattern = @"^(?:\+7|8)\d{10}$";
            return Regex.IsMatch(phone, phonePattern);
        }

        private bool IsDiscountCardNumberValid(int cardNumber)
        {
            return cardNumber >= 1005 && cardNumber <= 5000;
        }

        private bool IsDiscountValid(double discountRate)
        {
            return discountRate >= 0 && discountRate <= 1;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
