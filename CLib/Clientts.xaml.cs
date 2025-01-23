using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace CLib
{
    public partial class Clientts : Window
    {
        private BookstoreDBEntities2 _context;
        public ObservableCollection<Customers> Customers { get; set; }

        public Clientts()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
            Customers = new ObservableCollection<Customers>();
            LoadCustomers();
            this.DataContext = this;
        }

        // Загрузка данных клиентов
        private void LoadCustomers()
        {
            var customers = _context.Customers.ToList();  // Загружаем всех клиентов
            Customers.Clear();
            foreach (var customer in customers)
            {
                Customers.Add(customer);  // Добавляем каждого клиента в коллекцию
            }
            CustomersDataGrid.ItemsSource = Customers;  // Устанавливаем источник данных для DataGrid
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем значения фильтров для поиска
            string nameFilter = NameFilterTextBox.Text;  // Получаем фамилию клиента
            string phoneFilter = PhoneFilterTextBox.Text;  // Получаем телефон клиента
            string lastnameFilter = LastNameFilterTextBox.Text;

            // Применяем фильтрацию на коллекцию клиентов
            var filteredCustomers = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(nameFilter))
            {
                filteredCustomers = filteredCustomers.Where(c => c.FirstName.Contains(nameFilter));
            }
            if (!string.IsNullOrEmpty(nameFilter))
            {
                filteredCustomers = filteredCustomers.Where(c => c.LastName.Contains(lastnameFilter));
            }

            if (!string.IsNullOrEmpty(phoneFilter))
            {
                filteredCustomers = filteredCustomers.Where(c => c.Phone.Contains(phoneFilter));
            }

            // Обновляем DataGrid с отфильтрованными данными
            CustomersDataGrid.ItemsSource = filteredCustomers.ToList();
        }

        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            var addCustomerDialog = new AddCustomerDialog();
            if (addCustomerDialog.ShowDialog() == true)
            {
                var newCustomer = addCustomerDialog.Customer;
                _context.Customers.Add(newCustomer);
                _context.SaveChanges();
                Customers.Add(newCustomer); // Добавляем клиента в коллекцию
                MessageBox.Show("Клиент успешно добавлен.");
            }
        }

        private void UpdateCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersDataGrid.SelectedItem is Customers selectedCustomer)
            {
                var editDialog = new EditCustomerDialog(selectedCustomer);
                if (editDialog.ShowDialog() == true)
                {
                    _context.SaveChanges(); // Сохраняем изменения в базе данных
                    ReloadData(); // Перезагружаем данные
                    MessageBox.Show("Информация о клиенте обновлена.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите клиента для редактирования.");
            }
        }

        private void DeleteCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersDataGrid.SelectedItem is Customers selectedCustomer)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этого клиента?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _context.Customers.Remove(selectedCustomer);
                    _context.SaveChanges();
                    ReloadData(); // Перезагружаем данные
                    MessageBox.Show("Клиент удален.");
                }
            }
        }

        private void ViewPurchaseHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersDataGrid.SelectedItem is Customers selectedCustomer)
            {
                var purchaseHistoryWindow = new PurchaseHistoryWindow(selectedCustomer.ID_Customer);
                purchaseHistoryWindow.Show();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите клиента, чтобы увидеть его историю покупок.");
            }
        }

        private void FilterCustomers(object sender, RoutedEventArgs e)
        {
            string nameFilter = NameFilterTextBox.Text.ToLower();
            string lastNameFilter = LastNameFilterTextBox.Text.ToLower();
            string phoneFilter = PhoneFilterTextBox.Text.ToLower();

            var filteredCustomers = _context.Customers
                .Where(c =>
                    c.FirstName.ToLower().Contains(nameFilter) &&
                    c.LastName.ToLower().Contains(lastNameFilter) &&
                    c.Phone.ToLower().Contains(phoneFilter))
                .ToList();

            CustomersDataGrid.ItemsSource = filteredCustomers; // Устанавливаем отфильтрованный список
        }
        private void ReloadData()
        {
            _context.Dispose(); // Освобождаем предыдущий контекст
            _context = new BookstoreDBEntities2(); // Создаём новый контекст
            LoadCustomers(); // Загружаем данные клиентов заново
        }

        // Переключение на другие окна
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            Glavnaya glavWindow = new Glavnaya();
            glavWindow.Show();
            this.Close();
        }

        private void TovarButton_Click(object sender, RoutedEventArgs e)
        {
            Tovar tovarWindow = new Tovar();
            tovarWindow.Show();
            this.Close();
        }

        private void PostavkiButton_Click(object sender, RoutedEventArgs e)
        {
            Postavki postavWindow = new Postavki();
            postavWindow.Show();
            this.Close();
        }

        private void ProdajiButton_Click(object sender, RoutedEventArgs e)
        {
            Prodaji saleWindow = new Prodaji();
            saleWindow.Show();
            this.Close();
        }

        private void KlientsButton_Click(object sender, RoutedEventArgs e)
        {
            Clientts clientWindow = new Clientts();
            clientWindow.Show();
            this.Close();
        }
    }
}
