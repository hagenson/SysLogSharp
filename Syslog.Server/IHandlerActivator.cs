using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syslog.Server
{
  public interface IHandlerActivator<T>
  {
    T Activate(IDictionary<string, string> settings);
  }
}
