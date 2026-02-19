using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using OfficeOpenXml;
using Selerant.ApplicationBlocks.PathManagement;
using Selerant.DevEx.BusinessLayer.Configuration;
using Selerant.DevEx.MiddleLayerAdapter.Exporters;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.Utils
{
    public class DIRDataSetExporter : DataSetExporter
    {
        #region Constants

        private const string OpenXmlExcelTemplateTitleStyleName = "Title";

        #endregion

        public DIRDataSetExporter(DataSet dataSet) : base(dataSet)
        {
        }

        public void ExportToExcelFile(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();

            if (extension == ".xlsx")
            {
                ExportToOpenXmlExcelFile(fileName);
            }
        }

        /// <summary>
		/// Exports the given file to the Open Xml Excel format (xlsx).
		/// </summary>
		/// <param name="fileName"></param>
        private void ExportToOpenXmlExcelFile(string fileName)
        {
            // If a target file already exists, delete it because the ExcelPackage object does not overwrite any
            // existing content.
            if (File.Exists(fileName))
                File.Delete(fileName);

            // Provide a template file name for applying styles.
            string templateFileName = PathFinder.Instance.GetFilePath(PathFinder.FileKeys.OpenXmlExcelExportDataTemplate_File);

            using (ExcelPackage package = new ExcelPackage(new FileInfo(fileName), new FileInfo(templateFileName)))
            {
                if (dataSet.Tables.Count > 0)
                {
                    package.Workbook.Worksheets.ToList().RemoveAll(x => x.Name.StartsWith("Sheet"));

                    int sheetIndex = 1;

                    foreach (DataTable table in dataSet.Tables)
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Index == sheetIndex);
                        sheetIndex++;

                        if (worksheet == null)
                            worksheet = package.Workbook.Worksheets.Add(table.TableName);
                        else
                            worksheet.Name = table.TableName;

                        WriteColumnsToOpenXmlExcelWorksheet(table.Columns, worksheet, 1);
                        WriteRowsToOpenXmlExcelWorksheet(table.Select(), worksheet, 2);
                    }
                }

                package.Save();
            }
        }

        private void WriteColumnsToOpenXmlExcelWorksheet(DataColumnCollection columns, ExcelWorksheet worksheet, int rowIndex)
        {
            int columnIndex = 1;

            ExcelRow excelRow = worksheet.Row(rowIndex);

            foreach (DataColumn column in columns)
            {
                ExcelRangeBase cell = worksheet.Cells[excelRow.Row, columnIndex];
                cell.Value = column.Caption;
                cell.StyleName = OpenXmlExcelTemplateTitleStyleName;

                worksheet.Column(columnIndex).Width = 25;

                columnIndex++;
            }
        }

        private void WriteRowsToOpenXmlExcelWorksheet(DataRow[] rows, ExcelWorksheet worksheet, int startRowIndex)
        {
            int rowIndex = startRowIndex;
            int rowCount = 0;

            foreach (DataRow row in rows)
            {
                if (rowCount > 100)
                {
                    Thread.Sleep(0);
                    rowCount = 0;
                }
                rowCount++;

                int columnIndex = 1;
                ExcelRow excelRow = worksheet.Row(rowIndex);

                foreach (object value in row.ItemArray)
                {
                    ExcelRangeBase cell = worksheet.Cells[excelRow.Row, columnIndex];

                    if (value is Array)
                    {
                        string cellValue = null;

                        foreach (object arrayValue in (Array)value)
                            cellValue += FormatValue(arrayValue) + "\n";

                        if (!string.IsNullOrEmpty(cellValue))
                            cellValue = cellValue.TrimEnd('\n');

                        cell.Value = cellValue;
                    }
                    else
                    {
                        string formattedValue = HttpUtility.HtmlDecode(FormatValue(value));
                        decimal outForDecimal;
                        DateTime dateTime;

                        if (IsNumericValue(formattedValue) && decimal.TryParse(formattedValue, out outForDecimal))
                        {
                            cell.Value = outForDecimal.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            if (DateTime.TryParse(formattedValue, out dateTime))
                            {
                                if (IsTimeValue(formattedValue))
                                {
                                    cell.Style.Numberformat.Format = "hh:mm:ss";
                                }
                                else
                                {
                                    cell.Style.Numberformat.Format = System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                                }

                                cell.Value = dateTime;
                            }
                            else
                            {
                                cell.Value = formattedValue;
                            }
                        }
                    }

                    columnIndex++;
                }

                rowIndex++;
                int relationCount = 0;

                foreach (DataRelation relation in row.Table.DataSet.Relations)
                {
                    if (relationCount > 100)
                    {
                        Thread.Sleep(0);
                        relationCount = 0;
                    }

                    relationCount++;

                    DataRow[] childrenRows = row.GetChildRows(relation);
                    if (childrenRows.Length > 0)
                    {
                        WriteChildrenRowsToOpenXmlExcelWorksheet(childrenRows, worksheet, rowIndex);

                        // Increment row index to the number of children rows plus 2 to take into account the row showing columns names.
                        rowIndex += childrenRows.Length + 2;
                    }
                }
            }
        }

        private void WriteChildrenRowsToOpenXmlExcelWorksheet(DataRow[] rows, ExcelWorksheet worksheet, int startRowIndex)
        {
            WriteColumnsToOpenXmlExcelWorksheet(rows[0].Table.Columns, worksheet, startRowIndex);
            WriteRowsToOpenXmlExcelWorksheet(rows, worksheet, startRowIndex + 1);
        }

        private string FormatValue(object value)
        {
            return FormattingContext.Format(value, CultureInfo);
        }
    }
}
