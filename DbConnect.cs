using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace  ToolITs.db
{
    public class DbConnect
    {

        private string Connection = ConfigurationManager.AppSettings["connstr"];
        private OracleConnection _connection;
        private OracleTransaction _transaction;

        public DbConnect()
        {
            _connection = new OracleConnection(Connection);
        }

        public DbConnect(string connstr)
        {
            _connection = new OracleConnection(connstr);
        }

        private void Begin()
        {
            Open();
            _transaction = _connection.BeginTransaction();
        }

        private void Commit()
        {
            _transaction.Commit();
            _transaction = null;
            Close();
        }

        private void Rollback()
        {
            _transaction.Rollback();
            _transaction = null;
            Close();
        }

        private void Open()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        private void Close()
        {
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
        }

        public OracleCommand InitSqlCommand(CommandType cmdType, string cmdText, params IDataParameter[] parameter)
        {
            OracleCommand command = new OracleCommand(cmdText, _connection);
            command.CommandType = cmdType;
            if (_transaction != null)
                command.Transaction = _transaction;
            if (parameter != null)
                command.Parameters.AddRange(parameter);
            return command;
        }

        public int ExecuteNonQuery(CommandType cmdType, string cmdText, params IDataParameter[] parameter)
        {
            Open();
            OracleCommand command = InitSqlCommand(cmdType, cmdText, parameter);
            int res = command.ExecuteNonQuery();
            Close();
            return res;
        }

        public int ExecuteNonQuery(string cmdText)
        {
            OracleCommand command = new OracleCommand(cmdText, _connection);
            try
            {
                Begin();
                int result = command.ExecuteNonQuery();
                Commit();
                return result;
            }
            catch (Exception ex)
            {
                Rollback();
                throw ex;
            }
            finally
            {
                command.Dispose();
                Close();
            }
        }

        public void ExecuteBatchNonQuery(List<string> SqlList)
        {
            OracleCommand command = new OracleCommand();
            try
            {
                command.Connection = _connection;
                Begin();
                foreach (string sql in SqlList)
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
                Commit();
            }
            catch (Exception ex)
            {
                Rollback();
                throw ex;
            }
            finally
            {
                command.Dispose();
                Close();
            }
        }


        public DataSet ExcuteQuery(string sql)
        {
            DataSet ds = new DataSet();
            OracleCommand command = new OracleCommand();
            try
            {
                string[] temp = sql.Split(";".ToCharArray());
                command.Connection = _connection;
                for (int i = 0; i < temp.Length; i++)
                {
                    command.CommandText = temp[i];
                    OracleDataAdapter da = new OracleDataAdapter(command);
                    da.Fill(ds, i.ToString());
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Dispose();
            }
        }
        public DataSet ExecuteBatchQuery(string[] SQL)
        {
            OracleCommand command = new OracleCommand();
            int index = 0;
            try
            {
                DataSet ds = new DataSet();
                command.Connection = _connection;
                foreach (string sql in SQL)
                {
                    command.CommandText = sql;
                    OracleDataAdapter da = new OracleDataAdapter(command);
                    da.Fill(ds, index.ToString());
                    index++;
                }

                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Dispose();
            }
        }

    }
}