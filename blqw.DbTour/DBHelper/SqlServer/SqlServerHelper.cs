using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace blqw
{
    sealed class SqlServerHelper : DBHelper, IDBHelper
    {
        private string _tranPoint;
        SqlServerConnector _connector;

        protected override IConnector GetConnector()
        {
            return new SqlServerConnector(ConnectionString);
        }

        protected override DbProviderFactory Factory
        {
            get { return SqlClientFactory.Instance; }
        }

        protected override DbCommand GetCommand(CommandType commandType, string commandText, DbParameter[] parameters)
        {
            var cmd = new SqlCommand(commandText, _connector.DbConnection, _connector.Transaction);
            if (parameters != null && parameters.Length > 0)
            {
                var p = parameters as SqlParameter[];
                if (p == null)
                {
                    throw new ArgumentOutOfRangeException("DbParameter类型错误");
                }
                cmd.Parameters.AddRange(p);
            }
            return cmd;
        }

        protected override void Open()
        {
            base.Open();
            _connector = (SqlServerConnector)Connector;
        }

        public override void Close()
        {
            if (_connector != null)
            {
                _connector.Rollback(_tranPoint);
                if (_connector.Transaction != null)
                {
                    throw new NotSupportedException("顶级事务结束前,无法关闭连接");
                }
                _connector.CloseConnection();
            }
        }

        public override void Begin()
        {
            if (_tranPoint != null)
            {
                throw new NotSupportedException("重复开启事务");
            }
            Open();
            _tranPoint = _connector.BeginTransaction();
        }

        public override void Begin(IsolationLevel iso)
        {
            if (_tranPoint != null)
            {
                throw new NotSupportedException("重复开启事务");
            }
            Open();
            _tranPoint = _connector.BeginTransaction(iso);
        }

        public override void Commit()
        {
            if (_tranPoint == null)
            {
                throw new NotSupportedException("尚未开启事务");
            }
            _connector.Commit(_tranPoint);
            _tranPoint = null;
        }

        public override void Rollback()
        {
            if (_tranPoint == null)
            {
                throw new NotSupportedException("尚未开启事务");
            }
            _connector.Rollback(_tranPoint);
            _tranPoint = null;
        }

        public override void Dispose()
        {
            if (_connector != null)
            {
                ((SqlServerConnector)_connector).Rollback(_tranPoint);
                base.Dispose();
            }
        }
    }
}
