using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectConnectionPredictControl.CommenTool
{
    class ExcelUtils
    {
        private Application app;
        private Workbooks workbooks;
        private Workbook workbook;
        private Sheets sheets;
        private string fileName;

        public ExcelUtils(string fileName)
        {
            this.fileName = fileName;
            app = new Application();
            DefaultSetting();
            workbooks = app.Workbooks;
            workbook = workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            
        }

        public void Write(object[, ] data, string sheetName, object[, ] header)
        {
            Worksheet sheet;
            int rowNum = data.Length;
            int colunmNum = data.GetLength(1);
            if (sheetName == "EBCU1")
            {
                sheet = workbook.Worksheets[1];
            }
            else
            {
                object missing = System.Reflection.Missing.Value;
                sheet = workbook.Worksheets.Add(missing, missing, missing, missing);
            }
            sheet.Name = sheetName;
            Range range = sheet.Range[sheet.Cells[1, 1], sheet.Cells[1, colunmNum]];
            range.Value2 = header;
            range = sheet.Range[sheet.Cells[2, 1], sheet.Cells[rowNum + 1, colunmNum]];
            range.Value2 = data;
            workbook.Save();

        }

        public void Save()
        {
            workbook.SaveCopyAs(fileName);
            workbook.Close();
            app.Quit();

        }

        private void DefaultSetting()
        {
            app.Visible = false;
            app.DisplayAlerts = false;
            app.ScreenUpdating = false;
        }

        

    }
}
