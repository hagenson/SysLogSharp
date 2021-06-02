/*
 * Copyright 2010 Andrew Draut
 * 
 * This file is part of Syslog Sharp.
 * 
 * Syslog Sharp is free software: you can redistribute it and/or modify it under the terms of the GNU General 
 * Public License as published by the Free Software Foundation, either version 3 of the License, or (at 
 * your option) any later version.
 * 
 * Syslog Sharp is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even 
 * the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with Syslog Sharp. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Syslog.Server
{
  /// <summary>
  /// Methods and properties used by all handlers (modules).
  /// </summary>
  internal class MessageHandlerBase
  {
    private Assembly _assemblyRef;
    private Type _parserType;
    private IParser _parser;

    private Type _storerType;
    private IDataStore _dataStore;


    /// <summary>
    /// Creates a new instance of the class.
    /// </summary>
    /// <param name="assemblyName">The full compiled name of the handler assembly.</param>
    /// <param name="parserClassName">The name of the class that implements the <see cref="IParser"/> interface in the assembly.</param>
    /// <param name="storerClassName">The name of the class that implements the <see cref="IDataStore"/> interface in the assembly.</param>
    /// <param name="connectionString">The connection string, if required, for the storer class.  This parameter can be null.</param>
    public MessageHandlerBase(string assemblyName, string parserClassName, string storerClassName, string connectionString, IDictionary<string, string> handlerProperties)
    {
      AssemblyName = assemblyName;
      ParserClassName = parserClassName;
      StorerClassName = storerClassName;
      ConnectionString = connectionString;
      HandlerProperties = handlerProperties;
    }

    /// <summary>
    /// Gets or sets the full compiled name of the handler assembly.
    /// </summary>        
    public string AssemblyName { get; set; }

    /// <summary>
    /// Gets or sets the name of the class that implements the <see cref="IParser"/> interface in the assembly.
    /// </summary>
    public string ParserClassName { get; set; }

    /// <summary>
    /// Gets or sets the name of the class that implements the <see cref="IDataStore"/> interface in the assembly.
    /// </summary>
    public string StorerClassName { get; set; }

    /// <summary>
    /// Gets or sets the connection string for the storer class.
    /// </summary>
    public string ConnectionString { get; set; }

    public IDictionary<string, string> HandlerProperties { get; set; }

    /// <summary>
    /// Gets the assembly by the name defined in the parameter <see cref="AssemblyName"/>.
    /// </summary>
    /// <returns>Returns the found assembly.</returns>
    public Assembly GetAssembly()
    {
      if (_assemblyRef == null)
      {
        if (AssemblyName != null)
        {
          _assemblyRef = Assembly.Load(AssemblyName);
        }
      }
      return _assemblyRef;
    }

    /// <summary>
    /// Gets a reference to the class in the <see cref="AssemblyName"/> that matches <see cref="ParserClassName"/>.
    /// </summary>
    /// <returns>Returns the <see cref="IParser"/> class reference.</returns>
    public IParser GetParser()
    {
      if (_parser == null)
      {
        if (_parserType == null)
        {
          _parserType = Type.GetType(ParserClassName);
        }

        if (_parserType != null && _parser == null)
        {
          _parser =  Initialise<IParser>(_parserType, HandlerProperties);
        }
      }

      return _parser;
    }

    /// <summary>
    /// Gets a reference to the class in the <see cref="AssemblyName"/> that matches <see cref="StorerClassName"/>.
    /// </summary>
    /// <returns>Returns the <see cref="IDataStore"/> class reference.</returns>
    public IDataStore GetStorer()
    {
      if (_dataStore == null)
      {
        if (_storerType == null)
        {

          _storerType = Type.GetType(StorerClassName);
        }

        if (_storerType != null && _dataStore == null)
        {
          if (typeof(AbstractDatabaseDataStore).IsAssignableFrom(_storerType))
          {
            _dataStore = (IDataStore)Activator.CreateInstance(_storerType, ConnectionString);
          }
          else
          {
            _dataStore = Initialise<IDataStore>(_storerType, HandlerProperties);
          }
        }
      }
      return _dataStore;
    }

    protected virtual T Initialise<T>(Type type, IDictionary<string, string> settings) where T: class
    {
      T result = null;
      ConstructorInfo cons = type.GetConstructor(new Type[] { typeof(IDictionary<string, string>) });
      if (cons != null)
      {
        // Pass settings to constructor
        result = (T)cons.Invoke(new object[]{settings});
      }
      else
      {
        // Use default constructor
        cons = type.GetConstructor(Type.EmptyTypes);
        if (cons != null)
        {
          result = (T)cons.Invoke(new object[0]);
          // Set the properties using reflection
          result.GetType().GetProperties()
            .All(prop =>
            {
              string val = null;
              if (settings.TryGetValue(prop.Name, out val))
              {
                object valObj = Convert.ChangeType(val, prop.PropertyType);
                prop.SetValue(result, valObj);
              }
              return true;
            });
        }
        else
        {
          throw new MissingMethodException(String.Format("No constructor found for Type {0}.", type.FullName));
        }
      }

      return result;
    }
  }
}
