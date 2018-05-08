using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Documentation.Data.DAL.Intefraces
{
    public interface IRepository<T>
    {
        int Insert(T entity);
        int Update(T entity, object key);
        int ExecuteSQL(string sqlQuery, object[] parameters);
        int Delete(object key, bool physiaclDelete = true);
        IQueryable<T> GetAll(RowStatus rowstatus = RowStatus.EXISTS, params Expression<Func<T, object>>[] navigationProperties);
        T GetById(object id);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] navigationProperties);
    }
}
