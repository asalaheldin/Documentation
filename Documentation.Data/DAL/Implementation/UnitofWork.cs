using Documentation.Data.DAL.Intefraces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Documentation.Data.DAL.Implementation
{
    public class UnitofWork : IUnitofWork
    {
        DbContextTransaction dbContextTransaction = null;
        private readonly Lazy<IDocumentationContext> _context;
        public UnitofWork(Lazy<IDocumentationContext> context)
        {
            _context = context;
        }
        private IDocumentationContext Context => _context.Value;



        public int SaveChanges()
        {
            try
            {
                return Context.SaveChanges();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public void Dispose()
        {
            Context.Dispose();
            GC.SuppressFinalize(this);
        }

        public void BeginTransaction()
        {
            dbContextTransaction = Context.Database().BeginTransaction();
        }

        public void RollBack()
        {
            if (dbContextTransaction != null)
                dbContextTransaction.Rollback();
        }

        public void Commit()
        {
            try
            {
                Context.SaveChanges();
                if (dbContextTransaction != null)
                    dbContextTransaction.Commit();
            }
            catch (Exception ex)
            {
                if (dbContextTransaction != null)
                    dbContextTransaction.Rollback();
            }
        }
        public T Add<T>(T entity) where T : class
        {
            return Context.Add<T>(entity);
        }
        public T Find<T>(object key) where T : class
        {
            return Context.Find<T>(key);
        }
        public void SetValues<T>(T oldEntity, T newEntity) where T : class
        {
            Context.SetValues<T>(oldEntity, newEntity);
        }
        public int ExecuteSQLCommand(string sqlQuery, object[] parameters)
        {
            return Context.ExecuteSQL(sqlQuery, parameters);
        }
        public T Delete<T>(T entity) where T : class
        {
            return Context.Delete<T>(entity);
        }
        public IQueryable<T> GetAll<T>(RowStatus rowStatus = RowStatus.EXISTS, params Expression<Func<T, object>>[] navigationProperties) where T : class
        {
            return Context.GetAll<T>(rowStatus, navigationProperties);
        }
        public IQueryable<T> FindBy<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] navigationProperties) where T : class
        {
            return Context.FindBy<T>(predicate, navigationProperties);
        }
    }
}
