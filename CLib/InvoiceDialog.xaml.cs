using System.Text;
using System.Windows;

namespace CLib
{
    public partial class InvoiceDialog : Window
    {
        public InvoiceDialog(Suppliers supplier)
        {
            InitializeComponent();
            GenerateInvoice(supplier);
        }

        private void GenerateInvoice(Suppliers supplier)
        {
            var invoice = new StringBuilder();
            invoice.AppendLine($"Компания: {supplier.CompanyName}");
            invoice.AppendLine($"Контактное лицо: {supplier.ContactPerson}");
            invoice.AppendLine($"Телефон: {supplier.Phone}");
            invoice.AppendLine($"Адрес: {supplier.Address}");
            invoice.AppendLine("Товары:");

            foreach (var shipment in supplier.Shipments)
            {
                invoice.AppendLine($"- {shipment.Products.Name}, Количество: {shipment.Quantity}, Дата: {shipment.ShipmentDate.ToShortDateString()}");
            }

            InvoiceText.Text = invoice.ToString();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
