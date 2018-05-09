using Documentation.Data.DAL.Intefraces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Documentation.Data.DAL.Implementation
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly Lazy<IUnitofWork> _unitofWork;
        public Repository(Lazy<IUnitofWork> unitofWork)
        {
            _unitofWork = unitofWork;
        }
        private IUnitofWork UnitofWork => _unitofWork.Value;
        public int Insert(T entity)
        {
            try
            {
                UnitofWork.Add<T>(entity);
                return UnitofWork.SaveChanges();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public int Update(T entity, object key)
        {
            try
            {
                T existing = UnitofWork.Find<T>(key) as T;
                if (existing != null)
                {
                    UnitofWork.SetValues<T>(existing, entity);
                    return UnitofWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
            return -1;
        }

        public int ExecuteSQL(string sqlQuery, object[] parameters)
        {
            try
            {
                return UnitofWork.ExecuteSQLCommand(sqlQuery, parameters);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }


        private int DeletePhysical(T entity)
        {

            try
            {
                UnitofWork.Delete<T>(entity);
                return UnitofWork.SaveChanges();
            }
            catch (Exception ex)
            {
                return -1;
            }

        }
        public T GetById(object id)
        {
            try
            {
                return UnitofWork.Find<T>(id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int Delete(object key, bool physiaclDelete = true)
        {
            try
            {
                T row = GetById(key);
                if (row != null)
                {
                    if (physiaclDelete == false)
                    {
                        // (row as EntityBase).IsDeleted = true;
                        row.GetType().GetProperty("IsDeleted").SetValue(row, true);
                        return Update(row, key);
                    }
                    else
                    {
                        return DeletePhysical(row);
                    }
                }
                return UnitofWork.SaveChanges();
            }
            catch (Exception ex)
            {
                return -1;
            }

        }

        public IQueryable<T> GetAll(RowStatus rowstatus = RowStatus.EXISTS, params Expression<Func<T, object>>[] navigationProperties)
        {
            try
            {
                return UnitofWork.GetAll<T>(rowstatus, navigationProperties);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] navigationProperties)
        {
            try
            {
                return UnitofWork.FindBy(predicate, navigationProperties);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
