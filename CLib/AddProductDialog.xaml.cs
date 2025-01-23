using System;
using System.Windows;

namespace CLib
{
    public partial class AddProductDialog : Window
    {
        public Products Product { get; private set; }

        public AddProductDialog()
        {
            InitializeComponent();
            Product = new Products(); // Инициализируем новый объект Product
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка на корректность ввода
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(AuthorTextBox.Text) ||
                    string.IsNullOrWhiteSpace(GenreTextBox.Text) ||
                    !decimal.TryParse(PriceTextBox.Text, out decimal price) ||
                    !int.TryParse(StockQuantityTextBox.Text, out int stockQuantity))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Заполняем свойства модели
                Product.Name = NameTextBox.Text;
                Product.Author = AuthorTextBox.Text;
                Product.Category = GenreTextBox.Text;
                Product.UnitPrice = price;
                Product.StockQuantity = stockQuantity;
                Product.ShelfQuantity = stockQuantity; // Например, по умолчанию на полке столько же
                Product.LastReceivedDate = DateTime.Now;

                // Устанавливаем результат успешного завершения диалога
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Отменяем добавление товара
            this.DialogResult = false;
        }
    }
}
