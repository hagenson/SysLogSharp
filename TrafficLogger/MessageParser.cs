using Syslog.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Syslog.TrafficLogger
{
    public class MessageParser : IParser
    {
        public MessageParser()
        {
            Trace.WriteLine("Creating DbStorer");
            typeRegex = new Regex(" type=traffic ");
            subtypeRegex = new Regex(" subtype=allowed ");
            srcRegex = new Regex(" src=([^ ]*) ");
            dstRegex = new Regex(" dst=([^ ]*) ");
            sentRegex = new Regex(" sent=([0-9]+) ");
            rcvdRegex = new Regex(" rcvd=([0-9]+) ");
        }


        public string[] Parse(SyslogMessage message)
        {
            Trace.WriteLine("Parsing message");

            if (message == null || String.IsNullOrWhiteSpace(message.Message))
            {
                Trace.WriteLine("Empty message");
                return null;
            }

            // Is this an interesting message?
            if (!typeRegex.IsMatch(message.Message) || !subtypeRegex.IsMatch(message.Message))
            {
                Trace.WriteLine("Message rejected - no regex match");
                return null;
            }

            // Get the bits we want
            string[] result = new string[5];
            result[0] = message.Timestamp.ToUniversalTime().ToString("yyyy-MM-DD hh:mm:ss");
            result[1] = srcRegex.Match(message.Message).Value;
            result[2] = dstRegex.Match(message.Message).Value;
            result[3] = sentRegex.Match(message.Message).Value;
            result[4] = rcvdRegex.Match(message.Message).Value;

            Trace.WriteLine("Message parsed");

            return result;
        }

        private Regex typeRegex;
        private Regex subtypeRegex;
        private Regex dstRegex;
        private Regex srcRegex;
        private Regex sentRegex;
        private Regex rcvdRegex;


    }
}
