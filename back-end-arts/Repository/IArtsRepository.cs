using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_arts.Repository
{
    public interface IArtsRepository<T> where T: class
    {
        Task<T> GetById<TId>(TId id);
        Task<T> GetByName<TName>(TName name);
        Task<List<T>> ListAll();
        Task Insert(T entity);
        Task Delete(T entity);
        Task<T> Update(T entity);
        //Task<T> CheckPass(T entity);
        Task<List<T>> ListAsyncSpec(ISpecification<T> spec);
        Task<T> GetAsyncSpec(ISpecification<T> spec);
    }
}
