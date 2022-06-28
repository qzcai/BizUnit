//---------------------------------------------------------------------
// File: ObjectCreator.cs
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

using System;
using System.Reflection;

namespace BizUnit.TestSteps.BizTalk
{
    public class ObjectCreator
    {
        static public Type GetType(string typeName, string assemblyPath)
        {
            Type t;

            if (!string.IsNullOrEmpty(assemblyPath))
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                t = assembly.GetType(typeName, true, false);
            }
            else
            {
                t = Type.GetType(typeName);
            }

            return t;
        }
    }
}
