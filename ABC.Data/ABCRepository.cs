using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABC.Shared.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;



namespace ABC.Data
{
    public class ABCRepository : IRepository
    {
        private ABCDbContext _context;
        
        public ABCRepository (ABCDbContext context)
        {
            _context = context;
        }

        public async Task Delete<T>(List<T> inputModels) where T : class
        {
            await Task.Run(() => _context.RemoveRange(inputModels));
        }

        public IQueryable<T> GetAll<T>(bool tracking = false) where T : class
        {
            return tracking ? _context.Set<T>() : _context.Set<T>().AsNoTracking();
        }

        public async Task<List<T>> Insert<T>(List<T> inputModels) where T : class
        {
            await _context.Set<T>().AddRangeAsync(inputModels);

            return inputModels;
            //need to use generic dml result T
        }

        public async Task Update<T>(List<T> inputModels) where T : class
        {
            await Task.Run(() => _context.Set<T>().UpdateRange(inputModels));
        }

        public async Task<List<T>> SqlQuery<T>(string query) where T : class
        {
            var result = await _context.Set<T>().FromSqlRaw(query).ToListAsync();
            return result;
        }

        public async Task<int> Save()
        {
           return await _context.SaveChangesAsync();
        }

        public async Task BeginTransaction()
        {
            if(_context.Database.CurrentTransaction == null)
            {
                await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task RollbackTransaction()
        {
            if(_context.Database.CurrentTransaction != null)
            {
                await _context.Database.RollbackTransactionAsync();
            }

        }
    }
}
