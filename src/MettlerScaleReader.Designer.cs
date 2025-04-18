namespace MettlerToledoLoadCellTool
{

    partial class MettlerScaleReader
    {
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "a",
            "b"}, -1);
            this.eventBox = new System.Windows.Forms.ListBox();
            this.ucLoadcellBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.loadcellGroupBox = new System.Windows.Forms.GroupBox();
            this.closeScaleBtn = new System.Windows.Forms.Button();
            this.tarraScaleBtn = new System.Windows.Forms.Button();
            this.nullScaleBtn = new System.Windows.Forms.Button();
            this.openScaleBtn = new System.Windows.Forms.Button();
            this.tarraWeightLabel = new System.Windows.Forms.Label();
            this.weightLabel = new System.Windows.Forms.Label();
            this.printerGroupBox = new System.Windows.Forms.GroupBox();
            this.feedLabelBtn = new System.Windows.Forms.Button();
            this.printTestLabelBtn = new System.Windows.Forms.Button();
            this.closePrinterBtn = new System.Windows.Forms.Button();
            this.openPrinterBtn = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabScale = new System.Windows.Forms.TabPage();
            this.delButton = new System.Windows.Forms.Button();
            this.sumButton = new System.Windows.Forms.Button();
            this.recieptView = new System.Windows.Forms.ListView();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.itemBox = new System.Windows.Forms.ListBox();
            this.tabEdit = new System.Windows.Forms.TabPage();
            this.removeButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.colProduct = new System.Windows.Forms.ColumnHeader();
            this.colPrice = new System.Windows.Forms.ColumnHeader();
            this.tabDebug = new System.Windows.Forms.TabPage();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ucLoadcellBindingSource)).BeginInit();
            this.loadcellGroupBox.SuspendLayout();
            this.printerGroupBox.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabScale.SuspendLayout();
            this.tabEdit.SuspendLayout();
            this.tabDebug.SuspendLayout();
            this.SuspendLayout();
            // 
            // eventBox
            // 
            this.eventBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eventBox.DataSource = this.ucLoadcellBindingSource;
            this.eventBox.FormattingEnabled = true;
            this.eventBox.Location = new System.Drawing.Point(2, 137);
            this.eventBox.Margin = new System.Windows.Forms.Padding(2);
            this.eventBox.Name = "eventBox";
            this.eventBox.Size = new System.Drawing.Size(785, 394);
            this.eventBox.TabIndex = 0;
            // 
            // loadcellGroupBox
            // 
            this.loadcellGroupBox.Controls.Add(this.closeScaleBtn);
            this.loadcellGroupBox.Controls.Add(this.tarraScaleBtn);
            this.loadcellGroupBox.Controls.Add(this.nullScaleBtn);
            this.loadcellGroupBox.Controls.Add(this.openScaleBtn);
            this.loadcellGroupBox.Controls.Add(this.tarraWeightLabel);
            this.loadcellGroupBox.Controls.Add(this.weightLabel);
            this.loadcellGroupBox.Location = new System.Drawing.Point(216, 5);
            this.loadcellGroupBox.Margin = new System.Windows.Forms.Padding(2);
            this.loadcellGroupBox.Name = "loadcellGroupBox";
            this.loadcellGroupBox.Padding = new System.Windows.Forms.Padding(2);
            this.loadcellGroupBox.Size = new System.Drawing.Size(574, 127);
            this.loadcellGroupBox.TabIndex = 20;
            this.loadcellGroupBox.TabStop = false;
            this.loadcellGroupBox.Text = "UC loadcell";
            this.loadcellGroupBox.Enter += new System.EventHandler(this.loadcellGroupBox_Enter);
            // 
            // closeScaleBtn
            // 
            this.closeScaleBtn.Enabled = false;
            this.closeScaleBtn.Location = new System.Drawing.Point(4, 56);
            this.closeScaleBtn.Margin = new System.Windows.Forms.Padding(2);
            this.closeScaleBtn.Name = "closeScaleBtn";
            this.closeScaleBtn.Size = new System.Drawing.Size(89, 32);
            this.closeScaleBtn.TabIndex = 21;
            this.closeScaleBtn.Text = "Close scale";
            this.closeScaleBtn.UseVisualStyleBackColor = true;
            this.closeScaleBtn.Click += new System.EventHandler(this.closeScaleBtn_click);
            // 
            // tarraScaleBtn
            // 
            this.tarraScaleBtn.Enabled = false;
            this.tarraScaleBtn.Location = new System.Drawing.Point(97, 56);
            this.tarraScaleBtn.Margin = new System.Windows.Forms.Padding(2);
            this.tarraScaleBtn.Name = "tarraScaleBtn";
            this.tarraScaleBtn.Size = new System.Drawing.Size(89, 32);
            this.tarraScaleBtn.TabIndex = 20;
            this.tarraScaleBtn.Text = "Tarra";
            this.tarraScaleBtn.UseVisualStyleBackColor = true;
            this.tarraScaleBtn.Click += new System.EventHandler(this.tarraScaleBtn_Click_1);
            // 
            // nullScaleBtn
            // 
            this.nullScaleBtn.Enabled = false;
            this.nullScaleBtn.Location = new System.Drawing.Point(97, 20);
            this.nullScaleBtn.Margin = new System.Windows.Forms.Padding(2);
            this.nullScaleBtn.Name = "nullScaleBtn";
            this.nullScaleBtn.Size = new System.Drawing.Size(89, 32);
            this.nullScaleBtn.TabIndex = 19;
            this.nullScaleBtn.Text = "Null scale";
            this.nullScaleBtn.UseVisualStyleBackColor = true;
            this.nullScaleBtn.Click += new System.EventHandler(this.nullScaleBtn_Click_1);
            // 
            // openScaleBtn
            // 
            this.openScaleBtn.Location = new System.Drawing.Point(4, 20);
            this.openScaleBtn.Margin = new System.Windows.Forms.Padding(2);
            this.openScaleBtn.Name = "openScaleBtn";
            this.openScaleBtn.Size = new System.Drawing.Size(89, 32);
            this.openScaleBtn.TabIndex = 18;
            this.openScaleBtn.Text = "Open scale";
            this.openScaleBtn.UseVisualStyleBackColor = true;
            this.openScaleBtn.Click += new System.EventHandler(this.openScaleBtn_Click);
            // 
            // tarraWeightLabel
            // 
            this.tarraWeightLabel.AutoSize = true;
            this.tarraWeightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tarraWeightLabel.Location = new System.Drawing.Point(232, 59);
            this.tarraWeightLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.tarraWeightLabel.Name = "tarraWeightLabel";
            this.tarraWeightLabel.Size = new System.Drawing.Size(17, 24);
            this.tarraWeightLabel.TabIndex = 17;
            this.tarraWeightLabel.Text = "-";
            // 
            // weightLabel
            // 
            this.weightLabel.AutoSize = true;
            this.weightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.weightLabel.Location = new System.Drawing.Point(232, 23);
            this.weightLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.weightLabel.Name = "weightLabel";
            this.weightLabel.Size = new System.Drawing.Size(17, 24);
            this.weightLabel.TabIndex = 16;
            this.weightLabel.Text = "-";
            // 
            // printerGroupBox
            // 
            this.printerGroupBox.Controls.Add(this.feedLabelBtn);
            this.printerGroupBox.Controls.Add(this.printTestLabelBtn);
            this.printerGroupBox.Controls.Add(this.closePrinterBtn);
            this.printerGroupBox.Controls.Add(this.openPrinterBtn);
            this.printerGroupBox.Location = new System.Drawing.Point(7, 5);
            this.printerGroupBox.Margin = new System.Windows.Forms.Padding(2);
            this.printerGroupBox.Name = "printerGroupBox";
            this.printerGroupBox.Padding = new System.Windows.Forms.Padding(2);
            this.printerGroupBox.Size = new System.Drawing.Size(205, 127);
            this.printerGroupBox.TabIndex = 19;
            this.printerGroupBox.TabStop = false;
            this.printerGroupBox.Text = "UC Printer";
            this.printerGroupBox.Enter += new System.EventHandler(this.printerGroupBox_Enter);
            // 
            // feedLabelBtn
            // 
            this.feedLabelBtn.Enabled = false;
            this.feedLabelBtn.Location = new System.Drawing.Point(98, 56);
            this.feedLabelBtn.Margin = new System.Windows.Forms.Padding(2);
            this.feedLabelBtn.Name = "feedLabelBtn";
            this.feedLabelBtn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.feedLabelBtn.Size = new System.Drawing.Size(89, 32);
            this.feedLabelBtn.TabIndex = 3;
            this.feedLabelBtn.Text = "Feed label";
            this.feedLabelBtn.UseVisualStyleBackColor = true;
            this.feedLabelBtn.Click += new System.EventHandler(this.feedLabelBtn_Click);
            // 
            // printTestLabelBtn
            // 
            this.printTestLabelBtn.Enabled = false;
            this.printTestLabelBtn.Location = new System.Drawing.Point(98, 20);
            this.printTestLabelBtn.Margin = new System.Windows.Forms.Padding(2);
            this.printTestLabelBtn.Name = "printTestLabelBtn";
            this.printTestLabelBtn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.printTestLabelBtn.Size = new System.Drawing.Size(89, 32);
            this.printTestLabelBtn.TabIndex = 2;
            this.printTestLabelBtn.Text = "Print test label";
            this.printTestLabelBtn.UseVisualStyleBackColor = true;
            this.printTestLabelBtn.Click += new System.EventHandler(this.printTestLabelBtn_Click);
            // 
            // closePrinterBtn
            // 
            this.closePrinterBtn.Enabled = false;
            this.closePrinterBtn.Location = new System.Drawing.Point(5, 56);
            this.closePrinterBtn.Margin = new System.Windows.Forms.Padding(2);
            this.closePrinterBtn.Name = "closePrinterBtn";
            this.closePrinterBtn.Size = new System.Drawing.Size(89, 32);
            this.closePrinterBtn.TabIndex = 1;
            this.closePrinterBtn.Text = "Close Printer";
            this.closePrinterBtn.UseVisualStyleBackColor = true;
            this.closePrinterBtn.Click += new System.EventHandler(this.closePrinterBtn_Click);
            // 
            // openPrinterBtn
            // 
            this.openPrinterBtn.Location = new System.Drawing.Point(5, 20);
            this.openPrinterBtn.Margin = new System.Windows.Forms.Padding(2);
            this.openPrinterBtn.Name = "openPrinterBtn";
            this.openPrinterBtn.Size = new System.Drawing.Size(89, 32);
            this.openPrinterBtn.TabIndex = 0;
            this.openPrinterBtn.Text = "Open Printer";
            this.openPrinterBtn.UseVisualStyleBackColor = true;
            this.openPrinterBtn.Click += new System.EventHandler(this.openPrinterBtn_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabScale);
            this.tabControl1.Controls.Add(this.tabEdit);
            this.tabControl1.Controls.Add(this.tabDebug);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.ItemSize = new System.Drawing.Size(100, 50);
            this.tabControl1.Location = new System.Drawing.Point(55, 58);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(40, 3);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 600);
            this.tabControl1.TabIndex = 22;
            // 
            // tabScale
            // 
            this.tabScale.Controls.Add(this.delButton);
            this.tabScale.Controls.Add(this.sumButton);
            this.tabScale.Controls.Add(this.recieptView);
            this.tabScale.Controls.Add(this.button1);
            this.tabScale.Controls.Add(this.label1);
            this.tabScale.Controls.Add(this.itemBox);
            this.tabScale.Location = new System.Drawing.Point(4, 54);
            this.tabScale.Name = "tabScale";
            this.tabScale.Padding = new System.Windows.Forms.Padding(3);
            this.tabScale.Size = new System.Drawing.Size(792, 542);
            this.tabScale.TabIndex = 0;
            this.tabScale.Text = "Wiegen";
            this.tabScale.UseVisualStyleBackColor = true;
            // 
            // delButton
            // 
            this.delButton.Location = new System.Drawing.Point(8, 371);
            this.delButton.Name = "delButton";
            this.delButton.Size = new System.Drawing.Size(142, 77);
            this.delButton.TabIndex = 5;
            this.delButton.Text = "löschen";
            this.delButton.UseVisualStyleBackColor = true;
            // 
            // sumButton
            // 
            this.sumButton.Location = new System.Drawing.Point(156, 371);
            this.sumButton.Name = "sumButton";
            this.sumButton.Size = new System.Drawing.Size(250, 77);
            this.sumButton.TabIndex = 4;
            this.sumButton.Text = "Abrechnen";
            this.sumButton.UseVisualStyleBackColor = true;
            // 
            // recieptView
            // 
            this.recieptView.Location = new System.Drawing.Point(6, 6);
            this.recieptView.Name = "recieptView";
            this.recieptView.Size = new System.Drawing.Size(400, 352);
            this.recieptView.TabIndex = 3;
            this.recieptView.UseCompatibleStateImageBehavior = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(612, 364);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(174, 84);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 35F);
            this.label1.Location = new System.Drawing.Point(412, 374);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 54);
            this.label1.TabIndex = 1;
            this.label1.Text = "0.00 kg";
            // 
            // itemBox
            // 
            this.itemBox.FormattingEnabled = true;
            this.itemBox.ItemHeight = 29;
            this.itemBox.Location = new System.Drawing.Point(412, 6);
            this.itemBox.Name = "itemBox";
            this.itemBox.Size = new System.Drawing.Size(374, 352);
            this.itemBox.TabIndex = 0;
            // 
            // tabEdit
            // 
            this.tabEdit.Controls.Add(this.removeButton);
            this.tabEdit.Controls.Add(this.addButton);
            this.tabEdit.Controls.Add(this.textBox2);
            this.tabEdit.Controls.Add(this.textBox1);
            this.tabEdit.Controls.Add(this.listView1);
            this.tabEdit.Location = new System.Drawing.Point(4, 54);
            this.tabEdit.Name = "tabEdit";
            this.tabEdit.Size = new System.Drawing.Size(792, 542);
            this.tabEdit.TabIndex = 2;
            this.tabEdit.Text = "Bearbeiten";
            this.tabEdit.UseVisualStyleBackColor = true;
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(636, 54);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(156, 43);
            this.removeButton.TabIndex = 4;
            this.removeButton.Text = "entfernen";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(433, 54);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(156, 43);
            this.addButton.TabIndex = 4;
            this.addButton.Text = "hinzufügen";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F);
            this.textBox2.Location = new System.Drawing.Point(643, 3);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(149, 45);
            this.textBox2.TabIndex = 3;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F);
            this.textBox1.Location = new System.Drawing.Point(433, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(201, 45);
            this.textBox1.TabIndex = 2;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colProduct,
            this.colPrice});
            this.listView1.GridLines = true;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(424, 531);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // colProduct
            // 
            this.colProduct.Text = "Produkt";
            this.colProduct.Width = 260;
            // 
            // colPrice
            // 
            this.colPrice.Text = "Preis";
            this.colPrice.Width = 160;
            // 
            // tabDebug
            // 
            this.tabDebug.Controls.Add(this.eventBox);
            this.tabDebug.Controls.Add(this.loadcellGroupBox);
            this.tabDebug.Controls.Add(this.printerGroupBox);
            this.tabDebug.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.2F);
            this.tabDebug.Location = new System.Drawing.Point(4, 54);
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.Padding = new System.Windows.Forms.Padding(3);
            this.tabDebug.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabDebug.Size = new System.Drawing.Size(792, 542);
            this.tabDebug.TabIndex = 1;
            this.tabDebug.Text = "Test";
            this.tabDebug.UseVisualStyleBackColor = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // MettlerScaleReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.18F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MettlerScaleReader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Mettler Toledo UC Loadcell test tool";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MettlerScaleReader_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ucLoadcellBindingSource)).EndInit();
            this.loadcellGroupBox.ResumeLayout(false);
            this.loadcellGroupBox.PerformLayout();
            this.printerGroupBox.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabScale.ResumeLayout(false);
            this.tabScale.PerformLayout();
            this.tabEdit.ResumeLayout(false);
            this.tabEdit.PerformLayout();
            this.tabDebug.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox eventBox;
        private System.Windows.Forms.GroupBox loadcellGroupBox;
        private System.Windows.Forms.Button closeScaleBtn;
        private System.Windows.Forms.Button tarraScaleBtn;
        private System.Windows.Forms.Button nullScaleBtn;
        private System.Windows.Forms.Button openScaleBtn;
        private System.Windows.Forms.Label tarraWeightLabel;
        private System.Windows.Forms.Label weightLabel;
        private System.Windows.Forms.GroupBox printerGroupBox;
        private System.Windows.Forms.Button feedLabelBtn;
        private System.Windows.Forms.Button printTestLabelBtn;
        private System.Windows.Forms.Button closePrinterBtn;
        private System.Windows.Forms.Button openPrinterBtn;
        private System.Windows.Forms.BindingSource ucLoadcellBindingSource;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabScale;
        private System.Windows.Forms.TabPage tabDebug;
        private System.Windows.Forms.TabPage tabEdit;
        private System.Windows.Forms.Button delButton;
        private System.Windows.Forms.Button sumButton;
        private System.Windows.Forms.ListView recieptView;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox itemBox;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader colProduct;
        private System.Windows.Forms.ColumnHeader colPrice;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button removeButton;
    }

}