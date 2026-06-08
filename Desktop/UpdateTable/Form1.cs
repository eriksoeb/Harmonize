using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static UpdateTable.Form1;


namespace UpdateTable
{
    public partial class Form1 : Form
    {



        public static class Globals
        {
            public static String AppName = "Up2date Mgr";

            public static String Version = "Version";

            //public static int OnPrem = 0;
            public static String DBStr = GetgonnectionString();
       


            //før verdier
            public static String OldId1 = "";
            public static String OldId2 = "";
            public static String OldValue1 = "";
            public static String OldValue2 = "";


            //temp test dragdrop
            public static String DragDropCurveId = "";





            //numersike ider only PT DEFAULTs table må defineres i tabell pid1 pd2 osv tbc
            public static String Tablename = " Loadset ";   //Table default
            public static String Orderby = "' 1 desc '";   //order i select Table
            public static int Topp = 500;   //top
            public static String Pid1 = "Id";   //default name of key1
            public static String Pid2 = "NULL";   //name of key2 om det finnes
            //tbc
            public static String Pid3 = "NULL";   //name of key3 om det finnes


            public static String NodeText = "";   //yy Table Read




            //public static String OldDimId = "";
        }






        public Form1()
        {
            InitializeComponent();
            //MessageBox.Show(Globals.DBStr);
            toolStripStatusLabel1.Text = "User: " + Environment.UserName + " : " + Globals.DBStr;

            this.Text = Globals.AppName;  //setter navnet i appen windies


           


            //ingen database connection
            if (Globals.DBStr == "error")
            {
                MessageBox.Show("Cannot find database connection str", Globals.AppName);
                Environment.Exit(0);

            }



      

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            Globals.Version = fvi.FileVersion;
            //MessageBox.Show(Globals.Version);

            SqlConnection myconnbase = new SqlConnection();
            myconnbase.ConnectionString = Globals.DBStr;
            SqlCommand mycommando = new SqlCommand();
            mycommando.Connection = myconnbase;


            try
            {

                mycommando.CommandText = "execute Client_Version2 2, '" + Globals.Version + "'," + Environment.UserName; //Appid 2
                //MessageBox.Show(mycommando.CommandText);

                myconnbase.Open();
                //DataTable mydtable = new DataTable(); not needed
                SqlDataReader reader = mycommando.ExecuteReader();
                int MyStatusFlag = 0;
                String mymessage = ""; //warnig from version
                //String myenvironment = "";
                String myenvcolor = "";
                //Globals.OnPrem = 0;

                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        // MessageBox.Show(reader.GetString(reader.GetOrdinal("StatusMsg")) );
                        Globals.AppName = reader.GetString(reader.GetOrdinal("DispName"));
                        this.Text = Globals.AppName;
                        MyStatusFlag = reader.GetInt32(reader.GetOrdinal("StatusFlag"));
                        //Globals.OnPrem = reader.GetInt32(reader.GetOrdinal("OnPrem"));
                        mymessage = reader.GetString(reader.GetOrdinal("StatusMsg"));
                        //myenvironment = reader.GetString(reader.GetOrdinal("env"));

                        myenvcolor = reader.GetString(reader.GetOrdinal("EnvColor"));

                        //MessageBox.Show(myenvcolor);

                        if (MyStatusFlag == 1) //warning
                        {
                            MessageBox.Show(mymessage, "Warning " + Globals.AppName);
                        }
                        else if (MyStatusFlag <= -1) //err
                        {
                            MessageBox.Show(mymessage, "Error " + Globals.AppName);
                            // LogAndexit();
                        }
                    }


                }

                reader.Close();

                //string scolour = myenvcolor.ToString();
                //MessageBox.Show(myenvcolor);

                //Color colour = Color.FromArgb(Convert.ToInt32(myenvcolor));
                panel1.BackColor = Color.FromName(myenvcolor);
                panel2.BackColor = Color.FromName(myenvcolor);
                panel3.BackColor = Color.FromName(myenvcolor);
                this.BackColor = Color.FromName(myenvcolor);
                statusStrip1.BackColor = Color.FromName(myenvcolor);


            } //try slutt


            catch
            {
                MessageBox.Show("Ups: Cannot find server or current version \n" + Globals.DBStr + "\nVer: " + Globals.Version + "\nHarmonize", Globals.AppName);
                Environment.Exit(0); //avslutter appen
            }



            //TREE NODE  / hardkoder gruppene for now

            treeView1.Nodes.Add("Harmonize"); //denne er i=0
            treeView1.Nodes[0].Nodes.Add("Read Tables");   //0

            treeView1.Nodes[0].Nodes.Add("Views");  //1
            treeView1.Nodes[0].Nodes.Add("Stored Procedures");  //2

            treeView1.Nodes[0].Nodes.Add("Update Tables");  //3


            SqlConnection myconnbase2 = new SqlConnection();
            myconnbase2.ConnectionString = Globals.DBStr;
            SqlCommand mycommando2 = new SqlCommand();
            mycommando2.Connection = myconnbase2;



            try
            {


                mycommando2.CommandText = "execute Client_Mgr_Objects " + Environment.UserName; //eriks1
                //MessageBox.Show(mycommando.CommandText);

                myconnbase2.Open();

                //DataTable mydtable = new DataTable(); not needed
                SqlDataReader reader2 = mycommando2.ExecuteReader();
                //int MyStatusFlag = 0;
                //String mymessage = ""; //warnig from version
                //Globals.OnPrem = 0;

                if (reader2.HasRows)
                {

                    while (reader2.Read())
                    {
                        // MessageBox.Show(reader.GetString(reader.GetOrdinal("StatusMsg")) );
                        //Globals.AppName = reader.GetString(reader.GetOrdinal("Name"));

                        if (reader2.GetString(reader2.GetOrdinal("Otype")) == "U") //usr table
                        {

                            treeView1.Nodes[0].Nodes[0].Nodes.Add(reader2.GetString(reader2.GetOrdinal("Name")));
                        }

                        else if (reader2.GetString(reader2.GetOrdinal("Otype")) == "V")
                        {
                            treeView1.Nodes[0].Nodes[1].Nodes.Add(reader2.GetString(reader2.GetOrdinal("Name")));
                        }


                        else if (reader2.GetString(reader2.GetOrdinal("Otype")) == "P") //proc
                        {
                            treeView1.Nodes[0].Nodes[2].Nodes.Add(reader2.GetString(reader2.GetOrdinal("Name")));
                        }


                        else if (reader2.GetString(reader2.GetOrdinal("Otype")) == "UP") //update
                        {
                            treeView1.Nodes[0].Nodes[3].Nodes.Add(reader2.GetString(reader2.GetOrdinal("Name")));
                        }
                        



                        //treeView1.Nodes[0].Nodes[0].Nodes.Add("My R Table");


                    }

                }

                reader2.Close();
                myconnbase2.Close();


            } //try



            catch
            {
                MessageBox.Show("Kan ikke finne Mgr Update config :-)", Globals.AppName);
                Environment.Exit(0); //avslutter appen
            }


            //treeView1.Nodes[0].Nodes[1].Nodes.Add("My Up Table");
            //treeView1.Nodes[0].Nodes[1].Nodes.Add("Loadsets");


            treeView1.ExpandAll();

            ShowData_btn_Click();  //viser alle data initielt. da med loadset eller verdien av Pid






        }


        private void Exit_btn_Click(object sender, EventArgs e)
        {

            System.Windows.Forms.Application.Exit();
        }



        private static string GetgonnectionString()
        {
            string basepath = Application.StartupPath;
            string txtpath = basepath + @"/connection.txt";  //filename
            string connectionstring = ""; // "Data Sou";
            if (File.Exists(txtpath))
            {
                using (StreamReader sr = new StreamReader(txtpath))
                {
                    //leser foreste linje fra der exe fila ligger
                    string ss = sr.ReadLine();
                    //MessageBox.Show(ss.ToString());
                    connectionstring = (ss.ToString());
                }
                //MessageBox.Show(ss.ToString());
                return connectionstring;
            }
            else
            {
                MessageBox.Show("connection.txt file missing in: " + basepath);
                return ("error");
            }
        }



        //viser data: Show kalles med globale args for now.. tbc
        private void ShowData_btn_Click()
        {
            SqlConnection conn1 = new SqlConnection();
            conn1.ConnectionString = Globals.DBStr;
            SqlCommand command1 = new SqlCommand();
            command1.Connection = conn1;
            try
            {

                command1.CommandText = "Client_Select_Table " + Globals.Tablename + ", " + Globals.Topp + ", " + Globals.Orderby;
                //command1.CommandText = "execute Client_Select_Table " + Globals.Tablename + ", " + Globals.Topp + ", " + Globals.Orderby;
                //trenger ikke exec
                DataTable Loadset = new DataTable("Datagridname");

                SqlDataAdapter adapter1 = new SqlDataAdapter(command1);
                adapter1.Fill(Loadset);
                dataGridView1.DataSource = Loadset;
              
                if (Globals.NodeText == "Stored Procedures")
                {

                    dataGridView1.Columns[Globals.Pid1].Width = 400;
                    //dataGridView1.Columns[0].Width = 500;
                    dataGridView1.BackgroundColor = Color.White;
                    dataGridView1.GridColor = Color.White;  //whit var gray
                    dataGridView1.ReadOnly = true;
                    dataGridView1.MultiSelect = true;
                }



                else if (Globals.NodeText == "Update Tables")
                {
                    dataGridView1.ReadOnly = false;
                    dataGridView1.Columns[Globals.Pid1].Width = 100;
                    dataGridView1.BackgroundColor = Color.DarkRed;  //updatemode
                    dataGridView1.GridColor = Color.DarkRed;
                    //dataGridView1.Columns["Updated"].ReadOnly = true;  //cpmmon ok om alle har disse 2 !!!!!!!   feiler elelers
                    //dataGridView1.Columns["Updatedby"].ReadOnly = true; // comon
                    dataGridView1.MultiSelect = false;

                }

                else //view tables V U
                {
                    //dataGridView1.Columns[Globals.Pid1].Width = 120; //feiler
                    dataGridView1.Columns[0].Width = 120;
                    dataGridView1.ReadOnly = true;
                    dataGridView1.BackgroundColor = Color.White;
                    dataGridView1.GridColor = Color.Gray;
                    dataGridView1.MultiSelect = false;

                }



            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ups : " + ex.Message, Globals.AppName + " On DeepWater");
            }
            finally
            {
                ; //here you can add any code you want to be executed
                //regardless if an exception is thrown or not
                //ShowData_btn_Click();
            }



        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Data error, Check format", Globals.AppName);
        }










      
      

        //testing before deleteing
        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row == null) return;

            // Get ID safely
            if (!int.TryParse(e.Row.Cells[0].Value?.ToString(), out int myId))
                return;


            string idcolName = dataGridView1.Columns[0].Name;  //guessing idcol must ne the first one..

            // ✅ Confirmation box
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete ID {myId}?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.No)
            {
                e.Cancel = true; // STOP deletion
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Globals.DBStr))
                using (SqlCommand cmd = new SqlCommand("Client_DeleteFromTable", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    //cmd.Parameters.AddWithValue("@TableName", Globals.Tablename);
                    //cmd.Parameters.AddWithValue("@Id", myId);
                  

                    cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 64).Value = Globals.Tablename;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = myId;
                    cmd.Parameters.Add("@idcolname", SqlDbType.NVarChar, 64).Value = idcolName;


                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database delete failed:\n" + ex.Message,
                                Globals.AppName + " Delete",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                e.Cancel = true; // Cancel grid deletion if DB fails
            }
        }




        private void dataGridView1_Click(object sender, EventArgs e)
        {
            int rowindex = dataGridView1.CurrentCell.RowIndex;
            int columnindex = dataGridView1.CurrentCell.ColumnIndex;

            Globals.OldId1 = dataGridView1.Rows[rowindex].Cells[Globals.Pid1].Value.ToString();
            Globals.OldValue1 = dataGridView1.Rows[rowindex].Cells[columnindex].Value.ToString();

            //MessageBox.Show(Globals.OldId1 + "  -  " + Globals.OldValue1, "debug es");


            Globals.OldId1 = dataGridView1.Rows[rowindex].Cells[Globals.Pid1].Value.ToString();
            Globals.OldValue1 = dataGridView1.Rows[rowindex].Cells[columnindex].Value.ToString();

            if (Globals.Pid2 != "NULL")
            {
                Globals.OldId2 = dataGridView1.Rows[rowindex].Cells[Globals.Pid2].Value.ToString();
            }



            //MessageBox.Show("klikket på Loadset:  " + Globals.OldLoadsetId + " -> " + Globals.OldValue); // verdien er ok

        }

        private void button1_Click(object sender, EventArgs e)
        {

            MessageBox.Show("demo", Globals.AppName);
        }



        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string colName = dataGridView1.Columns[e.ColumnIndex].Name;
            string idcolName = dataGridView1.Columns[0].Name;  //guessing idcol must ne the first one..

            object cellValue = dataGridView1.Rows[e.RowIndex]
                                            .Cells[e.ColumnIndex]
                                            .Value;

            object newVal = cellValue ?? DBNull.Value;

            if (!int.TryParse(
                dataGridView1.Rows[e.RowIndex].Cells[0].Value?.ToString(),
                out int myId))
                return;

            string updateType = "U"; // string.IsNullOrEmpty(Globals.OldId1) ? "I" : "U";

            if (Globals.OldId1 == "")
            {
                Globals.OldId1 = "NULL"; 
                updateType = "I"; //MessageBox.Show(" null -> insert ??");
                             }
            else if (Globals.Pid1 != "NULL") { 
                updateType = "U"; 
            }


            //MessageBox.Show(updateType);





                try
            {
                using (SqlConnection conn = new SqlConnection(Globals.DBStr))
                using (SqlCommand cmd = new SqlCommand("Client_Update_Table2", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@tableName", SqlDbType.NVarChar).Value = Globals.Tablename;
                    cmd.Parameters.Add("@col", SqlDbType.NVarChar).Value = colName;
                    cmd.Parameters.Add("@val", SqlDbType.NVarChar).Value = newVal;
                    cmd.Parameters.Add("@idcolname", SqlDbType.NVarChar).Value = idcolName;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = myId;
                    cmd.Parameters.Add("@updatetype", SqlDbType.Char).Value = updateType;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database update failed:\n" + ex.Message,
                                Globals.AppName + " Update",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            // Optional refresh
            ShowData_btn_Click();
        }







        //loadsets table
        private void Refresh_btn_Click(object sender, EventArgs e)
        {

            Globals.Pid1 = "Id";  //needed for upadte & show
            Globals.Pid2 = "NULL";
            Globals.Tablename = "Loadset ";
            Globals.Orderby = "' 1 asc' ";
            // MessageBox.Show("Refresh btn click " + Globals.Tablename);
            dataGridView1.ReadOnly = true;
            ShowData_btn_Click();

        }

     
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {


            TreeNode oMainNode = e.Node; //current
            //TreeNode oMainNodeForelder = e.Node.Parent;
            table_label2.Text = e.Node.Text;
            PrintNodesRecursive(oMainNode, e.Node.Parent); //parent


        }




        public void PrintNodesRecursive(TreeNode oCurrentNode, TreeNode oParentNode)
        {
            if (oCurrentNode.Level == 2 || oCurrentNode.Text == "Dette er bunnn nivået mao")
            {
               

                SqlConnection myconnbase3 = new SqlConnection();
                myconnbase3.ConnectionString = Globals.DBStr;
                SqlCommand mycommando3 = new SqlCommand();
                mycommando3.Connection = myconnbase3;


                //må evt sende med paranet node.. om skille på view select og helptext
                try
                {


                    mycommando3.CommandText = "execute Client_Mgr_GetObject " + oCurrentNode.Text; //har allerede access                                                      
                    myconnbase3.Open();
                    SqlDataReader reader3 = mycommando3.ExecuteReader();


                    Globals.Tablename = oCurrentNode.Text;

                    if (reader3.HasRows)
                    {

                        while (reader3.Read())
                        {
                            
                            Globals.Orderby = "'" + (reader3.GetString(reader3.GetOrdinal("Orderby"))) + "'";
                            Globals.Pid1 = (reader3.GetString(reader3.GetOrdinal("Pid1")));
                            Globals.Pid2 = (reader3.GetString(reader3.GetOrdinal("Pid2")));
                            Globals.Topp = (reader3.GetInt32(reader3.GetOrdinal("Topp")));
                            //Globals.Pid3 = (reader2.GetString(reader2.GetOrdinal("Pid3")));                      

                            Globals.NodeText = oParentNode.Text;


                        } //wholse
                    }//if

                    reader3.Close();
                    myconnbase3.Close();


                    ShowData_btn_Click();

                } //try
                catch (SqlException ex)
                {
                    MessageBox.Show("Ups : " + ex.Message, Globals.AppName + "1 DeepWater");

                }

                finally
                {
                    //; //here you can add any code you want to be executed
                    //regardless if an exception is thrown or not
                    //ShowData_btn_Click();
                }










            }
        }

        private void About_btn_Click(object sender, EventArgs e)
        {

            MessageBox.Show(Globals.Version + "\n\nHarmonize\n" + $"© {DateTime.Now.Year} Eriksberg", Globals.AppName);

        }

     


     


       




        


    }//The
        }//End
