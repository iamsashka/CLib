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
        /// <summary>
        /// Конструктор инициализирует компонент окна, создаёт контекст базы данных и коллекции для продуктов, магазинов и клиентов. 
        /// Также выполняется загрузка данных из базы данных.
        /// LoadData(): Загружает данные о продуктах, магазинах и клиентах в соответствующие коллекции.
        /// </summary>
        public AddSaleDialog()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
            Products = new ObservableCollection<Products>();
            Stores = new ObservableCollection<Stores>();
            Customers = new ObservableCollection<Customers>();

            DataContext = this;

            LoadData();
        }
        /// <summary>
        /// Загружает данные из базы данных для продуктов, магазинов и клиентов и добавляет их в соответствующие коллекции (Products, Stores, Customers).
        /// </summary>
        private void LoadData()
        {
            var products = _context.Products.ToList();
            var stores = _context.Stores.ToList();
            var customers = _context.Customers.ToList();

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
        /// <summary>
        /// Обрабатывает событие нажатия на кнопку сохранения, создавая запись о продаже в базе данных.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedProduct = (Products)ProductComboBox.SelectedItem;
                var selectedStore = (Stores)StoreComboBox.SelectedItem;
                var selectedCustomer = (Customers)CustomerComboBox.SelectedItem;
                var paymentMethod = PaymentMethodTextBox.Text;
                var saleDate = SaleDatePicker.SelectedDate.Value;
                var totalCost = decimal.Parse(TotalCostTextBox.Text);
                var quantity = int.Parse(QuantityTextBox.Text);

                var sale = new Sales
                {
                    Product_ID = selectedProduct.ID_Product,
                    Customer_ID = selectedCustomer.ID_Customer,
                    Store_ID = selectedStore?.Store_ID,
                    PaymentMethod = paymentMethod,
                    SaleDate = saleDate,
                    TotalCost = (int)totalCost
                };

                _context.Sales.Add(sale);
                _context.SaveChanges();

                var productSale = new ProductSales
                {
                    Sales_ID = sale.ID_Sale,
                    Product_ID = selectedProduct.ID_Product,
                    Quantity = quantity
                };

                _context.ProductSales.Add(productSale);
                _context.SaveChanges();

                selectedProduct.StockQuantity -= quantity;
                _context.SaveChanges();

                LoadData();

                MessageBox.Show("Продажа успешно сохранена!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }
        /// <summary>
        /// Пересчитывает итоговую стоимость продажи на основе выбранного продукта и введённого количества.
        /// </summary>
        private void RecalculateTotalCost()
        {
            try
            {
                var selectedProduct = (Products)ProductComboBox.SelectedItem;

                if (selectedProduct == null || string.IsNullOrWhiteSpace(QuantityTextBox.Text))
                {
                    TotalCostTextBox.Text = "0.00";
                    return;
                }

                if (int.TryParse(QuantityTextBox.Text, out int quantity) && quantity >= 0)
                {
                    decimal totalCost = selectedProduct.UnitPrice * quantity;
                    TotalCostTextBox.Text = totalCost.ToString("0.00");
                }
                else
                {
                    TotalCostTextBox.Text = "0.00"; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при пересчёте суммы: {ex.Message}");
            }
        }
        /// <summary>
        ///  Обрабатывает событие изменения текста в поле ввода количества товара.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuantityTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            RecalculateTotalCost();
        }
        /// <summary>
        /// Обрабатывает событие изменения выбранного продукта в комбинированном списке.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RecalculateTotalCost();
        }

        /// <summary>
        /// Обрабатывает событие нажатия на кнопку "Отмена" и закрывает окно.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
