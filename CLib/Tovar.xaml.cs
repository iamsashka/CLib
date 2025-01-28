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
        /// <summary>
        /// Инициализирует компоненты окна, создает контекст для работы с базой данных, загружает список товаров и фильтры, 
        /// а также проверяет наличие товаров с низким уровнем остатка.
        /// </summary>
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
        /// <summary>
        /// Вспомогательный класс для хранения информации об уведомлениях (сообщение и время).
        /// </summary>
        public class Notification
        {
            public string Message { get; set; }
            public DateTime TimeStamp { get; set; }
        }
        /// <summary>
        /// Метод для отображения уведомления на экране. 
        /// Уведомление появляется с анимацией появления, а затем исчезает через 5 секунд с анимацией исчезновения. 
        /// Если количество уведомлений больше 5, то старые удаляются.
        /// </summary>
        /// <param name="message"></param>
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
        /// <summary>
        /// Метод проверяет на то, есть ли товары с остатками, меньше или равными заданному порогу (lowStockThreshold). 
        /// Если такие товары есть, выводится уведомление с их названиями и количеством.
        /// </summary>
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
        /// <summary>
        /// Метод который загружает все товары из базы данных и отображает их в SalesDataGrid.
        /// </summary>
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
        /// <summary>
        /// Метод загружает данные для фильтров (по названию, автору и категории) из базы данных.
        /// </summary>
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
        /// <summary>
        ///  Метод открывает диалог для добавления нового товара. После успешного добавления товар сохраняется в базе данных и отображается в списке.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Метод открывает диалог для редактирования выбранного товара. После внесения изменений товар сохраняется в базе данных, и список обновляется.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        ///  Метод удаляет выбранный товар из базы данных и обновляет список товаров.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Метод для фильтрации товаров по введенным значениям в комбобоксах (название, автор, категория). 
        /// После фильтрации товары отображаются в SalesDataGrid.
        /// sender: Источник события
        /// e: Данные события
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Метод выполняет поиск товаров по фильтрам (название, автор, категория) и отображает результаты в SalesDataGrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Метод сбрасывает все фильтры и восстанавливает исходный список товаров, загружая их из базы данных.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Обработчик для перехода между окнами: переход к окну профиля.
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
        /// Обработчик для перехода между окнами: Переход к главному окну.
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
        /// Обработчик для перехода между окнами: Переход к окну товаров (текущее окно).
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
        /// Обработчик для перехода между окнами: Переход к окну поставок.
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
        /// Обработчик для перехода между окнами: Переход к окну продаж.
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
        /// Обработчик для перехода между окнами: Переход к окну клиентов.
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
