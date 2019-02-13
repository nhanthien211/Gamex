﻿using System;
using GamexEntity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

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

        public IEnumerable<T> GetList(
            Expression<Func<T, bool>> filter = null,
            params Expression<Func<T, object>>[] paths)
        {
            IQueryable<T> query = dbSet;
            if (paths != null && paths.Length > 0)
            {
                foreach (var path in paths)
                {
                    query = query.Include(path);
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            
            return query.ToList();
        }

        public T GetSingle(
            Expression<Func<T, bool>> filter = null,
            params Expression<Func<T, object>>[] paths)
        {
            IQueryable<T> query = dbSet;

            if (paths != null && paths.Length > 0)
            {
                foreach (var path in paths)
                {
                    query = query.Include(path);
                }
            }

            if (filter != null)
            {
                return query.FirstOrDefault(filter);
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        } 

        public IEnumerable<TResult> GetListProjection<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> filter = null,
            params Expression<Func<T, object>>[] paths)
        {
            IQueryable<T> query = dbSet;

            if (paths != null && paths.Length > 0)
            {
                foreach (var path in paths)
                {
                    query = query.Include(path);
                }
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.Select(selector).ToList();
        }

        public TResult GetSingleProjection<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> filter = null,
            params Expression<Func<T, object>>[] paths)
        {
            IQueryable<T> query = dbSet;

            if (paths != null && paths.Length > 0)
            {
                foreach (var path in paths)
                {
                    query = query.Include(path);
                }
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.Select(selector).FirstOrDefault();
        }

        public T GetById(object id)
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
        }
    }
}
