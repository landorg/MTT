using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using static System.Windows.Forms.LinkLabel;

namespace MTTApp
{
    internal class Reciept
    {



        public List<Article> articles;
        public decimal sum;
        public decimal mwst;

        public Reciept()
        {
            articles = new List<Article>();
            sum = 0;
            mwst = 0;
        }

        private void reSum()
        {
            sum = 0;
            foreach (Article b in articles)
            {
                sum += b.price;
            }
            mwst = Decimal.Round(sum * 0.1m, 2);
        }

        internal void add(Article a)
        {
            if (a.product.piecePrice)
            {
                Article existingArticle = null;
                foreach (Article b in articles )
                {
                    if (b.product.name == a.product.name)
                    {
                        existingArticle = b;
                        break;
                    }

                }
                if (existingArticle != null)
                {
                    existingArticle.Weight += 1;
                } else
                {
                    articles.Add(a);
                }
            }
            else
            {
                articles.Add(a);
            }

            this.reSum();

            MTT mtt = (MTT)Application.OpenForms["MTT"];
            mtt.refreshReciept();
        }

        internal void remove(int selectedArticle)
        {
            articles.Remove(articles[selectedArticle]);
            this.reSum();

            MTT mtt = (MTT)Application.OpenForms["MTT"];
            mtt.refreshReciept();
        }

        internal void save()
        {
            MTT mtt = (MTT)Application.OpenForms["MTT"];

            string ts = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string basename = $"C:/MTT/Rechnungen/Rechnung-{ts}";

            mtt.logToBox(basename + ".txt");

            FileStream fs = System.IO.File.Create(basename + ".txt");
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(json);
                sw.Close();
            }

            string dateDisplay = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            using (StreamWriter sw = new StreamWriter(basename + ".csv", false, new System.Text.UTF8Encoding(true)))
            {
                sw.WriteLine("Datum;Produkt;Menge;Einheit;Preis/Einheit;Gesamtpreis");
                bool first = true;
                foreach (Article a in articles)
                {
                    string datumCol = first ? dateDisplay : "";
                    string unit = a.product.piecePrice ? "stk" : "kg";
                    string weightStr = a.product.piecePrice
                        ? Decimal.Round(a.Weight, 0).ToString()
                        : Decimal.Round(a.Weight, 2).ToString("0.00").Replace('.', ',');
                    string unitPrice = Decimal.Round(a.product.price, 2).ToString("0.00").Replace('.', ',');
                    string linePrice = Decimal.Round(a.price, 2).ToString("0.00").Replace('.', ',');
                    sw.WriteLine($"{datumCol};{a.product.name};{weightStr};{unit};{unitPrice};{linePrice}");
                    first = false;
                }
                sw.WriteLine($";MwSt 10%;;;;{Decimal.Round(mwst, 2).ToString("0.00").Replace('.', ',')}");
                sw.WriteLine($";Summe;;;;{Decimal.Round(sum, 2).ToString("0.00").Replace('.', ',')}");
            }
        }
    }
}