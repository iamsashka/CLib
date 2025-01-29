using System;
using System.Windows;

namespace CLib
{
    public partial class EditProductDialog : Window
    {
        private Products _product;
        /// <summary>
        /// Конструктор класса, который принимает объект товара (Products) для редактирования. 
        /// В нем инициализируется компоненты окна, и устанавливается текущий товар как DataContext, чтобы привязать поля ввода к свойствам товара.
        /// product: Товар, который нужно отредактировать (тип Products).
        /// </summary>
        /// <param name="product"></param>
        public EditProductDialog(Products product)
        {
            InitializeComponent();
            _product = product;
            this.DataContext = _product;
        }
        /// <summary>
        ///  Метод, который проверяет, что все поля корректно заполнены. 
        ///  Если все проверки пройдены, данные товара обновляются, а результат диалога устанавливается в true, чтобы подтвердить изменения.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
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

                _product.Name = NameTextBox.Text;
                _product.Author = AuthorTextBox.Text;
                _product.Category = GenreTextBox.Text;
                _product.UnitPrice = price;
                _product.StockQuantity = stockQuantity;

                DialogResult = true;
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при редактировании товара: {ex.Message}");
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
        /// Метод, который закрывает диалоговое окно без сохранения изменений, устанавливая результат диалога в false.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
