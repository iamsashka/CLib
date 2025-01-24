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
        private BookstoreDBEntities2 _context;
        public ObservableCollection<Products> Products { get; set; }
        public ObservableCollection<Notification> Notifications { get; set; }

        public Tovar()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
            Products = new ObservableCollection<Products>();
            LoadProducts();
            LoadFilters();
            this.DataContext = this;
            Notifications = new ObservableCollection<Notification>();
            CheckLowStock();
        }

        public class Notification
        {
            public string Message { get; set; }
            public DateTime TimeStamp { get; set; }
        }

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

            var fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            notificationTextBlock.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };

            timer.Tick += (s, e) =>
            {
                timer.Stop();

                var fadeOutAnimation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(300)
                };

                fadeOutAnimation.Completed += (sender, args) =>
                {
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
                int lowStockThreshold = 5;
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

        private void LoadProducts()
        {
            try
            {
                var products = _context.Products.ToList();
                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
                SalesDataGrid.ItemsSource = Products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}");
            }
        }

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

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var addProductDialog = new AddProductDialog();
            if (addProductDialog.ShowDialog() == true)
            {
                var newProduct = addProductDialog.Product;
                _context.Products.Add(newProduct);
                _context.SaveChanges();
                Products.Add(newProduct);
                MessageBox.Show("Товар успешно добавлен!");
            }
        }

        private void UpdateProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (SalesDataGrid.SelectedItem is Products selectedProduct)
            {
                var editDialog = new EditProductDialog(selectedProduct);
                if (editDialog.ShowDialog() == true)
                {
                    _context.SaveChanges();
                    LoadProducts();
                    MessageBox.Show("Товар успешно обновлен!");
                }
            }
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (SalesDataGrid.SelectedItem is Products selectedProduct)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить этот товар?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _context.Products.Remove(selectedProduct);
                    _context.SaveChanges();
                    Products.Remove(selectedProduct);
                    MessageBox.Show("Товар успешно удален!");
                }
            }
        }

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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string nameFilter = NameFilterComboBox.Text.ToLower();
            string authorFilter = AuthorFilterComboBox.Text.ToLower();
            string genreFilter = GenreFilterComboBox.Text.ToLower();

            var searchResults = _context.Products.Where(p =>
                (string.IsNullOrEmpty(nameFilter) || p.Name.ToLower().Contains(nameFilter)) &&
                (string.IsNullOrEmpty(authorFilter) || p.Author.ToLower().Contains(authorFilter)) &&
                (string.IsNullOrEmpty(genreFilter) || p.Category.ToLower().Contains(genreFilter))
            ).ToList();

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
                NameFilterComboBox.SelectedIndex = -1;
                AuthorFilterComboBox.SelectedIndex = -1;
                GenreFilterComboBox.SelectedIndex = -1;

                LoadProducts();

                MessageBox.Show("Фильтры сброшены, список товаров восстановлен.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сбросе фильтров: {ex.Message}");
            }
        }

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
