using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using static System.Windows.Forms.LinkLabel;

namespace MTT
{
    internal static class DB
    {

        private const String filename = "C:/MTT/products.json";

        public static List<Product> products = new List<Product>();

        public static void save()
        {
            FileStream fs = System.IO.File.Create(filename);
            string json = JsonConvert.SerializeObject(products);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(json);
                sw.Close();
            }
        }
        public static void load()
        {

            MTT mtt = (MTT)Application.OpenForms["MTT"];

            if (!System.IO.File.Exists(filename))
            {
                save();
            }

            StreamReader sr = new StreamReader(filename);
            string jsonString = sr.ReadToEnd();
            products = JsonConvert.DeserializeObject<List<Product>>(jsonString);

            //eventBox.Items.Insert(0, productsList.ToString());
        }
    }
}