
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace SQLAddIn
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        private dynamic GetExcelApplicationSelection()
        {
            Excel.Application excelApp = Globals.ThisAddIn.Application;
            return excelApp.Selection;
        }

        public Excel.Worksheet GetActiveWorkSheet()
        {
            return (Excel.Worksheet)Application.ActiveSheet;
        }

        public Excel.Range GetActiveCell()
        {
            var selection = GetExcelApplicationSelection();
            return selection != null && selection.Cells.Count == 1 ? selection : null;
        }

        public Excel.Range GetActiveCells()
        {
            var selection = GetExcelApplicationSelection();
            return selection != null ? selection : null;
        }

        public string GetActiveCellAddress()
        {
            var activeCell = GetActiveCell();
            return activeCell != null ? activeCell.get_Address() : null;
        }


        public string GetActiveCellString()
        {
            var activeCell = GetActiveCell();
            return activeCell != null ? activeCell.Value2 as string : null;
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
