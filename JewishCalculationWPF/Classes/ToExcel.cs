using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JewishCalculationWPF.Classes
{
    class ToExcel
    {
        internal void GetExcel()
        {
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "iLinks";
                excelPackage.Workbook.Properties.Title = "Еврейский расчет";
                //excelPackage.Workbook.Properties.Subject = "EPPlus demo export data";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add($"{DateTime.Now:dd_MM_yyyy}");

                AddCells(worksheet);

                //Save your file
                FileInfo fi = new FileInfo(Directory.GetCurrentDirectory() + $"\\Еврейский расчет {DateTime.Now:dd_MM_yyyy}.xlsx");
                try
                {
                    if (fi.Exists)
                    {
                        if (MessageBox.Show($"Файл {fi.Name} уже существует.\nПерезаписать файл?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            //var t1 = Process.GetProcessesByName("EXCEL");
                            //foreach (var t in t1)
                            //{
                            //    var test = t.MainWindowTitle.ToUpper();
                            //    if (test.Equals(fi.Name))//валится здесь почему то ¯\_(ツ)_/¯
                            //    {
                            //        MessageBox.Show($"Файл {fi.Name} открыт.\nЗакройте файл и повторите!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                            //        return;
                            //    } 
                            //}
                            fi.Delete();
                        }
                        else return;
                    }
                    excelPackage.SaveAs(fi);
                    MessageBox.Show($"Файл {fi.Name} создан!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }


        private void AddCells(ExcelWorksheet ws)
        {

            int r = 1, c = 3;

            ///Названия товаров
            foreach (var p in Models.Products)
            {
                ws.Column(c).Width = 20;
                ws.Cells[r, c].Style.WrapText = true;
                ws.Cells[r, c].Value = p.Name;
                c++;
            }

            AddThinBorder(ws, r, 3, r, c - 1);
            AddThickBorder(ws, r, 3, r, c - 1);

            r++;
            c = 3;

            ws.Column(c - 1).Width = 14;
            ws.Cells[r, c - 1].Value = "Количество";

            foreach (var p in Models.Products)
            {
                ws.Cells[r, c].Value = p.Quantity;
                c++;
            }

            r++;
            c = 3;

            ws.Cells[r, c - 1].Style.WrapText = true;
            ws.Cells[r, c - 1].Value = "Цена за шт / кг";

            foreach (var p in Models.Products)
            {
                ws.Cells[r, c].Value = p.Price;
                c++;
            }

            r++;
            c = 3;

            ws.Cells[r, c - 1].Value = "Сумма";

            AddThinBorder(ws, r - 2, 2, r, 2);
            AddThickBorder(ws, r - 2, 2, r, 2);

            foreach (var p in Models.Products)
            {
                ws.Cells[r, c].Value = p.Sum;
                c++;
            }

            ws.Column(c).Width = 11;
            ws.Cells[r - 1, c].Value = "Общая сумма";
            ws.Cells[r, c].Formula = $"SUM({ws.Cells[r, 3].Address}:{ws.Cells[r, c - 1].Address})";//Service.Products.Sum(n => n.sum);

            AddThinBorder(ws, r - 1, c, r, c);
            AddThickBorder(ws, r - 1, c, r, c);

            int rfSum = r;

            AddThinBorder(ws, r - 2, 3, r, c - 1);
            AddThickBorder(ws, r - 2, 3, r, c - 1);

            r++;
            c = 3;

            ///Добавление персон
            foreach (var p in Models.Persons)
            {
                ws.Cells[r, c - 1].Value = p.FIO;
                r++;
            }

            ws.Cells[r - Models.Persons.Count, c - 1, r - 1, c - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            AddThinBorder(ws, r - Models.Persons.Count, 2, r - 1, 2);
            AddThickBorder(ws, r - Models.Persons.Count, 2, r - 1, 2);

            r -= Models.Persons.Count;

            ///Проверяем количество персон и количество потреблений по персонам. Если одинаково, то добавляем потребление
            if (Models.Persons.Count == Models.Consumptions.Count)
            {
                foreach (var cons in Models.Consumptions)
                {
                    foreach (var p in cons.products)
                    {
                        ws.Cells[r, c].Value = p.Quantity;
                        c++;
                    }
                    r++;
                    c = 3;
                }
            }

            AddThinBorder(ws, r - Models.Persons.Count, 3, r - 1, Models.Products.Count + 2);
            AddThickBorder(ws, r - Models.Persons.Count, 3, r - 1, Models.Products.Count + 2);

            ///Проверка количества
            foreach (var p in Models.Products)
            {
                ws.Cells[r, c].Formula = $"IF(SUM({ws.Cells[r - Models.Persons.Count, c].Address}:{ws.Cells[r - 1, c].Address})={ws.Cells[2, c].Address}, \"сошлось\", \"не сошлось\")";
                c++;
            }

            AddThinBorder(ws, r, 3, r, Models.Products.Count + 2);
            AddThickBorder(ws, r, 3, r, Models.Products.Count + 2);

            r++;
            c = 3;

            ///Добавление персон
            foreach (var p in Models.Persons)
            {
                ws.Cells[r, c - 1].Value = p.FIO;
                r++;
            }

            ws.Cells[r - Models.Persons.Count, c - 1, r - 1, c - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            AddThinBorder(ws, r - Models.Persons.Count, 2, r - 1, 2);
            AddThickBorder(ws, r - Models.Persons.Count, 2, r - 1, 2);

            AddThinBorder(ws, r - Models.Persons.Count, 3, r - 1, Models.Products.Count + 2);
            AddThickBorder(ws, r - Models.Persons.Count, 3, r - 1, Models.Products.Count + 2);

            r -= Models.Persons.Count;

            ///Добавление суммы по каждому использованному товару
            foreach (var p in Models.Products)
            {
                for (int i = 0; i < Models.Persons.Count; i++)
                {
                    ws.Cells[r, c].Formula = $"{ws.Cells[r - i - (Models.Persons.Count + 4), c].Address}*{ws.Cells[r - i - (Models.Persons.Count + 1), c].Address}";
                    r++;
                }
                r -= Models.Persons.Count;
                c++;
            }

            r += Models.Persons.Count;
            c = 3;
            ///Проверка суммы
            foreach (var p in Models.Products)
            {
                ws.Cells[r, c].Formula = $"IF(SUM({ws.Cells[r - Models.Persons.Count, c].Address}:{ws.Cells[r - 1, c].Address})={ws.Cells[4, c].Address}, \"сошлось\", \"не сошлось\")";
                c++;
            }

            AddThinBorder(ws, r, 3, r, Models.Products.Count + 2);
            AddThickBorder(ws, r, 3, r, Models.Products.Count + 2);

            r -= Models.Products.Count + 2;

            ws.Cells[r, c].Style.WrapText = true;
            ws.Cells[r, c].Value = "Сколько кто должен";
            r++;
            for (int i = 0; i < Models.Persons.Count; i++)
            {
                ws.Cells[r + i, c].Formula = $"SUM({ws.Cells[r + i, 3].Address}:{ws.Cells[r + i, c - 1].Address})";
            }
            r += Models.Persons.Count;
            ws.Cells[r, c].Formula = $"IF(SUM({ws.Cells[r - Models.Persons.Count, c].Address}:{ws.Cells[r - 1, c].Address})={ws.Cells[rfSum, c].Address}, \"сошлось\", \"не сошлось\")";

            AddThinBorder(ws, r - Models.Persons.Count - 1, Models.Products.Count + 3, r, Models.Products.Count + 3);
            AddThickBorder(ws, r - Models.Persons.Count - 1, Models.Products.Count + 3, r, Models.Products.Count + 3);

            AddThickBorder(ws, r - Models.Persons.Count - 1, Models.Products.Count + 3, r - Models.Persons.Count - 1, Models.Products.Count + 3);
            AddThickBorder(ws, r, Models.Products.Count + 3, r, Models.Products.Count + 3);

            ws.Cells[1, 2, r, c].Style.Font.Name = "Consolas";
            ws.Cells[1, 2, r, c].Style.Font.Size = 9;
            ws.View.FreezePanes(1, 3);
        }
        private void AddThinBorder(ExcelWorksheet ws, int RowStart, int ColumnStart, int RowEnd, int ColumnEnd)
        {
            ws.Cells[RowStart, ColumnStart, RowEnd, ColumnEnd].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[RowStart, ColumnStart, RowEnd, ColumnEnd].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[RowStart, ColumnStart, RowEnd, ColumnEnd].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[RowStart, ColumnStart, RowEnd, ColumnEnd].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        }

        private void AddThickBorder(ExcelWorksheet ws, int RowStart, int ColumnStart, int RowEnd, int ColumnEnd)
        {
            ws.Cells[RowStart, ColumnStart, RowStart, ColumnEnd].Style.Border.Top.Style = ExcelBorderStyle.Thick;
            ws.Cells[RowStart, ColumnEnd, RowEnd, ColumnEnd].Style.Border.Right.Style = ExcelBorderStyle.Thick;
            ws.Cells[RowEnd, ColumnStart, RowEnd, ColumnEnd].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ws.Cells[RowStart, ColumnStart, RowEnd, ColumnStart].Style.Border.Left.Style = ExcelBorderStyle.Thick;
        }
    }
}
