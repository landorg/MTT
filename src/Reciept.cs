using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using static System.Windows.Forms.LinkLabel;

namespace MTT
{
    internal class Reciept
    {



        public List<Article> articles;
        public decimal sum;

        public Reciept()
        {
            articles = new List<Article>();
            sum = 0;
        }

        private void reSum()
        {
            sum = 0;
            foreach (Article b in articles)
            {
                sum += b.price;
            }
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

            string filename = $"C:/MTT/Rechnungen/Rechnung-{System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.txt";

            mtt.logToBox(filename);

            FileStream fs = System.IO.File.Create(filename);
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(json);
                sw.Close();
            }
        }
    }
}