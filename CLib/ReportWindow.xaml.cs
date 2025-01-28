using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CLib
{
    public partial class ReportWindow : Window
    {
        private BookstoreDBEntities2 _context;
        /// <summary>
        /// Конструктор класса, инициализирует окно и устанавливает контекст базы данных.
        /// </summary>
        /// <param name="context"></param>
        public ReportWindow(BookstoreDBEntities2 context)
        {
            InitializeComponent();
            _context = context;
            LoadSalesData();
        }
        /// <summary>
        /// Загружает данные о продажах из базы данных и отображает их в ReportDataGrid.
        /// </summary>
        private void LoadSalesData()
        {
            var salesData = _context.Sales.Select(s => new
            {
                ID_Sale = s.ID_Sale,
                SaleDate = s.SaleDate,
                PaymentMethod = s.PaymentMethod
            }).ToList();

            ReportDataGrid.ItemsSource = salesData;
        }
        /// <summary>
        /// Обрабатывает нажатие кнопки для печати чека. Проверяет, существует ли ID продажи и вызывает метод для печати чека.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintReceiptButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int saleId)
            {
                PrintReceipt(saleId);
            }
            else
            {
                MessageBox.Show("ID продажи не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Создает и отображает чек для указанной продажи. В чеке выводятся данные о продаже (номер, дата, метод оплаты) и информация о продуктах, связанных с продажей.
        /// </summary>
        /// <param name="saleId"></param>
        private void PrintReceipt(int saleId)
        {
            var sale = _context.Sales.FirstOrDefault(s => s.ID_Sale == saleId);
            if (sale == null)
            {
                MessageBox.Show("Продажа не найдена.", "Ошибка");
                return;
            }

            var receipt = new System.Text.StringBuilder();
            receipt.AppendLine($"Чек №{sale.ID_Sale}");
            receipt.AppendLine($"Дата: {sale.SaleDate}");
            receipt.AppendLine($"Метод оплаты: {sale.PaymentMethod}");
            receipt.AppendLine("-------------------------");

            var products = _context.ProductSales.Where(ps => ps.Sales_ID == saleId).ToList();
            foreach (var item in products)
            {
                var product = _context.Products.FirstOrDefault(p => p.ID_Product == item.Product_ID);
                if (product != null)
                {
                    receipt.AppendLine($"{product.Name} x{item.Quantity}");
                }
            }

            MessageBox.Show(receipt.ToString(), "Чек");
        }
        /// <summary>
        /// Закрывает текущее окно, когда нажимается кнопка "Закрыть".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
    }
}
