namespace GeekShop.Domain
{
    public interface ICRUD
    {
        void Add(Product product);
        IEnumerable<Product> GetAll();
        Product Get(int id);
        void Delete(int id);
    }
}
