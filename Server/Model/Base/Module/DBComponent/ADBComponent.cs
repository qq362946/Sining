using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sining.Module
{
    public abstract class ADBComponent : Component
    {
        public abstract void Init();
        public abstract void BeginTran();
        public abstract void RollbackTran();
        public abstract void CommitTran();
        public abstract STask<long> Count<T>(string collection = null) where T : class;

        public abstract STask<long> Count<T>(Expression<Func<T, bool>> filter, string collection = null)
            where T : class;

        public abstract STask<bool> Exist<T>(string collection = null) where T : class;

        public abstract STask<bool> Exist<T>(Expression<Func<T, bool>> filter, string collection = null)
            where T : class;

        public abstract STask<long> UpdateRange<T>(List<T> range) where T : class, new();
        public abstract STask<T> Query<T>(long id, string collection = null) where T : Component;

        public abstract STask<List<T>> QueryByPage<T>(Expression<Func<T, bool>> filter, int pageIndex, int pageSize,
            string collection = null) where T : class;

        public abstract STask<T> First<T>(Expression<Func<T, bool>> filter, string collection = null)
            where T : class;

        public abstract STask<List<T>> Query<T>(Expression<Func<T, bool>> filter, string collection = null)
            where T : class;

        public abstract STask<List<T>> Query<T>(long taskId, Expression<Func<T, bool>> filter, string collection = null)
            where T : class;

        public abstract STask Query(long id, List<string> collectionNames, List<Component> result);
        public abstract STask<List<T>> QueryJson<T>(string json, string collection = null) where T : Component;

        public abstract STask<List<T>> QueryJson<T>(long taskId, string json, string collection = null)
            where T : Component;
        public abstract STask Insert<T>(T entity, string collection = null) where T : Component, new();
        public abstract STask InsertBatch<T>(IEnumerable<T> list, string collection = null) where T : Component;
        public abstract STask Save<T>(T entity, string collection = null) where T : Component, new();
        public abstract STask Save<T>(long taskId, T entity, string collection = null) where T : Component, new();
        public abstract STask Save(long id, List<Component> entities);

        public abstract SVoid SaveNotWait<T>(T entity, long taskId = 0, string collection = null)
            where T : Component, new();

        public abstract STask<long> Remove<T>(long id, string collection = null) where T : Component, new();
        public abstract SVoid RemoveNoWait<T>(long id, string collection = null) where T : Component, new();
        public abstract STask<long> Remove<T>(long taskId, long id, string collection = null) where T : Component, new();

        public abstract STask<long> Remove<T>(Expression<Func<T, bool>> filter, string collection = null)
            where T : Component, new();

        public abstract STask<long> Remove<T>(long taskId, Expression<Func<T, bool>> filter, string collection = null)
            where T : Component, new();

        public abstract SVoid RemoveNoWait<T>(Expression<Func<T, bool>> filter, string collection = null)
            where T : Component, new();
    }
}