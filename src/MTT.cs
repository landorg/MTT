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
                
            Printer.Instance.PrintReciept(recieptList, reciept.sum, reciept.mwst);
        }

        private Reciept reciept = new Reciept();

        private void MTT_Load(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabDebug);

            DB.load();
            refreshDbList();
            refreshHistoryList();

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
            _dbSortedProducts = DB.products.OrderBy(p => p.group).ThenBy(p => p.name).ToList();
            if (_dbSortColumn >= 0)
            {
                bool asc = _dbSortOrder == SortOrder.Ascending;
                switch (_dbSortColumn)
                {
                    case 0: _dbSortedProducts = asc ? _dbSortedProducts.OrderBy(p => p.name).ToList() : _dbSortedProducts.OrderByDescending(p => p.name).ToList(); break;
                    case 1: _dbSortedProducts = asc ? _dbSortedProducts.OrderBy(p => p.price).ToList() : _dbSortedProducts.OrderByDescending(p => p.price).ToList(); break;
                    case 2: _dbSortedProducts = asc ? _dbSortedProducts.OrderBy(p => p.piecePrice).ToList() : _dbSortedProducts.OrderByDescending(p => p.piecePrice).ToList(); break;
                    case 3: _dbSortedProducts = asc ? _dbSortedProducts.OrderBy(p => p.group).ToList() : _dbSortedProducts.OrderByDescending(p => p.group).ToList(); break;
                }
            }

            string currentGroupText = txtGroup.Text;
            txtGroup.Items.Clear();
            foreach (Product p in _dbSortedProducts)
                if (!string.IsNullOrEmpty(p.group) && !txtGroup.Items.Contains(p.group))
                    txtGroup.Items.Add(p.group);
            txtGroup.Text = currentGroupText;

            ShowDbPage();
            RebuildProductButtons();
        }

        private void ShowDbPage()
        {
            int pageCount = Math.Max(1, (_dbSortedProducts.Count + DbPageSize - 1) / DbPageSize);
            _dbPage = Math.Max(0, Math.Min(_dbPage, pageCount - 1));

            dbList.Items.Clear();
            int start = _dbPage * DbPageSize;
            int end = Math.Min(start + DbPageSize, _dbSortedProducts.Count);
            for (int i = start; i < end; i++)
            {
                var p = _dbSortedProducts[i];
                ListViewItem item = new ListViewItem(p.name);
                item.SubItems.Add(p.price.ToString());
                item.SubItems.Add(p.piecePrice ? "stk" : "kg");
                item.SubItems.Add(p.group ?? "");
                dbList.Items.Add(item);
            }

            bool multi = pageCount > 1;
            btnDbPrev.Visible = multi;
            labelDbPage.Visible = multi;
            btnDbNext.Visible = multi;
            if (multi)
            {
                labelDbPage.Text = $"{_dbPage + 1} / {pageCount}";
                btnDbPrev.Enabled = _dbPage > 0;
                btnDbNext.Enabled = _dbPage < pageCount - 1;
            }
        }

        private void btnDbPrev_Click(object sender, EventArgs e) { _dbPage--; ShowDbPage(); }
        private void btnDbNext_Click(object sender, EventArgs e) { _dbPage++; ShowDbPage(); }

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
                    btn.Size = new Size(g == "" ? 70 : Math.Max(70, g.Length * 14 + 16), 44);
                    btn.Font = new Font("Microsoft Sans Serif", 14F);
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
                btn.Font = new Font("Microsoft Sans Serif", 14F);
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
        private int _dbSortColumn = -1;
        private SortOrder _dbSortOrder = SortOrder.None;
        private int _dbPage = 0;
        private const int DbPageSize = 15;
        private List<Product> _dbSortedProducts = new List<Product>();

        private void dbList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _dbSortColumn)
                _dbSortOrder = _dbSortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            else
            {
                _dbSortColumn = e.Column;
                _dbSortOrder = SortOrder.Ascending;
            }
            _dbPage = 0;
            refreshDbList();
        }

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
                int id = selectedDBProduct?.ID ?? 0;
                Product p = new Product(txtName.Text, piecePriceCheckbox.Checked, decimal.Parse(txtPreis.Text)) { group = txtGroup.Text.Trim(), ID = id };
                DB.add(p);
                int idx = _dbSortedProducts.FindIndex(x => x.name == p.name);
                if (idx >= 0)
                {
                    int targetPage = idx / DbPageSize;
                    if (targetPage != _dbPage) { _dbPage = targetPage; ShowDbPage(); }
                    int posInPage = idx % DbPageSize;
                    if (posInPage < dbList.Items.Count)
                    {
                        dbList.Items[posInPage].Selected = true;
                        dbList.EnsureVisible(posInPage);
                    }
                }

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
            mwstLabel.Text = $"{Decimal.Round(reciept.mwst, 2):0.00} €";
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

        private void printButton_Click(object sender, EventArgs e)
        {
            if (reciept.articles.Count == 0) return;
            Printer.Instance.PrintReciept(recieptList, reciept.sum, reciept.mwst);
        }

        private void sumButton_Click(object sender, EventArgs e)
        {
            reciept.save();
            reciept = new Reciept();
            refreshReciept();
            refreshHistoryList();
        }

        // --- History tab ---

        private Reciept selectedHistoryReciept;
        private int _histPage = 0;
        private const int HistPageSize = 14;
        private struct HistEntry { public string date, sum, file; }
        private List<HistEntry> _histEntries = new List<HistEntry>();
        private int _histDetailPage = 0;
        private const int HistDetailPageSize = 9;
        private struct HistDetailEntry { public string name, weight, unitPrice, total; }
        private List<HistDetailEntry> _histDetailEntries = new List<HistDetailEntry>();

        internal void refreshHistoryList()
        {
            _histEntries = new List<HistEntry>();
            string[] files = System.IO.Directory.GetFiles("C:/MTT/Rechnungen/", "Rechnung-*.txt");
            System.Array.Sort(files);
            System.Array.Reverse(files);
            foreach (string file in files)
            {
                string datePart = System.IO.Path.GetFileNameWithoutExtension(file).Substring("Rechnung-".Length);
                string displayDate = datePart;
                try
                {
                    System.DateTime dt = System.DateTime.ParseExact(datePart, "yyyy-MM-dd_HH-mm-ss", null);
                    displayDate = dt.ToString("dd.MM.yyyy HH:mm");
                }
                catch { }

                decimal fileSum = 0;
                try
                {
                    string json = System.IO.File.ReadAllText(file);
                    var r = Newtonsoft.Json.JsonConvert.DeserializeObject<Reciept>(json);
                    fileSum = r != null ? r.sum : 0;
                }
                catch { }

                _histEntries.Add(new HistEntry { date = displayDate, sum = $"{Decimal.Round(fileSum, 2):0.00} €", file = file });
            }
            _histPage = 0;
            ShowHistPage();
        }

        private void ShowHistPage()
        {
            int pageCount = Math.Max(1, (_histEntries.Count + HistPageSize - 1) / HistPageSize);
            _histPage = Math.Max(0, Math.Min(_histPage, pageCount - 1));

            historyList.Items.Clear();
            int start = _histPage * HistPageSize;
            int end = Math.Min(start + HistPageSize, _histEntries.Count);
            for (int i = start; i < end; i++)
            {
                var e = _histEntries[i];
                ListViewItem item = new ListViewItem(e.date);
                item.SubItems.Add(e.sum);
                item.Tag = e.file;
                historyList.Items.Add(item);
            }

            labelHistPage.Text = $"{_histPage + 1} / {pageCount}";
            btnHistPrev.Enabled = _histPage > 0;
            btnHistNext.Enabled = _histPage < pageCount - 1;
        }

        private void btnHistPrev_Click(object sender, EventArgs e) { _histPage--; ShowHistPage(); }
        private void btnHistNext_Click(object sender, EventArgs e) { _histPage++; ShowHistPage(); }

        private void ShowHistDetailPage()
        {
            int pageCount = Math.Max(1, (_histDetailEntries.Count + HistDetailPageSize - 1) / HistDetailPageSize);
            _histDetailPage = Math.Max(0, Math.Min(_histDetailPage, pageCount - 1));

            historyDetailList.Items.Clear();
            int start = _histDetailPage * HistDetailPageSize;
            int end = Math.Min(start + HistDetailPageSize, _histDetailEntries.Count);
            for (int i = start; i < end; i++)
            {
                var d = _histDetailEntries[i];
                ListViewItem item = new ListViewItem(d.name);
                item.SubItems.Add(d.weight);
                item.SubItems.Add(d.unitPrice);
                item.SubItems.Add(d.total);
                historyDetailList.Items.Add(item);
            }

            labelHistDetailPage.Text = $"{_histDetailPage + 1} / {pageCount}";
            btnHistDetailPrev.Enabled = _histDetailPage > 0;
            btnHistDetailNext.Enabled = _histDetailPage < pageCount - 1;
        }

        private void btnHistDetailPrev_Click(object sender, EventArgs e) { _histDetailPage--; ShowHistDetailPage(); }
        private void btnHistDetailNext_Click(object sender, EventArgs e) { _histDetailPage++; ShowHistDetailPage(); }

        private void historyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (historyList.SelectedItems.Count == 0) return;
            string file = (string)historyList.SelectedItems[0].Tag;
            try
            {
                string json = System.IO.File.ReadAllText(file);
                Reciept r = Newtonsoft.Json.JsonConvert.DeserializeObject<Reciept>(json);
                if (r == null) return;

                if (r.mwst == 0 && r.sum > 0)
                    r.mwst = Decimal.Round(r.sum * 0.1m, 2);

                _histDetailEntries = new List<HistDetailEntry>();
                foreach (Article a in r.articles)
                {
                    string weight = a.product.piecePrice
                        ? Decimal.Round(a.Weight, 0).ToString()
                        : Decimal.Round(a.Weight, 2).ToString();
                    _histDetailEntries.Add(new HistDetailEntry
                    {
                        name = a.product.name,
                        weight = $"{weight} {(a.product.piecePrice ? "stk" : "kg")}",
                        unitPrice = $"{Decimal.Round(a.product.price, 2):0.00} €",
                        total = $"{Decimal.Round(a.price, 2):0.00} €"
                    });
                }
                _histDetailPage = 0;
                ShowHistDetailPage();

                historyInfoLabel.Text = $"MwSt 10%: {r.mwst:0.00} €     Summe: {r.sum:0.00} €";
                selectedHistoryReciept = r;
            }
            catch (Exception ex)
            {
                logToBox($"Error loading receipt: {ex.Message}", "error");
            }
        }

        private void historyPrintBtn_Click(object sender, EventArgs e)
        {
            if (selectedHistoryReciept == null) return;
            Printer.Instance.PrintReciept(historyDetailList, selectedHistoryReciept.sum, selectedHistoryReciept.mwst);
        }

    }
}