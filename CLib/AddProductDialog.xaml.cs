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
                    ShowError("Название товара может содержать только буквы.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(AuthorTextBox.Text) || AuthorTextBox.Text.Trim().Split(' ').Length > 5 || !System.Text.RegularExpressions.Regex.IsMatch(AuthorTextBox.Text, @"^[a-zA-Zа-яА-Я\s]+$"))
                {
                    ShowError("Автор должен содержать до 5 слов и только буквы.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(GenreTextBox.Text) || !System.Text.RegularExpressions.Regex.IsMatch(GenreTextBox.Text, @"^[a-zA-Zа-яА-Я]+$"))
                {
                    ShowError("Жанр может содержать только буквы.");
                    return;
                }

                if (!TryParsePositiveDecimal(PriceTextBox.Text, out decimal price))
                {
                    ShowError("Цена должна быть положительным числом больше 0.");
                    return;
                }

                if (!TryParsePositiveInt(StockQuantityTextBox.Text, out int stockQuantity))
                {
                    ShowError("Количество на складе должно быть положительным числом больше 0.");
                    return;
                }

                Product.Name = NameTextBox.Text;
                Product.Author = AuthorTextBox.Text;
                Product.Category = GenreTextBox.Text;
                Product.UnitPrice = price;
                Product.StockQuantity = stockQuantity;
                Product.ShelfQuantity = stockQuantity;
                Product.LastReceivedDate = DateTime.Now;

                DialogResult = true;
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при добавлении товара: {ex.Message}");
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

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
