using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SMS_Service.Infrastructure
{
   public interface IRepository<TEntity> where TEntity : class
   {
       //-------Definition Public Functions Models-----------//
       void Insert(TEntity entity);
       void Update(TEntity entity);

       void Delete(object Id);
       void Delete(TEntity entity);
       void Delete(Expression<Func<TEntity, bool>> where); //lambda expression

       TEntity GetById(object Id);
       IEnumerable<TEntity> GetAll();
       TEntity Get(Expression<Func<TEntity, bool>> where);
       IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> where);

       //---------------Async Functions----------------//
       Task<List<TEntity>> GetAllAsync();
       Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where);
       Task<List<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> where);
   }
}
