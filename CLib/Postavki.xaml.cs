using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CLib
{
    public partial class Postavki : Window
    {
        private BookstoreDBEntities2 _context;
        /// <summary>
        /// Конструктор класса, инициализирует окно и создаёт контекст базы данных (BookstoreDBEntities2), с помощью которого осуществляется доступ к данным. 
        /// Также вызывается метод LoadSuppliers, который загружает список поставщиков из базы данных.
        /// </summary>
        public Postavki()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
            LoadSuppliers();
        }
        /// <summary>
        /// Метод, который загружает список всех поставщиков из базы данных и отображает их в SalesDataGrid. 
        /// Если возникает ошибка при загрузке данных, она обрабатывается и выводится сообщение с ошибкой.
        /// </summary>
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
        /// <summary>
        /// Открывает диалоговое окно для добавления нового поставщика. 
        /// Если пользователь успешно добавляет поставщика, обновляется список поставщиков в SalesDataGrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddSupplierDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadSuppliers();
            }
        }
        /// <summary>
        /// Открывает диалоговое окно для добавления новой партии. 
        /// Если пользователь успешно добавляет информацию о партии, выводится сообщение об успешном обновлении данных. 
        /// В случае ошибки показывается сообщение об ошибке.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddShipmentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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
        /// <summary>
        /// Открывает диалоговое окно для создания накладной, если выбран поставщик в таблице. 
        /// Если поставщик не выбран, выводится сообщение с просьбой выбрать поставщика.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        ///  Открывает главное окно программы и закрывает текущее окно Postavki.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }
        /// <summary>
        /// Обработчик события для кнопки "Товары".
        ///  Открывает окно с товарами и закрывает текущее окно Postavki.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TovarButton_Click(object sender, RoutedEventArgs e)
        {
            Tovar tovar = new Tovar();
            tovar.Show();
            Close();
        }
        /// <summary>
        /// Обработчик события для кнопки "Продажи"
        ///  Открывает окно с продажами и закрывает текущее окно Postavki.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProdajiButton_Click(object sender, RoutedEventArgs e)
        {
            Prodaji prodaji = new Prodaji();
            prodaji.Show();
            Close();
        }
        /// <summary>
        /// Обработчик события для кнопки "Клиенты"
        ///  Открывает окно с клиентами и закрывает текущее окно Postavki.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KlientsButton_Click(object sender, RoutedEventArgs e)
        {
            Clientts klients = new Clientts();
            klients.Show();
            Close();
        }
        /// <summary>
        /// Обработчик события для кнопки "Профиль"
        ///  Открывает окно профиля и закрывает текущее окно Postavki.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow profile = new MainWindow();
            profile.Show();
            Close();
        }
        /// <summary>
        /// Обработчик события для кнопки "Поставки"
        /// Открывает окно поставок и закрывает текущее окно Postavki.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PostavkiButton_Click(object sender, RoutedEventArgs e)
        {
            Postavki postavki = new Postavki();
            postavki.Show();
            this.Close();
        }
    }
}
