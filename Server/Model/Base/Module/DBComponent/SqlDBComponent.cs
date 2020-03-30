using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sining.Event;
using Sining.Model;
using Sining.Tools;
using SqlSugar;

namespace Sining.Module
{
    [ComponentSystem]
    public class SqlDBComponentAwakeSystem : AwakeSystem<SqlDBComponent, string, string, string>
    {
        protected override void Awake(SqlDBComponent self, string connectionString, string dbType, string dbName)
        {
            self.Awake(connectionString, dbType, dbName);
        }
    }

    public class SqlDBComponent : ADBComponent
    {
        private SqlSugarClient _connection;
        private ConnectionConfig _connectionConfig;

        public void Awake(string connectionString, string dbType, string dbName)
        {
            if (dbType == "MySql")
            {
                _connectionConfig = new ConnectionConfig()
                {
                    ConnectionString = connectionString,
                    DbType = (DbType) Enum.Parse(typeof(DbType), dbType),
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.Attribute
                };

                _connection = new SqlSugarClient(_connectionConfig);

                return;
            }

            throw new Exception($"{dbType} method is not currently supported");
        }

        public override void Init()
        {
            // [SugarColumn(IsNullable =false ,IsPrimaryKey =true,IsIdentity =true)]
            // [SugarColumn(IsNullable = true)]
            // [SugarColumn(IsIgnore =true)]
            // [SugarColumn(Length = 21)]
            _connection.CodeFirst.InitTables<TestPostModel>();
        }

        #region Count

        public override async STask<long> Count<T>(string collection = null)
        {
            return await _connection.Queryable<T>().CountAsync();
        }
        public override async STask<long> Count<T>(Expression<Func<T, bool>> filter, string collection = null)
        {
            return await _connection.Queryable<T>().Where(filter).CountAsync();
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

        #region Query

        public override async STask<T> Query<T>(long id, string collection = null)
        {
            return await _connection.Queryable<T>().Where(d => d.Id == id).FirstAsync();
        }
        public override async STask<List<T>> QueryByPage<T>(Expression<Func<T, bool>> filter, int pageIndex, int pageSize,
            string collection = null)
        {
            return await _connection.Queryable<T>().Where(filter).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .ToListAsync();
        }
        public override async STask<T> First<T>(Expression<Func<T, bool>> filter, string collection = null)
        {
            return await _connection.Queryable<T>().Where(filter).FirstAsync();
        }
        public override async STask<List<T>> Query<T>(Expression<Func<T, bool>> filter, string collection = null)
        {
            return await _connection.Queryable<T>().Where(filter).ToListAsync();
        }
        public override STask<List<T>> Query<T>(long taskId, Expression<Func<T, bool>> filter, string collection = null)
        {
            throw new NotImplementedException();
        }
        public override STask Query(long id, List<string> collectionNames, List<Component> result)
        {
            throw new NotImplementedException();
        }
        public override STask<List<T>> QueryJson<T>(string json, string collection = null) 
        {
            throw new NotImplementedException();
        }
        public override STask<List<T>> QueryJson<T>(long taskId, string json, string collection = null) 
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Insert

        public override async STask Insert<T>(T entity, string collection = null)
        {
            if (entity == null)
            {
                Log.Error($"save entity is null: {typeof(T).Name}");

                return;
            }

            await _connection.Insertable(entity.Clone()).ExecuteCommandAsync();
        }

        public override STask InsertBatch<T>(IEnumerable<T> list, string collection = null)
        {
            throw new NotImplementedException();
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

            await _connection.Updateable(entity.Clone()).ExecuteCommandAsync();
        }
        public override STask Save<T>(long taskId, T entity, string collection = null)
        {
            throw new NotImplementedException();
        }
        public override STask Save(long id, List<Component> entities)
        {
            throw new NotImplementedException();
        }
        public override async SVoid SaveNotWait<T>(T entity, long taskId = 0, string collection = null)
        {
            await Save(entity);
        }

        #endregion

        #region Remove

        public override async STask<long> Remove<T>(long id, string collection = null)
        {
            return await Remove<T>(d => d.Id == id);
        }

        public override async SVoid RemoveNoWait<T>(long id, string collection = null)
        {
            await Remove<T>(id);
        }

        public override STask<long> Remove<T>(long taskId, long id, string collection = null)
        {
            throw new NotImplementedException();
        }

        public override async STask<long> Remove<T>(Expression<Func<T, bool>> filter, string collection = null)
        {
            return await _connection.Deleteable<T>().Where(filter).ExecuteCommandAsync();
        }

        public override STask<long> Remove<T>(long taskId, Expression<Func<T, bool>> filter, string collection = null)
        {
            throw new NotImplementedException();
        }

        public override async SVoid RemoveNoWait<T>(Expression<Func<T, bool>> filter, string collection = null)
        {
            await Remove<T>(filter);
        }

        #endregion
    }
}