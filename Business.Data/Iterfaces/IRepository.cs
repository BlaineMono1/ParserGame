using Business.Data.BaseEntities;

namespace Business.Data.Iterfaces
{
    public interface IRepository<T> where T : IBaseEntity
    {
        Task<List<T>> GetAllList();
        Task<T?> GetById(Guid id);
        Task Update(T entity);
        Task Delete(Guid id);
        Task Add(T entity);
    }
}
