namespace MTTApp
{
    internal class Article
    {
        public Article() { }

        private decimal weight;
        public decimal Weight
        {
            set
            {
                weight = value;
                if (product != null)
                    price = weight * product.price;
            }
            get
            {
                return weight;
            }
        }
        public Product product;
        public decimal price;
        public Article(Product p, decimal weight)
        {
            this.product = p;
            this.weight = weight;
            this.price = weight * p.price;

        }
    }
}