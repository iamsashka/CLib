using CLib;
using System.Windows.Controls;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CLib
{
    public partial class AddShipmentDialog : Window
    {
        private readonly BookstoreDBEntities2 _context;
        public List<Shipments> NewShipments { get; set; }
        /// <summary>
        /// Инициализирует окно, создаёт контекст базы данных (BookstoreDBEntities2), 
        /// инициализирует список новых партий (NewShipments) и загружает существующие партии из базы данных с помощью метода LoadShipments().
        /// </summary>
        public AddShipmentDialog()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
            NewShipments = new List<Shipments>();
            LoadShipments();
        }
        /// <summary>
        /// Метод, который загружает список всех партий из базы данных и отображает их в ShipmentsDataGrid. 
        /// Если возникнет ошибка при загрузке данных, выводится сообщение об ошибке.
        /// </summary>
        private void LoadShipments()
        {
            try
            {
                var shipments = _context.Shipments.ToList();
                ShipmentsDataGrid.ItemsSource = shipments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке партий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        ///Этот метод сохраняет партии в базе данных:
        ///Перебирает все записи из ShipmentsDataGrid.
        ///Для каждой партии проверяет её данные с помощью метода ValidateShipment(). Если есть ошибки валидации, выводится сообщение с перечнем ошибок.
        ///Находит товар в базе данных по ID продукта и увеличивает его количество на количество поступившей партии.
        ///Если партия не добавлена в контекст, она добавляется в базу данных.
        ///После сохранения данных вызывается SaveChanges() для сохранения изменений в базе данных.
        ///Если все данные сохранены успешно, появляется сообщение об успешном сохранении, и окно закрывается.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (Shipments shipment in ShipmentsDataGrid.Items)
                {
                    if (shipment == null) continue;

                    var validationErrors = ValidateShipment(shipment);
                    if (validationErrors.Any())
                    {
                        MessageBox.Show($"Ошибка в записи: {string.Join(", ", validationErrors)}",
                                        "Ошибка валидации",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                        continue;
                    }

                    var product = _context.Products.FirstOrDefault(p => p.ID_Product == shipment.Product_ID);
                    if (product == null)
                    {
                        MessageBox.Show($"Товар с ID {shipment.Product_ID} не найден!",
                                        "Ошибка",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                        continue;
                    }

                    product.StockQuantity += shipment.Quantity;

                    if (!_context.Shipments.Local.Contains(shipment))
                    {
                        _context.Shipments.Add(shipment);
                    }
                }

                _context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении партий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Метод валидации данных партии:
        /// Проверяет, что ID товара больше нуля.
        /// Проверяет, что количество больше нуля.
        /// Проверяет, что дата поступления не является пустой и не указывает на будущее.
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns> Возвращает список строк с ошибками валидации.</returns>
        private List<string> ValidateShipment(Shipments shipment)
        {
            var errors = new List<string>();

            if (shipment.Product_ID <= 0)
                errors.Add("ID Товара должен быть положительным числом");

            if (shipment.Quantity <= 0)
                errors.Add("Количество должно быть больше нуля");

            if (shipment.ShipmentDate == default)
                errors.Add("Дата поступления не указана");
            else if (shipment.ShipmentDate > DateTime.Now)
                errors.Add("Дата поступления не может быть в будущем");

            return errors;
        }
        /// <summary>
        /// Закрывает окно без сохранения изменений, устанавливая DialogResult = false.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
