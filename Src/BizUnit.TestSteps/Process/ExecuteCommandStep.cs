//---------------------------------------------------------------------
// File: ExecuteCommandStep.cs
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

namespace BizUnit.TestSteps.Process
{
    /// <summary>
    /// The ExecuteCommandStep executes a program from the command line, command line arguments may be supplied also.
    /// </summary>
    public class ExecuteCommandStep : TestStepBase
    {
        /// <summary>
        /// The name of the program to execute
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// The command line paramters
        /// </summary>
        public string ProcessParams { get; set; }

        /// <summary>
        /// The working directory to run the program from
        /// </summary>
        public string WorkingDirectory { get; set; }

        public override void Execute(Context context)
        {
            context.LogInfo("ExecuteCommandStep about to execute the command: {0} params: {1}, working directory: {2}", ProcessName, ProcessParams, WorkingDirectory);

            var process = new System.Diagnostics.Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    Arguments = ProcessParams,
                    FileName = ProcessName,
                    WorkingDirectory = WorkingDirectory
                }
            };

            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            var exitCode = process.ExitCode;

            if (0 != exitCode)
            {
                throw new ApplicationException(string.Format("ExecuteCommandStep received an exit code: {0} while executing process {1} {2}\n\nOutput: {3}", exitCode, ProcessName, ProcessParams, output));
            }

            context.LogInfo("ExecuteCommandStep {0} output:\n{1}", ProcessName, output);
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(ProcessName, nameof(ProcessName));
            ProcessName = context.SubstituteWildCards(ProcessName);

            // Arguments - optional
            if (!string.IsNullOrEmpty(ProcessParams))
            {
                ProcessParams = context.SubstituteWildCards(ProcessParams);
            }

            // WorkingDirectory - optional
            if (!string.IsNullOrEmpty(WorkingDirectory))
            {
                ProcessParams = context.SubstituteWildCards(WorkingDirectory);
            }
        }
    }
}
