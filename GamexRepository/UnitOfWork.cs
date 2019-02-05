using GamexEntity;
using System;

namespace GamexRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        private GamexContext dbContext;

        public UnitOfWork(GamexContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                    dbContext = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int SaveChanges()
        {
            return dbContext.SaveChanges();
        }
    }
}
