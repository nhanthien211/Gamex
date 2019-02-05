using GamexEntity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GamexRepository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly GamexContext context;
        private DbSet<T> dbSet;

        public Repository(GamexContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }

        public T GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public void Insert(T entity)
        {
            dbSet.Add(entity);
        }

        public void Delete(object id)
        {
            T entity = dbSet.Find(id);
            if (context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }


    }
}
