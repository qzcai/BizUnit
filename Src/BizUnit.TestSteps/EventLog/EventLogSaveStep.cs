//---------------------------------------------------------------------
// File: EventLogSaveStep.cs
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

using BizUnit.Core.TestBuilder;
using System;
using System.IO;
using System.Management;

namespace BizUnit.TestSteps.EventLog
{
    /// <summary>
    /// The EventLogSaveStep test step clears the event log.
    /// </summary>
    public class EventLogSaveStep : TestStepBase
    {
        ///<summary>
        /// The name of the machine(s) where the event log should be saved from, multiple servers are specified through the use of a comma delimiter
        ///</summary>
        public string Server { get; set; }

        /// <summary>
        /// The local path to save the event log to, minus the file name or trailing backslash, the resulting filename will be SERVERNAME.evt
        /// </summary>
        public string DestinationPath { get; set; }

        /// <summary>
        /// The event log to save, e.g. Application, System etc.
        /// </summary>
        public string EventLog { get; set; }

        public override void Execute(Context context)
        {
            var servers = Server.Split(',');


            foreach (var server in servers)
            {
                var logPath = DestinationPath + @"\" + server;
                if (Directory.Exists(logPath))
                {
                    Directory.Delete(logPath, true);
                }
                Directory.CreateDirectory(logPath);
                context.LogInfo("About to save the event on server {0} to the following directory: {1}", server, logPath);

                string path = @"root\cimv2";
                if ((server.ToUpper() != Environment.MachineName.ToUpper()))
                {
                    path = string.Format(@"\\{0}\{1}", server, path);
                }
                var options = new ConnectionOptions
                {
                    Impersonation = ImpersonationLevel.Impersonate,
                    EnablePrivileges = true
                };
                var scope = new ManagementScope(path, options);

                var query = new SelectQuery("Select * from Win32_NTEventLogFile");
                var searcher = new ManagementObjectSearcher(scope, query);

                foreach (var logFileObject in searcher.Get())
                {
                    if (string.IsNullOrEmpty(EventLog))
                    {
                        if ((uint)logFileObject.GetPropertyValue("NumberOfRecords") == 0)
                            continue;
                    }
                    else
                    {
                        var logName = (string)logFileObject.GetPropertyValue("LogfileName");
                        if (logName.ToUpper() != EventLog.ToUpper())
                            continue;
                    }

                    try
                    {
                        var logFile = logPath + @"\" + logFileObject.GetPropertyValue("LogfileName") + ".evtx";
                        ((ManagementObject)logFileObject).InvokeMethod("BackupEventLog", new object[] { logFile });
                    }
                    catch (Exception ex)
                    {
                        //access denied on method call if scope path referes to the same  
                        context.LogException(ex);
                        throw;
                    }
                }
            }
        }

        public override void Validate(Context context)
        {
            if (string.IsNullOrEmpty(Server))
            {
                Server = Environment.MachineName;
            }
        }
    }
}
