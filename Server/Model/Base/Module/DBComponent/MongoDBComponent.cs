using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Driver;
using Sining.Event;
using Sining.Tools;

namespace Sining.Module
{
    [ComponentSystem]
    public class MongoDBComponentAwakeSystem: AwakeSystem<MongoDBComponent, string, string>
    {
        protected override void Awake(MongoDBComponent self, string connectionString, string dbName)
        {
            self.Awake(connectionString, dbName);
        }
    }
    
    public class MongoDBComponent : ADBComponent
    {
        private MongoClient _mongoClient;
        private IMongoDatabase _mongoDatabase;
        public void Awake(string connectionString, string dbName)
        {
            _mongoClient = new MongoClient(connectionString);
            _mongoDatabase = _mongoClient.GetDatabase(dbName);
        }
        public override void Init() { }
        public override T GetConnection<T>() => _mongoClient as T;
        public override void BeginTran(IClientSessionHandle clientSessionHandle = null)
        {
            clientSessionHandle?.StartTransaction(new TransactionOptions(
                readConcern: ReadConcern.Snapshot,
                writeConcern: WriteConcern.WMajority));
        }
        public override void RollbackTran(IClientSessionHandle clientSessionHandle = null)
        {
            clientSessionHandle?.AbortTransaction();
        }
        public override void CommitTran(IClientSessionHandle clientSessionHandle = null)
        {
            clientSessionHandle?.CommitTransaction();
        }
        private IMongoCollection<T> GetCollection<T>(string collection=null)
        {
            return _mongoDatabase.GetCollection<T>(collection ?? typeof (T).Name);
        }
        private IMongoCollection<Component> GetCollection(string name)
        {
            return _mongoDatabase.GetCollection<Component>(name);
        }
        
        #region Count

        public override async STask<long> Count<T>(string collection = null)
        {
            return await GetCollection<T>(collection).CountDocumentsAsync(d => true);
        }
        public override async STask<long> Count<T>(Expression<Func<T, bool>> filter, string collection = null)
        {
            return await GetCollection<T>(collection).CountDocumentsAsync(filter);
        }

        #endregion

        #region Exist

        public override async STask<bool> Exist<T>(string collection = null)
        {
            return (await Count<T>(collection)) > 0;
        }
        public override async STask<bool> Exist<T>(Expression<Func<T, bool>> filter, string collection = null)
        {
            return (await Count(filter, collection)) > 0;
        }

        #endregion
        
        public override STask<long> UpdateRange<T>(List<T> range)
        {
            throw new NotImplementedException();
        }

        #region Query

        public override async STask<T> Query<T>(long id, string collection = null)
        {
            var cursor = await GetCollection<T>(collection).FindAsync(d => d.Id == id);

            return await cursor.FirstOrDefaultAsync();
        }
        public override async STask<List<T>> QueryByPage<T>(Expression<Func<T, bool>> filter, int pageIndex, int pageSize,
            string collection = null)
        {
            return await GetCollection<T>(collection).Find(filter).Skip((pageIndex - 1) * pageSize).Limit(pageSize)
                .ToListAsync();
        }
        public override async STask<T> First<T>(Expression<Func<T, bool>> filter, string collection = null)
        {
            var cursor = await GetCollection<T>(collection).FindAsync(filter);

            return await cursor.FirstOrDefaultAsync();
        }
        public override async STask<List<T>> Query<T>(Expression<Func<T, bool>> filter, string collection = null)
        {
            var cursor = await GetCollection<T>(collection).FindAsync(filter);

            return await cursor.ToListAsync();
        }
        public override async STask<List<T>> Query<T>(long taskId, Expression<Func<T, bool>> filter, string collection = null)
        {
            var cursor = await GetCollection<T>(collection).FindAsync(filter);

            return await cursor.ToListAsync();
        }
        public override async STask Query(long id, List<string> collectionNames, List<Component> result)
        {
            if (collectionNames == null || collectionNames.Count == 0)
            {
                return;
            }

            foreach (var collectionName in collectionNames)
            {
                var cursor = await GetCollection(collectionName).FindAsync(d => d.Id == id);

                var e = await cursor.FirstOrDefaultAsync();

                if (e == null) continue;

                result.Add(e);
            }
        }
        public override async STask<List<T>> QueryJson<T>(string json, string collection = null)
        {
            FilterDefinition<T> filterDefinition = new JsonFilterDefinition<T>(json);
            var cursor = await GetCollection<T>(collection).FindAsync(filterDefinition);
            return await cursor.ToListAsync();
        }
        public override async STask<List<T>> QueryJson<T>(long taskId, string json, string collection = null)
        {
            FilterDefinition<T> filterDefinition = new JsonFilterDefinition<T>(json);
            var cursor = await GetCollection<T>(collection).FindAsync(filterDefinition);
            return await cursor.ToListAsync();
        }
        
        #endregion

        #region Insert

        public override async STask Insert<T>(T entity, string collection = null)
        {
            await Save(entity);
        }

        public override async STask InsertBatch<T>(IEnumerable<T> list, string collection = null)
        {
            if (collection == null)
            {
                collection = typeof (T).Name;
            }

            await GetCollection(collection).InsertManyAsync(list);
        }

        #endregion

        #region Save

        public override async STask Save<T>(T entity, string collection = null)
        {
            if (entity == null)
            {
                Log.Error($"save entity is null: {typeof(T).Name}");

                return;
            }

            var cloneEntity = entity.Clone();

            if (collection == null) collection = cloneEntity.GetType().Name;

            await GetCollection(collection).ReplaceOneAsync(d => d.Id == cloneEntity.Id, cloneEntity,
                new ReplaceOptions() {IsUpsert = true});
        }
        public override async STask Save<T>(long taskId, T entity, string collection = null)
        {
            if (entity == null)
            {
                Log.Error($"save entity is null: {typeof(T).Name}");

                return;
            }

            var cloneEntity = entity.Clone();

            if (collection == null) collection = cloneEntity.GetType().Name;

            await GetCollection(collection).ReplaceOneAsync(d => d.Id == cloneEntity.Id, cloneEntity,
                new ReplaceOptions {IsUpsert = true});
        }
        public override async STask Save(long id, List<Component> entities)
        {
            if (entities == null)
            {
                Log.Error($"save entity is null");
                return;
            }

            var cloneEntities = entities.Clone();

            foreach (var entity in cloneEntities)
            {
                if (entity == null)
                {
                    continue;
                }

                await GetCollection(entity.GetType().Name)
                    .ReplaceOneAsync(d => d.Id == entity.Id, entity, new ReplaceOptions {IsUpsert = true});
            }
        }
        public override async SVoid SaveNotWait<T>(T entity, long taskId = 0, string collection = null)
        {
            if (taskId == 0)
            {
                await Save(entity, collection);

                return;
            }

            await Save(taskId, entity, collection);
        }

        #endregion

        #region Remove

        public override async STask<long> Remove<T>(long id, string collection = null)
        {
            var result = await GetCollection<T>(collection).DeleteOneAsync(d => d.Id == id);

            return result.DeletedCount;
        }
        public override async SVoid RemoveNoWait<T>(long id, string collection = null)
        {
            await Remove<T>(id, collection);
        }
        public override async STask<long> Remove<T>(long taskId, long id, string collection = null)
        {
            var result = await GetCollection<T>(collection).DeleteOneAsync(d => d.Id == id);

            return result.DeletedCount;
        }
        public override async STask<long> Remove<T>(Expression<Func<T, bool>> filter, string collection = null)
        {
            var result = await GetCollection<T>(collection).DeleteManyAsync(filter);

            return result.DeletedCount;
        }
        public override async STask<long> Remove<T>(long taskId, Expression<Func<T, bool>> filter, string collection = null)
        {
            var result = await GetCollection<T>(collection).DeleteManyAsync(filter);

            return result.DeletedCount;
        }
        public override async SVoid RemoveNoWait<T>(Expression<Func<T, bool>> filter, string collection = null)
        {
            await Remove(filter, collection);
        }

        #endregion

        #region Index

        /// <summary>
        /// 创建数据库索引
        /// </summary>
        /// <param name="keys"></param>
        /// <typeparam name="T"></typeparam>
        /// <code>
        /// 使用例子(可多个):
        /// 1 : Builders.IndexKeys.Ascending(d=>d.Id)
        /// 2 : Builders.IndexKeys.Descending(d=>d.Id).Ascending(d=>d.Name)
        /// 3 : Builders.IndexKeys.Descending(d=>d.Id),Builders.IndexKeys.Descending(d=>d.Name)
        /// </code>
        public async STask CreateIndex<T>(params IndexKeysDefinition<T>[] keys) where T : Component
        {
            if (keys == null)
            {
                return;
            }

            var indexModels = new List<CreateIndexModel<T>>();

            foreach (var indexKeysDefinition in keys)
            {
                indexModels.Add(new CreateIndexModel<T>(indexKeysDefinition));
            }

            await GetCollection<T>().Indexes.CreateManyAsync(indexModels);
        }
        public async STask CreateIndex<T>(string collection, params IndexKeysDefinition<T>[] keys) where T : Component
        {
            if (keys == null)
            {
                return;
            }

            var indexModels = new List<CreateIndexModel<T>>();

            foreach (var indexKeysDefinition in keys)
            {
                indexModels.Add(new CreateIndexModel<T>(indexKeysDefinition));
            }

            await GetCollection<T>(collection).Indexes.CreateManyAsync(indexModels);
        }

        #endregion
    }
}