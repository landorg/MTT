namespace MTT
{
    internal class Article
    {
        public float weight;
        public Product product;
        public float price;
        public Article(Product p, float weight)
        {
            this.product = p;
            this.weight = weight;
            this.price = weight * p.Price;

        }
    }
}