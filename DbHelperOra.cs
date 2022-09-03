using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DenseWarehouse
{
    public class DbHelperOra
    {
        private string connectionString;
        public DbHelperOra(string conn)
        {
            connectionString = conn;
        }


        /// <summary>
        /// 執行存儲過程
        /// </summary>
        /// <param name=Procedere Name>Procedure 名字</param>
        /// <param name=parameters>參數傳入</param>
        /// <returns>參數輸出</returns>
        public OracleParameter[] Procedure(string commandtext, OracleParameter[] parameters)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleCommand cmd = connection.CreateCommand();
                cmd.CommandText = commandtext;
                cmd.CommandType = CommandType.StoredProcedure;//指明是執行存儲過程
                foreach (OracleParameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();//执行存储过程 
                }
                catch (Exception E)
                {
                    Console.WriteLine(string.Format("Oracle ERROR:[{0}] : {1}",E.StackTrace , E.Message));
                    return null;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
                return parameters;
            }
        }
        /// <summary>
        /// 執行SQL查詢語句
        /// </summary>
        /// <param name="SQLString"></param>
        /// <returns></returns>
        public DataTable Query(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                DataTable dt = new DataTable();

                try
                {
                    connection.Open();
                    OracleDataAdapter command = new OracleDataAdapter(SQLString, connection);
                    command.Fill(dt);
                    //OracleCommand ocmd = new OracleCommand(SQLString, connection);
                    //ocmd.CommandTimeout = 5;
                    //command.SelectCommand = ocmd;
                    //command.Fill(dt);
                }
                catch (OracleException ex)
                {
                    dt = null;
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                return dt;
            }
        }
        /// <summary>
        /// SQL執行語句
        /// </summary>
        /// <param name="SQLString"></param>
        public bool ExecuteSql(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        return true;

                    }
                    catch ( OracleException E)
                    {
                        connection.Close();
                        return false;
                        //throw new Exception(E.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>     
        public void ExecuteSqlTran(ArrayList SQLStringList)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                OracleTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                string strsql = "";
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (Exception ex)
                {
                    tx.Rollback();

                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 判斷DB是否連接成功
        /// </summary>
        /// <returns></returns>
        public bool ConnectStatus()
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    //判斷連接DB是否成功
                    if (connection.State == ConnectionState.Open)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }

                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
