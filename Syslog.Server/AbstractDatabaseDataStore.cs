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

using System.Collections.Generic;
using System.Threading;

namespace Syslog.Server
{
    /// <summary>
    /// Base class for all storage classes that use a connection string.  This class is thread-safe.
    /// </summary>
	public abstract class AbstractDatabaseDataStore : IDataStore
	{
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="connectionString">The used to connect to a data store.</param>
        protected AbstractDatabaseDataStore(string connectionString)
		{
			ConnectionString = connectionString;
		}

		private string _connectionString;
        /// <summary>
        /// Gets or sets the connectionString
        /// </summary>
		public string ConnectionString
		{
			get
			{
				lock (this)
				{
					try
					{
						return _connectionString;
					}
					finally
					{
						Monitor.Pulse(this);
					}
				}
			}
			set
			{
				lock (this)
				{
					_connectionString = value;
					Monitor.Pulse(this);
				}
			}
		}

	    public abstract bool StoreMessages(IEnumerable<string[]> messages);
	}
}
