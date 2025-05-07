using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
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

        internal void refreshDbList()
        {
            dbList.Items.Clear();
            dbList2.Items.Clear();
            foreach (Product p in DB.products)
            {
                ListViewItem item = new ListViewItem(p.name);
                item.SubItems.Add(p.price.ToString());
                item.SubItems.Add(p.piecePrice ? "stk" : "kg");

                dbList.Items.Add(item);
                dbList2.Items.Add((ListViewItem)item.Clone());
            }
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

                removeButton.Enabled = true;
            }
            else
            {
                selectedDBProduct = null;
                piecePriceCheckbox.Checked = false;
                txtName.Text = "";
                txtPreis.Text = "";

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
                Product p = new Product(txtName.Text, piecePriceCheckbox.Checked, decimal.Parse(txtPreis.Text));
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

                item.SubItems.Add(Decimal.Round(a.price, 2).ToString() + " €");

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
        private void dbList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dbList2.SelectedItems.Count == 1)
            {

                string selectedProductName = dbList2.SelectedItems[0].Text;
                selectedProduct = DB.products.Find(x => x.name == selectedProductName);

                if (selectedProduct.piecePrice)
                {
                    addArticleButton.Text = "+ 1 Stück";
                }
                else
                {
                    addArticleButton.Text = "abwiegen";
                }

            } else {
                addArticleButton.Text = "";
            } 

        }


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