using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;



namespace CSharp
{


    public partial class Form1 : Form
    {




        public static class Globals
        {
           

            //henter connection
           // public static String DataApi = "http:osv data api line2";
            
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

            //public static readonly String TotalStr = "  commented out th e avove 2712User:";




            public static String freq = "MONTHLY";
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

            //new for makeing sure intervals and drop downs are read
            dataGridView2.CurrentCellDirtyStateChanged += DataGridView2_CurrentCellDirtyStateChanged;

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);


            this.Text = Globals.AppName;  //setter navnet i appen windies


            //string mylsname = "'" + "All"+ "'"; //ny etter combp


            //MessageBox.Show(Application.StartupPath);



            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            Globals.Version = fvi.FileVersion;
            //MessageBox.Show(Globals.AppName + "debug \n" + Globals.Version, Globals.AppName);

            LogMessage();



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

                        /*
                        if (MyStatusFlag == 1) //warning
                        {
                            MessageBox.Show(mymessage, "Warning " + Globals.AppName + "\n" + Globals.Version);
                        }
                        else if (MyStatusFlag <= -1) //err
                        {
                            MessageBox.Show(mymessage, "Error " + Globals.AppName + "\n"+Globals.Version);
                            LogAndexit(); 
                        }
                      */  


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




            toolStripStatusLabel1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Curves Selected" + Globals.TotalStr;


            this.contextMenuStrip1.Items.Add("Apply Interval to All");
            this.contextMenuStrip1.Items.Add("Apply Function to All");
            this.contextMenuStrip1.Items.Add("Apply Type to All");
            this.contextMenuStrip1.Items.Add("Apply Stacking to All");

            //initierer grid1 med dummy colomn header for userfriendlyness

            DataTable Curve = new DataTable("Curve");
            DataColumn c0 = new DataColumn("CurveId");
            //DataColumn c0 = new DataColumn("Id");
            Curve.Columns.Add(c0);
            //close tbcCurve.Columns[0].Width = 60; //curveid

            DataColumn c1 = new DataColumn("CurveName");
            //DataColumn c1 = new DataColumn("Series");
            Curve.Columns.Add(c1);

            DataColumn c2 = new DataColumn("Descr");
            Curve.Columns.Add(c2);
            // definere all tbc

            DataColumn c3 = new DataColumn("SetId");
            Curve.Columns.Add(c3);
            DataColumn c4 = new DataColumn("Freq");
            Curve.Columns.Add(c4);
            DataColumn c5 = new DataColumn("Unit");
            Curve.Columns.Add(c5);

            DataColumn c6 = new DataColumn("Url");
            Curve.Columns.Add(c6);
            DataColumn c7 = new DataColumn("Source");
            Curve.Columns.Add(c7);
            DataColumn c8 = new DataColumn("Updated");
            Curve.Columns.Add(c8);
            DataColumn c9 = new DataColumn("NumObs");
            Curve.Columns.Add(c9);








            //if pct tbc
            //DataColumn c14 = new DataColumn("LastValue"); //vurdere genersik navn
            //Curve.Columns.Add(c14);

            dataGridView1.DataSource = Curve;
            dataGridView1.Columns["CurveName"].Width = 190;
            //dataGridView1.Columns["Series"].Width = 190;
            dataGridView1.Columns["Descr"].Width = 210;
            // definere default bredde

            //dataGridView1.Columns["LastValue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //ma evt definere flere. eller bruke samme kolonnenavn
           

            dataGridView1.CellFormatting += new DataGridViewCellFormattingEventHandler(dataGridView1_CellFormatting);
            //this stkker for å formattere
            //dataGridView1.Columns[2].Width = 180; //descr widt
            //dataGridView1.Columns["LastValue"].DataPropertyName = "LastValue";
            //dataGridView1.Columns["LastValue"].DefaultCellStyle.Format = "0.00##";




            // GRID 2  2 2 2 2 2 2  ///////
            dataGridView2.Columns.Add("CurveId", "CurveId");
            dataGridView2.Columns.Add("CurveName", "CurveName");


            //dataGridView2.Columns.Add("Id", "Id");
            //dataGridView2.Columns.Add("Series", "Series");





            dataGridView2.Columns["CurveId"].ReadOnly = true; //måtte flyye lenger ned...
            dataGridView2.Columns["CurveName"].ReadOnly = true; //måtte flyye lenger ned...

            //dataGridView2.Columns["Id"].ReadOnly = true; //måtte flyye lenger ned...
            //dataGridView2.Columns["Series"].ReadOnly = true; //måtte flyye lenger ned...

            dataGridView2.Columns.Add("Column", "Descr");
            dataGridView2.Columns[0].Width = 60; //curveid



            //left rigth combo box kolonne 3

            DataGridViewComboBoxColumn cmblr = new DataGridViewComboBoxColumn();
            cmblr.HeaderText = "L/R";
            cmblr.Name = "cmblr";
            cmblr.MaxDropDownItems = 4;
            cmblr.Width = 50;
            cmblr.Items.Add("Left");
            cmblr.Items.Add("Rigth");
            //cmblr.SortMode = DataGridViewColumnSortMode.NotSortable;
            // cmblr.SortMode = DataGridViewColumnSortMode.Programmatic;
            //cmblr.SortMode = DataGridViewColumnSortMode.Automatic; maa vaere etter add
            dataGridView2.Columns.Add(cmblr);
            cmblr.SortMode = DataGridViewColumnSortMode.Automatic;
            //dataGridView2.Columns.Add("Column", "L/R");
            //cmblr.SortMode = sortable;

            //Funksjon Drop Down kolonne 4

            DataGridViewComboBoxColumn combofunk = new DataGridViewComboBoxColumn();
            combofunk.DataPropertyName = "ID";
            combofunk.HeaderText = "Function";
            combofunk.Width = 80;


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




            DataGridViewComboBoxColumn cmbflag = new DataGridViewComboBoxColumn();
            cmbflag.HeaderText = "F(n)Lag";
            cmbflag.Name = "cmbflag";
            cmbflag.MaxDropDownItems = 4;
            cmbflag.Width = 50;
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
    // end function lag drop down












            // interval drop down i Grid2 /////////////////// kolonne 5

            DataGridViewComboBoxColumn comboint = new DataGridViewComboBoxColumn();
            comboint.DataPropertyName = "ID";
            comboint.HeaderText = "Interval";
            comboint.Width = 100;


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
                comboint.Items.Add(mydr["IName"].ToString());
                // MessageBox.Show("Erik Soeberg 2030", "Interval");

            }

            myconn.Close();
            comboint.ValueMember = "ID";
            comboint.DisplayMember = "Interval";
            dataGridView2.Columns.Add(comboint);
            comboint.SortMode = DataGridViewColumnSortMode.Automatic;
            //comboint.Items.Insert(0, "Select Item");
            //comboint.Selected = 0;
            //slutt INTERVAL


            //nye felter Aggregatetion
            
            DataGridViewComboBoxColumn fcastint = new DataGridViewComboBoxColumn();
            fcastint.DataPropertyName = "ID";
            fcastint.HeaderText = "Aggregate";
            fcastint.Width = 90;

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
            cmblinetype.Name = "linetype";
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


            //Stakking
            DataGridViewComboBoxColumn cmbstacking = new DataGridViewComboBoxColumn();
            cmbstacking.HeaderText = "Stacking";
            cmbstacking.Name = "linetype";
            cmbstacking.MaxDropDownItems = 4;
            cmbstacking.Items.Add("None");
            cmbstacking.Items.Add("normal");
            cmbstacking.Items.Add("percent");
            dataGridView2.Columns.Add(cmbstacking);
            cmbstacking.SortMode = DataGridViewColumnSortMode.Automatic;



            //Order
            DataGridViewComboBoxColumn cmborder = new DataGridViewComboBoxColumn();
            cmborder.HeaderText = "Order";
            cmborder.Name = "Orderclm";
            cmborder.MaxDropDownItems = 4;
            //readonly
            //cmborder.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //cmborder.ReadOnly = true;


            //  for (int i = 1; i <= 15; i++)
            //{
            //  cmborder.Items.Add(i); //integer best for sorting
            //cmborder.Items.Add(i.ToString());
            //}

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
            //cmborder.Items.Add("99");


            dataGridView2.Columns.Add(cmborder);
            cmborder.SortMode = DataGridViewColumnSortMode.Automatic;



            //forsøker å forse singleclick funker brs
            //dataGridView2.EditMode = DataGridViewEditMode.EditOnEnter;

            GetMenuesFromFile();  //oppfrisker knapper iht til default language ved oppstart


            //default freq og deci





            //ikke mulige å charte foer velging
            //chartvalgte_btn.Enabled = false;
            //gamle
            //AllinOneChart.Enabled = false;
            //MultiChart_btn.Enabled = false;

            //nye
            NyChart_btn.Enabled = false;
            NyMulti_btn.Enabled = false;
            YearChartRepo_btn.Enabled = false;
            reportToolStripMenuItem1.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;

            //3 nye
            chartToolStripMenuItem.Enabled = false;
            multiChartToolStripMenuItem.Enabled = false;
            yearChartToolStripMenuItem.Enabled = false;





            Clear2_Btn.Enabled = false;
            ClearOne2_btn.Enabled = false;   
            //AsyncAll_btn.Enabled = false;
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
                MessageBox.Show("Problem creating "+Globals.UserDir, Globals.AppName);
            }

            


            string myPName = "dummy";
            string myCName = "dummy";
            string myLSName = "dummy";


            string myPNameprev = "prev";
            string myCNameprev = "prev";
            string myLSNameprev = "prev";

            int i = 0; //-1 om ikke all er harrkodet foerst
            int j = -1;



            //henter skigdata lager trær
            SqlConnection myconn = new SqlConnection();
            myconn.ConnectionString = Globals.DBStr;
            SqlCommand mycommand = new SqlCommand();
            mycommand.Connection = myconn;
            //mycommand.CommandText = "select Epodateval from " + Fname + " ('" + CurveId + "','" + Iname + "' ) order by VDate ";

            //mycommand.CommandText = "select Parent, LSG2.Name PName, Child, LSG.Name CName, LS.Name LSName from LSHier LSH left join LSGroups LSG on LSG.GrpId = LSH.Child left join LSGroups LSG2 on LSG2.GrpId = LSH.Parent left join LoadSets LS on LS.LoadSetId = LSH.LoadSetid order by 1,2,3,4,5";
            //proc instead m access
            //mycommand.CommandText = " EXECUTE GetLoadSetTree " + Environment.UserName;  //winodws user name
            mycommand.CommandText = " EXECUTE Client_GetSetTree "; 


            myconn.Open();

            DataTable mydt = new DataTable();
            SqlDataReader reader = mycommand.ExecuteReader();


            //dummy ikke def i basen
            treeView1.Nodes.Add("All"); //denne er i=0

            if (reader.HasRows)
            {

                while (reader.Read())
                {


                    myPName = (reader.GetString(reader.GetOrdinal("PName"))); //parent exterm
                    myCName = (reader.GetString(reader.GetOrdinal("CName"))); //Cname ssb
                    myLSName = (reader.GetString(reader.GetOrdinal("LSName"))); //LS Fornavn

                    //MessageBox.Show("kjører: "+ i+ myPName + j+ myCName + myLSName);

                    if (myPName != myPNameprev)  //ekster / intern i
                    {
                        treeView1.Nodes.Add(myPName);  //adder extern etter All
                        i = i + 1;
                        //MessageBox.Show(" del 1 i= "+ i + " " + myPName);
                    }

                    if (myCName != myCNameprev)
                    {
                        j = j + 1;
                        //MessageBox.Show("adder del2 node Cname j "+ j +  myCName);
                        treeView1.Nodes[i].Nodes.Add(myCName);  //adder ssb under extern

                    }



                    if (myPName != myPNameprev) //ny parent adder childen
                    {
                        j = 0;
                        //MessageBox.Show(" Del 3 firste barn adder:" + i + " , j:0hc " +j+ myLSName);
                        treeView1.Nodes[i].Nodes[j].Nodes.Add(myLSName);  // allt
                    }

                    if (myCName != myCNameprev && myPName == myPNameprev) //ny cname
                    {
                        //MessageBox.Show(myLSName + " del 4 " + myLSName);
                        treeView1.Nodes[i].Nodes[j].Nodes.Add(myLSName);
                    }

                    //flyttet opp hit
                    else if (myCName == myCNameprev && myPName == myPNameprev) //begge like  bare adder
                    {
                        // MessageBox.Show(myPName+myCName+ " adder del5  i:" + i + " , j:" + j + myLSName);
                        treeView1.Nodes[i].Nodes[j].Nodes.Add(myLSName);

                    }

                    myPNameprev = myPName;
                    myCNameprev = myCName;
                    myLSNameprev = myLSName;



                }  //while
            } //if
            else

            //ingen rader på kurven do nothing+
            {
                MessageBox.Show("Missing loadset hierarcky", Globals.AppName);
            }
            reader.Close();
            myconn.Close();
            //dummies
          
            //expander hele treet
            treeView1.ExpandAll();
            //treeView1.Nodes[2].EnsureVisible();  //not robust




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

            //label1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Curves Selected" + Globals.TotalStr;

            toolStripProgressBar1.Value = 30;
            toolStripStatusLabel1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Curves Selected" + Globals.TotalStr;
            toolStripProgressBar1.Value = 0;

        }

        private void selectedcurves_btn_Click(object sender, EventArgs e)
        {
        //kan slette denne om en bare bruker doubleclick
            string MyCurveName = "";
            string MyCurveId = "";
            string MyCurveDescr = "";
            string MyInterval = "";


            // foreach (DataGridViewRow row in dataGridView1.Rows)


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

                dataGridView2.Rows[n].Cells[6].Value = MyInterval;


                //label1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Curves Selected"  + Globals.TotalStr;

                toolStripStatusLabel1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Curves Selected" + Globals.TotalStr;

          
            }

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {


            DateTime buildDate = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);

           

            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;  //show path
            var year = DateTime.Now.Year;
            //MessageBox.Show("Ver: "+Globals.Version + " "+Globals.AppName+" "+year+"\n"+"Env: " +Globals.AppDir + "\n" + "Usr: " + Globals.UserDir + "\n"  +"Exe: " + System.IO.Path.GetDirectoryName(strExeFilePath), Globals.AppName);
            //  MessageBox.Show("Ver: " + Globals.Version + " " + Globals.AppName + " " + year + "\n" + "Usr: " + Globals.UserDir + "\n" + "Exe: " + System.IO.Path.GetDirectoryName(strExeFilePath), Globals.AppName + "\n\n\n" + "By : Eriksberg");


            //= $"© {DateTime.Now.Year} Erik Søberg";
/*
            MessageBox.Show(
    string.Format(
        "AppVer: {1}\n DBVer: {2} \n {3}\nUsr: {4}\nExe: {5}\n\nBy: ©Eriksberg\n\nBuild date: {6:yyyy-MM-dd HH:mm}",
        Globals.Version,
        Globals.DbVer,
        Globals.AppName,
        year,
        Globals.UserDir,
        Path.GetDirectoryName(strExeFilePath),
        buildDate
    ),
    Globals.AppName
);
*/


            MessageBox.Show(
$@"{Globals.AppName}

App version : {Globals.Version}
DB version  : {Globals.DbVer}
Year        : {year}
User dir    : {Globals.UserDir}
Executable  : {Path.GetDirectoryName(strExeFilePath)}
Connection  : {Path.GetDirectoryName(strExeFilePath)}

© Eriksberg

Build date  : {buildDate:yyyy-MM-dd HH:mm}",
Globals.AppName
);








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
            //label1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Curves Selected";
            toolStripStatusLabel1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Curves Selected" + Globals.TotalStr;
        }



        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            

            if (e.ColumnIndex == 3 && e.Value == null)    // leftRigth
            {
                e.Value = "Left";

            }

            if (e.ColumnIndex == 4 && e.Value == null)    // funksjon
            {
                e.Value = "None";

            }



            if (e.ColumnIndex == 5 && e.Value == null)    // Function Lag
            {
                e.Value = "1";
            }



            if (e.ColumnIndex == 6  && e.Value == null)    // interval  //koper default value kommer ikke hit lenger
            {
               e.Value = "AllData";
            }


            if (e.ColumnIndex == 7 && e.Value == null)    // Aggregate
            {
                e.Value = "None"; //fefault None
   
            }



            if (e.ColumnIndex == 8 && e.Value == null)    // charttype
            {
                e.Value = "spline";

            }
            if (e.ColumnIndex == 9 && e.Value == null)    // stacking
            {
                e.Value = "None";

            }

          //  if (e.ColumnIndex == 10 && e.Value == null)    // order
            //{
              //  e.Value = "9";

            //}





        }

       


        private void ClearOne2_btn_Click(object sender, EventArgs e)
        {
            //comboBox1.Text = "%"; es feb
            int rowIndex = dataGridView2.CurrentCell.RowIndex;
            dataGridView2.Rows.RemoveAt(rowIndex);
            //label1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Curves Selected";
            toolStripStatusLabel1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Curves Selected" + Globals.TotalStr;

        }

        private void dataGridView2_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                // MessageBox.Show("yea ore than one value selected");

                // chartvalgte_btn.Enabled = true;
                //gml
                //AllinOneChart.Enabled = true;
                // Map_btn.Enabled = false;  //tbc
                //MultiChart_btn.Enabled = true;

                //auhust 9
                NyChart_btn.Enabled = true;
                NyMulti_btn.Enabled = true;
                YearChartRepo_btn.Enabled = true;
                reportToolStripMenuItem1.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;

                chartToolStripMenuItem.Enabled = true;
                multiChartToolStripMenuItem.Enabled = true;
                yearChartToolStripMenuItem.Enabled = true;


                Clear2_Btn.Enabled = true;
                ClearOne2_btn.Enabled = true;


                //Nowcast_btn.Enabled = false;  //btn funker den bruker function TBC 
                //AsyncAll_btn.Enabled = true;
            }
            else
            {
                //chartvalgte_btn.Enabled = false;
                //AllinOneChart.Enabled = false;
               // Map_btn.Enabled = false; // tatt bort erik feb 22
                //MultiChart_btn.Enabled = false;


                NyChart_btn.Enabled = false;
                NyMulti_btn.Enabled = false;
                YearChartRepo_btn.Enabled = false;
                reportToolStripMenuItem1.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;

                chartToolStripMenuItem.Enabled = false;
                multiChartToolStripMenuItem.Enabled = false;
                yearChartToolStripMenuItem.Enabled = false;


                Clear2_Btn.Enabled = false;
                ClearOne2_btn.Enabled = false;
               // Nowcast_btn.Enabled = false;
               // AsyncAll_btn.Enabled = false;
            }

            //if (Globals.OnPrem ==1)
            // {
            //  AsyncAll_btn.Visible = false;
            // }

        }








        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {



            //selectedcurves_btn_Click();
            //MessageBox.Show("Doubleclicked !" , "C#harts");
            // DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex]; //Get selected Row


            string MyCurveName = "";
            string MyCurveId = "";
            string MyCurveDescr = "";
            string MyInterval = "";


            // foreach (DataGridViewRow row in dataGridView1.Rows)

            dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
            dataGridView2.EndEdit();

            foreach (DataGridViewRow row in dataGridView1.SelectedRows)


            {
                //currQty += row.Cells["qty"].Value;
                MyCurveId = row.Cells["CurveId"].Value.ToString();
                MyCurveName = row.Cells["CurveName"].Value.ToString();

                //MyCurveId = row.Cells["Id"].Value.ToString();
                //MyCurveName = row.Cells["Series"].Value.ToString();
                //erik


                MyCurveDescr = row.Cells["Descr"].Value.ToString();
                MyInterval = row.Cells["Interval"].Value.ToString();
                //MessageBox.Show(MyCurveName, "C#harts");


                //kopierer over
                int n = dataGridView2.Rows.Add();
                dataGridView2.Rows[n].Cells[0].Value = MyCurveId; 
                dataGridView2.Rows[n].Cells[1].Value = MyCurveName;
                dataGridView2.Rows[n].Cells[2].Value = MyCurveDescr;
                dataGridView2.Rows[n].Cells[6].Value = MyInterval;


                //dataGridView2.Rows[n].Cells[10].Value = dataGridView2.Rows.Count;


               int rowCount =    dataGridView2.Rows.Count -    (dataGridView2.AllowUserToAddRows ? 1 : 0);
               dataGridView2.Rows[n].Cells[10].Value =    Math.Min(rowCount, 9).ToString();


               // dataGridView2.Rows[n].Cells[10].Value = Math.Min(dataGridView2.Rows.Count, 199);

                // combobox has 200 values, after that u just get 199..


                //  label1.Text = dataGridView1.SelectedRows.Count.ToString() + " / "+ dataGridView1.Rows.Count.ToString() + "/ 20 392 Kurver" ;
                //label1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() ;
                //label1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Curves Selected";
                toolStripStatusLabel1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Curves Selected" + Globals.TotalStr;
                //More code here
            }







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

            string MyType = "spline";
            string MyStacking = "None";

            int rowIndex = dataGridView2.CurrentCell.RowIndex;

            //intervallet
            if (dataGridView2.Rows[rowIndex].Cells[6].Value == null)    // interval blitt ny nr 6 feb 10
            {
                MyInterval = "All Data";
            }
            else
            {
                MyInterval = dataGridView2.Rows[rowIndex].Cells[6].Value.ToString();
            }

            if (e.ClickedItem.Text == "Apply Interval to All")

            {
                //MessageBox.Show(MyInterval + " :loop " + e.ClickedItem.Text);


                for (int i = 0; i < dataGridView2.Rows.Count; i += 1)

                {
                    //setter over
                    dataGridView2.Rows[i].Cells[6].Value = MyInterval;

                }
            }





            //funksjon
            if (dataGridView2.Rows[rowIndex].Cells[4].Value == null)    // funk fremdeles 4
            {
                MyFunction = "None";
            }
            else
            {
                MyFunction = dataGridView2.Rows[rowIndex].Cells[4].Value.ToString();
            }

            if (e.ClickedItem.Text == "Apply Function to All")
            {
                //MessageBox.Show(MyFunction + " :loope" + e.ClickedItem.Text);


                for (int i = 0; i < dataGridView2.Rows.Count; i += 1)

                {
                    //setter over
                    dataGridView2.Rows[i].Cells[4].Value = MyFunction;

                }



            }  //slutt if FUNCTION



            //stack start
            //stack
            else if (dataGridView2.Rows[rowIndex].Cells[9].Value == null)    // stack nr 9
            {
                MyStacking = "None";
            }
            else
            {
                MyStacking = dataGridView2.Rows[rowIndex].Cells[9].Value.ToString();
            }
            if (e.ClickedItem.Text == "Apply Stacking to All")
            {
                for (int i = 0; i < dataGridView2.Rows.Count; i += 1)
                {
                    //setter over
                    dataGridView2.Rows[i].Cells[9].Value = MyStacking;
                }
            }  //slutt if stack



            //type start
            //type 88
            if (dataGridView2.Rows[rowIndex].Cells[8].Value == null)    // type nr 8
            {
                MyType = "spline";
            }
            else
            {
                MyType = dataGridView2.Rows[rowIndex].Cells[8].Value.ToString();
            }
            if (e.ClickedItem.Text == "Apply Type to All")
            {
                for (int i = 0; i < dataGridView2.Rows.Count; i += 1)
                {
                    dataGridView2.Rows[i].Cells[8].Value = MyType;
                }
            }  //slutt if stack



            //stack



            //else
            //{ MessageBox.Show(" Do Notting");
            //}


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

      //  private void gkBuyersToolStripMenuItem_Click(object sender, EventArgs e)
       // {
            //starter appen
            //Process.Start("J:\\GiskartK\\MyCharts\\ERIK\\admin.exe");  to be cont
        //}

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

            //making sure no more edit in the grid
            dataGridView2.EndEdit();

            //  MessageBox.Show("Installer nyknapp", Globals.AppName);


            //her eller overst
            string Fname = "";
            string Agg = "None";
            string AggDesc = "";

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

            string stacking = "";
            string myorder = "0";
            string linetype = "";
            int NumOfCurves = dataGridView2.Rows.Count;
            int NumOfCurvesPct = 100 / NumOfCurves;
            //int NumOfCurvesCur = 0;
            //int InnerSizePie = 10; // for pie starter på 10 og adder 10 ..max 10 stk tbc



            // var btn = sender as Button;
            // if (btn == null) return;



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

            // Use htmlfileName common directory
            //MessageBox.Show("Selected file: " + htmlfileName);




           var mfile = Globals.UserDir + "Mydata.js";
           string isoUtcDateTime = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss");





            //var mfile = Globals.UserDir + (string)NyChart_btn.Tag;


            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(mfile, false))
            {

                file.WriteLine("seriesData = [");
                file.WriteLine("{mytitle : 'Harmonize'  ," );
                file.WriteLine("mysubtitle : '',  ");
                file.WriteLine("freq : '"+Globals.freq+ "',");
                file.WriteLine("decimal : "+ Globals.deci.ToString() +"," );

                file.WriteLine("executed_UTC : '" + isoUtcDateTime + "',");

                //file.WriteLine("mytimezone : ''  ,");               

                if (Globals.BaseMnds >= 1900)
                {
                    file.WriteLine("mybaseperiod : " + Globals.BaseMnds.ToString() + ",");

                }
                else
                {
                    file.WriteLine("mybaseperiod : '',");

                }




                file.WriteLine("series: [");


                CurveId = "";
                CurveDescr = "";
                Fname = "";
                Agg = "None";
                AggDesc = "";
                FnameReplaced = "";
                Iname = "";
                LRName = "0";
                SerieName = "";
                CurveName = "";
                //Forecast = "0";
                FnLag = "1";
                //DynDateDesc = "";
                stacking =  "";
                myorder = "0";
                linetype = "";
                cnt = 0;
                myline = "";


                //MessageBox.Show(dataGridView2.Rows.Count.ToString());  //TBC remove

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    toolStripProgressBar1.Value = 20;

                    CurveId = (string)row.Cells[0].Value.ToString();
                    CurveDescr = (string)row.Cells[2].Value.ToString();


                    CurveName = (string)row.Cells[1].Value.ToString();  //arab ikke quote ..arab






                    //inn med   Aggregate first as it is used in func as well
                    if ((row.Cells[7].Value == null) || (row.Cells[7].Value.ToString() == ""))
                    {
                        //Forecast = "None";   // default Last Forecast
                        Agg = "None";   // default Last Forecast
                        AggDesc = "";
                    }
                    else
                    {
                        //Forecast = (string)row.Cells[7].Value.ToString();
                        Agg = (string)row.Cells[7].Value.ToString();
                        AggDesc = Agg;
                        //CurveDescr = CurveDescr + " forcast as of ";  // kanvære fcast uten funk

                    }



                    //inn med ny Function lag
                    if ((row.Cells[5].Value == null) || (row.Cells[5].Value.ToString() == "1"))
                    {
                        FnLag = "1";   // default 1 lag
                    }
                    else
                    {
                        FnLag = (string)row.Cells[5].Value.ToString();

                    }





                    //funksjon
                    if ((row.Cells[4].Value == null) || (row.Cells[4].Value.ToString() == "None"))  //ingen valgt FUNCTION
                    {

                        //Fname = "StpGetCurveDataOuter2_ut";   //kaller igjen på inner , sjd be config
                        Fname = "None";  //bruker get series
                                         // SerieName = AggDesc + " " + CurveDescr;
                        SerieName = CurveDescr;
                        FnameReplaced = "";
                        //DEFAULT DO NOTTING no funk
                    }

                    else
                    {
                        Fname = (string)row.Cells[4].Value.ToString();
                        

                        FnameReplaced = Fname.Replace("n)", FnLag + ")");  //pynter på navnet til funk med lag 123
                        FnameReplaced = Regex.Replace(FnameReplaced, @"[\[\]\(\)]", "");
                        SerieName = CurveDescr;

                        // SerieName = FnameReplaced + AggDesc + '(' + CurveDescr + ')';
                        //Fname = "[Pct(n)]";   denne er ok space i slutten ?
                        //PUTTER inn funk fra table navn paa funk maa finnes
                     

                    }

                    if ((row.Cells[3].Value == null) || (row.Cells[3].Value.ToString() == "Left"))
                    {
                        LRName = "0";   // default left rigjt 
                    }
                    else
                    {
                        LRName = "1"; // alrigth

                    }







                    //INTERVAL byttet til 6
                    if ((row.Cells[6].Value == null) || (row.Cells[6].Value.ToString() == "All Data"))
                    {
                        Iname = "AllData";   // INTERVAL
                    }
                    else
                    {
                        Iname = (string)row.Cells[6].Value.ToString();

                    }







                    //endret ti 8
                    if ((row.Cells[8].Value == null) || (row.Cells[8].Value.ToString() == "spline"))
                    {
                        linetype = "'spline'";   // line type
                    }
                    else
                    {
                        linetype = "'" + (string)row.Cells[8].Value.ToString() + "'";   //adde char39

                    }

                    //endrer til 9 2 opp
                    if ((row.Cells[9].Value == null) || (row.Cells[9].Value.ToString() == "None"))
                    {
                        stacking = "null";   // stacking null i highsoft
                    }
                    else
                    {
                        stacking = "'" + (string)row.Cells[9].Value.ToString() + "'";  //adee char 39

                    }


                    //order
                    if ((row.Cells[10].Value == null) || (row.Cells[10].Value.ToString() == "None"))
                    {
                        myorder = "9";   // stacking null i highsoft
                    }
                    else
                    {
                        myorder = row.Cells[10].Value.ToString();

                    }







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
                        sql_cmnd.Parameters.AddWithValue("@top", SqlDbType.Int).Value = Globals.MaxNum;  //from connection file
                        sql_cmnd.Parameters.AddWithValue("@sort", SqlDbType.NVarChar).Value = "asc";
                        sql_cmnd.Parameters.AddWithValue("@json", SqlDbType.NVarChar).Value = DBNull.Value;



                        toolStripStatusLabel1.Text = "StpGetSeries " + CurveName + "," + Globals.BaseMnds.ToString() + ", " + Iname + ", " + Fname + ", " + FnLag + ", " + Agg + ", "+Globals.MaxNum + ", " + "asc"; //det over i totalstr og vise til slutt

                        //toolStripStatusLabel1.Text = mycommand.CommandText; //trenger bare å vise siste evt hente det over i totalstr og vise til slutt


                        //MessageBox.Show("" + Globals.BaseMnds);

                        DataTable mydt = new DataTable();
                        SqlDataReader reader = sql_cmnd.ExecuteReader();

                        cnt = 0;
                        if (reader.HasRows)
                        {
                            // DynDateDesc = too early

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
                                        MessageBox.Show("Error reading "+CurveName +" not found'.\n" + ex.Message,
                                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return; // exit the method if needed
                                    }














                                    // ok fra her
                                    if (myline.IndexOf('{') >= 0)  //txt curve
                                    {
                                        //MessageBox.Show("txt curve");


                                        file.WriteLine("keys: ['t'], data :  [");
                                    }
                                    else
                                    {
                                        file.WriteLine("data :  [");  //vanlig kurve numeric

                                    }

                                }

                                //MessageBox.Show("2 kommer hit ??");

                                toolStripProgressBar1.Value = 70;
                                //file.WriteLine(reader.GetString(reader.GetOrdinal("Epodateval")) + ",");
                                //funker med både value og string
                                file.WriteLine(reader.GetValue(reader.GetOrdinal("Epodateval")) + ",");

                                toolStripProgressBar1.Value = 90;
                                //DynDateDesc = reader.GetDateTime(0).ToString("yyyy.MM.dd HH");  // må passe på å ha fdate i vdate for desc kolonne 0 er Vdate
                                //DynDateDesc = reader.IsDBNull(0) ? null : reader.GetDateTime(0).ToString("yyyy.MM.dd HH");



                            }

                        } //if
                        else

                        //ingen rader på kurven do nothing+
                        {
                            //file.WriteLine("data :  [");  //legger på datatomrad
                            file.WriteLine("data: [] }]}]");  //legger på datatomrad og lukker not needed if u never het to here..
                            toolStripProgressBar1.Value = 0;


                            MessageBox.Show("Curve : " + CurveId + " No rows in selected Interval or Access Denied", Globals.AppName);



                            // Exit the current method immediately
                            return;

                        }



                        // try}

                        //catch (Exception)
                        //{
                        //  MessageBox.Show("Nope ");\
                        //}



                        //sqlCon.Close();

                    } //use flyttet hit nedover

                    sqlCon.Close();
                    //reader.Close();




                   // was  file.WriteLine("].sort((a, b) => a[0] - b[0]),     ");

                    file.WriteLine("], "); //closing data new

                    file.WriteLine("name: '" + CurveName + "',");
                    file.WriteLine("type: " + linetype + ",");
                    file.WriteLine("stacking : " + stacking + ",");


                    file.WriteLine("yAxis : " + LRName + ",");


                    file.WriteLine("function:'" + FnameReplaced + "', ");


                    // 1. Extract epoch part  -----------------------creTING A  label for owerlay part
                    string epochPart = myline
                        .Trim('[', ']')
                        .Split(',')[0];

                    // 2. Convert to long
                    long epochMillis = long.Parse(epochPart, CultureInfo.InvariantCulture);

                    // 3. Convert epoch → DateTime (UTC)
                    DateTime date = DateTimeOffset
                        .FromUnixTimeMilliseconds(epochMillis)
                        .UtcDateTime;

                    // 4. Get year
                    string labelYear = date.Year.ToString();



                    file.WriteLine("labelyear: '" + labelYear + "', ");
                    file.WriteLine("order: " + myorder + ", ");

                    file.WriteLine("interval :'" + Iname + "', ");
                    file.WriteLine("aggregation: '" + AggDesc + "', ");
                    file.WriteLine("desc: '" + SerieName.Trim() + "', " + "              }, ");  //lukker curvenn




                   // file.WriteLine("desc: \"" + DynDateDesc + " " + SerieName.Trim() + " \", " + "              }, ");  //lukker curvenny  " //lukker curven org før forecastmay22







                }
                //for each



                //avslutter filen
                file.WriteLine("]  //eo series new");



                file.WriteLine("   }  ] ");

             

                toolStripProgressBar1.Value = 40;


                //Fname = (string)row.Cells[2].Value.ToString();
                // MessageBox.Show(CurveId + ":" + Fname, "C#harts");



            }



            //sjåw the  filr

            try
            {

                string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;  //show path of exe
                var myfiletoshow = System.IO.Path.GetDirectoryName(strExeFilePath) + "\\"+ htmlfileName;

                //System.Diagnostics.Process.Start("chrome.exe", "file:///" + myfiletoshow);

               // System.Diagnostics.Process.Start(      "chrome.exe",                 $"\"{myfiletoshow}\""                    );

                //default browser
                Process.Start(new ProcessStartInfo
                {
                    FileName = myfiletoshow,
                    UseShellExecute = true
                });





                // ok System.Diagnostics.Process.Start("chrome.exe", "file:///" + Globals.UserDir + htmlfileName);


            }
            catch (Win32Exception)
            {
                MessageBox.Show("Could not find html file or browser", Globals.AppName);
            }


            toolStripProgressBar1.Value = 50;
            toolStripProgressBar1.Value = 0;
        }



//end of high independent














        //        private void test_btn_Click(object sender, EventArgs e)
        //      {
        // [your_row].Cells[col].Style.BackColor = Color.Red;
        //    }





        private string GetMenuesFromFile()
        {
            string basepath = Application.StartupPath;
            string txtpath = basepath + @"/Menues.txt";  //filename
            //string connectionstring = ""; // 
            const Int32 BufferSize = 128;

            if (File.Exists(txtpath))
            {
                using (var fileStream = File.OpenRead(txtpath))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                {

                    
                    //dropper headeline tar denne fra connection.txt
                    //String line = streamReader.ReadLine();  //header tbcontinued
                                                     //  MessageBox.Show("header " + line.ToString());


                    
                    //string[] words = line.Split(';');
                    //List<ComboboxItem> list = new List<ComboboxItem>();
                    //ComboboxItem item = new ComboboxItem();
                    
                    //bygges opp først
                    //LangBox1.Items.Add(words[Globals.LangColumn]);


                    try //om antall kolonner er odde eller posisjon ikke passer
                    {


            

                        //while ((line = streamReader.ReadLine()) != null)
                        //{





                        String line = streamReader.ReadLine();
                        String[]words = line.Split(';');
                        //MessageBox.Show(words[0]);
                        // words[0].Text = words[1];
                        //  this.Controls.Find(words[0], true).FirstOrDefault().Text = words[2];

                        //sjd work but no App header name
                        this.Text = words[Globals.LangColumn];
                        Globals.AppName = words[Globals.LangColumn];



                        line = streamReader.ReadLine(); words = line.Split(';');
                        NyChart_btn.Text = words[Globals.LangColumn];
                        chartToolStripMenuItem.Text = words[Globals.LangColumn];

                        line = streamReader.ReadLine(); words = line.Split(';');
                        NyMulti_btn.Text = words[Globals.LangColumn];
                        multiChartToolStripMenuItem.Text = words[Globals.LangColumn];

                        //adding dec25 btn and menue same text
                        line = streamReader.ReadLine(); words = line.Split(';');
                        YearChartRepo_btn.Text = words[Globals.LangColumn];
                      




                        line = streamReader.ReadLine(); words = line.Split(';');
                        ClearOne2_btn.Text = words[Globals.LangColumn];

                        line = streamReader.ReadLine(); words = line.Split(';');
                        Clear2_Btn.Text = words[Globals.LangColumn];


                        line = streamReader.ReadLine(); words = line.Split(';'); //wildcatd
                        Wild_label3.Text = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        Substr_label4.Text = words[Globals.LangColumn];

                        //line = streamReader.ReadLine(); words = line.Split(';');         
                        //AllinOneChart.Text = words[Globals.LangColumn];



                        //line = streamReader.ReadLine(); words = line.Split(';');
                        //connect_btn.Text = words[Globals.LangColumn];

                        line = streamReader.ReadLine(); words = line.Split(';');
                        Search_btn.Text = words[Globals.LangColumn];

                      


                        line = streamReader.ReadLine(); words = line.Split(';');
                        fileToolStripMenuItem.Text = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');

                        //save new
                        saveAsToolStripMenuItem.Text = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        //retirve
                        retriveToolStripMenuItem.Text = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');

                        //fortsetther jan 2026




                        exitToolStripMenuItem.Text = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');



                        graphToolStripMenuItem.Text = words[Globals.LangColumn]; //menue
                        line = streamReader.ReadLine(); words = line.Split(';');
                        reportToolStripMenuItem.Text = words[Globals.LangColumn];
                        //repoToolStripMenuItem


                        line = streamReader.ReadLine(); words = line.Split(';');
                        OptionToolStripMenuItem.Text = words[Globals.LangColumn];
                        //undermenyeun på report:
                        //flyttet
                        line = streamReader.ReadLine(); words = line.Split(';');
                        reportToolStripMenuItem1.Text = words[Globals.LangColumn];
                        




                        line = streamReader.ReadLine(); words = line.Split(';');
                        parserStatsToolStripMenuItem.Text = words[Globals.LangColumn]; //admin
                        line = streamReader.ReadLine(); words = line.Split(';');
                        tableMgrToolStripMenuItem.Text = words[Globals.LangColumn]; //tablemgr




                        line = streamReader.ReadLine(); words = line.Split(';');
                        helpToolStripMenuItem.Text = words[Globals.LangColumn]; //help

                        //datetext
                      //  line = streamReader.ReadLine(); words = line.Split(';');
                     //   Date_label.Text = words[Globals.LangColumn]; //help
                                                                     //datetext
                        line = streamReader.ReadLine(); words = line.Split(';');
                        Base_Label.Text = words[Globals.LangColumn]; //base





                        //curve datagrid
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView1.Columns[0].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView1.Columns[1].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView1.Columns[2].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView1.Columns[3].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView1.Columns[4].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView1.Columns[5].HeaderText = words[Globals.LangColumn];


                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView1.Columns[6].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView1.Columns[7].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView1.Columns[8].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView1.Columns[9].HeaderText = words[Globals.LangColumn];








                        //Valgte curve datagrid2
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView2.Columns[0].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView2.Columns[1].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView2.Columns[2].HeaderText = words[Globals.LangColumn];
                        
                        //:axis:
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView2.Columns[3].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView2.Columns[4].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView2.Columns[5].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView2.Columns[6].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView2.Columns[7].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView2.Columns[8].HeaderText = words[Globals.LangColumn];
                        line = streamReader.ReadLine(); words = line.Split(';');
                        dataGridView2.Columns[9].HeaderText = words[Globals.LangColumn];



                        line = streamReader.ReadLine(); words = line.Split(';');
                        yearChartToolStripMenuItem.Text = words[Globals.LangColumn];



                        //DragDropCurveName = words[Globals.LangColumn];



                        /*funker for knapper og labels
                            var result = Controls.Find(words[0], true);
                        if (result == null || result.Length == 0)
                        {
                            // fail to find
                            MessageBox.Show(" Menues.txt does not exist: " + words[0]);
                        }

                        else { 
                            this.Controls.Find(words[0], true).FirstOrDefault().Text = words[Globals.LangColumn];
                            }
                        knapper og labels
                        */



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
                MessageBox.Show("Menues.txt file missing in: " + basepath, Globals.AppName);
                return ("error");
            }
        }

        private void LangBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("changing value : "+ LangBox1.Text + " index: "+ LangBox1.SelectedIndex.ToString())
            Globals.LangColumn = LangBox1.SelectedIndex;
            GetMenuesFromFile();
        }





        private void Search_btn_Click(object sender, EventArgs e)

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



            // MessageBox.Show(command.CommandText, "C#hartssearch ");

            toolStripProgressBar1.Value = 80;
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(sql_cmnd);
            adapter.Fill(data);
            toolStripProgressBar1.Value = 50;


            dataGridView1.DataSource = data;
            sqlCon.Close();
            } //use


            //sizes here ?
            dataGridView1.Columns[0].Width = 60; //curveid
            dataGridView1.Columns[4].Width = 50; //freq

            //blir extra TBC
            //dataGridView1.Columns[0].Name = "CurveId";
            //dataGridView1.Columns[0].HeaderText = "CurveId";
            //label1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Curves Selected" + Globals.TotalStr;

            toolStripProgressBar1.Value = 30;
            toolStripStatusLabel1.Text = dataGridView2.Rows.Count.ToString() + " of " + dataGridView1.Rows.Count.ToString() + " Curves Selected" + Globals.TotalStr;
            toolStripProgressBar1.Value = 0;



        }

       
        private void releasesToolStripMenuItem_Click(object sender, EventArgs e)
        {    
            /*
            try
            {

                System.Diagnostics.Process.Start("chrome.exe", "file:///" + Application.StartupPath + "/Releases.html");

            }
            catch (Win32Exception)
            {
                MessageBox.Show("Install Chrome / look for Releases.html", Globals.AppName);
            }
            */



            try
            {

                string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;  //show path of exe
                var myfiletoshow = System.IO.Path.GetDirectoryName(strExeFilePath) + "\\" + "Releases.html";


                Process.Start(new ProcessStartInfo
                {
                    FileName = myfiletoshow,
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
                sfd.FileName = "exportchart.csv";
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
            string delimiter = ";";

            // Header
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                sb.Append(dgv.Columns[i].HeaderText);
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
                    //string value = row.Cells[i].Value?.ToString() ?? "";
                    string value = row.Cells[i].FormattedValue?.ToString() ?? "";

                    // Escape quotes
                    value = value.Replace("\"", "\"\"");

                    // Quote if needed
                    if (value.Contains(delimiter) || value.Contains("\n") || value.Contains("\""))
                        value = $"\"{value}\"";

                    sb.Append(value);

                    if (i < dgv.Columns.Count - 1)
                        sb.Append(delimiter);
                }
                sb.AppendLine();
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

// retrieve part od datagrid 2

        private void retriveToolStripMenuItem_Clickv1(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV files (*.csv)|*.csv";
                ofd.Title = "Open CSV file";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    ImportCsvToDataGridView(dataGridView2, ofd.FileName);
                }
            }

        }


        private void retriveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV files (*.csv)|*.csv";
                ofd.Title = "Open CSV file";

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

                    string[] headers = ParseCsvLine(headerLine, ';');

                    if (headers.Length == 0)
                        throw new Exception("CSV header could not be parsed.");

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

                        string[] values = ParseCsvLine(line, ';');

                        if (values.Length == 0)
                            throw new Exception("Invalid CSV row detected.");

                        int rowIndex = dgv.Rows.Add();

                        for (int i = 0; i < dgv.Columns.Count && i < values.Length; i++)
                        {
                            dgv.Rows[rowIndex].Cells[i].Value = values[i];
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






        private void ImportCsvToDataGridViewv1(DataGridView dgv, string filePath)
        {
            dgv.Rows.Clear();

            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                // Read header
                string headerLine = reader.ReadLine();
                if (headerLine == null) return;

                string[] headers = ParseCsvLine(headerLine, ';');

                // Optional: auto-create columns if empty
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

                    string[] values = ParseCsvLine(line, ';');

                    int rowIndex = dgv.Rows.Add();

                    for (int i = 0; i < dgv.Columns.Count && i < values.Length; i++)
                    {
                        dgv.Rows[rowIndex].Cells[i].Value = values[i];
                    }
                }
            }
        }


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






        private string[] ParseCsvLinev1(string line, char delimiter)
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
                        // Escaped quote
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














    }
}

