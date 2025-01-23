using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace CLib
{
    public partial class Otchetixaml : Window
    {
        private ObservableCollection<object> _reportData;
        private readonly BookstoreDBEntities2 _context;

        public Otchetixaml()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
        }

        // Отчет: Доходы по магазинам за месяц
        private void GenerateRevenueReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _reportData = new ObservableCollection<object>(_context.Sales
                    .GroupBy(s => new { s.Store_ID, Month = s.SaleDate.Month })
                    .Select(g => new
                    {
                        Магазин = g.Key.Store_ID,
                        Месяц = g.Key.Month,
                        Доход = g.Sum(s => s.TotalCost)
                    }));

                ReportDataGrid.ItemsSource = _reportData;
                MessageBox.Show("Отчет по доходам успешно создан!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        // Отчет: Самые популярные книги за период
        private void GeneratePopularBooksReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _reportData = new ObservableCollection<object>(_context.Sales
                    .GroupBy(s => s.Products.Name)
                    .Select(g => new
                    {
                        Книга = g.Key,
                        Продажи = g.Count()
                    })
                    .OrderByDescending(x => x.Продажи)
                    .Take(10)); // Топ 10 книг

                ReportDataGrid.ItemsSource = _reportData;
                MessageBox.Show("Отчет по популярным книгам успешно создан!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Отчет: Продажи и остатки товаров за все время
        private void GenerateSalesAndStockReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Запрос данных о продажах и остатках
                _reportData = new ObservableCollection<object>(_context.Products
                    .Select(p => new
                    {
                        Товар = p.Name,
                        Автор = p.Author,
                        Категория = p.Category,
                        Остаток = p.StockQuantity,
                        // Преобразуем TotalCost в nullable тип и проверяем его на null
                        Продажи = _context.Sales
                            .Where(s => s.Product_ID == p.ID_Product)
                            .Sum(s => (int?)s.TotalCost) ?? 0 // Преобразуем TotalCost в int? и используем ?? для замены null на 0
                    }));

                // Отображаем отчет в DataGrid
                ReportDataGrid.ItemsSource = _reportData;
                MessageBox.Show("Отчет по продажам и остаткам успешно создан!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Отчет: Товары с низкими остатками
        private void GenerateLowStockReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _reportData = new ObservableCollection<object>(_context.Products
                    .Where(p => p.StockQuantity < 10) // Уровень ниже 10
                    .Select(p => new
                    {
                        Товар = p.Name,
                        Остаток = p.StockQuantity
                    }));

                ReportDataGrid.ItemsSource = _reportData;
                MessageBox.Show("Отчет по остаткам успешно создан!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Экспорт текущего отчета в PDF
        private void ExportToPDFButton_Click(object sender, RoutedEventArgs e)
        {
            if (_reportData == null || !_reportData.Any())
            {
                MessageBox.Show("Сначала создайте отчет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = "Отчет.pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    using (var stream = new FileStream(saveDialog.FileName, FileMode.Create))
                    {
                        var pdfDoc = new Document();
                        PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();

                        pdfDoc.Add(new Paragraph("Отчет"));
                        pdfDoc.Add(new Paragraph(" ")); // Пустая строка

                        foreach (var item in _reportData)
                        {
                            pdfDoc.Add(new Paragraph(item.ToString()));
                        }

                        pdfDoc.Close();
                    }

                    MessageBox.Show("Отчет успешно экспортирован в PDF.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
