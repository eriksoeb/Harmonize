using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Excel = Microsoft.Office.Interop.Excel;

namespace SQLAddIn
{

    public partial class Harmonize
    {


//private SqlConnection _sqlConnection;
private Worksheet _worksheet;
        // Class-level variable: store only the connection string
        private string _connectionString;

        int previousRowCount = 0; // store this  delete old work when refresh
        int prevActiveColoumn = -1; // store this - delete from here
        int prevActiveRow = -1; // store this  and here




        // Load connection string at add-in startup
        private void SQLAddIn_Load(object sender, RibbonUIEventArgs e)
        {

            try
            {
                         

                string folder = "C:\\Harmonize\\App";
                string connectionFile = Path.Combine(folder, "connection.txt");

                if (!File.Exists(connectionFile))
                {
                    MessageBox.Show("connection.txt not found:\n" + connectionFile, "Connection Error");
                    return;
                }

                // Read first line only
                string connectionString = File.ReadLines(connectionFile).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    MessageBox.Show("connection.txt is empty.", "Connection Error");
                    return;
                }

                _connectionString = connectionString;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Startup Error");
            }
        }

        private void SetMetaColumnNames(int rowIndex, int colIndex)
        {
            _worksheet.Cells[rowIndex, colIndex] = "Description";
            _worksheet.Cells[rowIndex, colIndex + 1] = "IndexName";
            // _worksheet.Cells[rowIndex, colIndex].EntireRow.Font.Bold = true;


            Range headerRange = _worksheet.Range[
             _worksheet.Cells[rowIndex, colIndex],
               _worksheet.Cells[rowIndex, colIndex + 1]
                ];

            headerRange.Font.Bold = true;

        }





        private bool TryParseHarmonize(string input, out string seriesName, out string[] parameters)
        {
            seriesName = null;
            parameters = null;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            var parts = input.Split('|');

            if (parts.Length < 2)
                return false;

            if (!parts[0].Equals("HARMONIZE", StringComparison.OrdinalIgnoreCase))
                return false;

            seriesName = parts[1].Trim();

            parameters = parts
                .Skip(2)
                .Select(p => p.Trim())
                .ToArray();

            return true;
        }




        private void btnExecuteSQL_Click(object sender, RibbonControlEventArgs e)
        {
            var cell = Globals.ThisAddIn.Application.ActiveCell;
            ExecuteSQLForCell(cell);
        }





        //private void btnExecuteSQL_Click(object sender, RibbonControlEventArgs e)
        private void ExecuteSQLForCell(Excel.Range cell)

        {
            _worksheet = Globals.ThisAddIn.GetActiveWorkSheet();
            Range activeCell = null;

            try
            {

                if (string.IsNullOrWhiteSpace(_worksheet.Range["A1"].Value))
                {
                    // Use A1 as the active cell
                    activeCell = _worksheet.Cells[1, 1];

                 

                    SetHeaderNameCell("HARMONIZE|CPI:C00.IDX|NULL|YearsLast5|NONE|OFF|asc", 1, 1);

                    // Metadata row (row 2)
                    SetMetaColumnNames(activeCell.Row + 1, activeCell.Column);
                 
                    GetDataFromDatabaseAndFillCellswMeta("execute StpGetMeta 'CPI:C00.IDX'", activeCell.Row + 1, activeCell.Column - 1);

                    // Data row (row 4)
                    SetColumnNameCells(activeCell.Row + 2, activeCell.Column);
                    GetDataFromDatabaseAndFillCells(
                          // This is ok, with new params, sample rows works good
                          "execute StpGetSeries 'CPI:C00.IDX',NULL,'YEARSLAST5',NULL,1,'client', 'NONE','OFF','asc',250, NULL",
                        activeCell.Row + 3,
                        activeCell.Column - 1
                    );

                    ((_worksheet.Cells[1, 1] as Range).EntireColumn).AutoFit();
                }




                else
                {
                    var activeCells = Globals.ThisAddIn.GetActiveCells();
                    foreach (Range ac in activeCells)
                    {
                        activeCell = ac;
                 

                        var activeCellString = ac.Value2 as string;

                        if (!TryParseHarmonize(activeCellString, out var seriesName, out var parameters))
                            continue;




                        // "execute Harmonize- 'PPI:SNN0.IDX',NULL,'YEARSLAS
                        Range highlightRange = _worksheet.Range[
                         _worksheet.Cells[activeCell.Row, activeCell.Column],
                          _worksheet.Cells[activeCell.Row, activeCell.Column + 1]
                            ];

                        (activeCell.EntireColumn).AutoFit();
                        //highlightRange.Interior.Color = System.Drawing.Color.LightGreen;
                        highlightRange.Interior.Color = System.Drawing.Color.LightBlue;




                        //new part decripion dec and Index in row 1
                        SetColumnNameCells(activeCell.Row + 1, activeCell.Column);
                        string sqlmeta = $"execute StpGetMeta '{seriesName}'";
                        GetDataFromDatabaseAndFillCellswMeta(sqlmeta, activeCell.Row + 1, activeCell.Column - 1);
                        //StpGetMeta returns one row with description, indexname 


                        //old part ok
                        SetColumnNameCells(activeCell.Row + 2, activeCell.Column);

               
                        //was ok
                        //need to tacke the parameters convert2freq and asc/desc TBC

                        string basis        = parameters.Length > 0 ? parameters[0].Trim() : "NULL";
                        string myInterval   = parameters.Length > 1 ? parameters[1].Trim() : "NULL";
                        string agg          = parameters.Length > 2 ? parameters[2].Trim() : "NONE";
                        string convert2freq = parameters.Length > 3 ? parameters[3].Trim() : "OFF";
                        string sort         = parameters.Length > 4 ? parameters[4].Trim() : "desc";

                        // --- Parameter validation ---
                        var validSort        = new[] { "asc", "desc" };
                        var validConvert     = new[] { "OFF", "MON", "Q", "ANN" };
                        var validAgg         = new[] { "NONE", "AVG_YEAR", "AVG_QUARTER", "AVG_MONTH", "AVG_DAY",
                                                       "SUM_YEAR", "SUM_DAY", "LAST_MONTH" };

                        if (!Array.Exists(validSort, v => v.Equals(sort, StringComparison.OrdinalIgnoreCase)))
                            throw new Exception($"Bad sort param '{sort}' — use: {string.Join(", ", validSort)}");

                        if (!Array.Exists(validConvert, v => v.Equals(convert2freq, StringComparison.OrdinalIgnoreCase)))
                            throw new Exception($"Bad convert2freq param '{convert2freq}' — use: {string.Join(", ", validConvert)}");

                        if (!Array.Exists(validAgg, v => v.Equals(agg, StringComparison.OrdinalIgnoreCase)))
                            throw new Exception($"Bad agg param '{agg}' — use: {string.Join(", ", validAgg)}");

                        if (!basis.Equals("NULL", StringComparison.OrdinalIgnoreCase) &&
                            !(int.TryParse(basis, out int basisYear) && basisYear >= 1950 && basisYear <= 2090))
                            throw new Exception($"Bad basis param '{basis}' — use a year 1950-2090 or NULL");
                        // --- End validation ---

                        string sql = $"execute StpGetSeries '{seriesName}',{basis},'{myInterval}',NULL,1,'client','{agg}','{convert2freq}','{sort}',250,NULL";

                        GetDataFromDatabaseAndFillCells(sql, activeCell.Row + 3, activeCell.Column - 1);
                    }
                }
            }
            catch (Exception ex)
            {
                if (activeCell != null)
                { 
                    // _worksheet.Cells[activeCell.Row + 3, activeCell.Column] = $"Error! {ex.Message} Ups!, Check name of the series and/or parameters.";
                    //ribbon_grp.Label = $"Error! {ex.Message} UPS!, Check SeriesName and parameters !.";
                    ribbon_grp.Label = $"⚠ {ex.Message}";
                    Globals.ThisAddIn.Application.StatusBar = $"Error: {ex.Message}";
                    Range highlightRange = _worksheet.Range[
                     _worksheet.Cells[activeCell.Row, activeCell.Column],
                      _worksheet.Cells[activeCell.Row, activeCell.Column + 1]
                        ];
                   highlightRange.Interior.Color = System.Drawing.Color.Red;
                   // highlightRange.Interior.Color = System.Drawing.Color.Yellow;

                    //sletter 2 nest rader med navn og dato header
                    Range clearRange = _worksheet.Range[
                    _worksheet.Cells[activeCell.Row+1, activeCell.Column],
                     _worksheet.Cells[activeCell.Row+2, activeCell.Column + 1]
                       ];

                    //clearRange.Clear();
                    clearRange.ClearContents(); //keeps formatting




                }


                else
                { 
                     //MessageBox.Show(ex.Message, "GET ERROR");
                    ribbon_grp.Label = "⚠ " + ex.Message;
                    Globals.ThisAddIn.Application.StatusBar = "Error: " + ex.Message;

            }

            }
          
        }




        private void SetHeaderNameCell(string name, int rowIndex, int colIndex)
        {
            _worksheet.Cells[rowIndex, colIndex] = name;
            //_worksheet.Cells[rowIndex, colIndex].EntireRow.Font.Bold = true;


            // Create a Range for this cell only ?
            Range writeRange = _worksheet.Range[
                _worksheet.Cells[rowIndex, colIndex],
                _worksheet.Cells[rowIndex, colIndex+1] //the extra empy cell as well -not needed
            ];

            writeRange.Font.Bold = true;
            //writeRange.Interior.Color = System.Drawing.Color.LightGreen;
            writeRange.Interior.Color = System.Drawing.Color.LightBlue;


        }

        private void SetColumnNameCells(int rowIndex, int colIndex)
        {
            _worksheet.Cells[rowIndex, colIndex] = "Date"; ///UPS
            _worksheet.Cells[rowIndex, colIndex + 1] = "Value";
            //_worksheet.Cells[rowIndex, colIndex].EntireRow.Font.Bold = true;

            // Create a Range for these two cells
            Range writeRange = _worksheet.Range[
                _worksheet.Cells[rowIndex, colIndex],
                _worksheet.Cells[rowIndex, colIndex + 1]
            ];

            // Set light blue fill
            writeRange.Font.Bold = true;
            writeRange.Interior.Color = System.Drawing.Color.LightBlue;


        }


        private void GetDataFromDatabaseAndFillCellswMeta(string sql, int rowStart, int colOffset)
        {
            //2 extra
            ClearOldDataCells(rowStart, colOffset, previousRowCount, 2);

            var dataTable = new System.Data.DataTable();

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var adapter = new SqlDataAdapter(sql, conn))
                {
                    adapter.Fill(dataTable);
                }
            }

            if (dataTable.Rows.Count == 0) return;

            // We expect ONE row
            var row = dataTable.Rows[0];

            // Adjust column names depending on your SQL result!
            // Example assumes: Descr, IndexName --this is ok
            string description = row["Descr"]?.ToString();
            string indexName = row["IndexName"]?.ToString();

            // Write to Excel (row 2 typically)
            _worksheet.Cells[rowStart, 1 + colOffset] = description;
            _worksheet.Cells[rowStart, 2 + colOffset] = indexName;

            // Optional formatting
            Range writeRange = _worksheet.Range[
                _worksheet.Cells[rowStart, 1 + colOffset],
                _worksheet.Cells[rowStart, 2 + colOffset]
            ];

            //  writeRange.Font.Italic = true;
            //writeRange.Font.Color = System.Drawing.Color.LightBlue;
            writeRange.Interior.Color = System.Drawing.Color.LightBlue;
        }

        private void GetDataFromDatabaseAndFillCells(string sql, int rowStart, int colOffset)
        {


            // if at same place as last time I delete, same num of rows as last time
            if (colOffset == prevActiveColoumn && rowStart == prevActiveRow) {
                ClearOldDataCells(rowStart, colOffset, previousRowCount, 2);
            }
          
            var dataTable = new System.Data.DataTable();



         

            // OPEN FRESH CONNECTION per read
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var adapter = new SqlDataAdapter(sql, conn))
                {
                    adapter.Fill(dataTable);
                }
            }

            if (dataTable.Rows.Count == 0) return;

            int rows = dataTable.Rows.Count;
            object[,] data = new object[rows, 2];



            for (int i = 0; i < rows; i++)
            {
                data[i, 0] = Convert.ToDateTime(dataTable.Rows[i][0]).ToOADate();
                data[i, 1] = dataTable.Rows[i][1] == DBNull.Value ? (object)null : dataTable.Rows[i][1];
            }

            Range startCell = _worksheet.Cells[rowStart, 1 + colOffset];
            Range endCell = _worksheet.Cells[rowStart + rows - 1, 2 + colOffset];
            Range writeRange = _worksheet.Range[startCell, endCell];

            Range dateColRange = _worksheet.Range[
                _worksheet.Cells[rowStart, 1 + colOffset],
                _worksheet.Cells[rowStart + rows - 1, 1 + colOffset]
            ];

            dateColRange.NumberFormat = "General";            // break any locked format
            dateColRange.NumberFormat = "[$-409]yyyy-mm-dd";  // locale-safe date format
            writeRange.Value2 = data;                         // write AFTER format is set



            // _worksheet.Cells[rowStart - 2, 1 + colOffset] = $"{rows} rows";
            ribbon_grp.Label = "Status OK. Read from database : " + $"{rows} rows";
            Globals.ThisAddIn.Application.StatusBar = false;
            previousRowCount = rows;
            prevActiveColoumn = colOffset;
            prevActiveRow = rowStart;
        }




/*
        private void ClearOldDataCells_old(int rowDeltaIndex, int columnDeltaIndex)
        {
            if (rowDeltaIndex <= 2) return;  // safety for Excel rows

            var cellValue = _worksheet.Cells[rowDeltaIndex - 2, 1 + columnDeltaIndex].Value;

            if (cellValue != null)
            {
                var text = cellValue.ToString();

                if (text.Contains("rows"))
                {
                    var oldDataRowCount = Convert.ToInt32(text.Replace("rows", string.Empty).Trim());

                    for (var i = 0; i < oldDataRowCount; i++)
                    {
                        _worksheet.Cells[rowDeltaIndex + i, 1 + columnDeltaIndex] = null;
                        _worksheet.Cells[rowDeltaIndex + i, 2 + columnDeltaIndex] = null;
                    }
                }
            }
        }
*/


        private void ClearOldDataCells(int startRow, int startColumn, int oldRowCount, int columnCount)
        {
            // Scan down the date column (startColumn + 1) and clear date+value rows
            // as long as the date cell has content — ignores oldRowCount entirely.
            int dateCol = startColumn + 1;
            int row = startRow;

            while (true)
            {
                var dateCell = _worksheet.Cells[row, dateCol] as Range;
                if (dateCell == null || dateCell.Value2 == null)
                    break;

                (_worksheet.Cells[row, dateCol] as Range).ClearContents();
                (_worksheet.Cells[row, dateCol + 1] as Range).ClearContents();
                row++;
            }
        }









        // PUT SERIES back to db:    
        //sample header SetHeaderNameCell("HARMONIZE|CPI:C00.IDX|NULL|YearsLast5|AVG_MONTH",


        private void Put_btn_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                var app = Globals.ThisAddIn.Application;
                var worksheet = Globals.ThisAddIn.GetActiveWorkSheet();
                var activeCells = Globals.ThisAddIn.GetActiveCells();
                int rowsProcessed = 0;

                if (activeCells == null)
                {
                    //MessageBox.Show("No header cells selected.", "PUT ERROR");
                  
                    ribbon_grp.Label = "No header cells selected.";
                    return;
                }
               

                string mydescr = activeCells.Offset[1, 0].Value2?.ToString() ?? "";
                string myunit = activeCells.Offset[1, 1].Value2?.ToString() ?? "";


                // Create a Range for these two cells actice cell, and the cell in column+1
                Range writeRange = _worksheet.Range[
     _worksheet.Cells[activeCells.Row, activeCells.Column],
     _worksheet.Cells[activeCells.Row, activeCells.Column + 1]
 ];

                writeRange.Font.Bold = true;
                writeRange.Interior.Color = System.Drawing.Color.Orange;



             
                // Validate headers are in row 1
                foreach (Range cell in activeCells)
                {
                    if (cell.Row != 1)
                    {
                        ribbon_grp.Label = "Please select header cells in row 1 only.";
                       // MessageBox.Show("Please select header cells in row 1 only.", "PUT ERROR");
                        return;
                    }
                }

                app.ScreenUpdating = false;
                List<string> updatedSeries = new List<string>();

                // Open one connection per PUT batch
                using (SqlConnection sqlCon = new SqlConnection(_connectionString))
                {
                    sqlCon.Open();

                    foreach (Range headerCell in activeCells)
                    {
                        /*
                        string header = worksheet.Cells[1, headerCell.Column].Value2 as string;
                        if (string.IsNullOrWhiteSpace(header)) continue;

                        var seriesPart = header.Split('(')[0].Trim();
                        var parts = seriesPart.Split(':');
                        if (parts.Length < 2) continue;

                        string loadset = parts[0].Trim();
                        string seriesName = parts[1].Trim();
                        */

                        //new part
                        string header = worksheet.Cells[1, headerCell.Column].Value2 as string;
                        if (string.IsNullOrWhiteSpace(header)) continue;

                        var parts = header.Split('|');

                        if (parts.Length < 2 ||
                            !parts[0].Equals("HARMONIZE", StringComparison.OrdinalIgnoreCase))
                            continue;

                        var seriesPart = parts[1].Trim();

                        var idx = seriesPart.IndexOf(':');
                        if (idx == -1) continue;

                        string loadset = seriesPart.Substring(0, idx).Trim();
                        string seriesName = seriesPart.Substring(idx + 1).Trim();

                        int startRow = headerCell.Row + 3;
                        int col = headerCell.Column;

                        // Find last row
                        int lastRow = worksheet.Cells[worksheet.Rows.Count, col]
                                        .End(Excel.XlDirection.xlUp).Row;

                        if (lastRow < startRow) continue;

                        Range readStart = worksheet.Cells[startRow, col];
                        Range readEnd = worksheet.Cells[lastRow, col + 1];
                        Range readRange = worksheet.Range[readStart, readEnd];

                        object[,] values = readRange.Value2 as object[,];
                        if (values == null) continue;

                        // Use one command for all rows of this series
                        using (SqlCommand cmd = new SqlCommand("dbo.UTILS_PutTime", sqlCon))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add("@passedlsname", SqlDbType.NVarChar);
                            cmd.Parameters.Add("@sname", SqlDbType.NVarChar);
                            cmd.Parameters.Add("@sdesc", SqlDbType.NVarChar).Value = DBNull.Value;
                            cmd.Parameters.Add("@unit_id", SqlDbType.Int).Value = DBNull.Value;
                            cmd.Parameters.Add("@datestr", SqlDbType.DateTime);
                            cmd.Parameters.Add("@value", SqlDbType.Decimal);

                            for (int i = 1; i <= values.GetLength(0); i++)
                            {
                                // Only stop when the DATE is missing — that's the end of the series
                                if (values[i, 1] == null) break;

                                DateTime vDate;
                                if (values[i, 1] is double d)
                                    vDate = DateTime.FromOADate(d);
                                else if (!DateTime.TryParse(values[i, 1].ToString(), out vDate))
                                    continue;

                                // NULL value → send DBNull to DB instead of skipping/crashing
                                if (values[i, 2] == null)
                                {
                                    cmd.Parameters["@value"].Value = DBNull.Value;
                                }
                                else
                                {
                                    if (!decimal.TryParse(values[i, 2].ToString(), out decimal val))
                                        continue;
                                    cmd.Parameters["@value"].Value = val;
                                }

                                //if unit missing it fails..
                                cmd.Parameters["@passedlsname"].Value = loadset;
                                cmd.Parameters["@sname"].Value = seriesName;
                                cmd.Parameters["@datestr"].Value = vDate;

                                cmd.ExecuteNonQuery();
                                rowsProcessed = i;
                            }
                        }

                        // Update series info
                        //using (SqlCommand updateCmd = new SqlCommand("dbo.UTILS_UpdateSingleCurveinfo", sqlCon))
                        // UTILS_UpdateMetaXLS
                       //was 2 arg  using (SqlCommand updateCmd = new SqlCommand("dbo.UTILS_UpdateSingleCurveinfo2arg", sqlCon))
                        using (SqlCommand updateCmd = new SqlCommand("dbo.UTILS_UpdateMetaXLS", sqlCon))

                        {
                            updateCmd.CommandType = CommandType.StoredProcedure;
                            //updateCmd.Parameters.Add("@seriesname", SqlDbType.NVarChar).Value = $"{loadset}:{seriesName}";

                            updateCmd.Parameters.Add("@loadsetname", SqlDbType.NVarChar).Value = $"{loadset}";
                            updateCmd.Parameters.Add("@seriesname", SqlDbType.NVarChar).Value = $"{seriesName}";

                            updateCmd.Parameters.Add("@description", SqlDbType.NVarChar, 255).Value = mydescr;
                            updateCmd.Parameters.Add("@unitname", SqlDbType.NVarChar, 64).Value = myunit;



                            updateCmd.ExecuteNonQuery();
                        }

                        updatedSeries.Add(seriesName);
                    }
                }

                app.ScreenUpdating = true;

                if (updatedSeries.Count > 0)
                {
                    /*
                    MessageBox.Show(
                        $"Updated series:\nRows: {rowsProcessed}\n" +
                        string.Join("\n", updatedSeries.Distinct()),
                        "Harmonize 2026";
                    */

                       ribbon_grp.Label = $"Upserted {rowsProcessed} rows ({updatedSeries.Count} series)";

                   // );
                }
                else
                {
                    //MessageBox.Show("No valid data found.", "PUT");
                    ribbon_grp.Label = "Warning: PUT - No valid series or parameters found in active header row.";
                    writeRange.Interior.Color = System.Drawing.Color.Red;

                }
            }
            catch (Exception ex)
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
               // MessageBox.Show(ex.Message, "PUT ERROR");
                ribbon_grp.Label = "ERROR: "+ ex.Message;
            }
        }





        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                string version = System.Reflection.Assembly
                    .GetExecutingAssembly()
                    .GetName()
                    .Version
                    .ToString();

                /*
                string folder1 = System.IO.Path.GetDirectoryName(
                    System.Reflection.Assembly
                    .GetExecutingAssembly()
                    .Location);
                

                string folder = AppDomain.CurrentDomain.BaseDirectory;

                string folder3 = Assembly.GetExecutingAssembly().Location;
                */

                string machine = Environment.MachineName;
                string user = Environment.UserName;

                string message =
                    "Harmonize Excel Add-in\n\n" +
                    "Version: " + version + "\n\n" +
                    "User: " + user + "\n\n" +
                    "Machine: " + machine + "\n\n" +
                    "Connection: C:\\Harmonize\\App\\connection.txt" + "\n\n\n" +
                    "Contact: Erik.Soberg@ssb.no" + "\n\n";
                // "Folder1: " + folder1 + "\n" +
                // "Folder3: " + folder3 + "\n" +
                // "Install Folder:\n" + folder;

                MessageBox.Show(
                    message,
                    "About Harmonize",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "About Error");
                

            }
        }



        private void button2_Click(object sender, RibbonControlEventArgs e)
        {
            try

            {
                ribbon_grp.Label = "Testing connection..";


                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("dbo.GetOriginalLogin", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {


                           
                                string message =
                                 $"Login: {reader["LoginName"]?.ToString()}\n" +
                                 $"Database: {reader["CurrentDatabase"]?.ToString()}\n" +
                                    // $"DatabaseId: {reader["DatabaseId"]?.ToString()}\n" +
                                       $"Appname: {reader["Appname"]?.ToString()}\n" +
                                 $"Instance: {reader["SqlInstanceName"]?.ToString()}\n" +                       
                                 $"Server Machine: {reader["ServerMachine"]?.ToString()}\n" +
                                         $"Client Machine: {reader["ClientMachine"]?.ToString()}\n" +


                                 $"SQL Version: {reader["SqlVersion"]?.ToString()}\n" +

                                 $"Product Level: {reader["SqlProductLevel"]?.ToString()}";

                                MessageBox.Show(message, "Harmonize", MessageBoxButtons.OK, MessageBoxIcon.Information);



                            }
                        }
                    }
                }
                ribbon_grp.Label = "Testing database connection..OK";

            }
           
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed:\n{ex.Message}");
            }
        }

        private void RefreshAll_btn_Click(object sender, RibbonControlEventArgs e)
        {

           // MessageBox.Show("Start refresh ..");
            Excel.Application app = _worksheet.Application;

            bool prevScreenUpdating = app.ScreenUpdating;
            bool prevEnableEvents = app.EnableEvents;
            int numlinks =0;

            try
            {
                app.ScreenUpdating = false;
                app.EnableEvents = false;

                foreach (Excel.Range cell in _worksheet.UsedRange)
                {
                    if (cell.Value2 == null) continue;

                    string text = cell.Value2.ToString();

                    //if (!text.StartsWith("Get:")) continue;
                  //  if (!text.Contains(":")) continue;
                    if (!text.StartsWith("HARMONIZE|")) continue;
                    numlinks++;
                    cell.Activate();      // required for your legacy logic
                                         
                    ExecuteSQLForCell(cell);
                }
                if (numlinks == 0)
                {
                    MessageBox.Show("No Harmonize Links to refresh, -Start using Getserie");
                    ribbon_grp.Label =  "Ups, -No Series found to refresh in current sheet";
                }
                else
                {
                    ribbon_grp.Label = "Refreshed :"+numlinks.ToString() +" Series";

                }
            }
            finally
            {
                app.ScreenUpdating = prevScreenUpdating;
                app.EnableEvents = prevEnableEvents;
            }




        }

        private void delete_serie_btn_Click(object sender, RibbonControlEventArgs e)
        {
            //deleteseries
            try
            {
                var app = Globals.ThisAddIn.Application;
                var worksheet = Globals.ThisAddIn.GetActiveWorkSheet();
                var activeCells = Globals.ThisAddIn.GetActiveCells();
               

                if (activeCells == null)
                {
                    //MessageBox.Show("No header cells selected.", "PUT ERROR");

                    ribbon_grp.Label = "No header cells selected.";
                    return;
                }


                string mydescr = activeCells.Offset[1, 0].Value2?.ToString() ?? "";
                string myunit = activeCells.Offset[1, 1].Value2?.ToString() ?? "";


                // Create a Range for these two cells actice cell, and the cell in column+1
                Range writeRange = _worksheet.Range[
     _worksheet.Cells[activeCells.Row, activeCells.Column],
     _worksheet.Cells[activeCells.Row, activeCells.Column + 1]
 ];

                writeRange.Font.Bold = false;
                writeRange.Interior.Color = System.Drawing.Color.Gray;



                
                // Validate headers are in row 1
                foreach (Range cell in activeCells)
                {
                    if (cell.Row != 1)
                    {
                        ribbon_grp.Label = "Please select header cells in row 1 only.";
                        // MessageBox.Show("Please select header cells in row 1 only.", "PUT ERROR");
                        return;
                    }
                }

                app.ScreenUpdating = false;
                List<string> updatedSeries = new List<string>();

                // Open one connection per PUT batch
                using (SqlConnection sqlCon = new SqlConnection(_connectionString))
                {
                    sqlCon.Open();

                    foreach (Range headerCell in activeCells)
                    {
                    
                        //new part
                        string header = worksheet.Cells[1, headerCell.Column].Value2 as string;
                        if (string.IsNullOrWhiteSpace(header)) continue;

                        var parts = header.Split('|');

                        if (parts.Length < 2 ||
                            !parts[0].Equals("HARMONIZE", StringComparison.OrdinalIgnoreCase))
                            continue;

                        var seriesPart = parts[1].Trim();

                        var idx = seriesPart.IndexOf(':');
                        if (idx == -1) continue;

                        string loadset = seriesPart.Substring(0, idx).Trim();
                        string seriesName = seriesPart.Substring(idx + 1).Trim();



                        // calling delete proc
                        

                        using (SqlCommand updateCmd = new SqlCommand("dbo.UTILS_DeletefromXLS", sqlCon))

                        {
                            updateCmd.CommandType = CommandType.StoredProcedure;
                            //updateCmd.Parameters.Add("@seriesname", SqlDbType.NVarChar).Value = $"{loadset}:{seriesName}";

                            updateCmd.Parameters.Add("@loadsetname", SqlDbType.NVarChar).Value = $"{loadset}";
                            updateCmd.Parameters.Add("@seriesname", SqlDbType.NVarChar).Value = $"{seriesName}";




                            updateCmd.ExecuteNonQuery();
                        }

                        updatedSeries.Add(seriesName);
                    }
                }

                app.ScreenUpdating = true;

                if (updatedSeries.Count > 0)
                {
                   

                    ribbon_grp.Label = $"Deleted  ({updatedSeries.Count} series)";

                    // );
                }
                else
                {
                    //MessageBox.Show("No valid data found.", "PUT");
                    ribbon_grp.Label = "Warning: Delete - No valid series or parameters found in active header row.";
                    writeRange.Interior.Color = System.Drawing.Color.Red;

                }
            }
            catch (Exception ex)
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
                ribbon_grp.Label = "ERROR: " + ex.Message;
            }


        }//delete end









    }
}
