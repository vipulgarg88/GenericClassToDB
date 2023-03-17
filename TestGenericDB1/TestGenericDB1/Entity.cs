using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;



namespace TestGenericDB1
{

    // C# Code
    public interface IEntity<T> where T : class
    {
        IList<T> List();

        IList<T> List(int? page, int? pageSize, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> sort);

        void Add(T item);

        T Get(Int64 Id);

        void Update(T item);

        bool Delete(T Item);

    }

    public class Entity<T> : IEntity<T> where T : class
    {

        public string fileOrServerOrConnection = "";
        // Inserts the data values in the LINQ to SQL generated class.
        public virtual void Add(T item)
        {
            using (DataContext db = new DataContext(fileOrServerOrConnection))
            {
                db.GetTable<T>().InsertOnSubmit(item);
                db.SubmitChanges();
            }
        }

        // Returns the list of the object of LINQ to SQL Class
        public virtual IList<T> List()
        {
            using (DataContext db = new DataContext(fileOrServerOrConnection))
            {
                return db.GetTable<T>().ToList();
            }
        }

        // Returns the list of the object of LINQ to SQL Class on the basis of parameters
        public virtual IList<T> List(int? page, int? pageSize, System.Linq.Expressions.Expression<Func<T, bool>> predicate, System.Linq.Expressions.Expression<Func<T, object>> sort)
        {
            var result = this.List().AsQueryable();

            if (predicate != null)
                result = result.Where(predicate);

            if (sort != null)
                result = result.OrderBy(sort);

            if (page.HasValue && pageSize.HasValue)
                result = result.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);

            return result.ToList();
        }

        // Returns the object on the basis of the objects ID, if ID datatype is Int64.
        public virtual T Get(Int64 value)
        {
            return Get(typeof(System.Int64), "ID", value);
        }

        // Returns the object on the basis of the objects ID, if ID datatype is String.
        public virtual T Get(string value)
        {
            return Get(typeof(System.String), "ID", value);
        }

        // Returns the object on the basis of the objects property column.
        public virtual T Get(Type propertyType, string propertyName, object propertyValue)
        {
            T result = null;
            using (DataContext db = new DataContext(fileOrServerOrConnection))
            {
                IQueryable<T> queryableData = db.GetTable<T>().AsQueryable<T>();
                if (queryableData != null)
                {
                    ParameterExpression pe = Expression.Parameter(typeof(T), "entity");

                    Expression left = Expression.Property(pe, GetPropertyInfo(propertyName));
                    Expression right = Expression.Constant(propertyValue, propertyType);

                    Expression predicateBody = Expression.Equal(left, right);

                    MethodCallExpression whereCallExpression = Expression.Call(
                        typeof(Queryable),
                        "Where",
                        new Type[] { queryableData.ElementType },
                        queryableData.Expression,
                        Expression.Lambda<Func<T, bool>>(predicateBody, new ParameterExpression[] { pe }));

                    IQueryable<T> results = queryableData.Provider.CreateQuery<T>(whereCallExpression);
                    foreach (T item in results)
                    {
                        result = item;
                        break;
                    }
                }
                return result;
            }
        }

        // Updates the data values in the LINQ to SQL generated class.
        public virtual void Update(T item)
        {
            using (DataContext db = new DataContext(fileOrServerOrConnection))
            {
                db.GetTable<T>().Attach(item, true);
                db.SubmitChanges();
            }
        }

        // Deletes the data values in the LINQ to SQL generated class.
        public virtual bool Delete(T Item)
        {
            using (DataContext db = new DataContext(fileOrServerOrConnection))
            {
                db.GetTable<T>().DeleteOnSubmit(Item);
                db.SubmitChanges();
                return true;
            }
        }

        private PropertyInfo GetPropertyInfo(string propertyName)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            PropertyInfo result = null;
            foreach (PropertyInfo pi in properties)
            {
                if (pi.Name.Equals(propertyName))
                {
                    result = pi;
                    break;
                }
            }
            return result;
        }

    }

}
