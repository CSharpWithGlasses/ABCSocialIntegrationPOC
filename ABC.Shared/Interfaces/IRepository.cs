using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABC.Shared.Interfaces.Repository
{
    public interface IRepository
    {
        Task<List<T>> Insert<T>(List<T> inputModel) where T : class;
        Task Update<T>(List<T> inputModel) where T : class;
        Task Delete<T>(List<T> inputModel) where T : class;
        IQueryable<T> GetAll<T>(bool tracking = false) where T : class;
        Task<List<T>> SqlQuery<T>(string query) where T : class;
        Task<int> Save();
        Task BeginTransaction();
        Task RollbackTransaction();
        
    }
}
