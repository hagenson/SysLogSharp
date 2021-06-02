using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Syslog.Server.Config;
using System.Xml;
using System.IO;

namespace Syslog.UnitTests.Config
{
  [TestClass]
  public class TestConfiguration
  {
    private class SectionStub : HandlerSection
    {
      public void LoadFromString(string xml)
      {
        using (XmlReader reader = new XmlTextReader(new StringReader(xml)))
        {
          base.DeserializeSection(reader);
        }
      }
    }

    [TestMethod]
    public void LoadSection()
    {
      SectionStub uut = new SectionStub();
      uut.LoadFromString(@"<handlerSection>
    <handlers>
      <add storageClassName=""Syslog.TrafficLogger.DbStorer,TrafficLogger"" parserClassName=""Syslog.TrafficLogger.MessageParer,TrafficLogger"" filterIPAdresses="""" connectionString=""Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=syslog;Data Source=.\sql2008r2"">
        <handlerProperties>
          <add key=""match"" value="" type=traffic .* subtype=allowed "" />
          <add key=""table"" value=""sys_log"" />
        </handlerProperties>
      </add>
    </handlers>
  </handlerSection>
");

      Assert.AreEqual(1, uut.Handlers.Count);
      Assert.AreEqual(2, uut.Handlers[0].HandlerProperties.Count);
      Assert.AreEqual(" type=traffic .* subtype=allowed ", uut.Handlers[0].HandlerProperties["match"].Value);
      Assert.AreEqual("sys_log", uut.Handlers[0].HandlerProperties["table"].Value);
    }
  }
}
