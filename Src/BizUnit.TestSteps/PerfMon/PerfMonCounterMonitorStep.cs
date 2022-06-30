//---------------------------------------------------------------------
// File: DelayStep.cs
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
using System.Diagnostics;

namespace BizUnit.TestSteps.PerfMon
{
    /// <summary>
    /// The PerfMonCounterMonitorStep is used to monitor a perfmon counter 
    /// until it reaches a target value. If the counter reaches the target 
    /// value within the timeout period the step will succeed, otherwise it 
    /// will fail.
    /// </summary>
    public class PerfMonCounterMonitorStep : TestStepBase
    {
        ///<summary>
        /// The name of the server to monitor the counter on
        ///</summary>
        public string Server { get; set; }

        ///<summary>
        /// The name of the perfmon category
        ///</summary>
        public string CategoryName { get; set; }

        ///<summary>
        /// The name of the perfmon counter to monitor
        ///</summary>
        public string CounterName { get; set; }

        ///<summary>
        /// he perfmon counter instance name
        ///</summary>
        public string InstanceName { get; set; }

        /// <summary>
        /// The target perfmon counter value, once the counter reaches this value the step will complete
        /// </summary>
        public float CounterTargetValue { get; set; }

        /// <summary>
        /// The length of the delay in miliseconds between checks of the counter
        /// </summary>
        public int SleepTime { get; set; }

        /// <summary>
        /// The maximum length of time the step will spend checking the counter (seconds)
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            context.LogInfo("About to start monitoring: {0}\\{1}\\{2}({3}) for the target value: {4}", Server, CategoryName, CounterName, InstanceName, CounterTargetValue);

            // Init perfmon counter...
            var perfCounter = new PerformanceCounter
            {
                CategoryName = CategoryName,
                CounterName = CounterName,
                MachineName = Server
            };

            if (null != InstanceName)
            {
                perfCounter.InstanceName = InstanceName;
            }

            // Set default value for sleepTime
            if (0 == SleepTime)
            {
                SleepTime = 100;
            }

            DateTime now = DateTime.Now;
            DateTime end = now;

            if (0 != TimeOut)
            {
                end = now.AddSeconds(TimeOut);
            }

            bool targetHit = false;

            do
            {
                if (perfCounter.NextValue() == CounterTargetValue)
                {
                    targetHit = true;
                    context.LogInfo("Target hit");

                }
                else if ((end > now) || (0 == TimeOut))
                {
                    System.Threading.Thread.Sleep(SleepTime);
                }
            } while ((!targetHit) && ((end > now) || (0 == TimeOut)));

            if (!targetHit)
            {
                throw new ApplicationException("The target perfmon counter was not hit!");
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
