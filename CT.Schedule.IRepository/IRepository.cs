using CT.Schedule.Domain;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace CT.Schedule.IRepository
{
    public interface IRepository<T, T1> where T : IAggregate<T1>
    {
        bool Save(T entity);        

        T FindOne(T1 id);

        IQueryable<T> Find(Expression<Func<T, bool>> query );

        bool Remove(T1 id);
    }
}
