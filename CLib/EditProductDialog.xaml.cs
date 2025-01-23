using System.Windows;

namespace CLib
{
    public partial class EditProductDialog : Window
    {
        private Products _product; // Локальная ссылка на редактируемый объект

        public EditProductDialog(Products product)
        {
            InitializeComponent();
            _product = product;
            this.DataContext = _product; // Привязка объекта для редактирования
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем ввод данных перед сохранением
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(AuthorTextBox.Text) ||
                string.IsNullOrWhiteSpace(GenreTextBox.Text) ||
                !decimal.TryParse(PriceTextBox.Text, out decimal price) ||
                !int.TryParse(StockQuantityTextBox.Text, out int stockQuantity))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Обновляем свойства объекта
            _product.Name = NameTextBox.Text;
            _product.Author = AuthorTextBox.Text;
            _product.Category = GenreTextBox.Text;
            _product.UnitPrice = price;
            _product.StockQuantity = stockQuantity;

            this.DialogResult = true; // Указываем, что изменения сохранены
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // Отмена изменений
        }
    }
}
