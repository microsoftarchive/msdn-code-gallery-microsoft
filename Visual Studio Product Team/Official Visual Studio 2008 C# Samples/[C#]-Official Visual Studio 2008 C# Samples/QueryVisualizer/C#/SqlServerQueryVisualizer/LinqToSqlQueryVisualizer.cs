using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.DebuggerVisualizers;
using System.Data.Linq.SqlClient;
using System.Windows.Forms;
using System.Linq.Expressions;
using System.Data.Linq.Provider;
using System.Linq;
using System.Data;
using System.Data.Linq;

[assembly: System.Diagnostics.DebuggerVisualizer(typeof(LinqToSqlQueryVisualizer.DialogChooser),
    typeof(LinqToSqlQueryVisualizer.SourceChooser),
    TargetTypeName = "System.Data.Linq.DataQuery`1, System.Data.Linq",
    Description = "Linq to SQL Debugger Visualizer")]

namespace LinqToSqlQueryVisualizer {

    internal struct ParameterText {
        public string Name;
        public string Value;
        public string SqlType;
    }

    internal struct SqlQueryText {
        public string Text;
        public ParameterText[] Params;
    }

    class SqlQueryInfo {
        SqlQueryText[] queries;

        public SqlQueryInfo() { 
        }

        public SqlQueryInfo(SqlQueryText[] queries) {
            this.queries = queries;
        }

        public SqlQueryText[] Queries {
            get { return this.queries; }
        }

        public void serialize(Stream stream) {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this.queries.Length);
            foreach (SqlQueryText query in this.queries) {
                formatter.Serialize(stream, query.Text);
                formatter.Serialize(stream, query.Params.Length);
                foreach (ParameterText param in query.Params) {
                    formatter.Serialize(stream, param.Name);
                    formatter.Serialize(stream, param.Value);
                    formatter.Serialize(stream, param.SqlType);
                }
            }
        }

        public static SqlQueryInfo deserialize(Stream stream) {
            SqlQueryInfo result = new SqlQueryInfo();
            BinaryFormatter formatter = new BinaryFormatter();
            int nQueries = (int) formatter.Deserialize(stream);
            result.queries = new SqlQueryText[nQueries];
            for (int i=0; i<nQueries; i++) {
                SqlQueryText query;
                query.Text = (string)formatter.Deserialize(stream);
                int nParams = (int)formatter.Deserialize(stream);
                query.Params = new ParameterText[nParams];
                for (int j = 0; j < nParams; j++) {
                    ParameterText p;
                    p.Name = (string) formatter.Deserialize(stream);
                    p.Value = (string)formatter.Deserialize(stream);
                    p.SqlType = (string)formatter.Deserialize(stream);
                    query.Params[j] = p;
                }
                result.Queries[i] = query;
            }
            return result;
        }
    }

    public class Visualizer {

        private static SqlQueryText[] GetFullQueryInfo(DataContext dataContext, IQueryable query) {
            System.Data.Common.DbCommand dbCommand = dataContext.GetCommand(query);

            SqlQueryText[] result = new SqlQueryText[1];
            for (int i = 0, n = 1; i < n; i++) {
                result[i].Text = dbCommand.CommandText;
                int nParams = dbCommand.Parameters.Count ;
                result[i].Params = new ParameterText[nParams];
                for (int j = 0; j < nParams; j++) {
                    ParameterText param = new ParameterText();
                    System.Data.Common.DbParameter pInfo = dbCommand.Parameters[j];
                    param.Name = pInfo.ParameterName;
                    param.SqlType = pInfo.DbType.ToString();
                    object paramValue = pInfo.Value;
                    if (paramValue == null) {
                        param.Value = null;
                    } else {
                        param.Value = pInfo.Value.ToString();
                    }
                    result[i].Params[j] = param;
                }
            }
            return result;
        }

        /// <summary>
        /// This method is called from the QueryVisualizer to copy the following data to the stream:
        /// 1. The query expression as string
        /// 2. SQL query text(s) / parameters
        /// 3. Connection string
        /// </summary>
        /// <param name="query"> The query exression to visualize </param>
        /// <param name="outgoingData"> The stream used for marshalling to the visualizer </param>
        /// 
        public static void StreamQueryInfo(DataContext dataContext, IQueryable query, Stream outgoingData) {
            BinaryFormatter formatter = new BinaryFormatter();
            if (dataContext == null) {
                formatter.Serialize(outgoingData, "None");
                formatter.Serialize(outgoingData, "No datacontext provided.");
                return;
            }
            Expression expr = query.Expression;
            if (expr == null) {
                formatter.Serialize(outgoingData, "None");
                formatter.Serialize(outgoingData, "Expression of the query is empty.");
                return;
            }
            formatter.Serialize(outgoingData, expr.ToString());

            try {
                SqlQueryInfo qi = new SqlQueryInfo(GetFullQueryInfo(dataContext, query));
                qi.serialize(outgoingData);
            } catch (Exception ex) {
                outgoingData.Position = 0;
                formatter.Serialize(outgoingData, "None");
                formatter.Serialize(outgoingData, string.Format(CultureInfo.CurrentUICulture, "Exception while serializing the query:\r\n{0}", ex.Message));
                return;
            }

            IDbConnection conn = dataContext.Connection;
            string connectionString = conn.ConnectionString;
            //Need to check for |DataDirectory| token in the connection string and replace with absolute path to allow
            if (connectionString.Contains("|DataDirectory|")) {
                connectionString = conn.ConnectionString.Replace(@"|DataDirectory|\", AppDomain.CurrentDomain.BaseDirectory);
            }
            //the Linq To Sql Query Visualizer to use the same connection string.
            formatter.Serialize(outgoingData, connectionString);
        }

        /// <summary>
        /// This method is called from the general Query Visualizer.
        /// It reads the query data from the stream, sets the corresponding fields in the
        /// QueryVisualizerFrom and displays it.
        /// </summary>
        /// <param name="windowService">Used to show the visualizer dialog</param>
        /// <param name="rawStream">The query data sent from the provider / visualizer proxy</param>
        public static void Display(IDialogVisualizerService windowService,Stream rawStream) {
            BinaryFormatter formatter = new BinaryFormatter();
            string expression = (string)formatter.Deserialize(rawStream);

            SqlQueryInfo qi = SqlQueryInfo.deserialize(rawStream);
            SqlQueryText[] infos = qi.Queries;

            string connectionString = (string)formatter.Deserialize(rawStream);

            QueryVisualizerForm form = new QueryVisualizerForm();
            form.SetTexts(expression, infos, connectionString);
            windowService.ShowDialog(form);
        }
    }
}
