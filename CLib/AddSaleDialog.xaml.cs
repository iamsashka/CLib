using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CLib
{
    public partial class AddSaleDialog : Window
    {
        private BookstoreDBEntities2 _context;
        public ObservableCollection<Products> Products { get; set; }
        public ObservableCollection<Stores> Stores { get; set; }
        public ObservableCollection<Customers> Customers { get; set; }

        public AddSaleDialog()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
            Products = new ObservableCollection<Products>();
            Stores = new ObservableCollection<Stores>();
            Customers = new ObservableCollection<Customers>();

            DataContext = this;  // Связываем DataContext с текущим окном, чтобы данные биндовались

            LoadData();
        }

        private void LoadData()
        {
            // Загрузка данных в ObservableCollection
            var products = _context.Products.ToList();
            var stores = _context.Stores.ToList();
            var customers = _context.Customers.ToList();

            // Обновление ObservableCollection, это автоматически обновит интерфейс
            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(product);
            }

            Stores.Clear();
            foreach (var store in stores)
            {
                Stores.Add(store);
            }

            Customers.Clear();
            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем данные с формы
                var selectedProduct = (Products)ProductComboBox.SelectedItem;
                var selectedStore = (Stores)StoreComboBox.SelectedItem;
                var selectedCustomer = (Customers)CustomerComboBox.SelectedItem;
                var paymentMethod = PaymentMethodTextBox.Text;
                var saleDate = SaleDatePicker.SelectedDate.Value;
                var totalCost = decimal.Parse(TotalCostTextBox.Text);
                var quantity = int.Parse(QuantityTextBox.Text);

                // Создаем новую запись о продаже
                var sale = new Sales
                {
                    Product_ID = selectedProduct.ID_Product,
                    Customer_ID = selectedCustomer.ID_Customer,
                    Store_ID = selectedStore?.Store_ID, // Может быть null, если магазин не выбран
                    PaymentMethod = paymentMethod,
                    SaleDate = saleDate,
                    TotalCost = (int)totalCost
                };

                // Сохраняем запись о продаже
                _context.Sales.Add(sale);
                _context.SaveChanges();

                // После сохранения добавляем товары в таблицу ProductSales
                var productSale = new ProductSales
                {
                    Sales_ID = sale.ID_Sale,
                    Product_ID = selectedProduct.ID_Product,
                    Quantity = quantity
                };

                _context.ProductSales.Add(productSale);
                _context.SaveChanges();

                // Обновляем количество товара на складе
                selectedProduct.StockQuantity -= quantity;
                _context.SaveChanges();

                // Обновление данных в интерфейсе
                LoadData();  // Перезагружаем данные в ComboBox'ы

                MessageBox.Show("Продажа успешно сохранена!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
