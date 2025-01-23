using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CLib
{
    public partial class Postavki : Window
    {
        private BookstoreDBEntities2 _context; // Контекст базы данных

        public Postavki()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
            LoadSuppliers();
        }

        // Загрузка данных поставщиков
        private void LoadSuppliers()
        {
            try
            {
                var suppliers = _context.Suppliers.ToList();
                SalesDataGrid.ItemsSource = suppliers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Кнопка "Добавление новых поставок"
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddSupplierDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadSuppliers();
            }
        }
        // Кнопка "Учет поступающих партий и товаров"
        private void AddShipmentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Открываем окно для учета партий
                var shipmentDialog = new AddShipmentDialog();
                if (shipmentDialog.ShowDialog() == true)
                {
                    MessageBox.Show("Информация о партиях успешно обновлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отслеживании партий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Кнопка "Формирование накладных"
        private void UpdateProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (SalesDataGrid.SelectedItem is Suppliers selectedSupplier)
            {
                var dialog = new InvoiceDialog(selectedSupplier);
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите поставщика для формирования накладной.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Переход к другим разделам
        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void TovarButton_Click(object sender, RoutedEventArgs e)
        {
            Tovar tovar = new Tovar();
            tovar.Show();
            Close();
        }

        private void ProdajiButton_Click(object sender, RoutedEventArgs e)
        {
            Prodaji prodaji = new Prodaji();
            prodaji.Show();
            Close();
        }

        private void KlientsButton_Click(object sender, RoutedEventArgs e)
        {
            Clientts klients = new Clientts();
            klients.Show();
            Close();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow profile = new MainWindow();
            profile.Show();
            Close();
        }
        private void PostavkiButton_Click(object sender, RoutedEventArgs e)
        {
            Postavki postavki = new Postavki();
            postavki.Show(); 
            this.Close();
        }
    }
}
