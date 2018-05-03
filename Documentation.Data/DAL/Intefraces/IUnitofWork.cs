using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Documentation.Data.DAL.Intefraces
{
    public interface IUnitofWork : IDisposable
    {
        int SaveChanges();
        void BeginTransaction();
        void RollBack();
        void Commit();
        T Add<T>(T entity) where T : class;
        T Find<T>(object key) where T : class;
        void SetValues<T>(T oldEntity, T newEntity) where T : class;
        int ExecuteSQLCommand(string sqlQuery, object[] parameters);
        T Delete<T>(T entity) where T : class;
        IQueryable<T> GetAll<T>(RowStatus rowStatus = RowStatus.EXISTS, params Expression<Func<T, object>>[] navigationProperties) where T : class;
        IQueryable<T> FindBy<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] navigationProperties) where T : class;
    }
}
