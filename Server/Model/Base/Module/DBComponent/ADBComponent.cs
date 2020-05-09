using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sining.Module
{
    public abstract class ADBComponent : Component
    {
        public string ConnectionString;
        public string DbName;
        public abstract void Init();
        public abstract T GetConnection<T>() where T : class;
        public abstract T CreateConnection<T>() where T : class;
        public abstract STask<long> Count<T>(string collection = null) where T : class;
        public abstract STask<long> Count<T>(Expression<Func<T, bool>> filter, string collection = null)
            where T : class;
        public abstract STask<bool> Exist<T>(string collection = null) where T : class;
        public abstract STask<bool> Exist<T>(Expression<Func<T, bool>> filter, string collection = null)
            where T : class;

        public abstract STask<long> UpdateRange<T>(object transactionSession, List<T> range) where T : class, new();
        public abstract STask<long> UpdateRange<T>(List<T> range) where T : class, new();
        public abstract STask<T> Query<T>(long id, string collection = null) where T : Component;
        public abstract STask<List<T>> QueryByPage<T>(Expression<Func<T, bool>> filter, int pageIndex, int pageSize,
            string collection = null) where T : class;
        public abstract STask<List<T>> QueryByPageOrderBy<T>(Expression<Func<T, bool>> filter, int pageIndex,
            int pageSize,
            Expression<Func<T, object>> orderByExpression, bool isAsc = true, string collection = null);
        public abstract STask<T> First<T>(Expression<Func<T, bool>> filter, string collection = null)
            where T : class;
        public abstract STask<List<T>> QueryOrderBy<T>(Expression<Func<T, bool>> filter,
            Expression<Func<T, object>> orderByExpression, bool isAsc = true, string collection = null);
        public abstract STask<List<T>> Query<T>(Expression<Func<T, bool>> filter, string collection = null)
            where T : class;
        public abstract STask<List<T>> Query<T>(long taskId, Expression<Func<T, bool>> filter, string collection = null)
            where T : class;
        public abstract STask Query(long id, List<string> collectionNames, List<Component> result);
        public abstract STask<List<T>> QueryJson<T>(string json, string collection = null) where T : Component;
        public abstract STask<List<T>> QueryJson<T>(long taskId, string json, string collection = null)
            where T : Component;
        public abstract STask Insert<T>(T entity, string collection = null) where T : Component, new();
        public abstract STask InsertBatch<T>(IEnumerable<T> list, string collection = null)
            where T : class, new();
        public abstract STask InsertBatch<T>(object transactionSession, IEnumerable<T> list, string collection = null)
            where T : class, new();
        public abstract STask Save<T>(object transactionSession, T entity, string collection = null) where T : Component, new();
        public abstract STask Save<T>(T entity, string collection = null) where T : Component, new();
        public abstract STask Save(long id, List<Component> entities);
        public abstract SVoid SaveNotWait<T>(T entity, string collection = null)
            where T : Component, new();

        public abstract STask<long> Remove<T>(object transactionSession, long id, string collection = null)
            where T : Component, new();
        public abstract STask<long> Remove<T>(long id, string collection = null) where T : Component, new();
        public abstract SVoid RemoveNoWait<T>(long id, string collection = null) where T : Component, new();

        public abstract STask<long> Remove<T>(object transactionSession, Expression<Func<T, bool>> filter,
            string collection = null)
            where T : Component, new();
        public abstract STask<long> Remove<T>(Expression<Func<T, bool>> filter, string collection = null)
            where T : Component, new();
        public abstract SVoid RemoveNoWait<T>(Expression<Func<T, bool>> filter, string collection = null)
            where T : Component, new();
    }
}