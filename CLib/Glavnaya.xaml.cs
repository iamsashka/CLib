﻿using System;
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

        public Glavnaya()
        {
            InitializeComponent();
            LoadSalesData();
        }

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

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            Glavnaya glavWindow = new Glavnaya();
            glavWindow.Show();
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
