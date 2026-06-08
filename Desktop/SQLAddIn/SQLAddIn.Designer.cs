namespace SQLAddIn
{
    partial class Harmonize : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Harmonize()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Harmonize));
            this.tab1 = this.Factory.CreateRibbonTab();
            this.ribbon_grp = this.Factory.CreateRibbonGroup();
            this.box3 = this.Factory.CreateRibbonBox();
            this.btnExecuteSQL = this.Factory.CreateRibbonButton();
            this.RefreshAll_btn = this.Factory.CreateRibbonButton();
            this.TestDBConnection = this.Factory.CreateRibbonButton();
            this.Put_btn = this.Factory.CreateRibbonButton();
            this.delete_serie_btn = this.Factory.CreateRibbonButton();
            this.button1 = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.ribbon_grp.SuspendLayout();
            this.box3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.Groups.Add(this.ribbon_grp);
            this.tab1.Label = "Harmonize";
            this.tab1.Name = "tab1";
            // 
            // ribbon_grp
            // 
            this.ribbon_grp.Items.Add(this.box3);
            this.ribbon_grp.Label = "Status:";
            this.ribbon_grp.Name = "ribbon_grp";
            // 
            // box3
            // 
            this.box3.BoxStyle = Microsoft.Office.Tools.Ribbon.RibbonBoxStyle.Vertical;
            this.box3.Items.Add(this.btnExecuteSQL);
            this.box3.Items.Add(this.RefreshAll_btn);
            this.box3.Items.Add(this.TestDBConnection);
            this.box3.Items.Add(this.Put_btn);
            this.box3.Items.Add(this.delete_serie_btn);
            this.box3.Items.Add(this.button1);
            this.box3.Name = "box3";
            // 
            // btnExecuteSQL
            // 
            this.btnExecuteSQL.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnExecuteSQL.Description = "read";
            this.btnExecuteSQL.Image = ((System.Drawing.Image)(resources.GetObject("btnExecuteSQL.Image")));
            this.btnExecuteSQL.Label = "GetSerie";
            this.btnExecuteSQL.Name = "btnExecuteSQL";
            this.btnExecuteSQL.ShowImage = true;
            this.btnExecuteSQL.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnExecuteSQL_Click);
            // 
            // RefreshAll_btn
            // 
            this.RefreshAll_btn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.RefreshAll_btn.Image = ((System.Drawing.Image)(resources.GetObject("RefreshAll_btn.Image")));
            this.RefreshAll_btn.Label = "&RefreshAll";
            this.RefreshAll_btn.Name = "RefreshAll_btn";
            this.RefreshAll_btn.ShowImage = true;
            this.RefreshAll_btn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.RefreshAll_btn_Click);
            // 
            // TestDBConnection
            // 
            this.TestDBConnection.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.TestDBConnection.Image = global::SQLAddIn.Properties.Resources.database;
            this.TestDBConnection.Label = "TestConnect";
            this.TestDBConnection.Name = "TestDBConnection";
            this.TestDBConnection.ShowImage = true;
            this.TestDBConnection.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button2_Click);
            // 
            // Put_btn
            // 
            this.Put_btn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.Put_btn.Description = "write";
            this.Put_btn.Image = ((System.Drawing.Image)(resources.GetObject("Put_btn.Image")));
            this.Put_btn.Label = "PutSerie";
            this.Put_btn.Name = "Put_btn";
            this.Put_btn.ShowImage = true;
            this.Put_btn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.Put_btn_Click);
            // 
            // delete_serie_btn
            // 
            this.delete_serie_btn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.delete_serie_btn.Image = ((System.Drawing.Image)(resources.GetObject("delete_serie_btn.Image")));
            this.delete_serie_btn.Label = "DeleteSeries";
            this.delete_serie_btn.Name = "delete_serie_btn";
            this.delete_serie_btn.ShowImage = true;
            this.delete_serie_btn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.delete_serie_btn_Click);
            // 
            // button1
            // 
            this.button1.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.button1.Image = global::SQLAddIn.Properties.Resources.Norway_Flag;
            this.button1.Label = "About";
            this.button1.Name = "button1";
            this.button1.ShowImage = true;
            this.button1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button1_Click);
            // 
            // Harmonize
            // 
            this.Name = "Harmonize";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.SQLAddIn_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.ribbon_grp.ResumeLayout(false);
            this.ribbon_grp.PerformLayout();
            this.box3.ResumeLayout(false);
            this.box3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnExecuteSQL;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton TestDBConnection;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton Put_btn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup ribbon_grp;
        internal Microsoft.Office.Tools.Ribbon.RibbonBox box3;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton RefreshAll_btn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton delete_serie_btn;
    }

    partial class ThisRibbonCollection
    {
        internal Harmonize SQLAddIn
        {
            get { return this.GetRibbon<Harmonize>(); }
        }
    }
}
