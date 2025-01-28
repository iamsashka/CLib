using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace CLib
{
    public partial class Clientts : Window
    {
        private BookstoreDBEntities2 _context;
        public ObservableCollection<Customers> Customers { get; set; }
        /// <summary>
        /// Конструктор класса Clientts, инициализирует компонент пользовательского интерфейса, создаёт объект контекста базы данных и загружает список клиентов. 
        /// Устанавливает текущий объект как контекст данных для привязки данных.
        /// </summary>
        public Clientts()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
            Customers = new ObservableCollection<Customers>();
            LoadCustomers();
            this.DataContext = this;
        }
        /// <summary>
        /// Метод загружает всех клиентов из базы данных и добавляет их в коллекцию Customers, которая затем привязывается к элементу DataGrid для отображения.
        /// </summary>
        private void LoadCustomers()
        {
            var customers = _context.Customers.ToList();
            Customers.Clear();
            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }
            CustomersDataGrid.ItemsSource = Customers;
        }
        /// <summary>
        /// Метод для фильтрации клиентов по введённым значениям в текстовые поля. 
        /// Он применяет фильтры для имени, фамилии и телефона и отображает отфильтрованный список в DataGrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string nameFilter = NameFilterTextBox.Text;
            string phoneFilter = PhoneFilterTextBox.Text;
            string lastnameFilter = LastNameFilterTextBox.Text;

            var filteredCustomers = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(nameFilter))
            {
                filteredCustomers = filteredCustomers.Where(c => c.FirstName.Contains(nameFilter));
            }
            if (!string.IsNullOrEmpty(lastnameFilter))
            {
                filteredCustomers = filteredCustomers.Where(c => c.LastName.Contains(lastnameFilter));
            }

            if (!string.IsNullOrEmpty(phoneFilter))
            {
                filteredCustomers = filteredCustomers.Where(c => c.Phone.Contains(phoneFilter));
            }

            CustomersDataGrid.ItemsSource = filteredCustomers.ToList();
        }
        /// <summary>
        /// Метод открывает диалог для добавления нового клиента. После добавления клиента в базу данных, он сохраняется и отображается в интерфейсе.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            var addCustomerDialog = new AddCustomerDialog();
            if (addCustomerDialog.ShowDialog() == true)
            {
                var newCustomer = addCustomerDialog.Customer;
                _context.Customers.Add(newCustomer);
                _context.SaveChanges();
                Customers.Add(newCustomer);
                MessageBox.Show("Клиент успешно добавлен.");
            }
        }
        /// <summary>
        /// Метод для редактирования выбранного клиента. 
        /// Если клиент выбран в DataGrid, открывается диалог для редактирования. 
        /// После изменения данных сохраняются в базе и данные обновляются в интерфейсе.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersDataGrid.SelectedItem is Customers selectedCustomer)
            {
                var editDialog = new EditCustomerDialog(selectedCustomer);
                if (editDialog.ShowDialog() == true)
                {
                    _context.SaveChanges();
                    ReloadData();
                    MessageBox.Show("Информация о клиенте обновлена.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите клиента для редактирования.");
            }
        }
        /// <summary>
        /// Метод для удаления выбранного клиента. 
        /// Если клиент выбран в DataGrid, появляется окно подтверждения удаления, и после подтверждения клиента удаляют из базы данных.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersDataGrid.SelectedItem is Customers selectedCustomer)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этого клиента?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _context.Customers.Remove(selectedCustomer);
                    _context.SaveChanges();
                    ReloadData();
                    MessageBox.Show("Клиент удален.");
                }
            }
        }
        /// <summary>
        /// Метод для просмотра истории покупок клиента. Если выбран клиент, открывается окно с его историей покупок.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Метод для фильтрации клиентов по имени, фамилии и телефону на основе введённых значений. 
        /// Применяет фильтры и отображает отфильтрованный список клиентов.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            CustomersDataGrid.ItemsSource = filteredCustomers;
        }
        /// <summary>
        /// Метод для перезагрузки данных из базы данных. Закрывает текущий контекст базы данных и создаёт новый.
        /// </summary>
        private void ReloadData()
        {
            _context.Dispose();
            _context = new BookstoreDBEntities2();
            LoadCustomers();
        }
        /// <summary>
        ///  Метод для перехода на окно профиля. Открывает окно MainWindow и закрывает текущее окно.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        /// <summary>
        /// Метод для перехода на основное окно. Открывает окно Glavnaya и закрывает текущее окно.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            Glavnaya glavWindow = new Glavnaya();
            glavWindow.Show();
            this.Close();
        }
        /// <summary>
        ///  Метод для перехода на окно товаров. Открывает окно Tovar и закрывает текущее окно.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TovarButton_Click(object sender, RoutedEventArgs e)
        {
            Tovar tovarWindow = new Tovar();
            tovarWindow.Show();
            this.Close();
        }
        /// <summary>
        /// Метод для перехода на окно поставок. Открывает окно Postavki и закрывает текущее окно.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PostavkiButton_Click(object sender, RoutedEventArgs e)
        {
            Postavki postavWindow = new Postavki();
            postavWindow.Show();
            this.Close();
        }
        /// <summary>
        ///Метод для перехода на окно продаж. Открывает окно Prodaji и закрывает текущее окно.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProdajiButton_Click(object sender, RoutedEventArgs e)
        {
            Prodaji saleWindow = new Prodaji();
            saleWindow.Show();
            this.Close();
        }
        /// <summary>
        /// Метод для перехода на окно клиентов. Открывает окно Clientts и закрывает текущее окно.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KlientsButton_Click(object sender, RoutedEventArgs e)
        {
            Clientts clientWindow = new Clientts();
            clientWindow.Show();
            this.Close();
        }
    }
}
