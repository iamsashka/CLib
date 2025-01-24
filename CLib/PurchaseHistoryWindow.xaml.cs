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

        public PurchaseHistoryWindow(int customerId)
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
            CustomerId = customerId;
            LoadPurchaseHistory();
        }

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

        private void ReloadData()
        {
            _context.Dispose();
            _context = new BookstoreDBEntities2();
            LoadPurchaseHistory();
        }
    }
}
