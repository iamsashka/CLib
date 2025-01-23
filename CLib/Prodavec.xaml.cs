using CLib;
using System.Windows;
using System.Data.Entity;
using System.Linq;
using System;
using System.Windows.Ink;

namespace CLib
{
    public partial class Prodavec : Window
    {
        private BookstoreDBEntities2 _context;

        public Prodavec()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
            LoadSales();
            LoadFilters();
        }

        private void LoadSales()
        {
            // Загружаем все продажи с продуктами и магазинами
            var sales = _context.Sales
                                .Include(s => s.Products)
                                .Include(s => s.Stores)
                                .ToList();

            SalesDataGrid.ItemsSource = sales;
        }

        private void LoadFilters()
        {
            ProductComboBox.ItemsSource = _context.Products.ToList();
            StoreComboBox.ItemsSource = _context.Stores.ToList();
        }

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

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            LoadSales(); // Сброс фильтрации и отображение всех данных
        }

        private void AddSaleButton_Click(object sender, RoutedEventArgs e)
        {
            var addSaleDialog = new AddSaleDialog(); // Диалог для добавления/редактирования
            if (addSaleDialog.ShowDialog() == true)
            {
                LoadSales(); // Перезагрузка данных после добавления
            }
        }

        private void EditSaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (SalesDataGrid.SelectedItem is Sales selectedSale)
            {
                var editSaleDialog = new AddSaleDialog(); // Передаём выбранную продажу
                if (editSaleDialog.ShowDialog() == true)
                {
                    LoadSales(); // Перезагрузка данных после редактирования
                }
            }
            else
            {
                MessageBox.Show("Выберите продажу для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

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
                    LoadSales(); // Перезагрузка данных после удаления
                }
            }
            else
            {
                MessageBox.Show("Выберите продажу для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }


        
    }
}
