using CLib;
using System.Windows;
using System.Data.Entity;
using System.Linq;
using System;

namespace CLib
{
    public partial class Prodavec : Window
    {
        private BookstoreDBEntities2 _context;
        /// <summary>
        /// Инициализирует окно Prodavec, устанавливает контекст данных _context и вызывает методы для загрузки данных (продаж и фильтров).
        /// </summary>
        public Prodavec()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
            LoadSales();
            LoadFilters();
        }
        /// <summary>
        /// Загружает данные о продажах из базы данных и отображает их в SalesDataGrid. 
        /// Для этого используются связанные данные из таблиц Products, Stores и Customers.
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
        /// аполняет элементы управления фильтрами (ProductComboBox и StoreComboBox) списками продуктов и магазинов из базы данных.
        /// </summary>
        private void LoadFilters()
        {
            ProductComboBox.ItemsSource = _context.Products.ToList();
            StoreComboBox.ItemsSource = _context.Stores.ToList();
        }
        /// <summary>
        /// Обрабатывает нажатие кнопки поиска. Фильтрует продажи в зависимости от выбранных фильтров (продукт и магазин).
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
        /// Обрабатывает нажатие кнопки для отображения истории продаж (перезагружает все продажи).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            LoadSales();
        }
        /// <summary>
        /// Обрабатывает нажатие кнопки для добавления новой продажи. 
        /// Открывает диалоговое окно для ввода данных продажи и, при подтверждении, перезагружает список продаж.
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
        /// Обрабатывает нажатие кнопки для редактирования выбранной продажи. Если продажа выбрана, открывает диалоговое окно редактирования.
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
        ///  Обрабатывает нажатие кнопки для удаления выбранной продажи. После подтверждения удаления, продажа удаляется из базы данных.
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
        /// Обрабатывает нажатие кнопки для перехода на главный экран приложения.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
