using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syslog.Server.Config
{
  public class PropertyCollection : ConfigurationElementCollection
  {
    protected override ConfigurationElement CreateNewElement()
    {
      throw new NotImplementedException();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      throw new NotImplementedException();
    }
  }
}
