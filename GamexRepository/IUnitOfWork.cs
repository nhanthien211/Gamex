using System;

namespace GamexRepository
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
    }
}
