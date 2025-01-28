using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace CLib
{
    public partial class PurchaseHistoryWindow : Window
    {
        private BookstoreDBEntities2 _context;
        public int CustomerId { get; }
        /// <summary>
        ///  Конструктор класса, который инициализирует компоненты интерфейса, 
        ///  устанавливает идентификатор клиента (который передается в параметре), и загружает историю покупок для этого клиента.
        /// </summary>
        /// <param name="customerId"></param>
        public PurchaseHistoryWindow(int customerId)
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
            CustomerId = customerId;
            LoadPurchaseHistory();
        }
        /// <summary>
        /// Метод, который загружает и отображает историю покупок для указанного клиента. Он получает все продажи, связанные с клиентом (по CustomerId), 
        /// а также информацию о продукции, связанной с каждой продажей. Затем эти данные преобразуются в список, 
        /// который можно привязать к элементу DataGrid для отображения.
        /// </summary>
        private void LoadPurchaseHistory()
        {
            var sales = _context.Sales
                .Where(s => s.Customer_ID == CustomerId)
                .Include(s => s.ProductSales.Select(ps => ps.Products))
                .ToList();

            var purchaseHistory = sales.Select(s => new
            {
                s.SaleDate,
                s.PaymentMethod,
                s.TotalCost,
                ProductNames = string.Join(", ", s.ProductSales.Select(ps => ps.Products.Name))
            }).ToList();

            PurchaseHistoryDataGrid.ItemsSource = purchaseHistory;
        }
        /// <summary>
        /// Метод для перезагрузки данных. Он закрывает текущий контекст базы данных (_context), 
        /// создает новый экземпляр контекста и снова загружает историю покупок для клиента.
        /// </summary>
        private void ReloadData()
        {
            _context.Dispose();
            _context = new BookstoreDBEntities2();
            LoadPurchaseHistory();
        }
    }
}
