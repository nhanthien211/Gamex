using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamexEntity;
using GamexRepository;

namespace GamexTest
{
    class Program
    {
        static void Main(string[] args)
        {
            GamexContext dbContext = new GamexContext();
            Repository<AspNetUsers> repo = new Repository<AspNetUsers>(dbContext);
            repo.
        }
    }
}
