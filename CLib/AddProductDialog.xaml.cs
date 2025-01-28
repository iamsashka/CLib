using System;
using System.Linq;
using System.Windows;

namespace CLib
{
    public partial class AddProductDialog : Window
    {
        public Products Product { get; private set; }
        /// <summary>
        /// Конструктор,который инициализирует компоненты диалогового окна и создает новый объект товара
        /// </summary>
        public AddProductDialog()
        {
            InitializeComponent();
            Product = new Products();
        }
        /// <summary>
        /// Метод, который выполняет проверку введенных данных в форму (название, автор, жанр, цена, количество на складе).
        /// Если все данные корректны, создается новый объект товара, и результат диалога устанавливается в true. 
        /// Если данные некорректны, показываются сообщения об ошибке.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) || !System.Text.RegularExpressions.Regex.IsMatch(NameTextBox.Text, @"^[a-zA-Zа-яА-Я]+$"))
                {
                    MessageBox.Show("Название товара может содержать только буквы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var authorText = AuthorTextBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(authorText) || authorText.Split(' ').Length > 5 || !System.Text.RegularExpressions.Regex.IsMatch(authorText, @"^[a-zA-Zа-яА-Я\s]+$"))
                {
                    MessageBox.Show("Автор должен содержать до 5 слов, разделённых пробелами и только буквы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(GenreTextBox.Text) || !System.Text.RegularExpressions.Regex.IsMatch(GenreTextBox.Text, @"^[a-zA-Zа-яА-Я]+$"))
                {
                    MessageBox.Show("Жанр может содержать только буквы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(PriceTextBox.Text) || !decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
                {
                    MessageBox.Show("Цена должна быть положительным числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(StockQuantityTextBox.Text) || !int.TryParse(StockQuantityTextBox.Text, out int stockQuantity) || stockQuantity <= 0)
                {
                    MessageBox.Show("Количество на складе должно быть положительным числом и больше 0.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Product.Name = NameTextBox.Text;
                Product.Author = AuthorTextBox.Text;
                Product.Category = GenreTextBox.Text;
                Product.UnitPrice = price;
                Product.StockQuantity = stockQuantity;
                Product.ShelfQuantity = stockQuantity;
                Product.LastReceivedDate = DateTime.Now;

                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Метод проверяет, состоит ли строка только из букв. Используется для проверки названия товара, автора и жанра.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool IsLettersOnly(string input)
        {
            return input.All(c => char.IsLetter(c));
        }
        /// <summary>
        /// ОБработчик событий с помощью которого,акрывается диалоговое окно без внесения изменений, устанавливая результат диалога в false.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
