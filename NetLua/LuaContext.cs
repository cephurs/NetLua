﻿/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013 Francesco Bertolaccini
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
 * the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Dynamic;

namespace NetLua
{
    /// <summary>
    /// Holds a scope and its variables
    /// </summary>
    public class LuaContext : DynamicObject
    {
        LuaContext parent;
        Dictionary<string, LuaObject> variables;

        /// <summary>
        /// Used to create scopes
        /// </summary>
        public LuaContext(LuaContext Parent)
        {
            parent = Parent;
            variables = new Dictionary<string, LuaObject>();
        }

        /// <summary>
        /// Creates a base context
        /// </summary>
        public LuaContext() : this(null) { }

        /// <summary>
        /// Sets or creates a variable in the local scope
        /// </summary>
        public void SetLocal(string Name, LuaObject Value)
        {
            if (!variables.ContainsKey(Name))
                variables.Add(Name, Value);
            else
                variables[Name] = Value;
        }

        /// <summary>
        /// Sets or creates a variable in the global scope
        /// </summary>
        public void SetGlobal(string Name, LuaObject Value)
        {
            if (parent == null)
                SetLocal(Name, Value);
            else
                parent.SetGlobal(Name, Value);
        }

        /// <summary>
        /// Returns the nearest declared variable value or nil
        /// </summary>
        public LuaObject Get(string Name)
        {
            if (variables.ContainsKey(Name))
                return variables[Name];
            else if (parent != null)
                return parent.Get(Name);
            else
                return LuaObject.Nil;
        }

        /// <summary>
        /// Sets the nearest declared variable or creates a new one
        /// </summary>
        public void Set(string Name, LuaObject Value)
        {
            if (parent == null || variables.ContainsKey(Name))
            {
                SetLocal(Name, Value);
            }
            else
            {
                parent.Set(Name, Value);
            }
        }

        #region DynamicObject
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = Get(binder.Name);
            if (result == LuaObject.Nil)
                return false;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Set(binder.Name, LuaObject.FromObject(value));
            return true;
        }
        #endregion
    }
}
