using System.Collections.Generic;

namespace GamexRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetByID(object id);
        void Insert(T entity);
        void Delete(object id);
        void Update(T entity);
    }
}
