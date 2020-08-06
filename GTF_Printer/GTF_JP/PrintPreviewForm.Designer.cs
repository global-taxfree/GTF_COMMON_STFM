namespace GTF_Printer.GTF_JP
{
    partial class PrintPreviewForm
    {
        ///// <summary>
        ///// Required designer variable.
        ///// </summary>
        //private System.ComponentModel.IContainer components = null;

        ///// <summary>
        ///// Clean up any resources being used.
        ///// </summary>
        ///// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        //#region Windows Form Designer generated code

        ///// <summary>
        ///// Required method for Designer support - do not modify
        ///// the contents of this method with the code editor.
        ///// </summary>
        //private void InitializeComponent()
        //{
        //    this.components = new System.ComponentModel.Container();
        //    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        //    this.Text = "PrintPreviewForm";
        //}

        //#endregion
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.printPreviewControl1 = new System.Windows.Forms.PrintPreviewControl();
            this.printDocument = new System.Drawing.Printing.PrintDocument();
            this.toolStripButton_Print = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_Close = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.metroStyleExtender1 = new MetroFramework.Components.MetroStyleExtender(this.components);
            this.BTN_PRINT = new MetroFramework.Controls.MetroButton();
            this.BTN_CLOSE = new MetroFramework.Controls.MetroButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // printPreviewControl1
            // 
            this.printPreviewControl1.AutoZoom = false;
            this.printPreviewControl1.Document = this.printDocument;
            this.printPreviewControl1.Location = new System.Drawing.Point(23, 91);
            this.printPreviewControl1.Name = "printPreviewControl1";
            this.printPreviewControl1.Size = new System.Drawing.Size(760, 575);
            this.printPreviewControl1.TabIndex = 2;
            this.printPreviewControl1.Zoom = 0.46107784431137727D;
            // 
            // printDocument
            // 
            this.printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
            // 
            // toolStripButton_Print
            // 
            this.toolStripButton_Print.Checked = true;
            this.toolStripButton_Print.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButton_Print.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_Print.Image = global::GTF_Printer.Properties.Resources.toolStripButton_Print_Image;
            this.toolStripButton_Print.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Print.Name = "toolStripButton_Print";
            this.toolStripButton_Print.Size = new System.Drawing.Size(36, 22);
            this.toolStripButton_Print.Text = "Print";
            this.toolStripButton_Print.Click += new System.EventHandler(this.toolStripButton_Print_Click);
            // 
            // toolStripButton_Close
            // 
            this.toolStripButton_Close.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_Close.Image = global::GTF_Printer.Properties.Resources.toolStripButton_Close_Image;
            this.toolStripButton_Close.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Close.Name = "toolStripButton_Close";
            this.toolStripButton_Close.Size = new System.Drawing.Size(40, 22);
            this.toolStripButton_Close.Text = "Close";
            this.toolStripButton_Close.Click += new System.EventHandler(this.toolStripButton_Close_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripComboBox
            // 
            this.toolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox.Name = "toolStripComboBox";
            this.toolStripComboBox.Size = new System.Drawing.Size(75, 23);
            this.toolStripComboBox.Visible = false;
            this.toolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_Print,
            this.toolStripButton_Close,
            this.toolStripSeparator1,
            this.toolStripComboBox});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(20, 60);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(760, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.Visible = false;
            // 
            // BTN_PRINT
            // 
            this.BTN_PRINT.Location = new System.Drawing.Point(23, 58);
            this.BTN_PRINT.Name = "BTN_PRINT";
            this.BTN_PRINT.Size = new System.Drawing.Size(75, 30);
            this.BTN_PRINT.TabIndex = 3;
            this.BTN_PRINT.Text = "Print";
            this.BTN_PRINT.UseSelectable = true;
            this.BTN_PRINT.Click += new System.EventHandler(this.BTN_PRINT_Click);
            // 
            // BTN_CLOSE
            // 
            this.BTN_CLOSE.Location = new System.Drawing.Point(102, 58);
            this.BTN_CLOSE.Name = "BTN_CLOSE";
            this.BTN_CLOSE.Size = new System.Drawing.Size(75, 30);
            this.BTN_CLOSE.TabIndex = 4;
            this.BTN_CLOSE.Text = "Close";
            this.BTN_CLOSE.UseSelectable = true;
            this.BTN_CLOSE.Click += new System.EventHandler(this.BTN_CLOSE_Click);
            // 
            // PrintPreviewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 680);
            this.Controls.Add(this.BTN_CLOSE);
            this.Controls.Add(this.BTN_PRINT);
            this.Controls.Add(this.printPreviewControl1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "PrintPreviewForm";
            this.Style = MetroFramework.MetroColorStyle.Orange;
            this.Text = "PrintPreview";
            this.Load += new System.EventHandler(this.PrintPreviewForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PrintPreviewControl printPreviewControl1;
        private System.Drawing.Printing.PrintDocument printDocument;
        private System.Windows.Forms.ToolStripButton toolStripButton_Print;
        private System.Windows.Forms.ToolStripButton toolStripButton_Close;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private MetroFramework.Components.MetroStyleExtender metroStyleExtender1;
        private MetroFramework.Controls.MetroButton BTN_PRINT;
        private MetroFramework.Controls.MetroButton BTN_CLOSE;
    }
}