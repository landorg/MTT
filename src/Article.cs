namespace MTT
{
    internal class Article
    {
        public decimal weight;
        public Product product;
        public decimal price;
        public Article(Product p, decimal weight)
        {
            this.product = p;
            this.weight = weight;
            this.price = weight * p.Price;

        }
    }
}