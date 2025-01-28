using System.Text;
using System.Windows;

namespace CLib
{
    public partial class InvoiceDialog : Window
    {
        /// <summary>
        /// Принимает объект Suppliers и инициализирует окно. В конструкторе вызывается метод GenerateInvoice, 
        /// который генерирует накладную для выбранного поставщика.
        /// </summary>
        /// <param name="supplier"></param>
        public InvoiceDialog(Suppliers supplier)
        {
            InitializeComponent();
            GenerateInvoice(supplier);
        }
        /// <summary>
        /// Генерирует строку накладной для выбранного поставщика, добавляя информацию о компании, контактных данных и товарах:
        /// Добавляет название компании, контактное лицо, телефон и адрес.
        /// Перебирает все поставки этого поставщика и для каждой поставки добавляет информацию о товаре (название товара, количество, дата поступления).
        /// </summary>
        /// <param name="supplier"></param>
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
        /// <summary>
        /// Закрывает окно накладной. Это выполняется при нажатии на кнопку "Закрыть".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
