using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
            if (product.ID != 0)
            {
                int idx = products.FindIndex(p => p.ID == product.ID);
                if (idx >= 0)
                    products[idx] = product;
                else
                    products.Add(product);
            }
            else
            {
                int existingIdx = products.FindIndex(p => p.name == product.name);
                if (existingIdx >= 0)
                    products.RemoveAt(existingIdx);
                product.ID = products.Count > 0 ? products.Max(p => p.ID) + 1 : 1;
                products.Add(product);
            }

            MTT mtt = (MTT)Application.OpenForms["MTT"];

            mtt.refreshDbList();
            DB.save();
        }

        public static void save()
        {
            FileStream fs = System.IO.File.Create(filename);
            string json = JsonConvert.SerializeObject(products, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(json);
                sw.Close();
            }

            using (StreamWriter sw = new StreamWriter("C:/MTT/products.csv", false, new System.Text.UTF8Encoding(true)))
            {
                sw.WriteLine("Name;Preis;Einheit;Gruppe");
                foreach (Product p in products)
                {
                    string preis = p.price.ToString("0.00").Replace('.', ',');
                    string unit = p.piecePrice ? "stk" : "kg";
                    string group = p.group ?? "";
                    sw.WriteLine($"{p.name};{preis};{unit};{group}");
                }
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

            // Assign IDs to products loaded from old JSON files that had none
            int nextId = products.Count > 0 ? products.Max(p => p.ID) + 1 : 1;
            foreach (var p in products)
                if (p.ID == 0) p.ID = nextId++;
            if (nextId > 1) save(); // persist newly assigned IDs

            //eventBox.Items.Insert(0, productsList.ToString());
        }
   }
}