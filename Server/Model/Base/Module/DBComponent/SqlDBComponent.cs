using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Driver;
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

    public class SqlDBComponent: ADBComponent
    {
        private SqlSugarClient _connection;
        private ConnectionConfig _connectionConfig;
        public void Awake(string connectionString, string dbType, string dbName)
        {
            try
            {
                _connectionConfig = new ConnectionConfig()
                {
                    ConnectionString = connectionString,
                    DbType = (DbType) Enum.Parse(typeof(DbType), dbType),
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.Attribute
                };
                
                _connection = new SqlSugarClient(_connectionConfig);
            }
            catch (Exception e)
            {
                throw new Exception($"{dbType} method is not currently supported {e}");
            }
        }
        public override void Init()
        {
            // [SugarColumn(IsNullable =false ,IsPrimaryKey =true,IsIdentity =true)]
            // [SugarColumn(IsNullable = true)]
            // [SugarColumn(IsIgnore =true)]
            // [SugarColumn(Length = 21)]
            //_connection.CodeFirst.InitTables<TestPostModel>();
        }
        public override T GetConnection<T>()=>_connection as T;
        public override void BeginTran(IClientSessionHandle clientSessionHandle = null)
        {
            _connection.BeginTran();
        }
        public override void RollbackTran(IClientSessionHandle clientSessionHandle = null)
        {
            _connection.RollbackTran();
        }
        public override void CommitTran(IClientSessionHandle clientSessionHandle = null)
        {
            _connection.CommitTran();
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
        
        public override async STask<long> UpdateRange<T>(List<T> range)
        {
            return await _connection.Updateable(range).ExecuteCommandAsync();
        }

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
        public override async STask<List<T>> QueryByPageOrderBy<T>(Expression<Func<T, bool>> filter, int pageIndex,
            int pageSize,
            Expression<Func<T, object>> orderByExpression, bool isAsc = true, string collection = null)
        {
            return await _connection.Queryable<T>().Where(filter)
                .OrderBy(orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc)
                .Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .ToListAsync();
        }
        public override async STask<T> First<T>(Expression<Func<T, bool>> filter, string collection = null)
        {
            return await _connection.Queryable<T>().Where(filter).FirstAsync();
        }
        public override async STask<List<T>> QueryOrderBy<T>(Expression<Func<T, bool>> filter,
            Expression<Func<T, object>> orderByExpression, bool isAsc = true, string collection = null)
        {
            return await _connection.Queryable<T>().Where(filter)
                .OrderBy(orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc).ToListAsync();
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

        public override async STask InsertBatch<T>(IEnumerable<T> list, string collection = null)
        {
            await _connection.Insertable<T>(list).ExecuteCommandAsync();
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
        public override STask Save(long id, List<Component> entities)
        {
            throw new NotImplementedException();
        }
        public override async SVoid SaveNotWait<T>(T entity, string collection = null)
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
        public override async STask<long> Remove<T>(Expression<Func<T, bool>> filter, string collection = null)
        {
            return await _connection.Deleteable<T>().Where(filter).ExecuteCommandAsync();
        }
        public override async SVoid RemoveNoWait<T>(Expression<Func<T, bool>> filter, string collection = null)
        {
            await Remove<T>(filter);
        }

        #endregion
    }
}