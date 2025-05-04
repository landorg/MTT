using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public static void remove(string productName)
        {
            products.Remove(new Product(productName));

            MTT mtt = (MTT)Application.OpenForms["MTT"];

            mtt.refreshDbList();
            DB.save();
        }

        public static void add(Product product)
        {
            bool contains = false;
            foreach (Product p in products)
            {
                if (product.name == p.name)
                {
                    contains = true; break;
                }
            }

            if (contains)
            {
                products.Remove(product);
            }
            products.Add(product);

            MTT mtt = (MTT)Application.OpenForms["MTT"];

            mtt.refreshDbList();
            DB.save();
        }

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

            //MTT mtt = (MTT)Application.OpenForms["MTT"];

            if (!System.IO.File.Exists(filename))
            {
                save();
            }

            StreamReader sr = new StreamReader(filename);
            string jsonString = sr.ReadToEnd();
            products = JsonConvert.DeserializeObject<List<Product>>(jsonString);
            sr.Close();

            //eventBox.Items.Insert(0, productsList.ToString());
        }
   }
}