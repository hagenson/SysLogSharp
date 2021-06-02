using Syslog.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Syslog.Server.BoundHandlers
{
  public class RegexParser : IParser
  {
    private List<Regex> colMatchers;
    public RegexParser(IDictionary<string, string> settings)
    {
      colMatchers = new List<Regex>();
      string accept;
      if (settings.TryGetValue("accept", out accept))
        acceptRegex = new Regex(accept);

      int i = 1;
      string pattern;
      while (settings.TryGetValue("col" + i.ToString(), out pattern))
      {
        colMatchers.Add(new Regex(pattern));
      }
    }


    public string[] Parse(SyslogMessage message)
    {
      lock (colMatchers)
      {
        if (message == null || String.IsNullOrWhiteSpace(message.Message))
          return null;

        // Is this an interesting message?
        if (acceptRegex != null && !acceptRegex.IsMatch(message.Message))
          return null;

        // Date is always column 0
        string[] result = new string[] { message.Timestamp.ToUniversalTime().ToString("yyyy-MM-DD hh:mm:ss") };

        // Get the bits we want
        result = result.Concat(
          colMatchers.Select(regex =>
          {
            Match match = regex.Match(message.Message);
            return match != null ? match.Value : null;
          }
          )).ToArray();
        return result;
      }
    }

    private Regex acceptRegex;
    private Regex dstRegex;
    private Regex srcRegex;
    private Regex sentRegex;
    private Regex rcvdRegex;


  }
}
