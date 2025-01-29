using System;
using System.Linq;
using System.Windows;

namespace CLib
{
    /// <summary>
    /// Логика взаимодействия для EditSupplierDialog.xaml
    /// Окно для редактирования данных о поставщике.
    /// </summary>
    public partial class EditSupplierDialog : Window
    {
        private readonly Suppliers _supplier;
        private readonly BookstoreDBEntities2 _context;

        /// <summary>
        /// Инициализирует новый экземпляр окна редактирования поставщика.
        /// </summary>
        /// <param name="supplier">Объект поставщика для редактирования.</param>
        public EditSupplierDialog(Suppliers supplier)
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2(); // Инициализация контекста базы данных
            _supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
            DataContext = _supplier; // Привязываем данные поставщика к окну
        }

        /// <summary>
        /// Обработчик события кнопки "Сохранить".
        /// Сохраняет изменения поставщика в базе данных.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные о событии.</param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка, что объект _supplier был изменен
                var supplierInDb = _context.Suppliers.FirstOrDefault(s => s.ID_Supplier == _supplier.ID_Supplier);
                if (supplierInDb != null)
                {
                    // Обновляем данные в базе
                    supplierInDb.CompanyName = _supplier.CompanyName;
                    supplierInDb.ContactPerson = _supplier.ContactPerson;
                    supplierInDb.Phone = _supplier.Phone;
                    supplierInDb.Address = _supplier.Address;

                    _context.SaveChanges();
                    DialogResult = true; // Закрываем окно с результатом успешного редактирования
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Поставщик не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Обработчик события кнопки "Отмена".
        /// Закрывает окно без сохранения изменений.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные о событии.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
