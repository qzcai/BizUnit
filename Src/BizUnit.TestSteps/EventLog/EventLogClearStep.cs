//---------------------------------------------------------------------
// File: EventLogClearStep.cs
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

namespace BizUnit.TestSteps.EventLog
{
    /// <summary>
    /// The EventLogClearStep test step clears the event log(s) on specified machines. Note: caution should be taken when clearing event log.
    /// </summary>
    public class EventLogClearStep : TestStepBase
    {
        ///<summary>
        /// The name(s) of the machine(s) where the event log should be cleared, a comma delimeted list may be specified
        ///</summary>
        public string Server { get; set; }

        /// <summary>
        /// The event log to clear, e.g. Application, System etc.
        /// </summary>
        public string EventLog { get; set; }

        public override void Execute(Context context)
        {
            var servers = Server.Split(',');

            foreach (var server in servers)
            {
                using (var log = new System.Diagnostics.EventLog(EventLog, server))
                {
                    context.LogInfo("About to clear the '{0}' event log on machine '{1}'of all entries.", EventLog,
                                    server);
                    log.Clear();
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
