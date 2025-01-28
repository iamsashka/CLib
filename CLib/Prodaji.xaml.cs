using CLib;
using System.Windows;
using System.Data.Entity;
using System.Linq;
using System;

namespace CLib
{
    public partial class Prodaji : Window
    {
        private BookstoreDBEntities2 _context;
        /// <summary>
        /// Конструктор инициализирует компонент окна и загружает данные о продажах и фильтры (продукты и магазины).
        /// </summary>
        public Prodaji()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
            LoadSales();
            LoadFilters();
        }
        /// <summary>
        /// Загружает все продажи из базы данных с учетом связей с продуктами, магазинами и покупателями.
        /// </summary>
        private void LoadSales()
        {
            var sales = _context.Sales
                                .Include(s => s.Products)
                                .Include(s => s.Stores)
                                .Include(s => s.Customers)
                                .ToList();

            SalesDataGrid.ItemsSource = sales;
        }
        /// <summary>
        /// Метод заполняет комбинированные списки ProductComboBox и StoreComboBox доступными продуктами и магазинами для фильтрации.
        /// </summary>
        private void LoadFilters()
        {
            ProductComboBox.ItemsSource = _context.Products.ToList();
            StoreComboBox.ItemsSource = _context.Stores.ToList();
        }
        /// <summary>
        /// Обрабатывает событие нажатия на кнопку поиска. Фильтрует данные о продажах по выбранным продуктам и магазинам.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var filteredSales = _context.Sales.AsQueryable();

            if (ProductComboBox.SelectedItem != null)
            {
                var selectedProduct = (Products)ProductComboBox.SelectedItem;
                filteredSales = filteredSales.Where(s => s.Products.ID_Product == selectedProduct.ID_Product);
            }

            if (StoreComboBox.SelectedItem != null)
            {
                var selectedStore = (Stores)StoreComboBox.SelectedItem;
                filteredSales = filteredSales.Where(s => s.Stores.Store_ID == selectedStore.Store_ID);
            }

            SalesDataGrid.ItemsSource = filteredSales.Include(s => s.Products).Include(s => s.Stores).ToList();
        }
        /// <summary>
        ///  Обрабатывает событие нажатия на кнопку "История". Загружает все продажи без фильтрации.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            LoadSales();
        }
        /// <summary>
        /// Открывает окно для добавления новой продажи.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSaleButton_Click(object sender, RoutedEventArgs e)
        {
            var addSaleDialog = new AddSaleDialog();
            if (addSaleDialog.ShowDialog() == true)
            {
                LoadSales();
            }
        }
        /// <summary>
        /// Открывает окно для редактирования выбранной продажи.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditSaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (SalesDataGrid.SelectedItem is Sales selectedSale)
            {
                var editSaleDialog = new AddSaleDialog();
                if (editSaleDialog.ShowDialog() == true)
                {
                    LoadSales();
                }
            }
            else
            {
                MessageBox.Show("Выберите продажу для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        /// <summary>
        ///  Удаляет выбранную продажу после подтверждения пользователя.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteSaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (SalesDataGrid.SelectedItem is Sales selectedSale)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить продажу с ID {selectedSale.ID_Sale}?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _context.Sales.Remove(selectedSale);
                    _context.SaveChanges();
                    LoadSales();
                }
            }
            else
            {
                MessageBox.Show("Выберите продажу для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        /// <summary>
        /// Открывает главное окно профиля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        /// <summary>
        /// Открывает окно "Главное".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            Glavnaya glavWindow = new Glavnaya();
            glavWindow.Show();
            this.Close();
        }
        /// <summary>
        /// Открывает окно "Товары".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TovarButton_Click(object sender, RoutedEventArgs e)
        {
            Tovar tovarWindow = new Tovar();
            tovarWindow.Show();
            this.Close();
        }
        /// <summary>
        /// Открывает окно "Поставки".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PostavkiButton_Click(object sender, RoutedEventArgs e)
        {
            Postavki postavWindow = new Postavki();
            postavWindow.Show();
            this.Close();
        }
        /// <summary>
        /// Открывает окно "Продажи".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ProdajiButton_Click(object sender, RoutedEventArgs e)
        {
            Prodaji saleWindow = new Prodaji();
            saleWindow.Show();
            this.Close();
        }
        /// <summary>
        /// Открывает окно "Клиенты".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KlientsButton_Click(object sender, RoutedEventArgs e)
        {
            Clientts clientWindow = new Clientts();
            clientWindow.Show();
            this.Close();
        }
    }
}
