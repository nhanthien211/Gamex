using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GamexRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetListByCondition(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = "");

        T GetSingle(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = "");
        T GetByID(object id);
        void Insert(T entity);
        void Delete(object id);
        void Update(T entity);
    }
}
