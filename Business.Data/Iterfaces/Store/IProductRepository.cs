using Business.Data.Models;

namespace Business.Data.Iterfaces.Store
{
    public interface IProductRepository<T>:IRepository<Product> where T : class,IBaseEntity
    {
        /// <summary>
        /// Получить объект(игру/подписку/аддон) через продукт
        /// </summary>
        /// <returns></returns>
        Task<T> GetTypeEntity();
    }
}
