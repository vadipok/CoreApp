using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Drawing.Chart;
using kern.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml.Table;
using OfficeOpenXml.Drawing;
using System.Drawing;
using PostgresApp;
using kern.Models.DataBase;
using System.Security.Principal;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using kern.Models.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace kern.Controllers
{
    public class ExcelController : Controller
    {

        private readonly IHostingEnvironment _appEnvironment;
        public ExcelController(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        public IActionResult Download()
        {
            byte[] fileContents;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Put whatever you want here in the sheet
                // For example, for cell on row1 col1
                worksheet.Cells[1, 1].Value = "Long text";

                worksheet.Cells[1, 1].Style.Font.Size = 12;
                worksheet.Cells[1, 1].Style.Font.Bold = true;

                worksheet.Cells[1, 1].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                double[] XArr = new double[21 - 4];
                double[] YArr = new double[21 - 4];

                //Пример заполнения ячеек
                for (int j = 4; j < 21; j++)
                {
                    worksheet.Cells[j, 1].Value = j;
                    worksheet.Cells[j, 2].Value = j*j;
                    XArr[j - 4] = (float)j;
                    YArr[j - 4] = (float)j * j;
                }

                Approximation approxim = new Approximation();
                approxim.XArr = XArr;
                approxim.YArr = YArr;
                approxim.Exponential();


                ExcelChart chart = worksheet.Drawings.AddChart("FindingsChart", OfficeOpenXml.Drawing.Chart.eChartType.XYScatter);
                chart.Title.Text = "Category Chart";
                chart.SetPosition(1, 0, 3, 0);
                chart.SetSize(800, 300);
                var ser1 = (ExcelChartSerie)(chart.Series.Add(worksheet.Cells["B4:B20"], worksheet.Cells["A4:A20"]));

                chart.Series[0].TrendLines.Add(eTrendLine.Exponential);

                // chart.YAxis.LogBase = Math.E;

                ser1.Header = "Category";

                // So many things you can try but you got the idea.

                // Finally when you're done, export it to byte array.
                fileContents = package.GetAsByteArray();
            }

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "test.xlsx"
            );
        }

        [HttpPost("UploadExcelFile")]
        public IActionResult Post(string RTwo, List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // Полный путь до файла
            var filePath = Path.GetTempFileName();

            // возвращаемый массив байтов
            byte[] fileContents;

            // Читаем файл
            var dictionary = new Dictionary<KeyValuePair<int, int>, Object>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyToAsync(stream);

                        using (ExcelPackage package = new ExcelPackage(stream))
                        {
                            if (package.Workbook.Worksheets.Count == 0)
                                return Ok(new { count = files.Count, size, filePath });
                            else
                            {
                                /*
                                foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                                {
                                    var cells = worksheet.Cells;
                                    dictionary = cells
                                        .GroupBy(c => new { c.Start.Row, c.Start.Column })
                                        .ToDictionary(
                                            rcg => new KeyValuePair<int, int>(rcg.Key.Row, rcg.Key.Column),
                                            rcg => cells[rcg.Key.Row, rcg.Key.Column].Value);
                                    Console.WriteLine(dictionary);
                                }*/

                                ExcelWorksheet worksheet = package.Workbook.Worksheets[2];
                                var cells = worksheet.Cells;
                                dictionary = cells
                                    .GroupBy(c => new { c.Start.Row, c.Start.Column })
                                    .ToDictionary(
                                        rcg => new KeyValuePair<int, int>(rcg.Key.Row, rcg.Key.Column),
                                        rcg => cells[rcg.Key.Row, rcg.Key.Column].Value);
                                Console.WriteLine(dictionary);
                            }
                        }
                    }
                }
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                double[] XArr = new double[19];
                double[] YArr = new double[19];

                // для форума данные, потом удалить
                XArr = new double[19] { 1.19, 1.79, 4.35, 4.94, 4.05, 4.74, 4.16, 1.7, 4.3, 3.02, 2.29, 2.76, 3.45, 3.58, 3.96, 3.11, 2.03, 2.67, 2.13 };
                YArr = new double[19] { 79, 209.67, 1026.71, 2161.7, 1192.74, 1128.06, 450.31, 169.13, 606.38, 763.49, 210.5, 309.34, 404.25, 1462.95, 797.18, 1540.24, 440.09, 352.22, 120.83 };

                //Пример заполнения ячеек
                // KeyValuePair<int, int> keyValue;
                int countRows = 0;
                double valueX = 0;
                double valueY = 0;
                for (int j = 1; j < 20; j++)
                {
                    try
                    {
                        countRows++;
                        // keyValue = new KeyValuePair<int, int>(j, 3);
                        // valueX = Math.Round(Convert.ToDouble(dictionary[keyValue]), 2);
                        valueX = XArr[j - 1];
                        worksheet.Cells[j, 1].Value = valueX;
                        // keyValue = new KeyValuePair<int, int>(j, 4);
                        // valueY = Math.Round(Convert.ToDouble(dictionary[keyValue]), 2);
                        valueY = YArr[j - 1];
                        worksheet.Cells[j, 2].Value = valueY;

                        XArr[j - 1] = valueX;
                        YArr[j - 1] = valueY;
                    }
                    catch(KeyNotFoundException ex)
                    {
                        // кидаем в консоль
                        Console.WriteLine(ex);
                    }
                }

                Approximation approxim = new Approximation();
                approxim.XArr = XArr;
                approxim.YArr = YArr;
                approxim.Exponential();

                ExcelChart chrt = worksheet.Drawings.AddChart("FindingsChart", OfficeOpenXml.Drawing.Chart.eChartType.XYScatter);
                chrt.Title.Text = "Category Chart";
                chrt.SetPosition(1, 0, 3, 0);
                // chart.SetSize(800, 300);
                var ser1 = (ExcelChartSerie)(chrt.Series.Add(worksheet.Cells["B1:B" + countRows.ToString()], worksheet.Cells["A1:A" + countRows.ToString()]));

                // chart.YAxis.LogBase = Math.E;

                var tl = chrt.Series[0].TrendLines.Add(eTrendLine.Exponential);
                tl.Name = "Test";
                tl.DisplayRSquaredValue = true;
                tl.DisplayEquation = true;
                tl.Forward = 15;
                tl.Backward = 1;
                tl.Intercept = 6;
                //tl.Period = 12;
                tl.Order = 5;

                var b1 = tl.ToString();
                var b2 = tl.GetHashCode();

                var r2 = chrt.Series[0].TrendLines.ToList();

                //var b5 = b3.StructLayoutAttribute;
                //var b6 = b3.GetFields();
                //var b7 = b3.GetNestedTypes();
                //var b8 = b3.GetProperties();
                // var b9 = b3.GetEnumValues();
                //var b10 = b3.ContainsGenericParameters;
                //var b11 = b3.GenericParameterAttributes;
                //var b12 = b3.GenericTypeArguments;
                //var b13 = b3.GetInterfaces();
                //var b14 = b3.StructLayoutAttribute;

                using (ApplicationContext db = new ApplicationContext())
                {
                    // создаем два объекта User
                    BaseField user1 = new BaseField { NameField = "Архангельское" };

                    // добавляем их в бд
                    db.BaseFields.Add(user1);
                    db.SaveChanges();

                    // получаем объекты из бд и выводим на консоль
                    var OilFields = db.BaseFields.ToList();
                    Console.WriteLine("Users list:");
                }

                var babac = User.Identity.Name;

                string strUserId = WindowsIdentity.GetCurrent().Name;

                chrt.Fill.Color = Color.LightSteelBlue;
                chrt.Border.LineStyle = eLineStyle.Dot;
                chrt.Border.Fill.Color = Color.Black;

                chrt.Legend.Font.Color = Color.Red;
                chrt.Legend.Font.Strike = eStrikeType.Double;
                chrt.Title.Font.Color = Color.DarkGoldenrod;
                chrt.Title.Font.LatinFont = "Arial";
                chrt.Title.Font.Bold = true;
                chrt.Title.Fill.Color = Color.White;
                chrt.Title.Border.Fill.Style = eFillStyle.SolidFill;
                chrt.Title.Border.LineStyle = eLineStyle.LongDashDotDot;
                chrt.Title.Border.Fill.Color = Color.Tomato;
                // chrt.DataLabel.ShowSeriesName = true;
                // chrt.DataLabel.ShowLeaderLines = true;
                chrt.EditAs = eEditAs.OneCell;
                chrt.DisplayBlanksAs = eDisplayBlanksAs.Span;
                chrt.Axis[0].Title.Text = "Axis 0";
                chrt.Axis[0].Title.Rotation = 90;
                chrt.Axis[0].Title.Overlay = true;
                chrt.Axis[1].Title.Text = "Axis 1";
                chrt.Axis[1].Title.AnchorCtr = true;
                chrt.Axis[1].Title.TextVertical = eTextVerticalType.Vertical270;
                chrt.Axis[1].Title.Border.LineStyle = eLineStyle.LongDashDotDot;


                // chart.Series[0].Border.Width = 5;

                // var t1 = chart.Series[0];
                // var t2 = t1.TrendLines.LastOrDefault();
                // t2.
                // t2.;
                // var t2 = t1.TrendLines[0];

                // Finally when you're done, export it to byte array.
                fileContents = package.GetAsByteArray();
            }

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "test.xlsx"
            );
        }


        [HttpPost("UploadExcelFileEconomist")]
        public IActionResult PostEconomist(string RTwo, List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // Полный путь до файла
            var filePath = Path.GetTempFileName();

            // возвращаемый массив байтов
            byte[] fileContents;


            int countSheet = 0;
            // Читаем файл
            var dictionary = new Dictionary<KeyValuePair<int, int>, Object>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyToAsync(stream);

                        using (ExcelPackage package = new ExcelPackage(stream))
                        {
                            countSheet = package.Workbook.Worksheets.Count;
                            if (countSheet == 0)
                                return Ok(new { count = files.Count, size, filePath });
                            else
                            {
                                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                                var cells = worksheet.Cells;
                                dictionary = cells
                                    .GroupBy(c => new { c.Start.Row, c.Start.Column })
                                    .ToDictionary(
                                        rcg => new KeyValuePair<int, int>(rcg.Key.Row, rcg.Key.Column),
                                        rcg => cells[rcg.Key.Row, rcg.Key.Column].Value);
                                cells.Dispose();
                                package.Dispose();
                            }
                        }

                        stream.Dispose();
                        stream.Close();
                    }
                }
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                //Пример заполнения ячеек
                KeyValuePair<int, int> keyValue;
                for (int j = 1; j < 20; j++)
                {
                    try
                    {
                        keyValue = new KeyValuePair<int, int>(j, 3);
                        try {
                            worksheet.Cells[j, 1].Value = Convert.ToDouble(dictionary[keyValue]);
                        }
                        catch (FormatException ex){
                            worksheet.Cells[j, 1].Value = 0;
                        }

                        keyValue = new KeyValuePair<int, int>(j, 4);
                        try {
                            worksheet.Cells[j, 2].Value = Convert.ToDouble(dictionary[keyValue]);
                        }
                        catch (FormatException ex) {
                            worksheet.Cells[j, 2].Value = 0;
                        }
                    }
                    catch (KeyNotFoundException ex)
                    {
                        // кидаем в консоль
                        Console.WriteLine(ex);
                    }
                }


                // Finally when you're done, export it to byte array.
                fileContents = package.GetAsByteArray();

                package.Dispose();
            }

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "test.xlsx"
            );
        }
    }
}