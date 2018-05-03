using Documentation.Data.DAL.Intefraces;
using Documentation.Data.Entities;
using Documentation.Ground;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Documentation.Data.DAL.Implementation
{
    public class DocumentationContext : DbContext, IDocumentationContext
    {
        public DocumentationContext()
            : base(Configurations.DocumentationConnectionString)
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
        public override int SaveChanges()
        {
            foreach (var ent in this.ChangeTracker.Entries().Where(p => p.State == System.Data.Entity.EntityState.Added || p.State == System.Data.Entity.EntityState.Deleted || p.State == System.Data.Entity.EntityState.Modified))
            {
                if (ent.Entity is Base)
                {
                    ((Base)ent.Entity).Updator = Guid.Parse(HttpContext.Current.User.GetUserId());
                    if (ent.State == EntityState.Added)
                    {
                        ((Base)ent.Entity).Creator = Guid.Parse(HttpContext.Current.User.GetUserId());
                    }
                    else
                    {
                        ((Base)ent.Entity).UpdatedOn = DateTime.Now;
                    }
                }
            }
            return base.SaveChanges();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Documentation.Data.Entities.Type> Types { get; set; }

        public void Dispose()
        {
            base.Dispose();
        }

        Database IDocumentationContext.Database()
        {
            return base.Database;
        }
        public T Add<T>(T entity) where T : class
        {
            return base.Set<T>().Add(entity);
        }
        public T Find<T>(object key) where T : class
        {
            return base.Set<T>().Find(key);
        }
        public void SetValues<T>(T oldEntity, T newEntity) where T : class
        {
            base.Entry<T>(oldEntity).CurrentValues.SetValues(newEntity);
        }
        public int ExecuteSQL(string sqlQuery, object[] parameters)
        {
            return base.Database.ExecuteSqlCommand(sqlQuery, parameters);
        }
        public T Delete<T>(T entity) where T : class
        {
            if (base.Entry<T>(entity).State == EntityState.Detached)
            {
                base.Set<T>().Attach(entity);
            }
            return base.Set<T>().Remove(entity);
        }
        public IQueryable<T> GetAll<T>(RowStatus rowStatus = RowStatus.EXISTS, params Expression<Func<T, object>>[] navigationProperties) where T : class
        {
            var all = base.Set<T>().AsQueryable<T>();
            //Apply eager loading
            if(navigationProperties != null)
                foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                    all = all.Include<T, object>(navigationProperty);

            if (typeof(T).IsSubclassOf(typeof(Base)))
            {
                switch (rowStatus)
                {
                    case RowStatus.DELETED:
                        return all.Where("IsDeleted", true);
                    case RowStatus.EXISTS:
                    default:
                        return all.Where("IsDeleted", false);
                }
            }
            return all;
        }
        public IQueryable<T> FindBy<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] navigationProperties) where T : class
        {
            var result = base.Set<T>().Where(predicate);
            if(navigationProperties != null)
                //Apply eager loading
                foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                    result = result.Include<T, object>(navigationProperty);
            return result;
        }
    }
}
