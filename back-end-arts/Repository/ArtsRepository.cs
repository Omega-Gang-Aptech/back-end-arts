using Ardalis.Specification;
using back_end_arts.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_arts.Repository
{
    public class ArtsRepository<T> : IArtsRepository<T> where T : class
    {
        public readonly ArtsDbWithKeyContext _db;
        public ArtsRepository(ArtsDbWithKeyContext db)
        {
            _db = db;
        }

        public async Task Delete(T entity)
        {
            _db.Set<T>().Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<T> GetById<TId>(TId id)
        {
            return await _db.Set<T>().FindAsync(id);
        }

        public async Task Insert(T entity)
        {
            _db.Set<T>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public Task<List<T>> ListAll()
        {
            return _db.Set<T>().ToListAsync();
        }

        public async Task<T> Update(T entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return entity;
        }
        //public async Task<T> GetByName<TName>(TName name)
        //{
        //    return await _db.Set<T>().FindAsync(name);
        //}

        ///
        private IQueryable<T> ApplySpecification<T>(ISpecification<T> spec) where T : class
        {
            var evaluator = new SpecificationEvaluator();
            return evaluator.GetQuery(_db.Set<T>().AsQueryable(), spec);
        }

        public async Task<T> GetAsyncSpec(ISpecification<T> spec)
        {
            var specificationResult = ApplySpecification(spec);
            return await specificationResult.FirstOrDefaultAsync();
        }

        public async Task<List<T>> ListAsyncSpec(ISpecification<T> spec)
        {
            var specificationResult = ApplySpecification(spec);
            return await specificationResult.ToListAsync();
        }

        public Task<T> GetByName<TName>(TName name)
        {
            throw new NotImplementedException();
        }

    }
    public class SpecificationEvaluator : ISpecificationEvaluator
    {
        public static SpecificationEvaluator Default { get; } = new SpecificationEvaluator();

        private readonly List<IEvaluator> evaluators = new List<IEvaluator>();

        public SpecificationEvaluator()
        {
            this.evaluators.AddRange(new IEvaluator[]
            {
                WhereEvaluator.Instance,
                OrderEvaluator.Instance,
                PaginationEvaluator.Instance,

            });
        }
        public SpecificationEvaluator(IEnumerable<IEvaluator> evaluators)
        {
            this.evaluators.AddRange(evaluators);
        }

        /// <inheritdoc/>
        public virtual IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> query, ISpecification<T, TResult> specification) where T : class
        {
            query = GetQuery(query, (ISpecification<T>)specification);

            return query.Select(specification.Selector);
        }

        /// <inheritdoc/>
        public virtual IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification, bool evaluateCriteriaOnly = false) where T : class
        {
            var evaluators = evaluateCriteriaOnly ? this.evaluators.Where(x => x.IsCriteriaEvaluator) : this.evaluators;

            foreach (var evaluator in evaluators)
            {
                query = evaluator.GetQuery(query, specification);
            }
            return query;
        }
    }
}
