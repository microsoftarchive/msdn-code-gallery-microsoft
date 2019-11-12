//Copyright (C) Microsoft Corporation.  All rights reserved.

using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.SqlClient;
using System.Data.Linq.Provider;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;


namespace LinqToSqlQueryVisualizer {


    /// <summary>
    /// The visualizer proxy for the general query visualizer
    /// It is responsible for serializing queries to a stream
    /// It delegates this task to the relevant provider specific query visualizer helper
    /// </summary>
    public class SourceChooser : VisualizerObjectSource {
        public override object CreateReplacementObject(object target, Stream incomingData) {
            return base.CreateReplacementObject(target, incomingData);
        }
        public override void TransferData(object target, Stream incomingData, Stream outgoingData) {
            base.TransferData(target, incomingData, outgoingData);
        }

        /// <summary>
        /// This method has to write the information about the object to visualize to the stream which is 
        /// then sent to the visualizer.
        /// This implementation retrieves the provider from the query and looks for the QueryVisualizer attribute.
        /// The attribute contains the assembly and class name of the provider specific query visualizer helper.
        /// These data are written to the stream and then this class is loaded and called to do the actual 
        /// serialization of the query.
        /// </summary>
        /// <param name="target"> The object to serialize, should be a query</param>
        /// <param name="outgoingData">Stream that receives the information about the query</param>
        public override void GetData(object target, Stream outgoingData) {
            SerializeTheQuery(target, outgoingData);
        }

        private static void Error(Stream str, string message) {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(str, "None");
            formatter.Serialize(str, message);
            return;
        }

        /// <summary>
        /// The unit tests will use this static version
        /// </summary>
        /// <param name="target"></param>
        /// <param name="outgoingData"></param>
        public static void SerializeTheQuery(object target, Stream stream) {
            // get query
            IQueryable query = target as IQueryable;
            if (query == null) {
                Error(stream, "Query visualizer invoked on non-IQueryable target.");
                return;
            }

            //get provider
            Type tQueryImpl = query.GetType();
            FieldInfo fiContext = tQueryImpl.GetField("context", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fiContext == null) {
                Error(stream, "Query field 'context' not found in type " + tQueryImpl.ToString() + ".");
                return;
            }

            Object objProvider = fiContext.GetValue(query);
            if (objProvider == null) {
                Error(stream, "Query field 'context' returned null.");
                return;
            }

            System.Data.Linq.DataContext dataContext = objProvider as System.Data.Linq.DataContext;
            if (dataContext == null) {
                Error(stream, "Query is not against a DataContext.");
                return;
            }

            //call visualizer to serialize query info
            Visualizer.StreamQueryInfo(dataContext, query, stream);
        }
    }


    /// <summary>
    /// The class that is supposed to define the visualizer UI / behavior
    /// Its implementation delegates this to a provider specific query visualizer
    /// </summary>
    public class DialogChooser : DialogDebuggerVisualizer {

        /// <summary>
        /// The format of the query information and the desired UI will usually depend on the Linq to SQLq provider
        /// used by the query. 
        /// Therefore in this general query visualizer we only read the assembly and class for the specific
        /// query visualizer from the Stream and call the method "Display" on this class, which in turn will
        /// read the query information and show the UI.         
        /// </summary>
        /// <param name="windowService"> used to display the UI </param>
        /// <param name="objectProvider"> used to retrieve the data (as Stream) from the visualizer proxy</param>
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider) {
            Stream rawStream = objectProvider.GetData();

            Visualizer.Display(windowService, rawStream);
        }
        public static void TestShow(object elementToVisualize) {
            VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(elementToVisualize, typeof(DialogChooser), typeof(SourceChooser));
            visualizerHost.ShowVisualizer();
        }

    }

    internal static class Utils {

        internal static string QuoteString(string raw) {
            return "'" + raw.Replace("'", "''") + "'";
        }

        /// <summary>
        /// Translates the sql query text (which contains parameter names)
        /// and the information about the parameters (SqlType and value as given by .ToString())
        /// into one sql string (that contains text representations of the values).
        /// Executing this string should be the same as executing the original query except in 
        /// corner cases (e.g. a string contains a parameter name, a decimal is given with too high precision)
        /// </summary>
        /// <param name="qt">query text and information about the parameters</param>
        /// <returns>the sql string to execute</returns>
        internal static string GetQueryTextWithParams(SqlQueryText qt) {
            string s = qt.Text;
            for (int i=qt.Params.Length-1; i >= 0; i--){
                ParameterText param = qt.Params[i];
                string val;
                switch (param.SqlType.ToString()) {
                    case "String":
                    case "Guid":
                    case "DateTime":
                        val = QuoteString(param.Value);
                        break;
                    case "Boolean":
                        if (param.Value == "True") {
                            val = "1";
                        } else if (param.Value == "False") {
                            val = "0";
                        } else {
                            throw new ArgumentException("Boolean value other than True or False");
                        }
                        break;
                    case "Time":
                        TimeSpan ts = TimeSpan.Parse(param.Value);
                        val = ts.Ticks.ToString(CultureInfo.CurrentUICulture);
                        break;
                    default:
                        val = param.Value;
                        break;
                }
                s = s.Replace(param.Name, val);
            }
            return s;
        }
    }

    internal static class QueryExecution {

        // reconstructs an object from its Clr type and the value string
        // (which was obtained as a result of calling the ToString method)
        private static object GetObject(string val, string sqlType) {

            DbType nnType = (DbType)Enum.Parse(typeof(DbType), sqlType.Trim());
            if (nnType == DbType.String) {
                return val;
            } else if (nnType == DbType.Int16) {
                return System.Int16.Parse(val, CultureInfo.CurrentUICulture);
            } else if (nnType == DbType.Int32) {
                return System.Int32.Parse(val, CultureInfo.CurrentUICulture);
            } else if (nnType == DbType.Int64) {
                return System.Int64.Parse(val, CultureInfo.CurrentUICulture);
            } else if (nnType == DbType.Byte) {
                return System.Byte.Parse(val, CultureInfo.CurrentUICulture);
            } else if (nnType == DbType.Double) {
                return System.Double.Parse(val, CultureInfo.CurrentUICulture);
            } else if (nnType == DbType.Single) {
                return System.Single.Parse(val, CultureInfo.CurrentUICulture);
            } else if (nnType == DbType.Decimal) {
                return System.Decimal.Parse(val, CultureInfo.CurrentUICulture);
            } else if (nnType == DbType.Boolean) {
                return System.Boolean.Parse(val);
            } else if (nnType == DbType.DateTime) {
                return System.DateTime.Parse(val, CultureInfo.CurrentUICulture);
            } else if (nnType == DbType.Time) {
                return System.TimeSpan.Parse(val);
            } else if (nnType == DbType.Guid) {
                return new Guid(val);
            } else {
                throw new NotSupportedException("Type " + sqlType + " is not supported for parameters in Linq to Sql Query Visualizer");
            }
        }



        /// <summary>
        /// Create a SqlCommand by creating parameters from the strings in qt.Params
        /// and using the text in qt.Text
        /// </summary>
        /// <param name="qt">SqlQueryText input</param>
        /// <param name="conn">SqlConnection to associate with the SqlCommand</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand GetSqlCommand(SqlQueryText qt, SqlConnection conn) {
            SqlCommand cmd = new SqlCommand(qt.Text, conn);
            foreach (ParameterText param in qt.Params) {
                System.Data.SqlClient.SqlParameter sqlParam = cmd.CreateParameter();
                sqlParam.ParameterName = param.Name;
                object val = GetObject(param.Value, param.SqlType);
                sqlParam.Value = val; 
                cmd.Parameters.Add(sqlParam);
            }
            return cmd;
        }

        // Execute queries using the original information about the query
        // This method constructs the query and parameters as in Linq to SQL
        // (not using the parameter values as part of the Sql text)
        internal static void ExecuteOriginalQueries(DataSet ds1, DataSet ds2, SqlQueryText[] infos, string connectionString) {
            SqlConnection conn = new SqlConnection(connectionString);

            // retrieve data
            using (conn) {
                conn.Open();

                SqlCommand cmd1 = GetSqlCommand(infos[0], conn);
                SqlDataAdapter adapter1 = new SqlDataAdapter(cmd1);
                adapter1.Fill(ds1);

                if (infos.Length > 1) {
                    SqlCommand cmd2 = GetSqlCommand(infos[1], conn);
                    SqlDataAdapter adapter2 = new SqlDataAdapter(cmd2);
                    adapter2.Fill(ds2);
                }
            }
        }

        // Executes Sql command as text
        internal static void ExecuteQuery(DataSet ds1, string cmd1, string connectionString) {
            SqlConnection conn = new SqlConnection(connectionString);
            using (conn) {
                conn.Open();
                SqlCommand sqlCmd1 = new SqlCommand(cmd1, conn);
                SqlDataAdapter adapter1 = new SqlDataAdapter(sqlCmd1);
                adapter1.Fill(ds1);
            }
        }

        // Executes Sql commands as text
        internal static void ExecuteQueries(DataSet ds1, DataSet ds2, string cmd1, string cmd2, string connectionString) {
            SqlConnection conn = new SqlConnection(connectionString);
            using (conn) {
                conn.Open();
                SqlCommand sqlCmd1 = new SqlCommand(cmd1, conn);
                SqlDataAdapter adapter1 = new SqlDataAdapter(sqlCmd1);
                adapter1.Fill(ds1);

                SqlCommand sqlCmd2 = new SqlCommand(cmd2, conn);
                SqlDataAdapter adapter2 = new SqlDataAdapter(sqlCmd2);
                adapter2.Fill(ds2);
            }
        }
    }
}
