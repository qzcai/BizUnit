//---------------------------------------------------------------------
// File: DBExecuteNonQueryStep.cs
//
// Summary:
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2015, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using BizUnit.Core.Common;
using BizUnit.Core.TestBuilder;
using System;
using System.Data.SqlClient;

namespace BizUnit.TestSteps.Sql
{
    ///<summary>
    /// The DBExecuteNonQueryStep executes a non-query SQL statement. The number of rows affected is asserted if the
    /// NumberOfRowsAffected element is specified
    ///</summary>
    public class DBExecuteNonQueryStep : TestStepBase
    {
        /// <summary>
        /// The connection string used for the DB query
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// The SQL query to be executed
        /// </summary>
        public SqlQuery SQLQuery { get; set; }

        /// <summary>
        /// The number of rows affected. This is an optional element. If specified, it causes the test step to raise an exception when the number of rows affected
        ///	by executing the non-query does not match the specified value
        /// </summary>
        public int NumberOfRowsAffected { get; set; }

        /// <summary>
        /// The number of seconds to wait before executing the step
        /// </summary>
        public int DelayBeforeExecution { get; set; }

        /// <summary>
        /// Execute()
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            context.LogInfo("Using database connection string: {0}", ConnectionString);
            string sqlQueryToExecute = SQLQuery.GetFormattedSqlQuery(context);

            // Sleep for delay seconds...
            if (0 < DelayBeforeExecution)
            {
                context.LogInfo("Sleeping for: {0} seconds", DelayBeforeExecution);
                System.Threading.Thread.Sleep(DelayBeforeExecution * 1000);
            }

            context.LogInfo("Executing database query: {0}", sqlQueryToExecute);
            int rowCount = ExecuteNonQuery(ConnectionString, sqlQueryToExecute);

            if (0 == NumberOfRowsAffected)
            {
                return;
            }

            if (NumberOfRowsAffected != rowCount)
            {
                throw new ApplicationException(string.Format("Number of rows expected to be returned by the query does not match the value specified in the test step. NumberOfRowsExpected: {0}, actual number of rows affected was: {1}", NumberOfRowsAffected, rowCount));
            }
            else
            {
                context.LogInfo("The number of rows affected by the query matched the value specified in the test step. {0} rows were affected", rowCount);
            }
        }

        /// <summary>
        /// Executes the SQL statement
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="sqlCommand">SQL statement to execute</param>
        public static int ExecuteNonQuery(string connectionString, string sqlCommand)
        {
            SqlConnection connection = null;
            int numberOfRowsAffected;

            try
            {
                connection = new SqlConnection(connectionString);
                var command = new SqlCommand(sqlCommand, connection);
                command.Connection.Open();
                numberOfRowsAffected = command.ExecuteNonQuery();
            }
            finally
            {
                if (null != connection)
                {
                    connection.Close();
                }
            }

            return numberOfRowsAffected;
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(ConnectionString, nameof(ConnectionString));
            ConnectionString = context.SubstituteWildCards(ConnectionString);

            if (null == SQLQuery)
            {
                throw new ArgumentNullException("SQLQuery is null");
            }
            SQLQuery.Validate(context);
        }
    }
}
