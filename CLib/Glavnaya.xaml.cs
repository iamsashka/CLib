using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Linq;

namespace CLib
{
    public partial class Glavnaya : Window
    {
        private MainWindow profileWindow;
        private Tovar tovarWindow;
        /// <summary>
        /// Конструктор класса, который инициализирует компоненты окна и загружает данные о продажах через метод LoadSalesData().
        /// </summary>
        public Glavnaya()
        {
            InitializeComponent();
            LoadSalesData();
        }
        /// <summary>
        ///  Обработчик для кнопки "Профиль". При нажатии открывается окно профиля (если оно не открыто) или активируется уже открытое окно. 
        ///  Текущее окно скрывается.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (profileWindow == null || !profileWindow.IsVisible)
            {
                profileWindow = new MainWindow();
                profileWindow.Show();
            }
            else
            {
                profileWindow.Activate();
            }

            this.Hide();
        }
        /// <summary>
        /// Обработчик для кнопки "Товары". При нажатии открывается окно товаров (если оно не открыто) или активируется уже открытое окно. 
        /// Текущее окно скрывается.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TovarButton_Click(object sender, RoutedEventArgs e)
        {
            if (tovarWindow == null || !tovarWindow.IsVisible)
            {
                tovarWindow = new Tovar();
                tovarWindow.Show();
            }
            else
            {
                tovarWindow.Activate();
            }

            this.Hide();
        }
        /// <summary>
        /// Обработчик для кнопки "Главная". Создает и открывает новое окно главной формы, а текущее окно закрывается.
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
        /// Обработчик для кнопки "Поставки". Открывает окно поставок и закрывает текущее окно.
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
        ///  Обработчик для кнопки "Продажи". Открывает окно продаж и закрывает текущее окно.
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
        /// Обработчик для кнопки "Клиенты". Открывает окно клиентов и закрывает текущее окно.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KlientsButton_Click(object sender, RoutedEventArgs e)
        {
            Clientts clientWindow = new Clientts();
            clientWindow.Show();
            this.Close();
        }
        /// <summary>
        /// Метод загружает данные о продажах из базы данных, используя контекст Entity Framework. 
        /// Он выполняет запрос, который извлекает информацию о продукте, количестве, магазине, способе оплаты, сумме продажи и количестве товара на складе для каждой продажи. 
        /// Загруженные данные затем привязываются к DataGrid.
        /// </summary>
        private void LoadSalesData()
        {
            try
            {
                using (var context = new BookstoreDBEntities2())
                {
                    var salesData = context.Sales
                        .Select(s => new SalesInfo
                        {
                            ProductName = context.Products
                                .Where(p => p.ID_Product == context.ProductSales
                                    .FirstOrDefault(ps => ps.Sales_ID == s.ID_Sale).Product_ID)
                                .Select(p => p.Name).FirstOrDefault(),

                            Quantity = context.ProductSales
                                .Where(ps => ps.Sales_ID == s.ID_Sale)
                                .Select(ps => ps.Quantity).FirstOrDefault(),

                            StoreName = context.Stores
                                .Where(st => st.Store_ID == s.Store_ID)
                                .Select(st => st.Name).FirstOrDefault(),

                            PaymentMethod = s.PaymentMethod,
                            TotalAmount = s.TotalCost,
                            SaleDate = s.SaleDate,

                            StockQuantity = context.Products
                                .Where(p => p.ID_Product == context.ProductSales
                                    .FirstOrDefault(ps => ps.Sales_ID == s.ID_Sale).Product_ID)
                                .Select(p => p.StockQuantity).FirstOrDefault()
                        })
                        .ToList();

                    SalesDataGrid.ItemsSource = salesData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }
        /// <summary>
        /// Вспомогательный класс, используемый для хранения информации о каждой продаже, которая будет отображаться в DataGrid.
        /// </summary>
        public class SalesInfo
        {
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public string StoreName { get; set; }
            public string PaymentMethod { get; set; }
            public double TotalAmount { get; set; }
            public DateTime SaleDate { get; set; }
            public int StockQuantity { get; set; }
        }
    }
}
