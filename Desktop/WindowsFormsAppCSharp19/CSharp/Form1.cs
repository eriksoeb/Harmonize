using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
// for testfor upgrade:

using System.Net.Http;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
//using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using static CSharp.Form1;
using static CSharp.Form1.Globals;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ExplorerBar;




namespace CSharp
{


    public partial class Form1 : Form
    {

        private System.Windows.Forms.ToolTip tip; //tooltip grid 1

        private ContextMenuStrip treeContextMenu;//ok bottom level
        private ContextMenuStrip treeContextMenu2; //upper levels for recalc refresh


        // ✅ Class-level colors list
        //private BindingList<ColorItem> colors;  // now accessible anywhere in Form1
        private BindingList<ColorItem> colors;  // class-level

        private DataTable Curve = new DataTable("Curve"); // ✅ global for this form


        //Nested class colors
        public class ColorItem
        {
            public string ColorName { get; set; }
            public string ColorCode { get; set; }
        }




        public class UpgradeResponse
        {
            public string product { get; set; }
            public string client { get; set; }
            public bool upgradeAvailable { get; set; }
            public string earliestUpdateDate { get; set; }
            public string latestVersion { get; set; }
            public string message { get; set; }
        }


        //for pxweb
        public class Root
        {
            public List<SeriesContainer> seriesData { get; set; }
        }

        public class SeriesContainer //px web common for all series
        {
            public string title { get; set; }
            public string subtitle { get; set; }
            public string freq { get; set; }
       
           public int deci { get; set; }

            public string executed_UTC { get; set; }
            public string mybaseperiod { get; set; }
            public List<SeriesItem> series { get; set; }
        }

        public class SeriesItem  //pxweb per series
        {
            public List<List<object>> data { get; set; }
            public string name { get; set; }
            public string unit { get; set; }
            public string desc { get; set; }

            public string source { get; set; }
            public string title { get; set; }




        }













        public static class Globals
        {
                       
            public static String UserDir = "C:/Harmonize_ie/My_Charts/";
            public static int LangColumn = 2;
            public static int MaxNum = 2000;
            public static String LangLineList = "Eng;No;osv;osv2";  //default in connection file


            //public static readonly String AppDir = "https://ssb-something.azurewebsites.net/";
            public static String AppDir = "auto-erik";


            public static readonly String DBStr = GetgonnectionString();
            //setter foldere og slikt
            //setter Languages



   

            public static readonly String TotalStr = "  User:" + Environment.UserName + " " + Globals.DBStr;
            //do not show passw..
            //public static readonly String TotalStr = "  User:" + Environment.UserName + " " + Globals.DBStr.Substring(0,Globals.DBStr.IndexOf("Pass"));



            public static String freq = "MONTHLY"; //only used for default px-web
            public static String deci = "1";
            public static String AppName = "Harmonize";
            //public static String TempDir = "C:\\osv"; //leses fra linje 4..? tbc userDir ?


            public static int BaseYear = 1; //arg sendes som int for baseyear 1 = None
            public static int BaseMnds = 0; //arg sendes som int med BaseYear

            public static String dateFormat = "'%Y %m %d %H:%M'";

            public static int OnPrem = 0; //kjorer appen mot on prembase asyn can soon go away in 1974
            public static String DbVer = "0.0.0.0"; //kjorer appen mot on prembase asyn

            public static int NumOfSearches = 0;

            public static String DragDropCurveId = "-1";
         
            public static String DragDropCurveDescr = "CurveDescr";


            // not needed public static String G_Interval = "G_Interval";


            public static String mylsname = "'" + "All" + "'"; //ny etter combp

            public static String Version = "1.9.0.0";  //skal ikke vere nødv her




            public static DataTable table = new DataTable();  //datatabel for argument til searcg procedyren


        }




        public Form1()
        {





        InitializeComponent();
        InitializeTreeContextMenu();  //bottom levels upload delete
        InitializeTreeContextMenu2(); // Top levels expand colapse
        toolStripStatusLabel1.ForeColor = Color.RoyalBlue;



            tip = new System.Windows.Forms.ToolTip();
            tip.InitialDelay = 100;
            tip.ReshowDelay = 50;
            tip.AutoPopDelay = 10000;
            tip.SetToolTip(dataGridView1, "Double click to select");





            colors = new BindingList<ColorItem>(); // fail assign to the field here 

            //new for makeing sure intervals and drop downs are read
            dataGridView2.CurrentCellDirtyStateChanged += DataGridView2_CurrentCellDirtyStateChanged;

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);

/*
            try
            {
                string test = Globals.AppName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
*/



            this.Text = Globals.AppName;  //setter navnet i appen windies


            //string mylsname = "'" + "All"+ "'"; //ny etter combp


            //MessageBox.Show(Application.StartupPath);



            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            Globals.Version = fvi.FileVersion;
            //MessageBox.Show(Globals.AppName + "debug \n" + Globals.Version, Globals.AppName);

            LogMessage();
            //Harmonize.HarmonizeInstaller.Version = Globals.Version; set manually



            //ingen database connection
            if (Globals.DBStr == "error")
            {
                MessageBox.Show(Globals.AppName + "Cannot find database connection str\n"+Globals.Version, Globals.AppName);
                Environment.Exit(0);

            }


    


            string[] mrow = Globals.LangLineList.Split(';');
            LangBox1.Text = mrow[Globals.LangColumn];

            //LangBox1.Items.Add(mrow[Globals.LangColumn]);
            //LangBox1.Items.Add(mrow[2]);

            foreach (var r in mrow)
            {
                LangBox1.Items.Add(r);
              //  MessageBox.Show("adding "+ r);
            }
          //  GetMenuesFromFile();  //oppfrisker knapper iht til default


       






            SqlConnection myconnbase = new SqlConnection();
            myconnbase.ConnectionString = Globals.DBStr;
            SqlCommand mycommando = new SqlCommand();
            mycommando.Connection = myconnbase;

            //MessageBox.Show(Globals.DBStr);

            try
            {

            mycommando.CommandText = "execute Client_Version2 1, '" + Globals.Version + "', '" + Environment.UserName+"'";
               // MessageBox.Show(mycommando.CommandText); Husk Initial catalog i connection

                myconnbase.Open();

                //DataTable mydtable = new DataTable(); not needed
                SqlDataReader reader = mycommando.ExecuteReader();
                int MyStatusFlag = 0;
                String mymessage = ""; //warnig from version
                String myenvcolor = "Control";
                Globals.OnPrem = 0; 
                //Globals.DbVer = "0.0.0.0";

                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        // MessageBox.Show(reader.GetString(reader.GetOrdinal("StatusMsg")) );
                       // already done Globals.AppName = reader.GetString(reader.GetOrdinal("DispName"));
                        //this.Text = Globals.AppName;
                        this.Text = reader.GetString(reader.GetOrdinal("DispName"));  //frabase overstyre
                        MyStatusFlag = reader.GetInt32(reader.GetOrdinal("StatusFlag"));
                       //Globals.OnPrem = reader.GetInt32(reader.GetOrdinal("OnPrem"));
                       Globals.DbVer = reader.GetString(reader.GetOrdinal("DbVer"));   // THE new

                        mymessage = reader.GetString(reader.GetOrdinal("StatusMsg"));
                        myenvcolor = reader.GetString(reader.GetOrdinal("EnvColor"));

                       // MessageBox.Show(myenvcolor); 

                    }


                }

                reader.Close();
                myconnbase.Close();


                var dbver = new Version(Globals.DbVer);
                var appver = new Version(Globals.Version);

                if (appver != dbver)
                {
                    MessageBox.Show( "OBS Need to upgrade to get versions in sync, AppVer: " + appver + " DbVer: " + dbver, "UPS");
                    //LogAndexit();
                }


                panel1.BackColor = Color.FromName(myenvcolor);
                //panel2.BackColor = Color.FromName(myenvcolor);
                //panel3.BackColor = Color.FromName(myenvcolor);
                toppanel2.BackColor = Color.FromName(myenvcolor);

                this.BackColor = Color.FromName(myenvcolor);
                statusStrip1.BackColor = Color.FromName(myenvcolor);
                menuStrip1.BackColor = Color.FromName(myenvcolor);



                //comboBasis Fylles basis box med noen år var None
                comboBasisBox.Items.Add("");
                comboBasisBox.Text = "";
                for (var i = 0; i < 20; i++)
                {
                    comboBasisBox.Items.Add(DateTime.Now.AddYears(i*-1).Year);
                }

               
            } //try


           
            catch
            {
                this.TopMost = true; // Here.
                MessageBox.Show("Can NOT connect to database. (windows-user:  "+Environment.UserName+ ")\n\n"+"Version: "+Globals.Version+" +  <-> DbVersion "+ Globals.DbVer +  "\nPlease see connection details in :\n" + Globals.AppDir + "\\connection.txt\n"+Globals.DBStr, Globals.AppName);

              

                Environment.Exit(0); //avslutter appen
             }
           
           



            Globals.table.Columns.Add("Item", typeof(string)); //tablene med argumenter
            Globals.table.Rows.Add("%");  //default
            Globals.table.Rows.Add("All");  //default
            //treeView1.ExpandAll(); her er ikke hele treet fylt opp




            //toolStripStatusLabel1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Series Selected" + Globals.TotalStr;

            ShowNormalStrip(dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Series Selected" + Globals.TotalStr);
            //for dbgrid2 r click
            this.contextMenuStrip1.Items.Add("Apply Axis to All");
            this.contextMenuStrip1.Items.Add("Apply Width to All");
            this.contextMenuStrip1.Items.Add("Apply Interval to All");
            this.contextMenuStrip1.Items.Add("Apply Function to All");
            this.contextMenuStrip1.Items.Add("Apply Aggregate to All");
            this.contextMenuStrip1.Items.Add("Apply Type to All");
            this.contextMenuStrip1.Items.Add("Apply Stacking to All");
            this.contextMenuStrip1.Items.Add("Apply Convert to All");




            //initierer grid1 med dummy colomn header for userfriendlyness

            //move this to be more current context
            //DataTable Curve = new DataTable("Curve");

           // DataColumn c0 = new DataColumn("CurveId");
            //Curve.Columns.Add(c0);
            //to set invisible...false
          
            //was1
            DataColumn c0 = new DataColumn("CurveName");
            Curve.Columns.Add(c0);


            DataColumn c1 = new DataColumn("Descr");
            Curve.Columns.Add(c1);
            // definere all tbc

          
            DataColumn c2 = new DataColumn("UnitId");
            Curve.Columns.Add(c2);
            //false


            DataColumn c3 = new DataColumn("Freq");
            Curve.Columns.Add(c3);

            DataColumn c4 = new DataColumn("Unit");
            Curve.Columns.Add(c4);

            DataColumn c5 = new DataColumn("Url");
            Curve.Columns.Add(c5);

            DataColumn c6 = new DataColumn("Source");
            Curve.Columns.Add(c6);

            DataColumn c7 = new DataColumn("Updated");
            Curve.Columns.Add(c7);

            DataColumn c8 = new DataColumn("UpdatedBy");
            Curve.Columns.Add(c8);

            DataColumn c9 = new DataColumn("NumObs");
            Curve.Columns.Add(c9);

            //12+13
            DataColumn c10 = new DataColumn("LastObs");
            Curve.Columns.Add(c10);


            DataColumn c11 = new DataColumn("LastDiff");
            Curve.Columns.Add(c11);
                        
            DataColumn c12 = new DataColumn("MinDate");
            Curve.Columns.Add(c12);

            DataColumn c13 = new DataColumn("MaxDate");
            Curve.Columns.Add(c13);

            DataColumn c14 = new DataColumn("Interval");
            Curve.Columns.Add(c14);

            DataColumn c15 = new DataColumn("Hit");
            Curve.Columns.Add(c15);

            DataColumn c16 = new DataColumn("LoadsetId");
            Curve.Columns.Add(c16);

            DataColumn c17 = new DataColumn("Title");
            Curve.Columns.Add(c17);
            //--EventWaitHandleSecurity 0
            DataColumn c18 = new DataColumn("CurveId");
            Curve.Columns.Add(c18);


            //ERIK
            dataGridView1.DataSource = Curve;
            //jun23


            //dataGridView1.Columns["CurveName"].Width = 220;
            //dataGridView1.Columns["Series"].Width = 190;
            //dataGridView1.Columns["Descr"].Width = 210;
            // definere default bredde
            //viser ikke :
           // dataGridView1.Columns["CurveId"].Visible = false;
            
            dataGridView1.Columns["Freq"].Visible = false;
            dataGridView1.Columns["UnitId"].Visible = false;
            dataGridView1.Columns["Title"].Visible = false;
            dataGridView1.Columns["CurveId"].Visible = false;

            //dataGridView1.Columns["LastValue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //ma evt definere flere. eller bruke samme kolonnenavn


            //dataGridView1.CellFormatting += new DataGridViewCellFormattingEventHandler(dataGridView1_CellFormatting);
            //this stkker for å formattere
            //dataGridView1.Columns[2].Width = 180; //descr widt
            //dataGridView1.Columns["LastValue"].DataPropertyName = "LastValue";
            // dataGridView1.Columns["LastObs"].DefaultCellStyle.Format = "0.00##";


            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            //dataGridView1.Columns["LastObs"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns["LastObs"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["LastObs"].DefaultCellStyle.Format = "0.0#"; // up to 2
            dataGridView1.Columns["LastObs"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dataGridView1.Columns["LastDiff"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns["LastDiff"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["LastDiff"].DefaultCellStyle.Format = "0.0#"; // up to 2
            dataGridView1.Columns["LastDiff"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //waste for non decimal
           // dataGridView1.Columns["NumObs"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
           // dataGridView1.Columns["NumObs"].DefaultCellStyle.Format = "0"; // want no decimals??
           // dataGridView1.Columns["NumObs"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.ShowCellToolTips = false;
   






            // GRID 2  2 2 2 2 2 2  ///////
            dataGridView2.Columns.Add("Id", "Id");
            dataGridView2.Columns["Id"].Width = 50; //curveid
            dataGridView2.Columns["Id"].Visible = false; 

            dataGridView2.Columns.Add("Name", "Name");
            dataGridView2.Columns["Id"].ReadOnly = true; //måtte flyye lenger ned...
            dataGridView2.Columns["Name"].ReadOnly = true; //måtte flyye lenger ned...
            dataGridView2.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dataGridView2.Columns.Add("Descr", "Descr");
            dataGridView2.Columns["Descr"].Width = 150; //curveid
            dataGridView2.Columns["Descr"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;




            //color inn here:


            DataGridViewComboBoxColumn cmbcolor = new DataGridViewComboBoxColumn();

            cmbcolor.HeaderText = "Color";
            cmbcolor.Name = "Color";
            cmbcolor.SortMode = DataGridViewColumnSortMode.Automatic;
          
            SqlConnection mycolorconn = new SqlConnection();
            mycolorconn.ConnectionString = Globals.DBStr;
            SqlCommand mycolorcommand = new SqlCommand();
            mycolorcommand.Connection = mycolorconn;

            mycolorcommand.CommandText = "exec Client_GetColor"; //stproc
            DataTable mycolordt = new DataTable();
            SqlDataAdapter mycoloradapter = new SqlDataAdapter(mycolorcommand);
            mycoloradapter.Fill(mycolordt);


            foreach (DataRow mydr in mycolordt.Rows)
            {

                colors.Add(new ColorItem
                {
                    ColorName = mydr["ColorName"].ToString(),
                    ColorCode = mydr["ColorCode"].ToString()
                });


            }

            mycolorconn.Close();
           
      
            


            var colorColumn = new DataGridViewComboBoxColumn
            {
                Name = "Color",
                HeaderText = "Color",
                Width = 75,
                DataSource = colors,
                DisplayMember = "ColorName",   // what user sees
                ValueMember = "ColorCode",     // what gets stored/used
                FlatStyle = FlatStyle.Flat
            };

            dataGridView2.Columns.Add(colorColumn);
          






            //width

            DataGridViewComboBoxColumn cmbw = new DataGridViewComboBoxColumn();
            cmbw.HeaderText = "Width";
            cmbw.Name = "Width";
            cmbw.MaxDropDownItems = 4;
            cmbw.Width = 50;
            cmbw.Items.Add("1");
            cmbw.Items.Add("2");
            cmbw.Items.Add("3");
            cmbw.Items.Add("4");
            cmbw.Items.Add("5");
            cmbw.Items.Add("6");
            cmbw.Items.Add("7");
            cmbw.Items.Add("8");
            dataGridView2.Columns.Add(cmbw);
            cmbw.SortMode = DataGridViewColumnSortMode.Automatic;


            //Dash Style

            DataGridViewComboBoxColumn cmbdash = new DataGridViewComboBoxColumn();
            cmbdash.HeaderText = "Dash";
            cmbdash.Name = "Dash";
            cmbdash.MaxDropDownItems = 3;
            cmbdash.Width = 60;
            cmbdash.Items.Add("solid");
            cmbdash.Items.Add("dot");
            cmbdash.Items.Add("dash");
            
            dataGridView2.Columns.Add(cmbdash);
            cmbdash.SortMode = DataGridViewColumnSortMode.Automatic;







            DataGridViewComboBoxColumn cmblr = new DataGridViewComboBoxColumn();
            cmblr.HeaderText = "Axis";
            cmblr.Name = "Axis";
            cmblr.MaxDropDownItems = 4;
            cmblr.Width = 75;
            cmblr.Items.Add("Left");
            cmblr.Items.Add("Right");
            cmblr.Items.Add("Left-0");
            cmblr.Items.Add("Right-0");
            

            //cmblr.SortMode = DataGridViewColumnSortMode.Automatic; maa vaere etter add
            dataGridView2.Columns.Add(cmblr);
            cmblr.SortMode = DataGridViewColumnSortMode.Automatic;
            //dataGridView2.Columns.Add("Column", "L/R");
            //cmblr.SortMode = sortable;




            //Function Drop Down kolonne 4
            DataGridViewComboBoxColumn combofunk = new DataGridViewComboBoxColumn();
            combofunk.DataPropertyName = "ID";
            combofunk.HeaderText = "Function";
            combofunk.Name = "Function";
            combofunk.Width = 80;
            combofunk.SortMode = DataGridViewColumnSortMode.Automatic;




            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Globals.DBStr;  // "Data Source=HD-SXD7E-018;Initial Catalog=SimstData;Integrated Security=True";
            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText = "exec Client_Functions";
            //command.CommandText = "select * from functions where Active = 1 order by 1 ";
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
     
            try
            {
                adapter.Fill(dt);
          
            }

            catch { 
            MessageBox.Show("Ups! Functions \n"+Globals.DBStr, Globals.AppName+" "+Globals.Version);
            Environment.Exit(0); //avslutter appen
                            }
            


            foreach (DataRow dr in dt.Rows)
            {
                combofunk.Items.Add(dr["Fname"].ToString());
            }

            conn.Close();
            combofunk.ValueMember = "ID";
            combofunk.DisplayMember = "Function";
            dataGridView2.Columns.Add(combofunk);
            combofunk.SortMode = DataGridViewColumnSortMode.Automatic;


            //slutt funksjon ///////////////



            //Lag Step
            DataGridViewComboBoxColumn cmbflag = new DataGridViewComboBoxColumn();
            cmbflag.HeaderText = "Lag";
            cmbflag.Name = "Lag";
            cmbflag.MaxDropDownItems = 4;
            cmbflag.Width = 45;
            SqlConnection myconnlag = new SqlConnection();
            myconnlag.ConnectionString = Globals.DBStr;
            SqlCommand mycommandlag = new SqlCommand();
            mycommandlag.Connection = myconnlag;

            mycommandlag.CommandText = "exec Client_FunctionLag"; //View
            DataTable mydtlag = new DataTable();
            SqlDataAdapter myadapterlag = new SqlDataAdapter(mycommandlag);
            myadapterlag.Fill(mydtlag);

            foreach (DataRow mydr in mydtlag.Rows)
            {
                cmbflag.Items.Add(mydr["FnLag"].ToString()); //adder radene i kolonne 

            }
            myconnlag.Close();
            dataGridView2.Columns.Add(cmbflag);
            cmbflag.SortMode = DataGridViewColumnSortMode.Automatic;
            // end function lag drop down
            //inn hit kommer convert:

            //freqtypes convert 
            DataGridViewComboBoxColumn convertcbc = new DataGridViewComboBoxColumn();
            convertcbc.DataPropertyName = "Id";
            convertcbc.HeaderText = "Convert";
            convertcbc.Name = "Convert";
            convertcbc.Width = 70;

            SqlConnection convertconn = new SqlConnection();
            convertconn.ConnectionString = Globals.DBStr;
            SqlCommand convertcommand = new SqlCommand();
            convertcommand.Connection = convertconn;

            convertcommand.CommandText = "exec Client_FreqType";
            DataTable convertdt = new DataTable();
            SqlDataAdapter convertapter = new SqlDataAdapter(convertcommand);
            convertapter.Fill(convertdt);
            foreach (DataRow dr in convertdt.Rows)
            {
                convertcbc.Items.Add(dr["Name"].ToString());
            }
            convertconn.Close();
            convertcbc.ValueMember = "Id";
            convertcbc.DisplayMember = "Convert";
            dataGridView2.Columns.Add(convertcbc);
            convertcbc.SortMode = DataGridViewColumnSortMode.Automatic;







            // interval drop down i Grid2 /////////////////// kolonne 5

            DataGridViewComboBoxColumn comboint = new DataGridViewComboBoxColumn();
            comboint.DataPropertyName = "ID";
            comboint.HeaderText = "Interval";
            comboint.Name= "Interval";
            //comboint.Width = 100;
            
            SqlConnection myconn = new SqlConnection();
            myconn.ConnectionString = Globals.DBStr;
            SqlCommand mycommand = new SqlCommand();
            mycommand.Connection = myconn;

            mycommand.CommandText = "exec Client_IntervalV"; //View
            DataTable mydt = new DataTable();
            SqlDataAdapter myadapter = new SqlDataAdapter(mycommand);
            myadapter.Fill(mydt);

            foreach (DataRow mydr in mydt.Rows)
            {
                comboint.Items.Add(mydr["iName"].ToString());
             

            }

            myconn.Close();
            comboint.ValueMember = "ID";
            comboint.DisplayMember = "Interval";
            //dataGridView2.Columns["Interval"].Width = 160; // tbe fixed
            dataGridView2.Columns.Add(comboint);
       
            comboint.SortMode = DataGridViewColumnSortMode.Automatic;
            //slutt INTERVAL


            //nye felter Aggregatetion
            
            DataGridViewComboBoxColumn fcastint = new DataGridViewComboBoxColumn();
            fcastint.DataPropertyName = "ID";
            fcastint.HeaderText = "Aggregate";
            fcastint.Name = "Aggregate";
            fcastint.Width = 100;

            SqlConnection fcastconn = new SqlConnection();
            fcastconn.ConnectionString = Globals.DBStr;
            SqlCommand fcastcommand = new SqlCommand();
            fcastcommand.Connection = fcastconn;
            
            fcastcommand.CommandText = "exec Client_Aggregate"; 
            DataTable fcastdt = new DataTable();
            SqlDataAdapter fcastadapter = new SqlDataAdapter(fcastcommand);
            fcastadapter.Fill(fcastdt);
            foreach (DataRow dr in fcastdt.Rows)
            {
                fcastint.Items.Add(dr["Name"].ToString());
            }
            fcastconn.Close();
            fcastint.ValueMember = "ID";
            fcastint.DisplayMember = "Aggregate";
            dataGridView2.Columns.Add(fcastint);
            fcastint.SortMode = DataGridViewColumnSortMode.Automatic;


            //slutt






            //type bar line etc
            DataGridViewComboBoxColumn cmblinetype = new DataGridViewComboBoxColumn();
            cmblinetype.HeaderText = "Type";
            cmblinetype.Name = "Type";
            cmblinetype.Width = 80;
            cmblinetype.MaxDropDownItems = 4;
            cmblinetype.Items.Add("spline");
            cmblinetype.Items.Add("line");
            cmblinetype.Items.Add("column");
            //cmblinetype.Items.Add("bar");
            cmblinetype.Items.Add("area");
            cmblinetype.Items.Add("areaspline");
            //cmblinetype.Items.Add("pie"); //kan vente no need
            dataGridView2.Columns.Add(cmblinetype);
            cmblinetype.SortMode = DataGridViewColumnSortMode.Automatic;


            //Stacking
            DataGridViewComboBoxColumn cmbstacking = new DataGridViewComboBoxColumn();
            cmbstacking.HeaderText = "Stacking";
            cmbstacking.Name = "Stacking";
            cmbstacking.Width = 70;
            cmbstacking.MaxDropDownItems = 4;
            cmbstacking.Items.Add("None");
            cmbstacking.Items.Add("normal");
            cmbstacking.Items.Add("percent");
            dataGridView2.Columns.Add(cmbstacking);
            cmbstacking.SortMode = DataGridViewColumnSortMode.Automatic;



           





            //Order
            DataGridViewComboBoxColumn cmborder = new DataGridViewComboBoxColumn();
            cmborder.HeaderText = "Order";
            cmborder.Name = "Order";
            cmborder.MaxDropDownItems = 4;
            cmborder.Width = 40;
            //readonly
            //cmborder.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
          
            cmborder.Items.Add("0");
            cmborder.Items.Add("1");
            cmborder.Items.Add("2");
            cmborder.Items.Add("3");
            cmborder.Items.Add("4");
            cmborder.Items.Add("5");
            cmborder.Items.Add("6");
            cmborder.Items.Add("7");
            cmborder.Items.Add("8");
            cmborder.Items.Add("9");


            dataGridView2.Columns.Add(cmborder);
            cmborder.SortMode = DataGridViewColumnSortMode.Automatic;

            // end order clm

            //
           




            //siste nye unit for file csv later update
            dataGridView2.Columns.Add("UnitId", "UnitId");
           
            dataGridView2.Columns["UnitId"].Visible = false;

            //adding for px-web make and do invisible..
            dataGridView2.Columns.Add("Unit", "Unit"); //name
            dataGridView2.Columns.Add("Source", "Source"); //name
            dataGridView2.Columns.Add("Title", "Title"); //name


            dataGridView2.Columns["Unit"].Visible = false;
            dataGridView2.Columns["Source"].Visible = false;
            dataGridView2.Columns["Title"].Visible = false;

            dataGridView2.EditMode = DataGridViewEditMode.EditOnEnter;



            GetMenuesFromFile();  //oppfrisker knapper iht til default language ved oppstart
            UpdateButtonStates(); //ghoster buttons
           // no success Search_btn.Focus();


        }




        private void Form1_Load(object sender, EventArgs e)
        {




            //erik dec not needed
            try
            {
                System.IO.Directory.CreateDirectory(Globals.UserDir);
                //  File.Copy("J:MyCharts/Data/eksport.html", "C:/Horizon/test.html", true);



            }
            catch //( IOException copyError)
            {
                //MessageBox.Show("OBS Init Copy problem: " + copyError.Message, Globals.AppName);
                MessageBox.Show("Problem creating " + Globals.UserDir, Globals.AppName);
            }

            BuildTree();
            //Search_btn.Focus();
            this.ActiveControl = Search_btn;
        }




        private void BuildTree()
        {
            treeView1.BeginUpdate();     // prevents flicker
            treeView1.Nodes.Clear();     // important when refreshing

            //string myPName = "dummy";
            //string myCName = "dummy";
            //string myLSName = "dummy";

            string myPNameprev = "prev";
            string myCNameprev = "prev";
            //string myLSNameprev = "prev";

            int i = 0;
            int j = -1;

            using (SqlConnection myconn = new SqlConnection(Globals.DBStr))
            using (SqlCommand mycommand = new SqlCommand("Client_GetSetTree2", myconn))
            {
                mycommand.CommandType = CommandType.StoredProcedure;

                myconn.Open();

                using (SqlDataReader reader = mycommand.ExecuteReader())
                {
                    // Top node
                    //was --treeView1.Nodes.Add("All");

                    TreeNode allNode = treeView1.Nodes.Add("All");
                    allNode.Tag = true;   // Hardcode full access



                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string myPName = reader["PName"].ToString();
                            string myCName = reader["CName"].ToString();
                            string myLSName = reader["LSName"].ToString();
                            bool hasWriteAccess = Convert.ToBoolean(reader["HasWriteAccess"]); //now coming from proc
                           

                            if (myPName != myPNameprev)
                            {
                                treeView1.Nodes.Add(myPName);
                                i++;
                                j = -1;
                            }

                            if (myCName != myCNameprev)
                            {
                                j++;
                                treeView1.Nodes[i].Nodes.Add(myCName);
                            }

                            // wastreeView1.Nodes[i].Nodes[j].Nodes.Add(myLSName);
                            TreeNode lsNode = treeView1.Nodes[i].Nodes[j].Nodes.Add(myLSName);
                            lsNode.Tag = hasWriteAccess;

                            if (hasWriteAccess)
                            {
                               // lsNode.ForeColor = Color.DarkGreen;
                                lsNode.ForeColor = Color.OrangeRed;
                                //lsNode.NodeFont = new Font(treeView1.Font, FontStyle.Bold);
                            }
                            else
                            {
                                lsNode.ForeColor = Color.RoyalBlue;
                            }


                            myPNameprev = myPName;
                            myCNameprev = myCName;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Missing loadset hierarchy", Globals.AppName);
                    }
                }
            }

            treeView1.ExpandAll();
            treeView1.EndUpdate();
        }






        private void OutsideBtn_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }




        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.F) || (e.Control && e.KeyCode == Keys.S))
            {
                Search_txt.Focus();
                //MessageBox.Show("s eller F og highlihter søket");
            }
        }







        //gml søk kan tas bort aug24
        private void connect_btn_Click(object sender, EventArgs e)  //searcher
        {
            toolStripProgressBar1.Value = 40;
           string mysearchstr = "'" + Search_txt.Text + "'";
            Globals.table.Rows[0].SetField("Item", Search_txt.Text);
           //erik ut dec25 string mywildcard = "'" + "%IDX%" + "'";
         
            toolStripProgressBar1.Value = 60;
            SqlConnection conn = new SqlConnection();
     
            conn.ConnectionString = Globals.DBStr;

            toolStripProgressBar1.Value = 80;

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            

            var pList = new SqlParameter("@list", SqlDbType.Structured);
            pList.TypeName = "dbo.StringList";
            pList.Value = Globals.table;

            toolStripProgressBar1.Value = 90;
            command.Parameters.Add(pList);

           
            command.CommandText = "EXECUTE Client_Search " + " @list" + "," + mysearchstr + "," + mysearchstr;

            toolStripProgressBar1.Value = 80;
            DataTable data = new DataTable();

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(data);
            toolStripProgressBar1.Value = 50;


            dataGridView1.DataSource = data;
            conn.Close();
            toolStripProgressBar1.Value = 30;

            //sizes here ?
            dataGridView1.Columns[0].Width = 60; //curveid
            dataGridView1.Columns[4].Width = 50; //freq

            //blir extra TBC
            //dataGridView1.Columns[0].Name = "CurveId";
            //dataGridView1.Columns[0].HeaderText = "CurveId";


            toolStripProgressBar1.Value = 30;
            toolStripStatusLabel1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Series Selected" + Globals.TotalStr;
            toolStripProgressBar1.Value = 0;

        }

        private void selectedcurves_btn_Click(object sender, EventArgs e)
        {
        //kan slette denne om en bare bruker doubleclick
            string MyCurveName = "";
            string MyCurveId = "";
            string MyCurveDescr = "";
            string MyInterval = "";

            foreach (DataGridViewRow row in dataGridView1.SelectedRows)


            {
                //currQty += row.Cells["qty"].Value;
                MyCurveId = row.Cells["CurveId"].Value.ToString();
                MyCurveName = row.Cells["CurveName"].Value.ToString();

                //MyCurveId = row.Cells["Id"].Value.ToString();
                //MyCurveName = row.Cells["Series"].Value.ToString();

                MyCurveDescr = row.Cells["Descr"].Value.ToString();
                MyInterval = row.Cells["Interval"].Value.ToString(); //settes riktig ? men vises ikke pga combo


                //MessageBox.Show(MyCurveId, "C#harts"); //vises ikke


                //kopierer over
                int n = dataGridView2.Rows.Add();
                dataGridView2.Rows[n].Cells[0].Value = MyCurveId;
                dataGridView2.Rows[n].Cells[1].Value = MyCurveName;
                dataGridView2.Rows[n].Cells[2].Value = MyCurveDescr;

                //dataGridView2.Rows[n].Cells[6].Value = MyInterval;
                dataGridView2.Rows[n].Cells["Interval"].Value = MyInterval;


               
                //toolStripStatusLabel1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Series Selected" + Globals.TotalStr;

                ShowNormalStrip (dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Series Selected" + Globals.TotalStr);


            }

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DateTime buildDate = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);
            DateTime buildDate = GetBuildDate();
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;  //show path      

            MessageBox.Show(
$@"{Globals.AppName}
App version : {Globals.Version}
Sql version : {Globals.DbVer}
User dir    : {Globals.UserDir}
Executable  : {Path.GetDirectoryName(strExeFilePath)}
Connection  : {Path.GetDirectoryName(strExeFilePath)}

© Erik.Soberg@ssb.no

Build date  : {buildDate:yyyy-MM-dd HH:mm}",
Globals.AppName
);

        }


        public static DateTime GetBuildDate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var filePath = assembly.Location;

            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;

            byte[] buffer = new byte[2048];
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                stream.Read(buffer, 0, buffer.Length);
            }

            int headerPos = BitConverter.ToInt32(buffer, peHeaderOffset);
            int secondsSince1970 = BitConverter.ToInt32(buffer, headerPos + linkerTimestampOffset);

            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(secondsSince1970).ToLocalTime();
        }








        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        private void LogAndexit()
        {
            MessageBox.Show("Exit. Welcome Back !", Globals.AppName);
           // Application.Exit();
            Environment.Exit(0);
        }




        private void Clear_Btn_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
            UpdateButtonStates();
            //label1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Curves Selected";
           // toolStripStatusLabel1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Series Selected" + Globals.TotalStr;
        ShowNormalStrip( dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Series Selected" + Globals.TotalStr);
        
        
        }



        private void UpdateColor(int rowIndex)
        {
            // Safety checks
            if (rowIndex < 0 || rowIndex >= dataGridView2.Rows.Count)
                return;

            if (dataGridView2.Rows[rowIndex].IsNewRow)
                return;

            var cell = dataGridView2.Rows[rowIndex].Cells["Color"];

            if (cell?.Value == null)
                return;

            var cellValue = cell.Value.ToString();

            if (string.IsNullOrWhiteSpace(cellValue))
                return;

            Color c;

            try
            {
                if (cellValue.StartsWith("#"))
                    c = ColorTranslator.FromHtml(cellValue);
                else
                    c = Color.FromName(cellValue);
            }
            catch
            {
                return; // Invalid color input
            }

            dataGridView2.Rows[rowIndex].Cells[3].Style.BackColor = c;
            dataGridView2.Rows[rowIndex].Cells[3].Style.ForeColor = Color.White;
            dataGridView2.Rows[rowIndex].Cells[0].Style.ForeColor = c;
        }





        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //Name: Color

            if (dataGridView2.Columns[e.ColumnIndex].Name == "Color") {
                UpdateColor(e.RowIndex);

            }
            /*

                var cellValue = dataGridView2.Rows[e.RowIndex]
                                 .Cells["Color"]
                                 .Value?.ToString();

                if (string.IsNullOrWhiteSpace(cellValue))
                    return;

                Color c;

                if (cellValue.StartsWith("#"))
                    c = ColorTranslator.FromHtml(cellValue);
                else
                    c = Color.FromName(cellValue);



                dataGridView2.Rows[e.RowIndex].Cells[3].Style.BackColor = c;
                dataGridView2.Rows[e.RowIndex].Cells[3].Style.ForeColor = Color.White;
                dataGridView2.Rows[e.RowIndex].Cells[0].Style.ForeColor = c;

              
            }

            */

            if (dataGridView2.Columns[e.ColumnIndex].Name == "Dash" && e.Value == null)
            {
                e.Value = "solid";

            }


            if (dataGridView2.Columns[e.ColumnIndex].Name == "Width" && e.Value == null)
            {
                e.Value = "5"; //default

            }




            if (dataGridView2.Columns[e.ColumnIndex].Name == "Axis" && e.Value == null) 
                {
                e.Value = "Left";

            }

         
                if (dataGridView2.Columns[e.ColumnIndex].Name == "Function" && e.Value == null)
                {
                e.Value = "None";

            }




            if (dataGridView2.Columns[e.ColumnIndex].Name == "Lag" && e.Value == null)
            {
                e.Value = "1";
            }



            if (e.ColumnIndex == 6  && e.Value == null)    // interval  //koper default value kommer ikke hit lenger
            {
               e.Value = "AllData";
            }


            if (dataGridView2.Columns[e.ColumnIndex].Name == "Aggregate" && e.Value == null)    // Aggregate
            {
                e.Value = "None"; //fefault None
   
            }

            if (dataGridView2.Columns[e.ColumnIndex].Name == "Type" && e.Value == null)
                //if (e.ColumnIndex == 8 && e.Value == null)    // charttype
            {
                e.Value = "spline";

            }
            if (dataGridView2.Columns[e.ColumnIndex].Name == "Stacking" && e.Value == null)    // stacking
            {
                e.Value = "None";

            }
            dataGridView2.ClearSelection();  //ingen valgt

        }

       


        private void ClearOne2_btn_Click(object sender, EventArgs e)
        {
            //comboBox1.Text = "%"; es feb
            int rowIndex = dataGridView2.CurrentCell.RowIndex;
            dataGridView2.Rows.RemoveAt(rowIndex);
            //label1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Curves Selected";
            ShowNormalStrip( dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Series Selected" + Globals.TotalStr);
            UpdateButtonStates();
        }

       

       
            private void UpdateButtonStates()
        {
            // Exclude the 'new row' if present
            int rowCount = dataGridView2.AllowUserToAddRows
                ? dataGridView2.Rows.Count - 1
                : dataGridView2.Rows.Count;

            bool hasRows = rowCount > 0;

            // Enable/disable buttons based on whether any rows exist
            NyChart_btn.Enabled = hasRows;
            NyMulti_btn.Enabled = hasRows;
            YearChartRepo_btn.Enabled = hasRows;
            reportToolStripMenuItem1.Enabled = hasRows;
            saveAsToolStripMenuItem.Enabled = hasRows;

            chartToolStripMenuItem.Enabled = hasRows;
            multiChartToolStripMenuItem.Enabled = hasRows;
            yearChartToolStripMenuItem.Enabled = hasRows;

            Clear2_Btn.Enabled = hasRows;
            ClearOne2_btn.Enabled = hasRows;

            // Other buttons can be added here similarly
        }














        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {



            //selectedcurves_btn_Click();
            //MessageBox.Show("Doubleclicked !" , "C#harts");
            // DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex]; //Get selected Row


          //  string MyCurveName = "";
          //  string MyCurveId = "";
         //   string MyCurveDescr = "";
            //string MyInterval = "";


            // foreach (DataGridViewRow row in dataGridView1.Rows)

            dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
            dataGridView2.EndEdit();

            foreach (DataGridViewRow row in dataGridView1.SelectedRows)


            {
                          




                //kopierer over
                int n = dataGridView2.Rows.Add();

                dataGridView2.Rows[n].Cells["Id"].Value = row.Cells["CurveId"].Value.ToString();
               // dataGridView2.Columns["Id"].Visible = false; 

                dataGridView2.Rows[n].Cells["Name"].Value = row.Cells["CurveName"].Value.ToString();
                dataGridView2.Rows[n].Cells["Descr"].Value = row.Cells["Descr"].Value.ToString();

                dataGridView2.Rows[n].Cells["UnitId"].Value = row.Cells["UnitId"].Value.ToString();

                dataGridView2.Rows[n].Cells["Unit"].Value = row.Cells["Unit"].Value.ToString();
                dataGridView2.Rows[n].Cells["Source"].Value = row.Cells["Source"].Value.ToString();
                dataGridView2.Rows[n].Cells["Title"].Value = row.Cells["Title"].Value.ToString();


                /* not neeted celle formatting
                 dataGridView2.Rows[n].Cells["Axis"].Value = "Left";
                 dataGridView2.Rows[n].Cells["Function"].Value = "None";
                 dataGridView2.Rows[n].Cells["Type"].Value = "spline";
                 dataGridView2.Rows[n].Cells["Stacking"].Value = "normal";
                 dataGridView2.Rows[n].Cells["Aggregate"].Value = "None";
                 dataGridView2.Rows[n].Cells["Lag"].Value = "1";
                */


                //old dataGridView2.Rows[n].Cells[6].Value = MyInterval;
                // dataGridView2.Rows[n].Cells["Interval"].Value =   row.Cells["Interval"].Value;
                dataGridView2.Rows[n].Cells["Interval"].Value =   row.Cells["Interval"].Value ?? "AllData";
                dataGridView2.Rows[n].Cells["Convert"].Value = "OFF";



                int rowCount =    dataGridView2.Rows.Count -    (dataGridView2.AllowUserToAddRows ? 1 : 0);
              // dataGridView2.Rows[n].Cells[11].Value =    Math.Min(rowCount, 9).ToString();
                dataGridView2.Rows[n].Cells["Order"].Value = Math.Min(rowCount, 9).ToString();


                int colorIndex = (rowCount-1) % 10;
               
               var colorColumn =     (DataGridViewComboBoxColumn)dataGridView2.Columns["Color"];

                //  moving to top change of scopr
                //var colors =  (List<ColorItem>)colorColumn.DataSource;

                dataGridView2.Rows[n].Cells["Color"].Value = colors[colorIndex].ColorCode;
                //dataGridView2.Rows[n].Cells["Color"].Value = colors[colorIndex].ColorName;


                //coloring the text: do this in cellformmatting
                /*
                string colorValue = colors[colorIndex].ColorCode;

                // Convert to Color
                Color c;
                if (colorValue.StartsWith("#"))
                  c = ColorTranslator.FromHtml(colorValue);
                else
                  c = Color.FromName(colorValue);

              dataGridView2.Rows[n].Cells["Color"].Style.BackColor = c;  //10
                */

             
                ShowNormalStrip (dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Series Selected" + Globals.TotalStr);
                
            }



            UpdateButtonStates();
            dataGridView2.ClearSelection();
            
            //slutt copy


        }
























        private void dataGridView2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
                //viser menyen apply all
                //MessageBox.Show("Klikka", "C#harts");
            }

        }

      
             




        //hoyreklikk menyen i grid2
        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            string MyFunction = "None";
            string MyInterval = "init";
            string MyAggregate = "None";
            string MyConvert = "OFF";
            string MyType = "spline";
            string MyAxis = "Left";
            string MyWidth = "5";
            string MyStacking = "None";

            int rowIndex = dataGridView2.CurrentCell.RowIndex;

            //intervallet
           // if (dataGridView2.Rows[rowIndex].Cells[6].Value == null)    // interval blitt ny nr 6 feb 10
                if (dataGridView2.Rows[rowIndex].Cells["Interval"].Value == null)    // interval blitt ny nr 6 feb 10
                {
                MyInterval = "AllData";
            }
            else
            {
                //MyInterval = dataGridView2.Rows[rowIndex].Cells[6].Value.ToString();
                MyInterval = dataGridView2.Rows[rowIndex].Cells["Interval"].Value.ToString();
            }

            if (e.ClickedItem.Text == "Apply Interval to All")

            {
                //MessageBox.Show(MyInterval + " :loop " + e.ClickedItem.Text);


                for (int i = 0; i < dataGridView2.Rows.Count; i += 1)

                {
                    //setter over
                    //dataGridView2.Rows[i].Cells[6].Value = MyInterval;
                    dataGridView2.Rows[i].Cells["Interval"].Value = MyInterval;

                }
            } 





            //funksjon
            if (dataGridView2.Rows[rowIndex].Cells["Function"].Value == null)    // funk fremdeles 4
            {
                MyFunction = "None";
            }
            else
            {
               // MyFunction = dataGridView2.Rows[rowIndex].Cells[4].Value.ToString();
                MyFunction = dataGridView2.Rows[rowIndex].Cells["Function"].Value.ToString();
            }

            if (e.ClickedItem.Text == "Apply Function to All")
            {
                //MessageBox.Show(MyFunction + " :loope" + e.ClickedItem.Text);


                for (int i = 0; i < dataGridView2.Rows.Count; i += 1)

                {
                    //setter over
                    //dataGridView2.Rows[i].Cells[4].Value = MyFunction;
                    dataGridView2.Rows[i].Cells["Function"].Value = MyFunction;

                }



            }  //slutt if FUNCTION



            //stack start
            //stack
            else if (dataGridView2.Rows[rowIndex].Cells["Stacking"].Value == null)    // stack nr 9
            {
                MyStacking = "None";
            }
            else
            {
                MyStacking = dataGridView2.Rows[rowIndex].Cells["Stacking"].Value.ToString();
            }
            if (e.ClickedItem.Text == "Apply Stacking to All")
            {
                for (int i = 0; i < dataGridView2.Rows.Count; i += 1)
                {
                    //setter over
                    dataGridView2.Rows[i].Cells["Stacking"].Value = MyStacking;
                }
            }  //slutt if stack



            //type start
           
            if (dataGridView2.Rows[rowIndex].Cells["Type"].Value == null)    // type nr 8
            {
                MyType = "spline";
            }
            else
            {
                MyType = dataGridView2.Rows[rowIndex].Cells["Type"].Value.ToString();
            }
            if (e.ClickedItem.Text == "Apply Type to All")
            {
                for (int i = 0; i < dataGridView2.Rows.Count; i += 1)
                {
                    dataGridView2.Rows[i].Cells["Type"].Value = MyType;
                }
            }  //slut





            //start //axis new

            if (dataGridView2.Rows[rowIndex].Cells["Axis"].Value == null)    
            {
                MyAxis = "Left";
            }
            else
            {
                MyAxis = dataGridView2.Rows[rowIndex].Cells["Axis"].Value.ToString();
            }
            if (e.ClickedItem.Text == "Apply Axis to All")
            {
                for (int i = 0; i < dataGridView2.Rows.Count; i += 1)
                {
                    dataGridView2.Rows[i].Cells["Axis"].Value = MyAxis;
                }
            }  //slut




            //width

            if (dataGridView2.Rows[rowIndex].Cells["Width"].Value == null)
            {
                MyWidth = "5";
            }
            else
            {
                MyWidth = dataGridView2.Rows[rowIndex].Cells["Width"].Value.ToString();
            }
            if (e.ClickedItem.Text == "Apply Width to All")
            {
                for (int i = 0; i < dataGridView2.Rows.Count; i += 1)
                {
                    dataGridView2.Rows[i].Cells["Width"].Value = MyWidth;
                }
            }  //slutt Width



            if (dataGridView2.Rows[rowIndex].Cells["Aggregate"].Value == null)
            {
                MyAggregate = "None";
            }
            else
            {
                MyAggregate = dataGridView2.Rows[rowIndex].Cells["Aggregate"].Value.ToString();
            }
            if (e.ClickedItem.Text == "Apply Aggregate to All")
            {
                for (int i = 0; i < dataGridView2.Rows.Count; i += 1)
                {
                    dataGridView2.Rows[i].Cells["Aggregate"].Value = MyAggregate;
                }
            }



            //Convert force frequency 2026
            if (dataGridView2.Rows[rowIndex].Cells["Convert"].Value == null)
            {
                MyConvert = "OFF";
            }
            else
            {
                MyConvert = dataGridView2.Rows[rowIndex].Cells["Convert"].Value.ToString();
            }
            if (e.ClickedItem.Text == "Apply Convert to All")
            {
                for (int i = 0; i < dataGridView2.Rows.Count; i += 1)
                {
                    dataGridView2.Rows[i].Cells["Convert"].Value = MyConvert;
                }
            }












        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void monthlyReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //starter  System.Diagnostics;
            //MessageBox.Show("Wildcarding");
            //Process.Start("J:\\GISK\\Kart\\Data\\MndRappCforms.exetbc");

        }

        private void wildcardingToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //MessageBox.Show("Wildcarding");


            MessageBox.Show
           ("\r\n"
           + "%   zero or more characters bl % finds bl, black, blue\r\n"
           + "_   one single char h_t  finds hot, hat, and hit\r\n"
           + "[ ] any single char within brackets h[oa]t finds hot and hat, not hit\r\n"
           + "^   any char NOT in brackets h[^ oa]t finds hit, not hot and hat\r\n"
           + "-   range of characters c[a-b]t finds cat and cbt\r\n"
           , "SQL Wildcarding");






        }

        // private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        // {
        //     MessageBox.Show("searchon");
        // }

        private void dataGridView2_ColumnSortModeChanged(object sender, DataGridViewColumnEventArgs e)
        {
            MessageBox.Show("Ups sorrry", Globals.AppName);
        }

        private void toppanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        //private void button1_Click_2(object sender, EventArgs e)
        //{

        //trenode dummy button
        //treeView1.SelectedNode = treeNode;
        //MessageBox.Show(treeView1.SelectedNode.ToString());
        //  MessageBox.Show(((TreeView)sender).SelectedNode.Text)



        //}



        public void ListChildren(List<TreeNode> Nodes, TreeNode Node)
        {
            foreach (TreeNode thisNode in Node.Nodes)
            {
                //Nodes.Add(thisNode);
                //AddChildren(Nodes, thisNode);
                //MessageBox.Show(thisNode.Text);
            }
        }



        void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 16)
            {
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
                // 2 desimaler tusen formattering på colonne 14 som er sortering må lages generic 16 etter log+latt
                // e.Value = string.Format("{0:# ##0.##}", e.Value);
                //CustomTypeDescriptor order søket formatering
            }
        }





        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //  string selectedNodeText = e.Node.Text;
            // MessageBox.Show(e.Node.Text);
            //MessageBox.Show(treeView1.SelectedNode.LastNode.Text); siste men feiler i lvel3
            //List<TreeNode> Nodes = new List<TreeNode>();
            //ListChildren(Nodes, e.Node);  //liser subnoden men ikke laveste
            // ListChildren(Nodes, e.Node.LastNode);

            // string str = "";
            //foreach (TreeViewNode tnode in e.Node.Items)
            //   str += tnode.Text + ",";
            //MessageBox.Show(treeView1.SelectedNode.Level + " - " + treeView1.SelectedNode.Text + ", enode: "+ e.Node);
            //MessageBox.Show(GetDeepestChildren(e.Node));

            //TreeNode oMainNode = treeView1.Nodes[0];


            // Globals.mylsname = "";
            //  Globals.table.Rows.Add("%");
            TreeNode oMainNode = e.Node;


            Globals.table.Rows.Clear(); //sletter forrige valg
                                        // comboBox1.Text = "";
            //label1.Text = "";

            //dt.Dispose();
            //Globals.table.Rows.Add("%"); //search string foreste recorden
            Globals.table.Rows.Add(Search_txt.Text);

            PrintNodesRecursive(oMainNode);
            //connect_btn_Click(sender, e); //utfører søket. søk10 juli23
            Search_btn_Click(sender, e);
            //MessageBox.Show(Globals.mylsname);
            //comboBox1.Text = "rer, rere,3333";

        }



        public void PrintNodesRecursive(TreeNode oParentNode)
        {



            if (oParentNode.Level == 2 || oParentNode.Text == "All")
            {

                Globals.table.Rows.Add(oParentNode.Text);
                //comboBox1.Text = comboBox1.Text +" "+ oParentNode.Text;
                //label1.Text = label1.Text + "" + oParentNode.Text + ", ";




            }
            else //mellom nivå
            {
                // Start recursion on all subnodes.
                foreach (TreeNode oSubNode in oParentNode.Nodes)
                {
                    PrintNodesRecursive(oSubNode);
                }
            } //else
        }

        // private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        //{
        //naar søkeproc bytter søkes søket
        //connect_btn_Click(sender, e); //utfører søket.
        //}



       // private void comboBox1_TextChanged(object sender, EventArgs e)
        //{
            //MessageBox.Show(comboBox1.Text);
            //søker ikke naar appl starter men alltid ellers
          //  if (Globals.NumOfSearches >= 1)
           // {
             //   connect_btn_Click(sender, e); //utfører søket.
           // }
            //Globals.NumOfSearches = Globals.NumOfSearches + 1;
        //}





        private static string GetgonnectionString()  //was provate static
        {
            string basepath = Application.StartupPath;
            string txtpath = basepath + @"/connection.txt";  //filename
            string connectionstring = ""; // "Data Source=dbserver;Initial Catalog=dw;Integrated Security=True";
            
            if (File.Exists(txtpath))
            {
                using (StreamReader sr = new StreamReader(txtpath))
                {
               
                    //leser foreste linje fra der exe fila ligger
                    string ss = sr.ReadLine();
                    //MessageBox.Show(ss.ToString(), "getconn");
                    connectionstring = (ss.ToString());

                    //Globals.DataApi = sr.ReadLine();  //line2
                    //Globals.AppDir = sr.ReadLine();  //line3 home taken out
                    Globals.AppDir = Application.StartupPath;

                    Globals.UserDir = sr.ReadLine();  //line2 place to temp store

                    Globals.LangLineList = sr.ReadLine();  //line3 languages
                    Globals.LangColumn = Int32.Parse(sr.ReadLine()); //line4 default lang position
                    Globals.MaxNum = Int32.Parse(sr.ReadLine()); //line5 max rows to get




                    //bygges opp først
                    //LangBox1.Items.Add(words[Globals.LangColumn]);
                    //LangBox1.Items.Add("erik");

                    //MessageBox.Show(ss.ToString(), "getapi");
                    //MessageBox.Show(Globals.UserDir.ToString());

                    //Globals.UserDir = sr.ReadLine();  //line3

                }


                //MessageBox.Show(ss.ToString());
                return connectionstring;
                

            }
            else
            { 
            MessageBox.Show("connection.txt file missing in: " + basepath, Globals.AppName);
            return("error");
            }          
            }

    

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }









        private void tableMgrToolStripMenuItem_Click(object sender, EventArgs e)
        {


            string basepath = Application.StartupPath;
            string txtpath = basepath + @"/LoadsetMgr.exe";  //filename to start



            try
            {
                System.Diagnostics.Process.Start(txtpath);


            }
            catch (Win32Exception)
            {
                MessageBox.Show("Could not find application: "+ txtpath, Globals.AppName);
            }



            //MessageBox.Show("caller loadsetter updater ", Globals.AppName);



        }







        internal static bool ContainsArabicLetters(string text)
        {
            foreach (char character in text.ToCharArray())
            {
                if (character >= 0x600 && character <= 0x6ff)
                    return true;
                if (character >= 0x750 && character <= 0x77f)
                    return true;
                if (character >= 0xfb50 && character <= 0xfc3f)
                    return true;
                if (character >= 0xfe70 && character <= 0xfefc)
                    return true;
            }
            return false;
        }








        //the new thing
        private void NyChart_btn_Click(object sender, EventArgs e)
        {

            toolStripStatusLabel1.ForeColor = Color.RoyalBlue;
            //making sure no more edit in the grid
            dataGridView2.EndEdit();

            //  MessageBox.Show("Installer nyknapp", Globals.AppName);


            //her eller overst
            string Fname = "";
            string Agg = "None";
            string AggDesc = "";
            string Convert = "OFF";
            string UnitId = "";
            string Unit = "";
            string Title = "";
            string Source = "";



            string DashValue = "";
            string WidthValue = "5";

            string SerieName = "";  //orig used for decriptions
            string CurveName = "";
            string FnameReplaced = "";
            string CurveId = "";
            string CurveDescr = "";
            string Iname = "";
            string LRName = "";
            //string Forecast = "";
            string FnLag = "";
            //string DynDateDesc = "";

            string myline = "";  //line for txt curves
            int cnt = 0;
            int myzindex = 0;

            string stacking = "";
            string myorder = "0";
            //string mycolorname = "";
            string mycolor = "";
            string linetype = "";
            int NumOfCurves = dataGridView2.Rows.Count;
            int NumOfCurvesPct = 100 / NumOfCurves;
            //int NumOfCurvesCur = 0;
            //int InnerSizePie = 10; // for pie starter på 10 og adder 10 ..max 10 stk tbc
            int numseries = 0;

            // Declare the variable outside so it's accessible
            string htmlfileName = "";

            // Pattern matching with a fresh variable name
            if (sender is System.Windows.Forms.Button button)
            {
                htmlfileName = button.Tag?.ToString();
            }
            else if (sender is System.Windows.Forms.ToolStripMenuItem menuItem)
            {
                htmlfileName = menuItem.Tag?.ToString();
            }

            // Optional: check null/empty
            if (string.IsNullOrEmpty(htmlfileName))
            {
                MessageBox.Show("No html defined for this control...");
                return;
            }




            var mfile = Globals.UserDir + "Mydata.json";
            string isoUtcDateTime = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss");


            //var mfile = Globals.UserDir + (string)NyChart_btn.Tag;


            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(mfile, false))
            {

               // file.WriteLine("seriesData = ["); js til json
                file.WriteLine("[  {");

                file.WriteLine("\"title\":\"Harmonize\",");
                file.WriteLine("\"subtitle\":\"Harmonize\",");


                // file.WriteLine("freq : '" + Globals.freq + "',");
                //  file.WriteLine("decimal : " + Globals.deci.ToString() + ",");
                // file.WriteLine("executed_UTC : '" + isoUtcDateTime + "',");


                file.WriteLine($"\"freq\":\"{Globals.freq}\",");
                file.WriteLine($"\"deci\":{Globals.deci},");
                file.WriteLine($"\"executed_UTC\":\"{isoUtcDateTime}\",");







                //file.WriteLine("mytimezone : ''  ,");               

                if (Globals.BaseMnds >= 1900)
                {
                    //file.WriteLine("mybaseperiod : " + Globals.BaseMnds.ToString() +
                    //
                    file.WriteLine("\"mybaseperiod\":\"" + Globals.BaseMnds.ToString() + "\", ");

                }
                else
                {
                   // file.WriteLine("mybaseperiod : '',");
                    file.WriteLine("\"mybaseperiod\":\"\",");

                }




                file.WriteLine("\"series\": [");

                mycolor = "";
                UnitId = "";

                Unit = "";
                Title = "";
                Source = "";

                WidthValue = "5";
                DashValue = "solid";
                
                CurveId = "";
                CurveDescr = "";
                Fname = "";
                Agg = "None";
                Convert = "OFF";
                AggDesc = "";
                FnameReplaced = "";
                Iname = "";
                LRName = "0";
                SerieName = "";
                CurveName = "";
                //Forecast = "0";
                FnLag = "1";
                
                stacking = "";
                myorder = "0";
                myzindex = 9;
                linetype = "";
                cnt = 0;
                myline = "";
                numseries = 0;


                //MessageBox.Show(dataGridView2.Rows.Count.ToString());  //TBC remove

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    toolStripProgressBar1.Value = 20;
                    numseries = numseries + 1;

                    CurveId = (string)row.Cells[0].Value.ToString();
                    CurveDescr = (string)row.Cells[2].Value.ToString();


                    CurveName = (string)row.Cells[1].Value.ToString();  //arab ikke quote ..arab



                    //inn med   Aggregate first as it is used in func as well
                    if ((row.Cells["Aggregate"].Value == null) || (row.Cells["Aggregate"].Value.ToString() == ""))
                    {
                        //Forecast = "None";   // default Last Forecast
                        Agg = "None";   // d5efault Last Forecast
                        AggDesc = "";
                    }
                    else
                    {

                        Agg = (string)row.Cells["Aggregate"].Value.ToString();
                        AggDesc = Agg;
                        //CurveDescr = CurveDescr + " forcast as of ";  // kanvære fcast uten funk

                    }

                    //OFF ANN MON
                    Convert = (string)row.Cells["Convert"].Value.ToString();



                    //newstandard lag step 
                    var cellValue = row.Cells["Lag"].Value?.ToString();

                    FnLag = string.IsNullOrEmpty(cellValue) || cellValue == "1"
                        ? "1"   // default
                        : cellValue;




                    // funksjon new
                    var functionCellValue = row.Cells["Function"].Value?.ToString();

                    if (string.IsNullOrEmpty(functionCellValue) || functionCellValue == "None")
                    {
                        // ingen valgt FUNCTION
                        Fname = "None";          // bruker get series
                        SerieName = CurveDescr;
                        FnameReplaced = "";

                        // DEFAULT: do nothing, no function
                    }
                    else
                    {
                        Fname = functionCellValue;

                        // pynter på navnet til funk med lag 123
                        FnameReplaced = Fname.Replace("n)", FnLag + ")");
                        FnameReplaced = Regex.Replace(FnameReplaced, @"[\[\]\(\)]", "");

                        SerieName = CurveDescr;

                        // Alternative naming (kept commented as before)
                        // SerieName = FnameReplaced + AggDesc + '(' + CurveDescr + ')';
                    }





                    // Name of column: Axis
                    var axisValue = row.Cells["Axis"].Value?.ToString();

                    if (string.IsNullOrEmpty(axisValue) || axisValue == "Left")
                    {
                        LRName = "0";   // default left
                    }
                    else if (axisValue == "Left-0")
                    {
                        LRName = "2";
                    }
                    else if (axisValue == "Right")
                    {
                        LRName = "1";
                    }
                    else
                    {
                        LRName = "3";   // all right axes start at 0
                    }



                    // Name: Widthcmb -> Width
                    var WidthTempValue = row.Cells["Width"].Value?.ToString();

                    WidthValue = string.IsNullOrEmpty(WidthTempValue) || WidthTempValue == "5"
                        ? "5"
                        : WidthTempValue;



                    // Name: Dashcmb ->dash
                    var DashTempValue = row.Cells["Dash"].Value?.ToString();

                    DashValue = string.IsNullOrEmpty(DashTempValue) //|| DashTempValue == "solid"
                        ? "solid"
                        : $"{DashTempValue}";




                    // Name: Interval
                    var intervalValue = row.Cells["Interval"].Value?.ToString();

                    Iname = string.IsNullOrEmpty(intervalValue) || intervalValue == "AllData"
                        ? "AllData"
                        : intervalValue;


                    // Name: LineType
                    var lineTypeValue = row.Cells["Type"].Value?.ToString();

                   // linetype = string.IsNullOrEmpty(lineTypeValue) || lineTypeValue == "spline"
                    //    ? "'spline'"
                    //    : $"'{lineTypeValue}'";   // 



                    linetype = string.IsNullOrEmpty(lineTypeValue) || lineTypeValue == "spline"
                     ? "spline"
                    : $"{lineTypeValue}";   // no



                    // Name: Stacking
                    var stackingValue = row.Cells["Stacking"].Value?.ToString();

                    stacking = string.IsNullOrEmpty(stackingValue) || stackingValue == "None"
                        ? null                  // Highcharts null
                        : $"{stackingValue}";   // no 






                    // Name: Orderclm _-order
                    var orderValue = row.Cells["Order"].Value?.ToString();

                    if (string.IsNullOrEmpty(orderValue) || orderValue == "None")
                    {
                        myorder = "9";
                        myzindex = 9;
                    }
                    else
                    {
                        myorder = orderValue;

                        int rowIndex = row.Index;                 // 0-based
                        int maxIndex = dataGridView2.Rows.Count - 1;

                        myzindex = maxIndex - rowIndex;           // reversed z-index
                    }



                    UnitId = row.Cells["UnitId"].Value.ToString(); //unit for file
                    mycolor = row.Cells["Color"].Value.ToString(); //= colors[colorIndex].ColorCode;

                    Unit = row.Cells["Unit"].Value.ToString(); //unit for file
                    Source = row.Cells["Source"].Value.ToString(); //unit for px web
                    Title = row.Cells["Title"].Value.ToString(); //unit for px


                    //ny felle serie 
                    file.WriteLine("{");

                    toolStripProgressBar1.Value = 40;



                    SqlConnection sqlCon = null;
                    using (sqlCon = new SqlConnection(Globals.DBStr))
                    {
                        sqlCon.Open();
                        SqlCommand sql_cmnd = new SqlCommand("StpGetSeries", sqlCon);  // was Fname
                        sql_cmnd.CommandType = CommandType.StoredProcedure;
                        sql_cmnd.Parameters.AddWithValue("@CurveName", SqlDbType.NVarChar).Value = CurveName;
                        //sql_cmnd.Parameters.AddWithValue("@fdate", SqlDbType.DateTime).Value = DBNull.Value;  // kan uttga
                        sql_cmnd.Parameters.AddWithValue("@basis", SqlDbType.Int).Value = Globals.BaseMnds;  // basiss år+mnd for espn deler paa 1
                        sql_cmnd.Parameters.AddWithValue("@myinterval", SqlDbType.NVarChar).Value = Iname;

                        sql_cmnd.Parameters.AddWithValue("@myfunc", SqlDbType.NVarChar).Value = Fname;  //navn paa evt funksjon


                        sql_cmnd.Parameters.AddWithValue("@myfnlagint", SqlDbType.Int).Value = FnLag;

                        sql_cmnd.Parameters.AddWithValue("@format", SqlDbType.NVarChar).Value = "client";  //kan utga
                        sql_cmnd.Parameters.AddWithValue("@agg", SqlDbType.NVarChar).Value = Agg;
                   
                        sql_cmnd.Parameters.AddWithValue("@convert2freq", SqlDbType.NVarChar).Value = Convert;
                        sql_cmnd.Parameters.AddWithValue("@sort", SqlDbType.NVarChar).Value = "asc";
                        sql_cmnd.Parameters.AddWithValue("@top", SqlDbType.Int).Value = Globals.MaxNum;  //from connection file
                        sql_cmnd.Parameters.AddWithValue("@json", SqlDbType.NVarChar).Value = DBNull.Value;



//                      toolStripStatusLabel1.Text = "StpGetSeries " + CurveName + "," + Globals.BaseMnds.ToString() + ", " + Iname + ", " + Fname + ", " + FnLag + ", " + Agg + ", " + Globals.MaxNum + ", " + "asc"; //det over i totalstr og vise til slutt
                        toolStripStatusLabel1.Text = "StpGetSeries " + CurveName + "," + Globals.BaseMnds.ToString() + ", " + Iname + ", " + Fname + ", " + FnLag + ", " + Agg + ", " + Convert+ ", asc, "+ Globals.MaxNum; //det over i totalstr og vise til slutt


                        //toolStripStatusLabel1.Text = mycommand.CommandText; //trenger bare å vise siste evt hente det over i totalstr og vise til slutt


                        //MessageBox.Show("" + Globals.BaseMnds);

                        DataTable mydt = new DataTable();
                        try
                        {
                            SqlDataReader reader = sql_cmnd.ExecuteReader();  //evt try befor this for access

                            cnt = 0;
                            if (reader.HasRows)
                            {

                                while (reader.Read())
                                {
                                    cnt = cnt + 1;
                                    if (cnt == 1)

                                        
                                    {

                                        //myline = (reader.GetValue(reader.GetOrdinal("Epodateval")).ToString()); //ok foer arabisk 
                                        //ok2myline = (reader.GetString(reader.GetOrdinal("Epodateval")).ToString()); //prover m string

                                        try
                                        {
                                            // Attempt to read the column value
                                            myline = reader.GetValue(reader.GetOrdinal("Epodateval")).ToString();
                                        }
                                        catch (IndexOutOfRangeException ex)
                                        {
                                            // Column "Epodateval" does not exist
                                            MessageBox.Show(CurveName + " Data not found.\n" + ex.Message,
                                                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return; // exit the method if needed
                                        }
                                        catch (Exception ex)
                                        {
                                            // Any other error
                                            MessageBox.Show("Error reading " + CurveName + " not found'.\n" + ex.Message,
                                                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return; // exit the method if needed
                                        }





                                        // ok fra her
                                        if (myline.IndexOf('{') >= 0)  //txt curve
                                        {
                                            //MessageBox.Show("txt curve");


                                            file.WriteLine("\"keys\": ['t'], data :  [");
                                        }
                                        else
                                        {
                                            file.WriteLine("\"data\" :  [");  //vanlig kurve numeric

                                        }

                                    }

                            

                                    toolStripProgressBar1.Value = 70;
                                    //file.WriteLine(reader.GetString(reader.GetOrdinal("Epodateval")) + ",");
                                    //funker med både value og string
                                    if (cnt != 1)
                                        file.WriteLine(","); // write comma before all except first NEW
                                    //file.WriteLine(reader.GetValue(reader.GetOrdinal("Epodateval")) );
                                    var dateValue = reader.GetValue(reader.GetOrdinal("Epodateval"));
                                    file.Write($"{dateValue}");

                                    //file.WriteLine(reader.GetValue(reader.GetOrdinal("Epodateval")) + ",");

                                    



                                    toolStripProgressBar1.Value = 90;
                                    //DynDateDesc = reader.GetDateTime(0).ToString("yyyy.MM.dd HH");  // må passe på å ha fdate i vdate for desc kolonne 0 er Vdate
                                    //DynDateDesc = reader.IsDBNull(0) ? null : reader.GetDateTime(0).ToString("yyyy.MM.dd HH");



                                }

                            } //if
                            else

                            //ingen rader på kurven do nothing+mar1
                            {

                                //file.WriteLine("data: [] }]}]");  //legger på datatomrad og lukker not needed if u never het to here..
                                file.WriteLine("\"data\": [[] ,");  //legger på datatomrad mar1 json mar24
                                toolStripProgressBar1.Value = 0;

                                //MessageBox.Show("Series : " + CurveName + ":  Empty Interval ", Globals.AppName);
                                //toolStripStatusLabel1.Text = "Warning: Empty Result Interval for Series: " + CurveName;
                                ShowErrorStrip("Warning: Empty Result Interval for Series: " + CurveName);

                                //tbc if not want to return on empty interval
                                //
                                //labelyear = "";
                                //return; want to continue  \"

                            }



                        }
                        catch (Exception)
                        {

                            //MessageBox.Show("Series : " + CurveName + " Access Denied ", Globals.AppName);
                            //toolStripStatusLabel1.ForeColor = Color.Red;
                            //toolStripStatusLabel1.Text = "Error Access Denied for Series: " + CurveName;

                            ShowErrorStrip("Error Access Denied for Series: " + CurveName);
                            toolStripProgressBar1.Value = 0;
                            return;
                        }



                       

                    } //use flyttet hit nedover


                    file.WriteLine(); // end array

                    sqlCon.Close();
                    //reader.Close();


                    

                    // was  file.WriteLine("].sort((a, b) => a[0] - b[0]),     ");

                    file.WriteLine("], "); //closing data new    ????????????????????????????????????????++

                    file.WriteLine($"\"name\":\"{CurveName}\",");

                    file.WriteLine($"\"unitid\":\"{UnitId}\", ");
                   
                    file.WriteLine($"\"unit\":\"{Unit}\", ");
                    file.WriteLine($"\"title\":\"{Title}\", ");
                    file.WriteLine($"\"source\":\"{Source}\", ");

                    file.WriteLine($"\"type\":\"{linetype}\", ");

                    //file.WriteLine($"\"stacking\":\"{stacking}\", ");

                    if (stacking == null)
                        file.WriteLine("\"stacking\": null,");
                    else
                        file.WriteLine($"\"stacking\": \"{stacking}\",");






                    file.WriteLine($"\"yAxis\":{LRName}, ");  //int
                    file.WriteLine($"\"lineWidth\":{WidthValue}, ");  //int


                    file.WriteLine($"\"dashStyle\":\"{DashValue}\", ");

                    //color
                    file.WriteLine($"\"color\":\"{mycolor}\", ");
                    //colorname
                    //file.WriteLine($"color: '{mycolorname}', "); // name

                    file.WriteLine($"\"func\":  \"{FnameReplaced}\", ");

               

                    string labelYear = "";

                    if (!string.IsNullOrWhiteSpace(myline))
                    {
                        string trimmed = myline.Trim('[', ']');

                        string[] parts = trimmed.Split(',');

                        if (parts.Length > 0 &&
                            long.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out long epochMillis))
                        {
                            DateTime date = DateTimeOffset
                                .FromUnixTimeMilliseconds(epochMillis)
                                .UtcDateTime;

                            labelYear = date.Year.ToString();
                        }
                    }







                    file.WriteLine($"\"labelyear\":\"{labelYear}\", ");
                    file.WriteLine($"\"order\":{myorder}, ");
                    file.WriteLine($"\"zIndex\":{myzindex}, ");

                    file.WriteLine($"\"interval\":\"{Iname}\", ");

                    //file.WriteLine("aggregation: '" + AggDesc + "', ");
                    //TBC
                   // file.WriteLine($"aggregation: '{(string.IsNullOrEmpty(AggDesc) || AggDesc == "None" ? "" : AggDesc)}', ");

                    file.WriteLine($"\"aggregation\":\"{(string.IsNullOrEmpty(AggDesc) || AggDesc == "None" ? "" : AggDesc)}\", ");

                    //file.WriteLine($"\"desc\":\"{SerieName.Trim() + "', " + "              }, ");  //lukker curvenn
                    file.WriteLine($"\"desc\":\"{SerieName.Trim()}\" ");

                    //COMMA om ikke siste
                    if (dataGridView2.Rows.Count != numseries)
                        file.WriteLine( "  }, "); //more to come..
                    else file.WriteLine("  } " );  //last series




                    // file.WriteLine("desc: \"" + DynDateDesc + " " + SerieName.Trim() + " \", " + "              }, ");  //lukker curvenny  " //lukker curven org før forecastmay22




                }
                //for each



                //avslutter filen
                file.WriteLine("] ");    //eo series new



                file.WriteLine("   }  ] ");


                toolStripProgressBar1.Value = 40;


            }





            try
            {
                string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string folder = Path.GetDirectoryName(strExeFilePath);
                string myfiletoshow = Path.Combine(folder, htmlfileName);

                string dataFile = "Mydata.js";

                //nytt herfra
                string jsonFile = Path.Combine(Globals.UserDir, "Mydata.json");
                string jsFile = Path.Combine(Globals.UserDir, "Mydata.js");

                // Read existing JSON
                string json = File.ReadAllText(jsonFile);

                // Wrap it for JS
                string jsContent = "window.seriesData = " + json + ";";

                // Write JS file
                File.WriteAllText(jsFile, jsContent);




                var fileUri = new Uri(myfiletoshow);


               

                Process.Start(new ProcessStartInfo
                {
                    FileName = fileUri.ToString(),
                    Arguments = $"?filename={Uri.EscapeDataString(dataFile)}",
                    UseShellExecute = true
                });

            }



            catch (Win32Exception)
            {
                MessageBox.Show("Could not find html file or browser", Globals.AppName);
            }



            toolStripProgressBar1.Value = 50;
            toolStripProgressBar1.Value = 0;
        }



//end of high independent




        private string GetMenuesFromFile()
        {
            string basepath = Application.StartupPath;
            string txtpath = basepath + @"/Menues.csv";  //filename
            const Int32 BufferSize = 128;


            if (File.Exists(txtpath))
            {


                using (var fileStream = File.OpenRead(txtpath))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                {

                    try //om antall kolonner er odde eller posisjon ikke passer
                    {

                        String line = streamReader.ReadLine();
                        String[]words = line.Split(',');
                        //MessageBox.Show(words[0]);
                        // words[0].Text = words[1];
                        //  this.Controls.Find(words[0], true).FirstOrDefault().Text = words[2];

                        //sjd work but no App header name
                        this.Text = words[Globals.LangColumn];
                        Globals.AppName = words[Globals.LangColumn];



                        line = streamReader.ReadLine(); words = line.Split(',');
                        NyChart_btn.Text = words[Globals.LangColumn];
                        chartToolStripMenuItem.Text = words[Globals.LangColumn];

                        line = streamReader.ReadLine(); words = line.Split(',');
                        NyMulti_btn.Text = words[Globals.LangColumn];
                        multiChartToolStripMenuItem.Text = words[Globals.LangColumn];

                        //adding dec25 btn and menue same text
                        line = streamReader.ReadLine(); words = line.Split(',');
                        YearChartRepo_btn.Text = words[Globals.LangColumn];
                      




                        line = streamReader.ReadLine(); words = line.Split(',');
                        ClearOne2_btn.Text = words[Globals.LangColumn];

                        line = streamReader.ReadLine(); words = line.Split(',');
                        Clear2_Btn.Text = words[Globals.LangColumn];


                        line = streamReader.ReadLine(); words = line.Split(','); //wildcard
                        Wild_label3.Text = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        Substr_label4.Text = words[Globals.LangColumn];

                        
                        line = streamReader.ReadLine(); words = line.Split(',');
                        Search_btn.Text = words[Globals.LangColumn];

                      


                        line = streamReader.ReadLine(); words = line.Split(',');
                        fileToolStripMenuItem.Text = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');

                        //save new
                        saveAsToolStripMenuItem.Text = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        //retirve
                        retriveToolStripMenuItem.Text = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');

                        //copyDatafileToolStripMenuItem
                        //copy
                        copyDatafileToolStripMenuItem.Text = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');

                        
                        //convertLastesToPXToolStripMenuItem new
                        //convertLastesToPXToolStripMenuItem.Text = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');


                        exitToolStripMenuItem.Text = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');


                        graphToolStripMenuItem.Text = words[Globals.LangColumn]; //menue
                        line = streamReader.ReadLine(); words = line.Split(',');
                        reportToolStripMenuItem.Text = words[Globals.LangColumn];
                        //repoToolStripMenuItem


                        line = streamReader.ReadLine(); words = line.Split(',');
                        OptionToolStripMenuItem.Text = words[Globals.LangColumn];
                        //undermenyeun på report:
                        //flyttet
                        line = streamReader.ReadLine(); words = line.Split(',');
                        reportToolStripMenuItem1.Text = words[Globals.LangColumn];
                        




                        line = streamReader.ReadLine(); words = line.Split(',');
                        parserStatsToolStripMenuItem.Text = words[Globals.LangColumn]; //admin
                        line = streamReader.ReadLine(); words = line.Split(',');
                        tableMgrToolStripMenuItem.Text = words[Globals.LangColumn]; //tablemgr
                        line = streamReader.ReadLine(); words = line.Split(',');//new
                        upgradeToolStripMenuItem.Text = words[Globals.LangColumn]; //check updates



                        line = streamReader.ReadLine(); words = line.Split(',');
                        helpToolStripMenuItem.Text = words[Globals.LangColumn]; //help

                      
                        line = streamReader.ReadLine(); words = line.Split(',');
                        Base_Label.Text = words[Globals.LangColumn]; //base



                         // curve datagrid starts at 0
                             for (int i = 0; i < dataGridView1.Columns.Count; i++)  //Ups  length for hidden
                        {
                            line = streamReader.ReadLine();
                            words = line.Split(',');
                            dataGridView1.Columns[i].HeaderText = words[Globals.LangColumn];
                        }



                        //curve datagrid starts at 0
                        /*
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView1.Columns[0].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView1.Columns[1].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView1.Columns[2].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView1.Columns[3].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView1.Columns[4].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView1.Columns[5].HeaderText = words[Globals.LangColumn];


                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView1.Columns[6].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView1.Columns[7].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');//updated
                        dataGridView1.Columns[8].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');//by
                        dataGridView1.Columns[9].HeaderText = words[Globals.LangColumn];

                        line = streamReader.ReadLine(); words = line.Split(','); //obs
                        dataGridView1.Columns[10].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(','); //diff
                        dataGridView1.Columns[11].HeaderText = words[Globals.LangColumn];

                        line = streamReader.ReadLine(); words = line.Split(','); //
                        dataGridView1.Columns[12].HeaderText = words[Globals.LangColumn];
                       
                        line = streamReader.ReadLine(); words = line.Split(','); //min
                        dataGridView1.Columns[13].HeaderText = words[Globals.LangColumn];


                        line = streamReader.ReadLine(); words = line.Split(','); //max
                        dataGridView1.Columns[14].HeaderText = words[Globals.LangColumn];


                        line = streamReader.ReadLine(); words = line.Split(','); //interval
                        dataGridView1.Columns[15].HeaderText = words[Globals.LangColumn];


                        line = streamReader.ReadLine(); words = line.Split(','); //interval
                        dataGridView1.Columns[16].HeaderText = words[Globals.LangColumn];

                        line = streamReader.ReadLine(); words = line.Split(','); //interval
                        dataGridView1.Columns[17].HeaderText = words[Globals.LangColumn];

                        line = streamReader.ReadLine(); words = line.Split(','); //interval
                        dataGridView1.Columns[18].HeaderText = words[Globals.LangColumn];
                        */





                        // curve datagrid starts at 0
                      
                        for (int i = 0; i < 16; i++) //noen hidden er ikke i translatefila tbc
                        {
                            line = streamReader.ReadLine();
                            words = line.Split(',');
                            dataGridView2.Columns[i].HeaderText = words[Globals.LangColumn];
                        }
                      


                        //Valgte curve datagrid2
                        /*
                       
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView2.Columns[0].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView2.Columns[1].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView2.Columns[2].HeaderText = words[Globals.LangColumn];
                        
                        //:fom color to sort:
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView2.Columns[3].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView2.Columns[4].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView2.Columns[5].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView2.Columns[6].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView2.Columns[7].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView2.Columns[8].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView2.Columns[9].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView2.Columns[10].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView2.Columns[11].HeaderText = words[Globals.LangColumn];
                        //--AdjustableArrowCap another one
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView2.Columns[12].HeaderText = words[Globals.LangColumn];

                        //stack+order
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView2.Columns[13].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(',');
                        dataGridView2.Columns[14].HeaderText = words[Globals.LangColumn];
                     

                        */

                    

                        line = streamReader.ReadLine(); words = line.Split(',');
                        yearChartToolStripMenuItem.Text = words[Globals.LangColumn];

                        //deci & image date:
                        line = streamReader.ReadLine(); words = line.Split(',');
                        decimalToolStripMenuItem.Text = words[Globals.LangColumn];

                        line = streamReader.ReadLine(); words = line.Split(',');
                        imageDateToolStripMenuItem.Text = words[Globals.LangColumn];


                        //wildcards, release og about

                        line = streamReader.ReadLine(); words = line.Split(',');
                        wildcardingToolStripMenuItem.Text = words[Globals.LangColumn];

                        line = streamReader.ReadLine(); words = line.Split(',');
                        releasesToolStripMenuItem.Text = words[Globals.LangColumn];

                       






                    } //try
                    catch (IndexOutOfRangeException)
                    {
                        MessageBox.Show("No more Languages ?", Globals.AppName);
                        Globals.LangColumn = 0;
                    }

                    // while }
                }

                //MessageBox.Show(ss.ToString());
                return ("ok");
            }
            else
            {
                MessageBox.Show("Menues.csv file missing in: " + basepath, Globals.AppName);
                return ("error");
            }

        }

        private void LangBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("changing value : " + LangBox1.Text + " index: " + LangBox1.SelectedIndex.ToString());
            Globals.LangColumn = LangBox1.SelectedIndex;
            GetMenuesFromFile();
        }



            private void Search_btn_Click(object sender, EventArgs e)
        {
            RunSearch();
        }

        private void CleardataGridView1()
        {
            dataGridView1.DataSource = null;
        }



        private void RunSearch()
        {
            //argumenter wildcard ny search

            
            toolStripProgressBar1.Value = 80;

            SqlConnection sqlCon = null;
            using (sqlCon = new SqlConnection(Globals.DBStr))
            {
                sqlCon.Open();
                SqlCommand sql_cmnd = new SqlCommand("Client_Search", sqlCon);
                sql_cmnd.CommandType = CommandType.StoredProcedure;


            var pList = new SqlParameter("listOfIDstbl", SqlDbType.Structured);
            pList.TypeName = "dbo.StringList";
            pList.Value = Globals.table;


            toolStripProgressBar1.Value = 90;

                sql_cmnd.Parameters.Add(pList);

                sql_cmnd.Parameters.AddWithValue("@mywild", SqlDbType.NVarChar).Value = Wild_txt.Text;
                sql_cmnd.Parameters.AddWithValue("@mystr", SqlDbType.NVarChar).Value = Search_txt.Text;



            toolStripProgressBar1.Value = 80;
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(sql_cmnd);
            adapter.Fill(data);
            toolStripProgressBar1.Value = 50;


            dataGridView1.DataSource = data;

                // Format all DateTime columns
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    if (col.ValueType == typeof(DateTime))
                    {
                        col.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
                    }
                }


                sqlCon.Close();
            } //use


            //sizes here ?
            dataGridView1.Columns[0].Width = 60; //curveid
            dataGridView1.Columns[4].Width = 50; //freq


            toolStripProgressBar1.Value = 30;
            toolStripStatusLabel1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Series Selected" + Globals.TotalStr;
            toolStripProgressBar1.Value = 0;



        }

       
        private void releasesToolStripMenuItem_Click(object sender, EventArgs e)
        {    
           


            try
            {

               // string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;  //show path of exe
              //  var myfiletoshow = System.IO.Path.GetDirectoryName(strExeFilePath) + "\\" + "Releases.html";


                Process.Start(new ProcessStartInfo
                {
                    FileName = "www.harmonize.no/install.html",
                    UseShellExecute = true
                });



            }
            catch (Win32Exception)
            {
                MessageBox.Show("Could not find html file or browser", Globals.AppName);
            }















        }

       


    

        private void checkedListBoxMonths_SelectedIndexChanged(object sender, EventArgs e)
        {

            int i;
            string s = "";
            bool chk = false;


            if (comboBasisBox.Text == "") //om ingen år velger bort alle mnd
            {
                for (i = 0; i <= (checkedListBoxMonths.Items.Count - 1); i++)
                {
                    checkedListBoxMonths.SetItemChecked(i, false);

                }

            }





            for (i = 0; i <= (checkedListBoxMonths.Items.Count - 1); i++)
            {
                if (checkedListBoxMonths.GetItemChecked(i) && i > 0) //jan til dec  
                {
                    chk = true;
                    s = s + checkedListBoxMonths.Items[i].ToString();
                }

                if (checkedListBoxMonths.GetItemChecked(0) )  //all months first pos
                {
                    checkedListBoxMonths.SetItemChecked(i, false);  //setter alle til unselected
                    s = "";
                    chk = true;
                    checkedListBoxMonths.SetItemChecked(0,true);  //alle måneder sender bare året

                }

               
               }

            //if no year I set to a year

            if (comboBasisBox.Text == "" && chk == true)  //måder valgt men ingen year
            {
                comboBasisBox.Text = "2020";
                Globals.BaseYear = 2020;
            }


            if (comboBasisBox.Text == "" && chk == false)  //måder valgt men ingen year
            {
                comboBasisBox.Text = "";
                Globals.BaseYear = 1;
            }


            else
            {
                Globals.BaseYear = int.Parse(comboBasisBox.Text);
            }



            Globals.BaseMnds = int.Parse(Globals.BaseYear.ToString() +(s));


            // MessageBox.Show((Globals.BaseMnds));

            // MessageBox.Show("here:" +int.Parse(Globals.BaseMnds).ToString());
            //MessageBox.Show(Globals.BaseMnds.ToString());

        }




        //saver datagrid2 til en fil
        private void saveAsToolStripMenuItem_DoubleClick(object sender, EventArgs e)
        {



            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.Title = "Save as CSV";
                sfd.InitialDirectory = Globals.UserDir;   // <-- default folder
                sfd.RestoreDirectory = true;
                sfd.FileName = "Exportchartdef.csv";
                // sfd.InitialDirectory = @"C:\Temp";

                dataGridView2.EndEdit();
                dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ExportDataGridViewToCsv(dataGridView2, sfd.FileName);
                    MessageBox.Show("CSV file saved successfully.", "Export",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        private void ExportDataGridViewToCsv(DataGridView dgv, string filePath)
        {
            StringBuilder sb = new StringBuilder();
            string delimiter = ",";

            // Header
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                //sb.Append(dgv.Columns[i].HeaderText);
                sb.Append(dgv.Columns[i].Name);
                if (i < dgv.Columns.Count - 1)
                    sb.Append(delimiter);
            }
            sb.AppendLine();

            // Rows
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    string value = "";
   
                    if (dgv.Columns[i].Name == "Color") // or index == 10
                    {
                        value = row.Cells[i].Value?.ToString() ?? "";
                    }
                    else
                    {
                        value = row.Cells[i].FormattedValue?.ToString() ?? "";
                    }



                    // Escape quotes
                    value = value.Replace("\"", "\"\"");

                    // Quote if needed
                    if (value.Contains(delimiter) || value.Contains("\n") || value.Contains("\""))
                        value = $"\"{value}\"";

                    //sb.Append(value);   //Trim()
                    sb.Append(value.Trim());
                    if (i < dgv.Columns.Count - 1)
                        sb.Append(delimiter);
                }
                sb.AppendLine();
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }




        private void retriveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV files (*.csv)|*.csv";
                ofd.Title = "Open CSV file";
                ofd.InitialDirectory = Globals.UserDir;
                ofd.RestoreDirectory = true;
                // ofd.InitialDirectory = @"C:\Harmonize";   // <-- default folder

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ImportCsvToDataGridView(dataGridView2, ofd.FileName);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"The selected file is not a valid CSV file.\n\n{ex.Message}",
                            "CSV Import Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
            //test buttons chart / clear avail if success
            UpdateButtonStates(); //ghoster buttons
            dataGridView2.ClearSelection();  //ingen valgt
        }


        private void ImportCsvToDataGridView(DataGridView dgv, string filePath)
        {
            dgv.Rows.Clear();

            try
            {
                using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    // Read header
                    string headerLine = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(headerLine))
                        throw new Exception("CSV file is empty or missing header.");

                    string[] headers = ParseCsvLine(headerLine, ',');

                    if (headers.Length == 0)
                        throw new Exception("CSV header could not be parsed.");

                   // HEADER VALIDATION
                    if (!headers[0].Trim().Equals("Id", StringComparison.OrdinalIgnoreCase))
                        throw new Exception("Invalid Harmonize CSV file ... ");



                    // Create columns if empty
                    if (dgv.Columns.Count == 0)
                    {
                        foreach (string header in headers)
                            dgv.Columns.Add(header, header);
                    }

                    // Read rows
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] values = ParseCsvLine(line, ',');

                        if (values.Length == 0)
                            throw new Exception("Invalid CSV row detected.");

                        int rowIndex = dgv.Rows.Add();

                        for (int i = 0; i < headers.Length && i < values.Length; i++)
                        {
                            string columnName = headers[i].Trim();
                            string cellValue = values[i]?.Trim();

                            if (!dgv.Columns.Contains(columnName))
                                continue;

                            var column = dgv.Columns[columnName];

                            // Specialhandling for Color ComboBox column

                            if (columnName == "Interval")
                            {
                                dgv.Rows[rowIndex].Cells[columnName].Value = "AllData";
                            }



                                if (columnName == "Color")
                            {
                               // MessageBox.Show("calling set color..");
                                SetColorCellValue(dgv, rowIndex, cellValue);
                            }
                            else
                            {
                                //MessageBox.Show(columnName + "  ok?");
                                dgv.Rows[rowIndex].Cells[columnName].Value = cellValue?.Trim();
                               
                            }
                        }

                    }
                }
            }
            catch (IOException ex)
            {
                throw new Exception("Unable to read the file. It may be open or inaccessible.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("You do not have permission to access this file.", ex);
            }
        }



        private void SetColorCellValue(DataGridView dgv, int rowIndex, string importedColorCode)
        {
            if (string.IsNullOrWhiteSpace(importedColorCode))
            {
                dgv.Rows[rowIndex].Cells["Color"].Value = null;
                return;
            }

            string colorCode = importedColorCode.Trim();


            if (colors == null)
            {
                MessageBox.Show("colors list is not initialized!");
                return;
            }


            var existing = colors.FirstOrDefault(c =>
                c.ColorCode.Equals(colorCode, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                colors.Add(new ColorItem
                {
                    ColorName = "(Imported)",
                    ColorCode = colorCode
                });
            }

            dgv.Rows[rowIndex].Cells["Color"].Value = colorCode;
        }







      private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }
        //to be taken in use






        private string[] ParseCsvLine(string line, char delimiter)
        {
            List<string> result = new List<string>();
            StringBuilder current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == delimiter && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            if (inQuotes)
                throw new Exception("Malformed CSV: missing closing quote.");

            result.Add(current.ToString());
            return result.ToArray();
        }






        private void deciToolStripMenuItem_Click(object sender, EventArgs e)
        {
 
                if (sender is ToolStripMenuItem menuItem)
                {
                Globals.deci = menuItem.Tag?.ToString();

                foreach (ToolStripMenuItem item in menuItem.Owner.Items)
                {
                    item.Enabled = true;  // enable all first
                }
                menuItem.Enabled = false; // disable the one clicked



            }


        }


        private void FreqToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                Globals.freq = menuItem.Tag?.ToString();
                foreach (ToolStripMenuItem item in menuItem.Owner.Items)
                {
                    item.Enabled = true;  // enable all first
                }
                menuItem.Enabled = false; // disable the one clicked
            }
        }


        //try this to avoid problem with selection in datagrid2
        private void DataGridView2_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView2.IsCurrentCellDirty)
            {
                dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }



        //try to log
        /*
        void LogMessage()
        {
            try
            {
               string url = "https://www.eriksberg.no/Harmonizelog/Harmonizelog.php";

           string line =
           $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | " +
           $"PC={Environment.MachineName} | " +
           $"User={Environment.UserName} | " +
           $"Version={Globals.Version} | " +
           $"Country={RegionInfo.CurrentRegion.TwoLetterISORegionName} | " +
           $"Lang={CultureInfo.CurrentCulture.Name}  " ;


                using (WebClient wc = new WebClient())
                {
                  
                    wc.UploadString(url, line);
                }
            }
            catch
            {
                // Ignore if offline or server unreachable
            }
        }
        */





        /*
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            treeView1.SelectedNode = e.Node;

            //when having a message box it works better, when no message box I always  get to treecontextmenu1  not 2
           // MessageBox.Show(" click  : " + e.Node.Text  + " level " + e.Node.Nodes.Count.ToString());


             if (e.Node.Level == 0 && e.Node.Text== "All")
           
                {
                // Top-level node (e.g., "All") → no menu
               // MessageBox.Show(e.Node.Text);
               // MessageBox.Show(" if : " + e.Node.Text);
                treeContextMenu2.Show(treeView1, e.Location); //toogle
               
            }
            else if (e.Node.Nodes.Count == 0)
            {
                // Bottom-level node → show bottom menu
                //MessageBox.Show(" else if : " +e.Node.Text);
                treeContextMenu.Show(treeView1, e.Location);
            }
            else if (e.Node.Nodes.Count > 1)
            {
                // Parent node → show parent menu
                treeContextMenu2.Show(treeView1, e.Location); //toggle
                //MessageBox.Show(" else : " + e.Node.Text);
            }
        }

*/

        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            TreeNode node = treeView1.GetNodeAt(e.Location);
            if (node == null)
                return;

            treeView1.SelectedNode = node;

            if (node.Level == 0 && node.Text == "All")
            {
                treeContextMenu2.Show(treeView1, e.Location);
            }
            else if (node.Nodes.Count == 0)
            {
                treeContextMenu.Show(treeView1, e.Location);
            }
            else
            {
                treeContextMenu2.Show(treeView1, e.Location);
            }
        }











        private void LoadCsvToDataBase(string lname, string filePath)
        {
            int cnt =  0;
            int commitEvery = 50;  // commit after 100 rows
            string[] allowedFormats = new[]
            {
            "yyyyMMdd",
            "yyyy-MM-dd",
            "yyyy-MM-ddTHH:mm:ss", //24 hour clock
            "yyyy-MM-dd",
            "dd.MM.yyyy",
            "dd/MM/yyyy",
            "MM/dd/yyyy"
             };

            CleardataGridView1();
            DateTime startTime = DateTime.UtcNow;
            try
            {
                using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    // Read header
                    string headerLine = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(headerLine))
                        throw new Exception("CSV file is empty or missing header.");

                    string[] headers = ParseCsvLine(headerLine, ',');

                    if (headers.Length < 5)
                        throw new Exception("CSV header invalid or missing columns.");

                    // HEADER VALIDATION
                    if (!headers[0].Trim().Equals("name", StringComparison.OrdinalIgnoreCase))
                        throw new Exception("Invalid CSV file - expecting column 'name'.");

                    if (!headers[1].Trim().Equals("unit", StringComparison.OrdinalIgnoreCase))
                        throw new Exception("Invalid CSV file - expecting column 'unit'.");

                    if (!headers[2].Trim().Equals("date", StringComparison.OrdinalIgnoreCase))
                        throw new Exception("Invalid CSV file - expecting column 'date'.");

                    if (!headers[3].Trim().Equals("value", StringComparison.OrdinalIgnoreCase))
                        throw new Exception("Invalid CSV file - expecting column 'value'.");

                    if (!headers[4].Trim().Equals("desc", StringComparison.OrdinalIgnoreCase))
                        throw new Exception("Invalid CSV file - expecting column 'desc'.");


                    using (SqlConnection sqlCon = new SqlConnection(Globals.DBStr))
                    {
                        sqlCon.Open();

                        SqlTransaction transaction = sqlCon.BeginTransaction();   // start first transaction

                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (string.IsNullOrWhiteSpace(line))
                                continue;

                            string[] values = ParseCsvLine(line, ',');

                            if (values.Length < 5)
                                throw new Exception("Invalid CSV row detected.");

                            string sname = values[0]?.Trim();
                            string unit = values[1]?.Trim();
                            string dateString = values[2]?.Trim();
                            string valueString = values[3]?.Trim();
                            string desc = values[4]?.Trim();

                            cnt++;

                            if (!DateTime.TryParseExact(
                                dateString,
                                allowedFormats,
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out DateTime parsedDate))
                            {
                                throw new Exception($"Invalid date format: {dateString}");
                            }

                            if (!decimal.TryParse(
                                valueString,
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out decimal parsedValue))
                            {
                                throw new Exception($"Invalid numeric value: {valueString}");
                            }

                            using (SqlCommand sql_cmnd = new SqlCommand("UTILS_PutTime", sqlCon, transaction))
                            {
                                sql_cmnd.CommandType = CommandType.StoredProcedure;

                                sql_cmnd.Parameters.Add("@passedlsname", SqlDbType.NVarChar, 100).Value = lname;
                                sql_cmnd.Parameters.Add("@sname", SqlDbType.NVarChar, 100).Value = sname;
                                sql_cmnd.Parameters.Add("@sdesc", SqlDbType.NVarChar, 500).Value = desc;
                                sql_cmnd.Parameters.Add("@unit_id", SqlDbType.Int).Value = unit;
                                sql_cmnd.Parameters.Add("@datestr", SqlDbType.DateTime).Value = parsedDate;
                                sql_cmnd.Parameters.Add("@value", SqlDbType.Float).Value = parsedValue;

                                sql_cmnd.ExecuteNonQuery();
                            }

                            // commit every N rows
                            if (cnt % commitEvery == 0)
                            {
                                transaction.Commit();
                                toolStripStatusLabel1.Text = ($"Imported: {cnt} rows , to be cont ..");
                                transaction.Dispose();
                                transaction = sqlCon.BeginTransaction();   // start NEW transaction

                                // pulse bar between 40-60 so user sees activity
                                toolStripProgressBar1.Value = (cnt / commitEvery) % 2 == 0 ? 40 : 60;
                                Application.DoEvents();
                            }
                        }

                        // commit remaining rows
                        transaction.Commit();
                        transaction.Dispose();
                 

                    toolStripProgressBar1.Value = 10;



                    using (SqlTransaction updateTransaction = sqlCon.BeginTransaction())
                    {
                        using (SqlCommand sql_cmnd2 = new SqlCommand("UTILS_UpdateCurveinfo", sqlCon, updateTransaction))
                        {
                            sql_cmnd2.CommandType = CommandType.StoredProcedure;
                            sql_cmnd2.Parameters.Add("@loadsetName", SqlDbType.NVarChar, 100).Value = lname;

                            sql_cmnd2.ExecuteNonQuery();
                        }

                        updateTransaction.Commit();
                    }


                }


                    }




                    toolStripProgressBar1.Value = 0;
                    DateTime endTime = DateTime.UtcNow;

                    TimeSpan duration = endTime - startTime;
                    //search as well to refresh ?
                    RunSearch();
                    toolStripStatusLabel1.Text = ($"Imported: {cnt} rows finished in {duration.TotalSeconds:F2} seconds");

               
            }

            catch (IOException ex)
            {
                toolStripProgressBar1.Value = 0;
                throw new Exception("Unable to read the file. It may be open or inaccessible.", ex);

            }
            catch (UnauthorizedAccessException ex)
            {
                toolStripProgressBar1.Value = 0;
                throw new Exception("You do not have permission to access this file.", ex);
            }
        }


      




        private void Upload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV files (*.csv)|*.csv";
                ofd.Title = "Open CSV file";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (ShowCsvPreview(ofd.FileName) != DialogResult.OK)
                        return;

                    try
                    {
                       LoadCsvToDataBase(treeView1.SelectedNode.Text, ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        toolStripProgressBar1.Value = 0;
                        MessageBox.Show(
                            $"Access denied ? / The selected file is not a valid CSV file?\n\n{ex.Message}",
                            "CSV Import Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        private DialogResult ShowCsvPreview(string filePath)
        {
            const int previewRows = 10;
            var table = new System.Data.DataTable();

            try
            {
                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    string headerLine = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(headerLine))
                    {
                        MessageBox.Show("CSV file is empty.", "Preview", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return DialogResult.Cancel;
                    }

                    string[] headers = ParseCsvLine(headerLine, ',');
                    foreach (string h in headers)
                        table.Columns.Add(h.Trim());

                    int count = 0;
                    while (!reader.EndOfStream && count < previewRows)
                    {
                        string line = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        string[] vals = ParseCsvLine(line, ',');
                        // Pad or trim to match column count
                        var row = table.NewRow();
                        for (int i = 0; i < table.Columns.Count; i++)
                            row[i] = i < vals.Length ? vals[i] : "";
                        table.Rows.Add(row);
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not read file for preview:\n{ex.Message}", "Preview Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return DialogResult.Cancel;
            }

            // Build preview dialog
            using (var dlg = new Form())
            {
                dlg.Text = $"CSV Preview — {System.IO.Path.GetFileName(filePath)}  (first {previewRows} rows)";
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.Width = 697;
                dlg.Height = 380;
                dlg.MinimizeBox = false;
                dlg.MaximizeBox = false;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;

                var grid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    DataSource = table,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                    RowHeadersVisible = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    BackgroundColor = System.Drawing.SystemColors.Window,
                    BorderStyle = BorderStyle.None
                };

                var panel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    FlowDirection = FlowDirection.LeftToRight,
                    Height = 44,
                    Padding = new Padding(6)
                };

                var btnCancel = new System.Windows.Forms.Button { Text = "Ups, Cancel", DialogResult = DialogResult.Cancel, Width = 130 };
                var btnUpload = new System.Windows.Forms.Button { Text = "Looks good, Import!", DialogResult = DialogResult.OK, Width = 130 };
                dlg.AcceptButton = btnUpload;
                dlg.CancelButton = btnCancel;

                panel.Controls.Add(btnUpload);
                panel.Controls.Add(btnCancel);

                dlg.Controls.Add(grid);
                dlg.Controls.Add(panel);

                return dlg.ShowDialog(this);
            }
        }


      


        private async void Delete_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("Please select a loadset first.", Globals.AppName,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string loadsetName = treeView1.SelectedNode.Text;

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete data for:\n\n{loadsetName} ?",
                Globals.AppName,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            CleardataGridView1();
            toolStripProgressBar1.Value = 20;

            DateTime startTime = DateTime.UtcNow;

            try
            {
                using (SqlConnection sqlCon = new SqlConnection(Globals.DBStr))
                    //try dbo
                using (SqlCommand cmd = new SqlCommand("dbo.Stp_DeleteLoadsetDataOnly_Client", sqlCon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@loadsetname", SqlDbType.NVarChar, 64).Value = loadsetName;
                    cmd.Parameters.Add("@RowsDeleted", SqlDbType.Int).Direction = ParameterDirection.Output;

                    await sqlCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();

                    int rowsDeleted = (int)cmd.Parameters["@RowsDeleted"].Value;

                    toolStripProgressBar1.Value = 80;

                    TimeSpan duration = DateTime.UtcNow - startTime;

                    RunSearch(); // refresh UI after delete

                    toolStripStatusLabel1.Text =
                    $"Deleted successfully ({rowsDeleted} Series) in {duration.TotalSeconds:F2} sec.";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error deleting data:\n\n{ex.Message}",
                    Globals.AppName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                toolStripStatusLabel1.Text = "Delete failed.";
            }
            finally
            {
                toolStripProgressBar1.Value = 0;
            }
        }




        private async void Grant_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("Please select a loadset first.", Globals.AppName,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string loadsetName = treeView1.SelectedNode.Text;

            try
        
            {
                toolStripProgressBar1.Value = 75;
                using (SqlConnection sqlCon = new SqlConnection(Globals.DBStr))
                //try dbo
                using (SqlCommand cmd = new SqlCommand("dbo.CLIENT_grant_write_access", sqlCon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@loadsetname", SqlDbType.NVarChar, 64).Value = loadsetName;
    

                    await sqlCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    toolStripProgressBar1.Value = 50;
                    BuildTree();  //refreshing the tree
                    toolStripStatusLabel1.Text ="Write access Granted .";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error Grant: \n\n{ex.Message}",
                    Globals.AppName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                toolStripStatusLabel1.Text = "Grant failed.";
            }
            finally
            {
                toolStripProgressBar1.Value = 0;
            }
        }





        private async void Revoke_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("Please select a loadset first.", Globals.AppName,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string loadsetName = treeView1.SelectedNode.Text;

            try

            {
                toolStripProgressBar1.Value = 75;
                using (SqlConnection sqlCon = new SqlConnection(Globals.DBStr))
                //try dbo
                using (SqlCommand cmd = new SqlCommand("dbo.CLIENT_revoke_write_access", sqlCon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@loadsetname", SqlDbType.NVarChar, 64).Value = loadsetName;


                    await sqlCon.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    toolStripProgressBar1.Value = 50;
                    BuildTree();  //refreshing the tree
                    toolStripStatusLabel1.Text = "Write access Revoked .";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error Revoke: \n\n{ex.Message}",
                    Globals.AppName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                toolStripStatusLabel1.Text = "Revoke failed.";
            }
            finally
            {
                toolStripProgressBar1.Value = 0;
            }
        }









        private void InitializeTreeContextMenu()
        {
            treeContextMenu = new ContextMenuStrip();
            treeContextMenu.Items.Add("Upload data file...", null, Upload_Click);
            treeContextMenu.Items.Add("Delete dataset content", null, Delete_Click);
            treeContextMenu.Items.Add("Grant dataset write", null, Grant_Click);
            treeContextMenu.Items.Add("Revoke dataset write", null, Revoke_Click);

            treeContextMenu.Opening += TreeContextMenu_Opening;

            treeView1.ContextMenuStrip = treeContextMenu;

        }


        private void TreeContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                e.Cancel = true;
                return;
            }
            
            // Only apply logic to dataset level nodes based on write access 
            if (treeView1.SelectedNode.Tag is bool hasWriteAccess)
            {
                treeContextMenu.Items[0].Enabled = hasWriteAccess; // Upload
                treeContextMenu.Items[1].Enabled = hasWriteAccess; // Delete
                treeContextMenu.Items[2].Enabled = !hasWriteAccess;  // Grant: only when read-only
                treeContextMenu.Items[3].Enabled = hasWriteAccess;  // revoke
            }
            else
            {
                // Parent? nodes (Project / Customer etc.)
                treeContextMenu.Items[0].Enabled = false; 
                treeContextMenu.Items[1].Enabled = false;
              

            }
            


        }






        private void InitializeTreeContextMenu2()
        {
            treeContextMenu2 = new ContextMenuStrip();
            treeContextMenu2.Items.Add("Expand/Collapse...", null, ToggleTree_Click);
            treeContextMenu2.Items.Add("Refresh Tree", null, RefreshTree_Click);
        }


        private void RefreshTree_Click(object sender, EventArgs e)
        {
            BuildTree();
        }






        private void ToggleTree_Click(object sender, EventArgs e)
        {
            bool anyExpanded = false;
            // Check if any top-level node is expanded
            foreach (TreeNode node in treeView1.Nodes)
            {
                if (node.IsExpanded)
                {
                    anyExpanded = true;
                    break;
                }
            }
            if (anyExpanded)
            {
                // Collapse all nodes
                treeView1.CollapseAll();
            }
            else
            {
                // Expand all nodes
                treeView1.ExpandAll();
            }
        }


        //for collor
        private void dataGridView2_CurrentCellDirtyStateChanged_1(object sender, EventArgs e)
        {
            if (dataGridView2.IsCurrentCellDirty)
            {
                dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }

        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.Columns[e.ColumnIndex].Name == "Color")
            {
                UpdateColor(e.RowIndex);
            }

        }

        ///
        //testing for upgrades


        //log



        async void LogMessage()
        {

            if (Globals.MaxNum != 666) 
                    { 
            string url = "https://harmonize.no/api/testforupgrade.php?product=HARMONIZE&client=Start-" + Environment.UserName + "&version=" + Globals.Version;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string json = await client.GetStringAsync(url);

                    var result = JsonConvert.DeserializeObject<UpgradeResponse>(json);


                }
                //catch (Exception ex)
                catch (Exception)
                {
                    //MessageBox.Show("Harmonize.no -Error: " + ex.Message);
                }
            }
        }//if
        }




        private async void Check_Updates_Click(object sender, EventArgs e)
        {

            if (Globals.MaxNum != 666)
            {

                string url = "https://harmonize.no/api/testforupgrade.php?product=HARMONIZE&client=Utest-" + Environment.UserName + "&version=" + Globals.Version;
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        string json = await client.GetStringAsync(url);
                        var result = JsonConvert.DeserializeObject<UpgradeResponse>(json);
                        if (result.upgradeAvailable)
                        {
                            MessageBox.Show(
                                $"New version {result.latestVersion} available at Harmonize.no!\n\n{result.message}",
                                "Upgrade Available",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("You are running the latest version." + Globals.Version);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Harmonize.no -Error: " + ex.Message);
                    }
                }
            }

            else
            {
                MessageBox.Show("Could not verify latest version. Please See Harmonize.no " + Globals.Version);
            }

            }





        private void copyDatafileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sourceFile = Path.Combine(Globals.UserDir, "Mydata.js");
            string sourceFile2 = Path.Combine(Globals.UserDir, "Mydata.json");

            if (!File.Exists(sourceFile))
            {
                MessageBox.Show("Source file not found:\n" + sourceFile,
                    Globals.AppName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Title = "Save data file as...";
                dlg.Filter = "Json file for web use (*.json)|*.json";
                dlg.DefaultExt = "json";
                dlg.InitialDirectory = Globals.UserDir;
                dlg.FileName = "Mydata.json";
                // SaveFileDialog already prompts "overwrite?" natively
                dlg.OverwritePrompt = true;

                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                string targetFile = dlg.FileName;
                string targetFile2 = Path.ChangeExtension(targetFile, ".json");

                try
                {
                    //File.Copy(sourceFile, targetFile, true);
                    File.Copy(sourceFile2, targetFile2, true);

                    MessageBox.Show(targetFile2+" Successfully saved ",
                        Globals.AppName,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error copy file:\n\n" + ex.Message,
                        Globals.AppName,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }






        private void ShowErrorStrip(string message)
        {  
            toolStripStatusLabel1.Text = message;
            toolStripStatusLabel1.ForeColor = Color.DarkRed;
        }

        private void ShowNormalStrip(string message)
        {    
            toolStripStatusLabel1.Text = message;
            toolStripStatusLabel1.ForeColor = Color.RoyalBlue;
           // toolStripStatusLabel1.Font = _defaultStatusFont;
        }

        private void wipe_btn_Click(object sender, EventArgs e)
        {
            //spar language
            //dataGridView1.Rows.Clear(); resuse:
            dataGridView1.DataSource = null;

            dataGridView1.DataSource = Curve;
            //SetAutoScrollMargin til language

        }



        //px-web no point supporting obslotete data formats.
        /*
        private void convertLastesToPXToolStripMenuItem_Click(object sender, EventArgs e)
        {

        string myfile = Globals.UserDir + "\\Mydata.json";

            if (!File.Exists(myfile))
              //  MessageBox.Show("No file to convert: "+ myfile, "Harmonize");
            return;

            ConvertJsonToPx(myfile);
        }
        */


        /*
        static string SavePxFile(string pxText, string shortName)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {

                dlg.Title = $"Save {shortName} as PX file";
                dlg.Filter = "PX files (*.px)|*.px";
                dlg.FileName = $"{shortName}_{DateTime.Now:yyyyMM}.px";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(dlg.FileName, pxText, Encoding.GetEncoding(1252));
                     MessageBox.Show("PX file saved successfully!",  "Harmonize");
                    //Form1.toolStripStatusLabel1.Text = "PX file saved successfully! in : ";
                    return dlg.FileName;   // ✅ return actual path
                }
            }
            return null; // user cancelled
        }
        */


        /*
         //only used by pc web -- commented out
        static string FormatList(List<string> list)
        {
            return string.Join(",", list.Select(x => $"\"{x}\""));
        }
        */




        //px web conversion  need to fix yearly
        //commented out
        /*

                public static void ConvertJsonToPx(string jsonPath)
                {
                    // ✅ Just read JSON directly
                    string json = File.ReadAllText(jsonPath);

                    // ✅ Deserialize directly (no fixes needed)
                    var seriesData = JsonConvert.DeserializeObject<List<SeriesContainer>>(json);

                    var container = seriesData[0];
                    var seriesList = container.series;

                    string fullName = seriesList.FirstOrDefault()?.name ?? "PX";

                    string shortName = fullName.Contains(":")
                        ? fullName.Split(':')[0]
                        : fullName;

                    // --- Collect all months ---
                    string ToPxMonth(long ms)
                    {
                        var dt = DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime;
                        return dt.ToString("yyyy'M'MM");
                    }

                    var allMonths = seriesList
                        .SelectMany(s => s.data)
                        .Select(d => ToPxMonth(Convert.ToInt64(d[0])))
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();

                    var categories = seriesList.Select(s => s.desc).ToList();

                    // --- Build PX ---
                    var sb = new StringBuilder();

                    sb.AppendLine("CHARSET=\"ANSI\";");
                    sb.AppendLine("AXIS-VERSION=\"2013\";");
                    //sb.AppendLine($"TITLE=\"{container.mytitle}\";");
                    sb.AppendLine($"TITLE=\"{seriesList[0].title}\";");
                    sb.AppendLine($"SUBTITLE=\"{container.subtitle}\";");
                    //sb.AppendLine("SOURCE=\"Harmonize\";");
                    sb.AppendLine($"SOURCE=\"{seriesList[0].source}\";");
                    sb.AppendLine($"UNITS=\"{seriesList[0].unit}\";");
                    sb.AppendLine($"DECIMALS={container.deci};");

                    sb.AppendLine("STUB=\"Category\";");
                    sb.AppendLine("HEADING=\"Month\";");

                    sb.AppendLine($"VALUES(\"Category\")={FormatList(categories)};");
                    sb.AppendLine($"VALUES(\"Month\")={FormatList(allMonths)};");

                    sb.AppendLine("DATA=");

                    foreach (var s in seriesList)
                    {
                        var dict = s.data.ToDictionary(
                            d => ToPxMonth(Convert.ToInt64(d[0])),
                            d => Convert.ToDouble(d[1])
                        );

                        foreach (var m in allMonths)
                        {
                            if (dict.ContainsKey(m))
                                sb.Append(dict[m].ToString("0.###") + " ");
                            else
                                sb.Append(". ");
                        }

                        sb.AppendLine();
                    }

                    sb.AppendLine(";");

                    string savedFile = SavePxFile(sb.ToString(), shortName);

                    if (string.IsNullOrEmpty(savedFile))
                        return;

                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = savedFile,
                            UseShellExecute = true
                        });
                    }
                    catch (Win32Exception)
                    {
                        MessageBox.Show("Could not find " + savedFile + " or browser", Globals.AppName);
                    }
                }
        //end utkommentering px web
        */










        //skal være igjem 2
    }

}
