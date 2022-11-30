using ADCoreDataCommon.GameConst;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCoreDataCommon.SQLiteData
{
    public  class ADCoreSqlite
    {
        public  static  Dictionary<string,ADCoreSqlite> SqliteData = new Dictionary<string,ADCoreSqlite>();
        private static SQLiteConnection liteConnection;
        
        /// <summary>
        ///  数据库地址
        /// </summary>
        private  string DataSourcePath { get;set; }

        public ADCoreSqlite(string  fileName = null)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                fileName = GameConst.GameConst.DbPath;
            }
            DataSourcePath = fileName;
            if (!File.Exists(DataSourcePath))
            {
                CreateDataBase();
            }
            ConnectionSQLiteData();
            
        }
        /// <summary>
        ///  连接数据库
        /// </summary>
        private void ConnectionSQLiteData()
        {
            try
            {

                string connStr = string.Format("Data Source={0};Version=3;Max Pool Size=10;Journal Mode=Off;", DataSourcePath);
                liteConnection = new SQLiteConnection(connStr);
                Console .WriteLine(liteConnection.State);
               liteConnection.Open();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        ///  创建数据库 ， 如果数据库文件 存在则可以忽略
        /// </summary>
        public bool CreateDataBase()
        {

             string psth  =  Path.GetDirectoryName(DataSourcePath);
            if ((!string.IsNullOrEmpty(psth)) && (!Directory.Exists(psth)))

                Directory.CreateDirectory(psth);
            
            if (!File.Exists(DataSourcePath))
            {
                SQLiteConnection.CreateFile(DataSourcePath);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns></returns>
        public  SQLiteTransaction BeginTranaction()
        {
            try
            {
                var sqltrans = liteConnection.BeginTransaction(); 
                return sqltrans;
            }
            catch(Exception ex)
            {
                return null;
            }

        }
        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="liteTransaction"></param>
        public   void  CommitTransAction(SQLiteTransaction liteTransaction)
        {
            try
            {
                if (liteTransaction != null)
                {
                    liteTransaction.Commit();
                }
            }
            catch(Exception ex)
            {
                Console .WriteLine(ex.Message);
                return;
            }
        }
        /// <summary>
        /// 准备操作命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="connection"></param>
        /// <param name="sqlText"></param>
        /// <param name="data"></param>

        public  void  PrepareCommand(SQLiteCommand command, SQLiteConnection connection, string sqlText, Dictionary<String, String> data)
        {
            if (liteConnection.State != System.Data.ConnectionState.Open)
                 
               liteConnection .Open();
            command.Parameters.Clear();
            command.Connection = connection;
            command.CommandText = sqlText;
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 30;
            if (data != null && data.Count() > 0)
            {
                foreach(var sl in data)
                {
                    command.Parameters.AddWithValue(sl.Key, sl.Value);
                }
            }
        }

        private  void BeSureConnection()
        {
            if(liteConnection.State!=System.Data.ConnectionState.Open)
            {
                ConnectionSQLiteData();
            }
        }

        /// <summary>
        ///  查询返回dataset
        ///  
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public  DataSet ExecuteDataSet(string cmdText,Dictionary<string, string> data = null)
        {
            try
            {
                DataSet da = new DataSet();
                SQLiteCommand command = new SQLiteCommand();
                PrepareCommand(command, liteConnection,cmdText, data);
                var ds = new  SQLiteDataAdapter (command);
                ds.Fill(da);
                return da;
            
            
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 查询返回datatable
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string cmdText, Dictionary<string, string> data  )
        {
            try
            {
                var dt  = new DataTable();
                SQLiteCommand command = new SQLiteCommand();
                PrepareCommand(command, liteConnection, cmdText, data);
                SQLiteDataReader reader = command.ExecuteReader();
                dt.Load(reader);
                return dt;

            }
            catch(Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 返回一行数据
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public DataRow ExcuteDataRow(string cmdText, Dictionary <string ,string> data)
        {
             DataSet dataSet  = ExecuteDataSet(cmdText, data);
            if(dataSet!=null&&dataSet.Tables.Count>0&&dataSet.Tables[0].Rows.Count>0)
            {
                return  dataSet.Tables[0]   .Rows[0];
            }
            return null;
        }

        /// <summary>
        ///  查询
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public  int  ExcuteNonQuery(string cmdText, Dictionary<string, string> data=null)
        {
            try
            {
                int result = 0;
                var command = new SQLiteCommand();
                PrepareCommand(command, liteConnection, cmdText, data);
                result = command.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
                return -1;

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="da"></param>
        /// <returns></returns>
        public  SQLiteDataReader ExcuteReader(string cmdText,Dictionary<string,string>da = null)
        {
            var command = new SQLiteCommand(); 
            try
            {
                PrepareCommand(command,liteConnection ,cmdText,da);
                SQLiteDataReader reader= command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;

            }
            catch (Exception ex)
            {
                command.Dispose();
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText">sql语句</param>
        /// <returns></returns>
        public  List<Dictionary <String  ,String>> ExecuteReaderList(string cmdText)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            var ds = ExcuteReader(cmdText);
            int columcount = ds.FieldCount;
            while (ds.Read())
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                for (int i = 0; i < columcount; i++)
                {
                    object obj = ds.GetValue(i);
                    if (obj == null)
                    {
                        dic.Add(ds.GetName(i), "");
                    }
                    else
                    {
                        dic.Add(ds.GetName(i), obj.ToString());
                    }
                }
                list.Add(dic);
            }

            return list;
        }
        /// <summary>
        /// 返回结果集的第一行第一列 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="da"></param>
        /// <returns></returns>
        public object ExecuteScalar(string  cmdText ,Dictionary<string ,string>da= null)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand();
                PrepareCommand(cmd,liteConnection,cmdText,da);
                return cmd.ExecuteScalar();

            }
            catch(Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="recordCount">总记录数</param>
        /// <param name="pageIndex">页牵引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="cmdText">Sql命令文本</param>
        /// <param name="countText">查询总记录数的Sql文本</param>
        /// <param name="data">命令参数</param>
        /// <returns>DataSet</returns>
        public DataSet ExecutePager(ref int recordCount, int pageIndex, int pageSize, string cmdText, string countText, Dictionary<string, string> data = null)
        {
            if (recordCount < 0)
                recordCount = int.Parse(ExecuteScalar(countText, data).ToString());
            var ds = new DataSet();
            var command = new SQLiteCommand();
            PrepareCommand(command, liteConnection, cmdText, data);
            var da = new SQLiteDataAdapter(command);
            da.Fill(ds, (pageIndex - 1) * pageSize, pageSize, "result");
            return ds;
        }
        /// <summary>
        /// 重新组织数据库：VACUUM 将会从头重新组织数据库
        /// </summary>
        public void ResetDataBass(SQLiteConnection conn)
        {
            var cmd = new SQLiteCommand();
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Parameters.Clear();
            cmd.Connection = conn;
            cmd.CommandText = "vacuum";
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void CloseDb()
        {
            if (liteConnection.State == ConnectionState.Open)
                liteConnection.Close();

            liteConnection = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public Dictionary<string, string> ExecuteReaderOne(string cmdText)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var ds = ExcuteReader(cmdText);
            int columcount = ds.FieldCount;
            while (ds.Read())
            {
                for (int i = 0; i < columcount; i++)
                {
                    object obj = ds.GetValue(i);
                    if (obj == null)
                    {
                        dic.Add(ds.GetName(i), "");
                    }
                    else
                    {
                        dic.Add(ds.GetName(i), obj.ToString());
                    }
                }
                break;
            }

            return dic;
        }
        /// <summary>
        ///  备份数据库
        /// </summary>
        public void ExportSQLiteDb()
        {
            try
            {
                string path = Path.GetFileNameWithoutExtension(DataSourcePath);
                File.Copy(DataSourcePath, $"./db/backup/{path}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.db");
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        /// <summary>
        ///  初始化数据库
        ///  
        /// </summary>
        public void InitSQLiteDB()
        {
            var dss = ExecuteReaderList("SELECT name,seq FROM sqlite_sequence");
            var trs =  BeginTranaction();
            foreach (var ds in dss)
            {
                string name = ds["name"];
                if (name == "localInfos")
                {
                    continue;
                }
                ExcuteNonQuery($"DELETE FROM {name}");
                ExcuteNonQuery($"UPDATE sqlite_sequence SET seq=0 where name='{name}'");
            }
            CommitTransAction(trs);
        }
    }

}
