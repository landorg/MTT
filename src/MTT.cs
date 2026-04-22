using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;

using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing;
using System.Diagnostics;

namespace MTT
{
    public partial class MTT : Form
    {

        public MTT()
        {
            StreamWriter sw = File.AppendText("C:/MTT/log.txt");

            // this is important
            sw.AutoFlush = true;
            Console.SetError(sw);

            InitializeComponent();
        }

        private void openScaleBtn_Click(object sender, EventArgs e)
        {
            ScaleCell.init();
        }

        private void closeScaleBtn_click(object sender, EventArgs e)
        {
            ScaleCell.close();
        }

        //
        // Printer
        //

        private void openPrinterBtn_Click(object sender, EventArgs e)
        {
            Printer p = Printer.Instance;
        }      

        private void feedLabelBtn_Click(object sender, EventArgs e)
        {
            Printer.Instance.feedLabel();
        }

        private void closePrinterBtn_Click(object sender, EventArgs e)
        {
            Printer.Instance.close();
        }

        private void printTestLabelBtn_Click(object sender, EventArgs e)
        {
                
            Printer.Instance.PrintReciept(recieptList, reciept.sum);
        }

        private Reciept reciept = new Reciept();

        private void MTT_Load(object sender, EventArgs e)
        {
            DB.load();
            refreshDbList();

            ScaleCell.init();
        }
        private void MTT_FormClosing(object sender, FormClosingEventArgs e)
        {
            ScaleCell.close();
        }

        private string activeGroup = "";
        private int productPage = 0;
        private const int ProductPageSize = 8;
        private List<Product> filteredProducts = new List<Product>();

        internal void refreshDbList()
        {
            var sorted = DB.products.OrderBy(p => p.group).ThenBy(p => p.name).ToList();

            dbList.Items.Clear();
            string currentGroupText = txtGroup.Text;
            txtGroup.Items.Clear();
            foreach (Product p in sorted)
            {
                ListViewItem item = new ListViewItem(p.name);
                item.SubItems.Add(p.price.ToString());
                item.SubItems.Add(p.piecePrice ? "stk" : "kg");
                item.SubItems.Add(p.group ?? "");
                dbList.Items.Add(item);

                if (!string.IsNullOrEmpty(p.group) && !txtGroup.Items.Contains(p.group))
                    txtGroup.Items.Add(p.group);
            }
            txtGroup.Text = currentGroupText;

            RebuildProductButtons();
        }

        private void RebuildProductButtons()
        {
            productGroupPanel.Controls.Clear();
            productButtonPanel.Controls.Clear();
            selectedProduct = null;
            productPage = 0;

            var groups = new List<string> { "" };
            groups.AddRange(DB.products
                .Where(p => !string.IsNullOrEmpty(p.group))
                .Select(p => p.group)
                .Distinct()
                .OrderBy(g => g));

            if (groups.Count > 1)
            {
                foreach (string g in groups)
                {
                    var btn = new System.Windows.Forms.Button();
                    btn.Text = g == "" ? "Alle" : g;
                    btn.Size = new Size(g == "" ? 55 : Math.Max(55, g.Length * 11 + 16), 44);
                    btn.Font = new Font("Microsoft Sans Serif", 11F);
                    btn.Margin = new Padding(2);
                    btn.Tag = g;
                    btn.FlatStyle = FlatStyle.Flat;
                    if (g == activeGroup)
                        btn.BackColor = Color.SteelBlue;
                    btn.Click += GroupBtn_Click;
                    productGroupPanel.Controls.Add(btn);
                }
                productGroupPanel.Visible = true;
            }
            else
            {
                productGroupPanel.Visible = false;
            }

            FilterProducts(activeGroup);
        }

        private void GroupBtn_Click(object sender, EventArgs e)
        {
            var btn = (System.Windows.Forms.Button)sender;
            activeGroup = (string)btn.Tag;
            productPage = 0;
            foreach (System.Windows.Forms.Button b in productGroupPanel.Controls)
                b.BackColor = (string)b.Tag == activeGroup ? Color.SteelBlue : SystemColors.Control;
            FilterProducts(activeGroup);
        }

        private void FilterProducts(string group)
        {
            filteredProducts = DB.products
                .Where(p => group == "" || p.group == group)
                .OrderBy(p => p.group)
                .ThenBy(p => p.name)
                .ToList();
            ShowProductPage();
        }

        private void ShowProductPage()
        {
            productButtonPanel.Controls.Clear();
            selectedProduct = null;
            addArticleButton.Text = "";

            int pageCount = Math.Max(1, (filteredProducts.Count + ProductPageSize - 1) / ProductPageSize);
            productPage = Math.Max(0, Math.Min(productPage, pageCount - 1));

            int start = productPage * ProductPageSize;
            int end = Math.Min(start + ProductPageSize, filteredProducts.Count);

            for (int i = start; i < end; i++)
            {
                Product p = filteredProducts[i];
                var btn = new System.Windows.Forms.Button();
                string unit = p.piecePrice ? "stk" : "kg";
                btn.Text = $"{p.name}\n{p.price:0.00} €/{unit}";
                btn.Size = new Size(183, 63);
                btn.Font = new Font("Microsoft Sans Serif", 11F);
                btn.Margin = new Padding(3);
                btn.Tag = p;
                btn.FlatStyle = FlatStyle.Standard;
                btn.TextAlign = ContentAlignment.MiddleCenter;
                btn.Click += ProductBtn_Click;
                productButtonPanel.Controls.Add(btn);
            }

            bool multiPage = pageCount > 1;
            btnProductPrev.Visible = multiPage;
            labelPage.Visible = multiPage;
            btnProductNext.Visible = multiPage;
            if (multiPage)
            {
                labelPage.Text = $"{productPage + 1} / {pageCount}";
                btnProductPrev.Enabled = productPage > 0;
                btnProductNext.Enabled = productPage < pageCount - 1;
            }
        }

        private void btnProductPrev_Click(object sender, EventArgs e)
        {
            productPage--;
            ShowProductPage();
        }

        private void btnProductNext_Click(object sender, EventArgs e)
        {
            productPage++;
            ShowProductPage();
        }

        private void ProductBtn_Click(object sender, EventArgs e)
        {
            var btn = (System.Windows.Forms.Button)sender;
            foreach (System.Windows.Forms.Button b in productButtonPanel.Controls)
                b.BackColor = SystemColors.Control;
            btn.BackColor = Color.LightGreen;

            selectedProduct = (Product)btn.Tag;
            addArticleButton.Text = selectedProduct.piecePrice ? "+ 1 Stück" : "abwiegen";
        }

        private Product selectedDBProduct;

        private void dbList_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (dbList.SelectedItems.Count == 1)
            {

                string selectedDBProductName = dbList.SelectedItems[0].Text;
                selectedDBProduct = DB.products.Find(x => x.name == selectedDBProductName);

                piecePriceCheckbox.Checked = selectedDBProduct.piecePrice;
                txtName.Text = selectedDBProduct.name;
                txtPreis.Text = selectedDBProduct.price.ToString();
                txtGroup.Text = selectedDBProduct.group ?? "";

                removeButton.Enabled = true;
            }
            else
            {
                selectedDBProduct = null;
                piecePriceCheckbox.Checked = false;
                txtName.Text = "";
                txtPreis.Text = "";
                txtGroup.Text = "";

                removeButton.Enabled = false;
            }

            //dbList.SelectedItems
            // TODO

            //ListView.SelectedListViewItemCollection breakfast =
            //    this.ListView1.SelectedItems;

            //double price = 0.0;
            //foreach (ListViewItem item in breakfast)
            //{
            //    price += Double.Parse(item.SubItems[1].Text);
            //}

            //// Output the price to TextBox1.
            //TextBox1.Text = price.ToString();

        }
        private void removeButton_Click(object sender, EventArgs e)
        {
            DB.remove(dbList.SelectedItems[0].Text);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                Product p = new Product(txtName.Text, piecePriceCheckbox.Checked, decimal.Parse(txtPreis.Text)) { group = txtGroup.Text.Trim() };
                DB.add(p);

            }
            catch (Exception err)
            {
                logToBox(err.ToString(), "error");
                throw;
            }

        }

        private void addArticleButton_Click(object sender, EventArgs e)
        {
            if (selectedProduct != null)
            {
                if (selectedProduct.piecePrice)
                {
                    reciept.add(new Article(selectedProduct, 1));
                }
                else
                {
                    reciept.add(new Article(selectedProduct, currentWeight != null ? currentWeight.net : 0));
                }
            }
        }

        private void itemBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void logToBox(string message, string level = "info")
        {
            String ts = DateTime.Now.ToString();
            String log = $"[{level}] {ts} {message}";

            Console.Error.WriteLine(log);

            // Check if we are on the UI thread
            if (eventBox.InvokeRequired)
            {
                // We are on a worker thread, marshal the call to the UI thread
                // Using BeginInvoke for asynchronous execution (doesn't block the logger thread)
                eventBox.BeginInvoke(new Action(() =>
                {
                    // This code now runs on the UI thread
                    eventBox.Items.Insert(0, log);

                    if (eventBox.Items.Count > 2000)
                    {
                        for (int i = 2000; i < eventBox.Items.Count; i++)
                        {
                            eventBox.Items.Remove(eventBox.Items[i]);
                        }
                    }
                }));
            }
            else
            {
                // We are already on the UI thread, update directly
                eventBox.Items.Insert(0, log);
                if (eventBox.Items.Count > 2000)
                {
                    for (int i = 2000; i < eventBox.Items.Count; i++)
                    {
                        eventBox.Items.Remove(eventBox.Items[i]);
                    }
                }
            }
        }

        private void nullButton2_Click(object sender, EventArgs e)
        {
            ScaleCell.nullIt();
        }

        private void nullButton_Click(object sender, EventArgs e)
        {
            ScaleCell.nullIt();
        }

        private void tareButton_Click(object sender, EventArgs e)
        {
            ScaleCell.setTare();
        }


        private void eventBoxChange(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }


        private void tareButton2_Click(object sender, EventArgs e)
        {
            ScaleCell.setTare();
        }

        int pid;

        private void kbButton_Click(object sender, EventArgs e)
        {
            Process pKb = null;
            try
            {
                pKb = System.Diagnostics.Process.GetProcessById(pid);
                pKb.Kill();

            } catch (Exception err) {
                pKb = new Process();
                pKb.StartInfo.FileName = @"C:\Windows\System32\osk.exe";
                pKb.Start();
                //var onScreenKeyboardProcess = new ProcessStartInfo("C:\\Windows\\System32\\osk.exe")
                //{
                //    UseShellExecute = true
                //};
                //pKb = Process.Start(onScreenKeyboardProcess);
                pid = pKb.Id;


            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        public Weights currentWeight = new Weights(0,0,0,false);
        private decimal currentPrice;

        internal void SetWeights(decimal netWeight, decimal tareWeight, decimal grossWeight, bool instable)
        {
            currentWeight.net = netWeight;
            currentWeight.tare = tareWeight;
            currentWeight.gross = grossWeight;
            currentWeight.instable = instable;

            string prefix = instable ? "~ " : "";
            string netString = $"{prefix}{currentWeight.net}";
            string tareString = currentWeight.tare.ToString();
            string priceString = "0.00";

            if (currentWeight != null && selectedProduct != null)
            {

                currentPrice = Decimal.Round(currentWeight.net * selectedProduct.price, 2);
                priceString = $"{currentPrice:0.00}";
            }
            try
            {

                if (netLabel1.InvokeRequired)
                {
                    // We are on a worker thread, marshal the call to the UI thread
                    // Using BeginInvoke for asynchronous execution (doesn't block the logger thread)
                    eventBox.BeginInvoke(new Action(() =>
                    {
                        netLabel1.Text = netString;
                        netLabel2.Text = netString;
                        tareLabel1.Text = tareString;
                        tareLabel2.Text = tareString;
                        currentPriceLabel.Text = priceString;
                    }));
                } else {
                    netLabel1.Text = netString;
                    netLabel2.Text = netString;
                    tareLabel1.Text = tareString;
                    tareLabel2.Text = tareString;
                    currentPriceLabel.Text = priceString;
                }
            }
            catch (Exception ex)
            {
                this.logToBox($"Error setting weight: {ex.ToString()}");
                //eventBox.Items.Insert(0, "Error: parsing weight to string");
                //eventBox.Items.Insert(0, ex.Message);
            }
        }

        internal void refreshReciept()
        {
            recieptList.Items.Clear();
            foreach (Article a in reciept.articles)
            {
                ListViewItem item = new ListViewItem(a.product.name);

                string weight = (
                    a.product.piecePrice ?
                    Decimal.Round(a.Weight, 0).ToString() :
                    Decimal.Round(a.Weight, 2).ToString()
                );

                string amount = $"{weight} {(a.product.piecePrice ? " stk" : " kg")}";
                item.SubItems.Add(amount);

                item.SubItems.Add($"{Decimal.Round(a.product.price, 2):0.00} €");

                item.SubItems.Add($"{Decimal.Round(a.price, 2):0.00} €");

                recieptList.Items.Add(item);
            }
            sumLabel.Text = $"{Decimal.Round(reciept.sum, 2):0.00}";
        }

        private void recieptList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void delArticleButton_Click(object sender, EventArgs e)
        {
            if (recieptList.SelectedItems.Count > 0)
            {
                int selectedArticle = recieptList.SelectedItems[0].Index;
                //float selectedProductKgPrice = float.Parse(dbList2.SelectedItems[0].SubItems[1].Text);

                reciept.remove(selectedArticle);
            }
        }

        private void setWeightButton_Click(object sender, EventArgs e)
        {
            this.SetWeights(0.15M, 0, 0.15M, false);
        }

        private void piecePrice_CheckedChanged(object sender, EventArgs e)
        {
            if (piecePriceCheckbox.Checked)
            {
                label2.Text = "€/stk";
            } else { 
                label2.Text = "€/kg";
            }
        }

        private Product selectedProduct;


        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            base.OnPaint(e);
        }

        private void sumButton_Click(object sender, EventArgs e)
        {
            Printer.Instance.PrintReciept(recieptList, reciept.sum);
            reciept.save();
            reciept = new Reciept();

            refreshReciept();
        }

    }
}