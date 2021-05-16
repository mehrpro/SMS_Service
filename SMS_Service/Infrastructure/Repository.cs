using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SMS_Service.Infrastructure
{
    public abstract class Repository<TEntity> : IRepository<TEntity>, IDisposable where TEntity : class
    {
        private readonly DbContext db;

        private DbSet<TEntity> dbSet
        {
            get
            {
                return db.Set<TEntity>();

            }
        }

        public Repository(DbContext dbContext)
        {
            this.db = dbContext;
        }

        public void Delete(object Id)
        {
            var entity = GetById(Id);
            if (entity == null)
                throw new ArgumentException("on entity");
            dbSet.Remove(entity);
        }

        public void Delete(TEntity entity)
        {
            dbSet.Remove(entity);
        }

        public void Delete(Expression<Func<TEntity, bool>> where)
        {
            IEnumerable<TEntity> objects = dbSet.Where(where).AsEnumerable();
            foreach (TEntity obj in objects)
            {
                dbSet.Remove(obj);
            }
        }

        public TEntity Get(Expression<Func<TEntity, bool>> where)
        {
            return dbSet.Where(where).FirstOrDefault();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return dbSet.AsEnumerable();
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where)
        {
            return await dbSet.Where(where).FirstOrDefaultAsync();
        }

        public TEntity GetById(object Id)
        {
            return dbSet.Find(Id);
        }

        public IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> where)
        {
            return dbSet.Where(where).ToList();
        }

        public async Task<List<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> where)
        {
            return await dbSet.Where(where).ToListAsync();
        }

        public void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentException("no entity");

            //dbSet.Update(entity);
        }

        #region Dispose

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
