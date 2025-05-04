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

        public Reciept()
        {
            articles = new List<Article>();

        }

        internal void add(Article a)
        {
            articles.Add(a);

            MTT mtt = (MTT)Application.OpenForms["MTT"];
            mtt.refreshReciept();
        }
    }
}