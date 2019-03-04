using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GamexRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(object id);
        void Insert(T entity);
        void Delete(T entity);
        void Update(T entity);

        IEnumerable<T> GetList(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] paths);
        T GetSingle(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] paths);
        IEnumerable<TResult> GetPagingProjection<TResult, TKey>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> filter = null, Expression<Func<T, TKey>> sort = null, string sortColumnDirection = null, int take = 0, int skip = 0, params Expression<Func<T, object>>[] paths);
        IEnumerable<TResult> GetListProjection<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] paths);
        TResult GetSingleProjection<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] paths);

    }
}
