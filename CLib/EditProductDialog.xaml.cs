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
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(AuthorTextBox.Text) ||
                string.IsNullOrWhiteSpace(GenreTextBox.Text) ||
                !decimal.TryParse(PriceTextBox.Text, out decimal price) ||
                !int.TryParse(StockQuantityTextBox.Text, out int stockQuantity))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _product.Name = NameTextBox.Text;
            _product.Author = AuthorTextBox.Text;
            _product.Category = GenreTextBox.Text;
            _product.UnitPrice = price;
            _product.StockQuantity = stockQuantity;

            this.DialogResult = true;
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
