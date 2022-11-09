using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace UploadAndDownloadFile
{
    public class Data
    {
        private static SqlConnection con;
        private static SqlCommand command;
        private static SqlDataReader dataReader;

        public static void Connect()
        {
            string datasource = "."; //DIISI DENGAN IP SERVER ATAU ISI DENGAN TITIK (.) JIKA SERVER ADA DI LOKAL KOMPUTER ANDA.
            string database = "PROJECTSql"; //DIISI DENGAN NAMA DATABASE
            string userid = "sa"; //DIISI DENGAN USER ID UNTUK LOGIN KE DATABASE, SESUAI YANG DI SET PADA DATABASE SQL SERVER ANDA.
            string password = "123456"; //DIISI DENGAN PASSWORD YANG DI SET UNTUK LOGIN PADA DATABASE, UNTUK USER DIATAS.

            con = new SqlConnection($"data source={datasource};Database={database};user id={userid};password={password};");
            con.Open();
        }

        public static void Disconnect()
        {
            con.Close();
        }

        public static void Command(string sqlCommand, object[] paramValue)
        {
            using (command = new SqlCommand(sqlCommand, con, null))
            {
                if(paramValue!=null)
                {
                    for(int x=0; x<paramValue.Length; x++)
                    {
                        if(paramValue[x].GetType().Equals(typeof(byte[])))
                        {
                            SqlParameter param = new SqlParameter("@" + x, SqlDbType.VarBinary, -1);
                            param.Value = paramValue[x];
                            command.Parameters.Add(param);
                        }
                        else
                        {
                            SqlParameter param = null;
                            Type type = paramValue[x].GetType();
                            switch(type.Name)
                            {
                                case "String":
                                    param = new SqlParameter("@" + x, SqlDbType.VarChar, -1);
                                    break;
                                case "DateTime":
                                    param = new SqlParameter("@" + x, SqlDbType.DateTime);
                                    break;
                                case "Int64":
                                    param = new SqlParameter("@" + x, SqlDbType.BigInt);
                                    break;
                                case "Int32":
                                    param = new SqlParameter("@" + x, SqlDbType.Int);
                                    break;
                                case "Int16":
                                    param = new SqlParameter("@" + x, SqlDbType.SmallInt);
                                    break;
                                case "Byte":
                                    param = new SqlParameter("@" + x, SqlDbType.TinyInt);
                                    break;
                                case "Single":
                                case "Double":
                                case "Decimal":
                                    param = new SqlParameter("@" + x, SqlDbType.Float, 500);
                                    break;
                                case "Guid":
                                    param = new SqlParameter("@" + x, SqlDbType.UniqueIdentifier);
                                    break;
                                case "Byte[]":
                                    param = new SqlParameter("@" + x, SqlDbType.VarBinary, -1);
                                    break;
                                case "DBNull":
                                    param = new SqlParameter("@" + x, SqlDbType.VarChar, 500);
                                    param.SqlValue = "";
                                    break;
                            }
                            param.Value = paramValue[x];
                            command.Parameters.Add(param);
                        }
                    }
                }

                command.Prepare();
                command.ExecuteNonQuery();
            }
        }

        public static DataTable SelectDataTable(string sqlCommand, object[] paramValue)
        {
            DataTable dt = new DataTable();

            using (command = new SqlCommand(sqlCommand, con, null))
            {
                if (paramValue != null)
                {
                    for (int x = 0; x < paramValue.Length; x++)
                    {
                        if (paramValue[x].GetType().Equals(typeof(byte[])))
                        {
                            SqlParameter param = new SqlParameter("@" + x, SqlDbType.VarBinary, -1);
                            param.Value = paramValue[x];
                            command.Parameters.Add(param);
                        }
                        else
                        {
                            SqlParameter param = null;
                            Type type = paramValue[x].GetType();
                            switch (type.Name)
                            {
                                case "String":
                                    param = new SqlParameter("@" + x, SqlDbType.VarChar, -1);
                                    break;
                                case "DateTime":
                                    param = new SqlParameter("@" + x, SqlDbType.DateTime);
                                    break;
                                case "Int64":
                                    param = new SqlParameter("@" + x, SqlDbType.BigInt);
                                    break;
                                case "Int32":
                                    param = new SqlParameter("@" + x, SqlDbType.Int);
                                    break;
                                case "Int16":
                                    param = new SqlParameter("@" + x, SqlDbType.SmallInt);
                                    break;
                                case "Byte":
                                    param = new SqlParameter("@" + x, SqlDbType.TinyInt);
                                    break;
                                case "Single":
                                case "Double":
                                case "Decimal":
                                    param = new SqlParameter("@" + x, SqlDbType.Float, 500);
                                    break;
                                case "Guid":
                                    param = new SqlParameter("@" + x, SqlDbType.UniqueIdentifier);
                                    break;
                                case "Byte[]":
                                    param = new SqlParameter("@" + x, SqlDbType.VarBinary, -1);
                                    break;
                                case "DBNull":
                                    param = new SqlParameter("@" + x, SqlDbType.VarChar, 500);
                                    param.SqlValue = "";
                                    break;
                            }
                            param.Value = paramValue[x];
                            command.Parameters.Add(param);
                        }
                    }
                }
            }

            using (dataReader = command.ExecuteReader())
            {
                if(dataReader.HasRows)
                {
                    dt.Load(dataReader);
                }
            }

            return dt;
        }
    }
}
