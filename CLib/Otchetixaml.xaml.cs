using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace CLib
{
    public partial class Otchetixaml : Window
    {
        private ObservableCollection<object> _reportData;
        private readonly BookstoreDBEntities2 _context;
        /// <summary>
        /// Этот конструктор инициализирует компонент окна и создает экземпляр контекста базы данных BookstoreDBEntities2, 
        /// который используется для работы с данными.
        /// </summary>
        public Otchetixaml()
        {
            InitializeComponent();
            _context = new BookstoreDBEntities2();
        }
        /// <summary>
        /// Эта функция генерирует отчет по доходам магазинов за каждый месяц.
        /// Функция группирует продажи по магазинам и месяцам, подсчитывает доход и долю в общем доходе. 
        /// Результаты отображаются в ReportDataGrid, и появляется сообщение об успешном создании отчета.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateRevenueReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reportData = _context.Sales
                    .GroupBy(s => new { s.Store_ID, Month = s.SaleDate.Month })
                    .Select(g => new
                    {
                        Магазин = g.Key.Store_ID,
                        Месяц = g.Key.Month,
                        Доход = g.Sum(s => s.TotalCost),
                        Город = g.FirstOrDefault().Stores.Name, 
                        Доля_в_общем_доходе = g.Sum(s => s.TotalCost) / (double)_context.Sales.Sum(s => s.TotalCost) * 100
                    })
                    .ToList();

                _reportData = new ObservableCollection<object>(reportData);
                ReportDataGrid.ItemsSource = _reportData;

                ShowCustomMessage("Отчет по доходам успешно создан!", "Success");
            }
            catch (Exception ex)
            {
                ShowCustomMessage($"Ошибка при создании отчета: {ex.Message}", "Error");
            }
        }
        /// <summary>
        /// Функция отображает кастомное сообщение с цветовой кодировкой в зависимости от типа сообщения.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        private void ShowCustomMessage(string message, string messageType)
        {
            CustomMessageBox.Visibility = Visibility.Visible;
            CustomMessageBox.Text = message; 

           
            if (messageType == "Success")
            {
                CustomMessageBox.Background = new SolidColorBrush(Colors.Green);
                CustomMessageBox.Foreground = new SolidColorBrush(Colors.White);
            }
            else if (messageType == "Error")
            {
                CustomMessageBox.Background = new SolidColorBrush(Colors.Red);
                CustomMessageBox.Foreground = new SolidColorBrush(Colors.White);
            }

            var timer = new System.Threading.Timer((e) =>
            {
                Dispatcher.Invoke(() => CustomMessageBox.Visibility = Visibility.Hidden);
            }, null, 3000, System.Threading.Timeout.Infinite);
        }

        /// <summary>
        /// Открывает главное окно MainWindow и закрывает текущее окно.
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
        /// Генерирует отчет по популярным книгам, основываясь на продажах.
        /// Отчет выводит топ-10 книг по числу продаж, данные отображаются в ReportDataGrid, и появляется сообщение об успешном создании отчета.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    .Take(10));

                ReportDataGrid.ItemsSource = _reportData;
                ShowCustomMessage("Отчет по популярным книгам успешно создан!", "Информация");
            }
            catch (Exception ex)
            {
                ShowCustomMessage($"Ошибка при создании отчета: {ex.Message}", "Ошибка");
            }
        }
        /// <summary>
        /// Генерирует отчет по продажам и остаткам на складах.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateSalesAndStockReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _reportData = new ObservableCollection<object>(_context.Products
                    .Select(p => new
                    {
                        Товар = p.Name,
                        Автор = p.Author,
                        Категория = p.Category,
                        Остаток = p.StockQuantity,
                        Продажи = _context.Sales
                            .Where(s => s.Product_ID == p.ID_Product)
                            .Sum(s => (int?)s.TotalCost) ?? 0
                    }));

                ReportDataGrid.ItemsSource = _reportData;
                ShowCustomMessage("Отчет по продажам и остаткам успешно создан!", "Информация");
            }
            catch (Exception ex)
            {
                ShowCustomMessage($"Ошибка при создании отчета: {ex.Message}", "Ошибка");
            }
        }
        /// <summary>
        /// Генерирует отчет по товарам с низкими остатками (менее 10 единиц).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateLowStockReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _reportData = new ObservableCollection<object>(_context.Products
                    .Where(p => p.StockQuantity < 10)
                    .Select(p => new
                    {
                        Товар = p.Name,
                        Остаток = p.StockQuantity
                    }));

                ReportDataGrid.ItemsSource = _reportData;
                ShowCustomMessage("Отчет по остаткам успешно создан!", "Информация");
            }
            catch (Exception ex)
            {
                ShowCustomMessage($"Ошибка при создании отчета: {ex.Message}", "Ошибка");
            }
        }
        /// <summary>
        /// Открывает окно для генерации отчетов.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenReportWindowButton_Click(object sender, RoutedEventArgs e)
        {
            var reportWindow = new ReportWindow(_context);
            reportWindow.Show();
        }
        /// <summary>
        /// Экспортирует созданный отчет о доходах в формате PDF. Если отчет не был создан, выводится сообщение с ошибкой.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportToPDFButton_Click(object sender, RoutedEventArgs e)
        {
            if (_reportData == null || !_reportData.Any())
            {
                ShowCustomMessage("Сначала создайте отчет!", "Ошибка");
                return;
            }

            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = "Отчет_о_доходах_по_магазинам.pdf"
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

                        string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                        var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        var titleFont = new Font(baseFont, 16, Font.BOLD);
                        var headerFont = new Font(baseFont, 12, Font.BOLD);
                        var dataFont = new Font(baseFont, 10);

                        var titleParagraph = new Paragraph("ОТЧЕТ О ДОХОДАХ ПО МАГАЗИНАМ", titleFont)
                        {
                            Alignment = Element.ALIGN_CENTER,
                            Leading = 1.5f * 16
                        };
                        pdfDoc.Add(titleParagraph);

                        pdfDoc.Add(new Paragraph(" ")); 

                        var revenue = _reportData.Sum(x => (decimal)((dynamic)x).Доход);
                        var previousRevenue = _context.Sales
                            .Where(s => s.SaleDate.Month == DateTime.Now.Month - 1)
                            .Sum(s => (decimal?)s.TotalCost) ?? 0;
                        var growth = revenue - previousRevenue;
                        var percentage = previousRevenue != 0 ? (double)(growth / previousRevenue) * 100 : 0;

                        pdfDoc.Add(new Paragraph($"Общий доход: {revenue:F2} ₽", dataFont));
                        pdfDoc.Add(new Paragraph($"Количество магазинов: {_reportData.Count}", dataFont));
                        pdfDoc.Add(new Paragraph($"Средний доход на магазин: {revenue / _reportData.Count:F2} ₽", dataFont));
                        pdfDoc.Add(new Paragraph(" ")); 

                        var table = new PdfPTable(5)
                        {
                            WidthPercentage = 100
                        };
                        table.SetWidths(new float[] { 2, 2, 2, 2, 2 });

                       
                        table.AddCell(new PdfPCell(new Phrase("Магазин", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase("Месяц", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase("Доход", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase("Город", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase("Доля в общем доходе", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                        foreach (var item in _reportData)
                        {
                            table.AddCell(new Phrase(((dynamic)item).Магазин?.ToString() ?? "-", dataFont));
                            table.AddCell(new Phrase(((dynamic)item).Месяц?.ToString() ?? "-", dataFont));
                            table.AddCell(new Phrase(((dynamic)item).Доход.ToString("F2"), dataFont));
                            table.AddCell(new Phrase(((dynamic)item).Город?.ToString() ?? "-", dataFont));
                            table.AddCell(new Phrase(((dynamic)item).Доля_в_общем_доходе.ToString("F2"), dataFont));
                        }

                        pdfDoc.Add(table);
                        pdfDoc.Add(new Paragraph(" ")); 

                        
                        pdfDoc.Add(new Paragraph("Динамика доходов:", dataFont));
                        pdfDoc.Add(new Paragraph($"Рост доходов на {percentage:F2}% (на {growth:F2} ₽)", dataFont));

                        pdfDoc.Add(new Paragraph(" ")); 

                        pdfDoc.Add(new Paragraph("Рекомендации:", dataFont));
                        pdfDoc.Add(new Paragraph("1. Увеличить маркетинговые бюджеты для магазинов с положительной динамикой.", dataFont));
                        pdfDoc.Add(new Paragraph("2. Провести анализ причин снижения дохода в магазинах с отрицательной динамикой.", dataFont));

                        pdfDoc.Close();
                    }

                    ShowCustomMessage("Отчет успешно экспортирован в PDF.", "Информация");
                }
                catch (Exception ex)
                {
                    ShowCustomMessage($"Ошибка при экспорте: {ex.Message}", "Ошибка");
                }
            }
        }
        /// <summary>
        /// Экспортирует отчет по популярным книгам в формате PDF.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportToPDFPopularBooksReport_Click(object sender, RoutedEventArgs e)
        {
            if (_reportData == null || !_reportData.Any())
            {
                ShowCustomMessage("Сначала создайте отчет!", "Ошибка");
                return;
            }

            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = "Отчет_популярные_книги.pdf"
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

                        string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                        var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        var titleFont = new Font(baseFont, 16, Font.BOLD);
                        var headerFont = new Font(baseFont, 12, Font.BOLD);
                        var dataFont = new Font(baseFont, 10);

                       
                        pdfDoc.Add(new Paragraph("ОТЧЕТ О ПОПУЛЯРНЫХ КНИГАХ", titleFont) { Alignment = Element.ALIGN_CENTER });
                        pdfDoc.Add(new Paragraph(" ") { SpacingAfter = 10 }); 

                        
                        pdfDoc.Add(new Paragraph("Составлен в соответствии с актуальными данными о продажах.", dataFont));
                        pdfDoc.Add(new Paragraph($"Дата формирования: {DateTime.Now:dd.MM.yyyy}", dataFont));
                        pdfDoc.Add(new Paragraph(" ") { SpacingAfter = 10 });

                        
                        var table = new PdfPTable(2) { WidthPercentage = 100 };
                        table.SetWidths(new float[] { 3, 1 }); 

                        table.AddCell(new PdfPCell(new Phrase("Книга", headerFont))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            BackgroundColor = new BaseColor(230, 230, 230)
                        });
                        table.AddCell(new PdfPCell(new Phrase("Продажи", headerFont))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            BackgroundColor = new BaseColor(230, 230, 230)
                        });

                        foreach (var item in _reportData)
                        {
                            table.AddCell(new PdfPCell(new Phrase(((dynamic)item).Книга?.ToString() ?? "-", dataFont))
                            {
                                HorizontalAlignment = Element.ALIGN_LEFT
                            });
                            table.AddCell(new PdfPCell(new Phrase(((dynamic)item).Продажи.ToString(), dataFont))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER
                            });
                        }

                        pdfDoc.Add(table);

                        pdfDoc.Add(new Paragraph(" ") { SpacingAfter = 10 });
                        pdfDoc.Add(new Paragraph("Рекомендации:", headerFont));
                        pdfDoc.Add(new Paragraph("1. Поддерживать высокий уровень продаж для популярных книг.", dataFont));
                        pdfDoc.Add(new Paragraph("2. Анализировать причину низких продаж других книг и улучшать маркетинг.", dataFont));

                        pdfDoc.Close();
                    }

                    ShowCustomMessage("Отчет успешно экспортирован в PDF.", "Информация");
                }
                catch (Exception ex)
                {
                    ShowCustomMessage($"Ошибка при экспорте: {ex.Message}", "Ошибка");
                }
            }
        }
        /// <summary>
        /// Экспортирует отчет по товарам с низкими остатками в формате PDF.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportToPDFLowStockReport_Click(object sender, RoutedEventArgs e)
        {
            if (_reportData == null || !_reportData.Any())
            {
                ShowCustomMessage("Сначала создайте отчет!", "Ошибка");
                return;
            }

            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = "Отчет_товары_с_низкими_остатками.pdf"
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

                        string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                        var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        var titleFont = new Font(baseFont, 16, Font.BOLD);
                        var headerFont = new Font(baseFont, 12, Font.BOLD);
                        var dataFont = new Font(baseFont, 10);

                        pdfDoc.Add(new Paragraph("ОТЧЕТ ПО ТОВАРАМ С НИЗКИМИ ОСТАТКАМИ", titleFont) { Alignment = Element.ALIGN_CENTER });

                        pdfDoc.Add(new Paragraph(" ") { SpacingAfter = 10 }); 
                        pdfDoc.Add(new Paragraph($"Дата формирования: {DateTime.Now:dd.MM.yyyy}", dataFont));
                        pdfDoc.Add(new Paragraph($"Общее количество товаров с низким остатком: {_reportData.Count}", dataFont));

                        pdfDoc.Add(new Paragraph(" ") { SpacingAfter = 10 });
                        var table = new PdfPTable(2) { WidthPercentage = 100 };
                        table.SetWidths(new float[] { 3, 1 }); 

                        table.AddCell(new PdfPCell(new Phrase("Товар", headerFont))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            BackgroundColor = new BaseColor(230, 230, 230)
                        });
                        table.AddCell(new PdfPCell(new Phrase("Остаток", headerFont))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            BackgroundColor = new BaseColor(230, 230, 230)
                        });

                        foreach (var item in _reportData)
                        {
                            table.AddCell(new PdfPCell(new Phrase(((dynamic)item).Товар?.ToString() ?? "-", dataFont))
                            {
                                HorizontalAlignment = Element.ALIGN_LEFT
                            });
                            table.AddCell(new PdfPCell(new Phrase(((dynamic)item).Остаток.ToString(), dataFont))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER
                            });
                        }

                        pdfDoc.Add(table);

                        pdfDoc.Add(new Paragraph(" ") { SpacingAfter = 10 }); 
                        pdfDoc.Add(new Paragraph("Рекомендации:", headerFont));
                        pdfDoc.Add(new Paragraph("1. Рассмотреть возможность увеличения поставок для данных товаров.", dataFont));
                        pdfDoc.Add(new Paragraph("2. Провести анализ спроса для предотвращения дефицита.", dataFont));
                        pdfDoc.Add(new Paragraph("3. Настроить автоматическое уведомление о снижении остатков.", dataFont));

                        pdfDoc.Close();
                    }

                    ShowCustomMessage("Отчет успешно экспортирован в PDF.", "Информация");
                }
                catch (Exception ex)
                {
                    ShowCustomMessage($"Ошибка при экспорте: {ex.Message}", "Ошибка");
                }
            }
        }
        /// <summary>
        /// Экспортирует отчет по продажам и остаткам на складах в формате PDF.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportToPDFSalesAndStockReport_Click(object sender, RoutedEventArgs e)
        {
            if (_reportData == null || !_reportData.Any())
            {
                ShowCustomMessage("Сначала создайте отчет!", "Ошибка");
                return;
            }

            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = "Отчет_продажам_и_остаткам.pdf"
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

                        string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                        var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        var titleFont = new Font(baseFont, 16, Font.BOLD);
                        var headerFont = new Font(baseFont, 12, Font.BOLD);
                        var dataFont = new Font(baseFont, 10);

                        pdfDoc.Add(new Paragraph("ОТЧЕТ ПО ПРОДАЖАМ И ОСТАТКАМ", titleFont) { Alignment = Element.ALIGN_CENTER });
                        pdfDoc.Add(new Paragraph(" ")); 

                        var table = new PdfPTable(5) { WidthPercentage = 100 };
                        table.SetWidths(new float[] { 3, 2, 2, 2, 2 });

                        table.AddCell(new PdfPCell(new Phrase("Товар", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase("Автор", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase("Категория", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase("Остаток", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase("Продажи", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER });


                        foreach (var item in _reportData)
                        {
                            table.AddCell(new Phrase(((dynamic)item).Товар?.ToString() ?? "-", dataFont));
                            table.AddCell(new Phrase(((dynamic)item).Автор?.ToString() ?? "-", dataFont));
                            table.AddCell(new Phrase(((dynamic)item).Категория?.ToString() ?? "-", dataFont));
                            table.AddCell(new Phrase(((dynamic)item).Остаток.ToString(), dataFont));
                            table.AddCell(new Phrase(((dynamic)item).Продажи.ToString(), dataFont));
                        }

                        pdfDoc.Add(table);


                        pdfDoc.Add(new Paragraph(" ")); 
                        pdfDoc.Add(new Paragraph("Рекомендации:", headerFont));
                        pdfDoc.Add(new Paragraph("1. Рассмотрите увеличение закупок для товаров с низким остатком.", dataFont));
                        pdfDoc.Add(new Paragraph("2. Повышение маркетинговых усилий для популярных товаров с высоким спросом.", dataFont));
                        pdfDoc.Add(new Paragraph("3. Регулярное отслеживание остатков для предотвращения дефицита товара.", dataFont));

                        pdfDoc.Close();
                    }

                    ShowCustomMessage("Отчет успешно экспортирован в PDF.", "Информация");
                }
                catch (Exception ex)
                {
                    ShowCustomMessage($"Ошибка при экспорте: {ex.Message}", "Ошибка");
                }
            }
        }

    }
}
