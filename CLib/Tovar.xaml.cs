using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Data.Entity;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Threading;

namespace CLib
{
    public partial class Tovar : Window
    {
        private BookstoreDBEntities2 _context;  // Контекст базы данных
        public ObservableCollection<Products> Products { get; set; }  // Коллекция для привязки
        public ObservableCollection<Notification> Notifications { get; set; }

        public Tovar()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2(); // Инициализация контекста базы данных
            Products = new ObservableCollection<Products>();
            LoadProducts();  // Загрузка данных из базы
            LoadFilters();  // Загрузка данных для фильтров
            this.DataContext = this; // Привязка данных к окну
            Notifications = new ObservableCollection<Notification>();
            CheckLowStock();
        }
        // Модель уведомлений
        public class Notification
        {
            public string Message { get; set; }
            public DateTime TimeStamp { get; set; }
        }
        // Метод для отображения уведомлений
        private void ShowNotification(string message)
        {
            var notification = new Notification
            {
                Message = message,
                TimeStamp = DateTime.Now
            };

            Notifications.Add(notification);

            if (Notifications.Count > 5)
            {
                Notifications.RemoveAt(0);
            }

            var notificationTextBlock = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                Background = Brushes.Red,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Margin = new Thickness(5),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            NotificationStackPanel.Children.Add(notificationTextBlock);

            // Анимация появления уведомления
            var fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            notificationTextBlock.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            // Таймер для удаления уведомления
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };

            timer.Tick += (s, e) =>
            {
                timer.Stop();

                // Анимация исчезновения уведомления
                var fadeOutAnimation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(300)
                };

                fadeOutAnimation.Completed += (sender, args) =>
                {
                    // Удаляем уведомление после завершения анимации
                    NotificationStackPanel.Children.Remove(notificationTextBlock);
                };

                notificationTextBlock.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            };
            timer.Start();
        }

        private void CheckLowStock()
        {
            try
            {
                int lowStockThreshold = 5; // Порог низкого остатка
                var lowStockProducts = _context.Products.Where(p => p.StockQuantity <= lowStockThreshold).ToList();

                if (lowStockProducts.Any())
                {
                    string lowStockMessage = "Внимание! Низкий уровень остатка у следующих товаров:\n" +
                        string.Join("\n", lowStockProducts.Select(p => $"{p.Name} (Остаток: {p.StockQuantity})"));
                    ShowNotification(lowStockMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке остатков: {ex.Message}");
            }
        }

        // Загрузка данных в коллекцию Products
        private void LoadProducts()
        {
            try
            {
                var products = _context.Products.ToList(); // Получение всех записей
                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
                SalesDataGrid.ItemsSource = Products; // Привязка коллекции к DataGrid
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}");
            }
        }

        // Загрузка данных для фильтров
        private void LoadFilters()
        {
            try
            {
                NameFilterComboBox.ItemsSource = _context.Products.Select(p => p.Name).Distinct().ToList();
                AuthorFilterComboBox.ItemsSource = _context.Products.Select(p => p.Author).Distinct().ToList();
                GenreFilterComboBox.ItemsSource = _context.Products.Select(p => p.Category).Distinct().ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке фильтров: {ex.Message}");
            }
        }

        // Добавление нового товара
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var addProductDialog = new AddProductDialog(); // Окно добавления
            if (addProductDialog.ShowDialog() == true)
            {
                var newProduct = addProductDialog.Product; // Получение нового товара
                _context.Products.Add(newProduct);  // Добавление в базу
                _context.SaveChanges();  // Сохранение изменений
                Products.Add(newProduct); // Добавление в коллекцию
                MessageBox.Show("Товар успешно добавлен!");
            }
        }

        // Редактирование товара
        private void UpdateProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (SalesDataGrid.SelectedItem is Products selectedProduct)
            {
                var editDialog = new EditProductDialog(selectedProduct); // Окно редактирования
                if (editDialog.ShowDialog() == true)
                {
                    _context.SaveChanges(); // Сохранение изменений в базе
                    LoadProducts(); // Обновление коллекции
                    MessageBox.Show("Товар успешно обновлен!");
                }
            }
        }

        // Удаление товара
        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (SalesDataGrid.SelectedItem is Products selectedProduct)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить этот товар?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _context.Products.Remove(selectedProduct); // Удаление из базы
                    _context.SaveChanges(); // Сохранение изменений
                    Products.Remove(selectedProduct); // Удаление из коллекции
                    MessageBox.Show("Товар успешно удален!");
                }
            }
        }

        // Фильтрация товаров
        private void FilterProducts(object sender, EventArgs e)
        {
            string nameFilter = NameFilterComboBox.Text.ToLower();
            string genreFilter = GenreFilterComboBox.Text.ToLower();
            string authorFilter = AuthorFilterComboBox.Text.ToLower();

            var filteredProducts = _context.Products.Where(p =>
                (string.IsNullOrEmpty(nameFilter) || p.Name.ToLower().Contains(nameFilter)) &&
                (string.IsNullOrEmpty(genreFilter) || p.Category.ToLower().Contains(genreFilter)) &&
                (string.IsNullOrEmpty(authorFilter) || p.Author.ToLower().Contains(authorFilter))
            ).ToList();

            Products.Clear();
            foreach (var product in filteredProducts)
            {
                Products.Add(product);
            }
        }

        // Поиск товаров
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string nameFilter = NameFilterComboBox.Text.ToLower();
            string authorFilter = AuthorFilterComboBox.Text.ToLower();
            string genreFilter = GenreFilterComboBox.Text.ToLower();

            // Фильтрация по указанным критериям
            var searchResults = _context.Products.Where(p =>
                (string.IsNullOrEmpty(nameFilter) || p.Name.ToLower().Contains(nameFilter)) &&
                (string.IsNullOrEmpty(authorFilter) || p.Author.ToLower().Contains(authorFilter)) &&
                (string.IsNullOrEmpty(genreFilter) || p.Category.ToLower().Contains(genreFilter))
            ).ToList();

            // Очистка текущей коллекции и добавление результатов
            Products.Clear();
            foreach (var product in searchResults)
            {
                Products.Add(product);
            }
        }
        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Очистка значений в фильтрах
                NameFilterComboBox.SelectedIndex = -1;
                AuthorFilterComboBox.SelectedIndex = -1;
                GenreFilterComboBox.SelectedIndex = -1;

                // Восстановление полного списка товаров
                LoadProducts();

                MessageBox.Show("Фильтры сброшены, список товаров восстановлен.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сбросе фильтров: {ex.Message}");
            }
        }


        // Переход на главную страницу
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
