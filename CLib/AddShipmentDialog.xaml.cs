using CLib;
using System.Windows.Controls;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CLib
{
    /// <summary>
    /// Логика взаимодействия для AddShipmentDialog.xaml
    /// </summary>
    public partial class AddShipmentDialog : Window
    {
        private readonly BookstoreDBEntities2 _context;
        public List<Shipments> NewShipments { get; set; }
        public AddShipmentDialog()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
            NewShipments = new List<Shipments>();
            LoadShipments();
        }


        // Загрузка данных существующих партий
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

        // Обработка кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (Shipments shipment in ShipmentsDataGrid.Items)
                {
                    if (shipment == null) continue;

                    // Валидация данных
                    var validationErrors = ValidateShipment(shipment);
                    if (validationErrors.Any())
                    {
                        MessageBox.Show($"Ошибка в записи: {string.Join(", ", validationErrors)}",
                                        "Ошибка валидации",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                        continue;
                    }

                    // Проверяем, существует ли товар в базе
                    var product = _context.Products.FirstOrDefault(p => p.ID_Product == shipment.Product_ID);
                    if (product == null)
                    {
                        MessageBox.Show($"Товар с ID {shipment.Product_ID} не найден!",
                                        "Ошибка",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                        continue;
                    }

                    // Обновляем количество на складе
                    product.StockQuantity += shipment.Quantity;

                    // Добавляем партию, если она не добавлена
                    if (!_context.Shipments.Local.Contains(shipment))
                    {
                        _context.Shipments.Add(shipment);
                    }
                }

                // Сохраняем изменения в базе данных
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

        private List<string> ValidateShipment(Shipments shipment)
        {
            var errors = new List<string>();

            // Проверка ID товара
            if (shipment.Product_ID <= 0)
                errors.Add("ID Товара должен быть положительным числом");

            // Проверка количества
            if (shipment.Quantity <= 0)
                errors.Add("Количество должно быть больше нуля");

            // Проверка даты поступления
            if (shipment.ShipmentDate == default)
                errors.Add("Дата поступления не указана");
            else if (shipment.ShipmentDate > DateTime.Now)
                errors.Add("Дата поступления не может быть в будущем");

            return errors;
        }


        // Обработка кнопки "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
