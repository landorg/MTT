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
            articles.Add(a);
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
    }
}