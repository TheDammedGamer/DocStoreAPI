using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DocStoreAPI.Models;

namespace DocStoreAPI.Repositories
{
    public interface IRepository<T> where T : EntityBase
    {
        T GetById(int id);
        IEnumerable<T> List();
        IEnumerable<T> List(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void DeleteById(int id);
        void Delete(T entity);
        void Edit(T entity);
    }

}
