using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MTT
{
    internal class Product : IEquatable<Product>
    {
        public Product(string name, bool piecePrice = false, decimal price = 0)
        {
            //ID = iD;
            this.name = name;
            this.price = price;
            this.piecePrice = piecePrice;
        }

        public int ID { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public bool piecePrice { get; set; }

        public bool Equals(Product other)
        {
            if (this.name == other.name)
                return true;
            else
                return false;
        }
    }
}
