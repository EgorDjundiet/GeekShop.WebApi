﻿using GeekShop.Domain;

public interface IProductRepository
{
    Task Add(Product product);
    Task<IEnumerable<Product>> GetAll();
    Task<Product?> Get(int id);
    Task Delete(int id);
}