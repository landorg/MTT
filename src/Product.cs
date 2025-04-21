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
        public Product(string name, float price)
        {
            //ID = iD;
            Name = name;
            Price = price;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }

        public bool Equals(Product other)
        {
            if (this.Name == other.Name)
                return true;
            else
                return false;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

    }
}
