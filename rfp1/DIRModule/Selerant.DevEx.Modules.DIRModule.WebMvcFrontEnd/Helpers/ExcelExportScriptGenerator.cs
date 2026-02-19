using Selerant.ApplicationBlocks.PathManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Utils;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.HtmlGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers
{
    public class ExcelExportScriptGenerator
    {
        private DataSet DataSet { get; }
        private string FileName { get; }
        private BaseControllerData ControllerData { get; }

        public ExcelExportScriptGenerator(BaseControllerData controllerData, string fileName)
        {
            ControllerData = controllerData;
            FileName = fileName;
            DataSet = new DataSet();
        }

        public ExcelExportScriptGenerator AddData<T>(string gridId, ICollection<T> data, string tableName)
        {
            HtmlGridHelper helper = new SecureHtmlGridHelper(ControllerData, gridId, null, ControllerData.SecurityObject);
            DataTable dataTable = helper.ConvertToDataTable(data, helper.GetUIColumns());
            dataTable.TableName = tableName;
            DataSet.Tables.Add(dataTable);

            return this;
        }

        public string GetExecutingScript()
        {
            string path = Utilities.SafePathCombine(PathFinder.Instance.GetFolderPath(PathFinder.FolderKeys.ExportDataFolder), FileName);

            DIRDataSetExporter dataSetExporter = new DIRDataSetExporter(DataSet);
            dataSetExporter.ExportToExcelFile(path);

            string scriptToExecute = PrintHelper.GetOpenDocumentScript(new OpenDocumentContext
            {
                FolderKey = OpenDocumentFolderKeys.ExportDataFolder,
                FileName = FileName,
                IsTemporaryDocument = true
            });

            return scriptToExecute;
        }
    }
}