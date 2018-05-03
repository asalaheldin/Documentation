using Documentation.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Documentation.Data.DAL.Intefraces
{
    public interface IDocumentationContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Document> Documents { get; set; }
        DbSet<Documentation.Data.Entities.Type> Types { get; set; }
        void Dispose();
        int SaveChanges();
        Database Database();
        T Add<T>(T entity) where T : class;
        T Find<T>(object key) where T : class;
        void SetValues<T>(T oldEntity, T newEntity) where T : class;
        int ExecuteSQL(string sqlQuery, object[] parameters);
        T Delete<T>(T entity) where T : class;
        IQueryable<T> GetAll<T>(RowStatus rowStatus = RowStatus.EXISTS, params Expression<Func<T, object>>[] navigationProperties) where T : class;
        IQueryable<T> FindBy<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] navigationProperties) where T : class;
    }
}
