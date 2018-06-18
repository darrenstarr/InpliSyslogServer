using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("libsyslog.tests")]

namespace libsyslog
{
    using libsyslog.Model;
    using Sprache;
    using System;
    using System.Text;

    public static class SyslogMessageParser
    {
        public static SyslogMessage ParseMessage(byte [] message)
        {
            return Message.Parse(Encoding.UTF8.GetString(message));
        }

        // NILVALUE = "-"
        internal static readonly Parser<char> NilValue =
            Parse.Char('-')
            .Token();

        // PRIVAL          = 1*3DIGIT ; range 0 .. 191
        internal static readonly Parser<int> PriVal =
            Parse.Number
            .Select(x => int.Parse(x));

        // PRI = "<" PRIVAL ">"
        internal static readonly Parser<int> Pri =
            from leading in Parse.WhiteSpace.Many().Optional()
            from lt in Parse.Char('<')
            from priVal in PriVal
            from gt in Parse.Char('>')
            select priVal;

        // VERSION         = NONZERO-DIGIT 0*2DIGIT
        internal static readonly Parser<int> Version =
            from ws in Parse.WhiteSpace.Many().Optional()
            from value in Parse.Number
            select int.Parse(value);

        //    DATE-FULLYEAR   = 4DIGIT

        //    DATE-MONTH      = 2DIGIT  ; 01-12

        //    DATE-MDAY       = 2DIGIT  ; 01-28, 01-29, 01-30, 01-31 based on
        //                              ; month/year

        //    FULL-DATE       = DATE-FULLYEAR "-" DATE-MONTH "-" DATE-MDAY

        // MonthNameAbbreviation
        internal static readonly Parser<int> MonthNameAbbreviation =
            Parse.String("Jan").Select(x => 1)
            .Or(Parse.String("Feb").Select(x => 2))
            .Or(Parse.String("Mar").Select(x => 3))
            .Or(Parse.String("Apr").Select(x => 4))
            .Or(Parse.String("May").Select(x => 5))
            .Or(Parse.String("Jun").Select(x => 6))
            .Or(Parse.String("Jul").Select(x => 7))
            .Or(Parse.String("Aug").Select(x => 8))
            .Or(Parse.String("Sep").Select(x => 9))
            .Or(Parse.String("Oct").Select(x => 10))
            .Or(Parse.String("Nov").Select(x => 11))
            .Or(Parse.String("Dec").Select(x => 12))
            ;

        internal static readonly Parser<TimeSpan> TimeZone =
            Parse.String("UTC").Select(x => new TimeSpan())
            // GMT
            // Greenwich Mean Time, as UTC
            .Or(Parse.String("GMT").Select(x => new TimeSpan(0, 0, 0)))
            // BST
            // British Summer Time, as UTC + 1 hour
            .Or(Parse.String("BST").Select(x => new TimeSpan(1, 0, 0)))
            // IST
            // Irish Summer Time, as UTC + 1 hour
            .Or(Parse.String("IST").Select(x => new TimeSpan(1, 0, 0)))
            // WET
            // Western Europe Time, as UTC
            .Or(Parse.String("WET").Select(x => new TimeSpan(0, 0, 0)))
            // WEST
            // Western Europe Summer Time, as UTC + 1 hour
            .Or(Parse.String("WEST").Select(x => new TimeSpan(0, 0, 0)))
            // CET
            // Central Europe Time, as UTC + 1
            .Or(Parse.String("CET").Select(x => new TimeSpan(1, 0, 0)))
            // CEST
            // Central Europe Summer Time, as UTC + 2
            .Or(Parse.String("CEST").Select(x => new TimeSpan(2, 0, 0)))
            // EET
            // Eastern Europe Time, as UTC + 2
            .Or(Parse.String("EET").Select(x => new TimeSpan(2, 0, 0)))
            // EEST
            // Eastern Europe Summer Time, as UTC + 3
            .Or(Parse.String("EEST").Select(x => new TimeSpan(3, 0, 0)))
            // MSK
            // Moscow Time, as UTC + 3
            .Or(Parse.String("MSK").Select(x => new TimeSpan(3, 0, 0)))
            // MSD
            // Moscow Summer Time, as UTC + 4
            .Or(Parse.String("MSD").Select(x => new TimeSpan(4, 0, 0)))
            // AST
            // Atlantic Standard Time, as UTC -4 hours
            .Or(Parse.String("AST").Select(x => new TimeSpan(-4, 0, 0)))
            // ADT
            // Atlantic Daylight Time, as UTC -3 hours
            .Or(Parse.String("ADT").Select(x => new TimeSpan(-3, 0, 0)))
            // ET
            // Eastern Time, either as EST or EDT, depending on place and time of year
            .Or(Parse.String("ET").Select(x => new TimeSpan(-5, 0, 0)))
            // EST
            // Eastern Standard Time, as UTC -5 hours
            .Or(Parse.String("EST").Select(x => new TimeSpan(-5, 0, 0)))
            // EDT
            // Eastern Daylight Saving Time, as UTC -4 hours
            .Or(Parse.String("EDT").Select(x => new TimeSpan(-4, 0, 0)))
            // CT
            // Central Time, either as CST or CDT, depending on place and time of year
            .Or(Parse.String("CT").Select(x => new TimeSpan(-6, 0, 0)))
            // CST
            // Central Standard Time, as UTC -6 hours
            .Or(Parse.String("CST").Select(x => new TimeSpan(-6, 0, 0)))
            // CDT
            // Central Daylight Saving Time, as UTC -5 hours
            .Or(Parse.String("CDT").Select(x => new TimeSpan(-5, 0, 0)))
            // MT
            // Mountain Time, either as MST or MDT, depending on place and time of year
            .Or(Parse.String("MT").Select(x => new TimeSpan(-7, 0, 0)))
            // MST
            // Mountain Standard Time, as UTC -7 hours
            .Or(Parse.String("MST").Select(x => new TimeSpan(-7, 0, 0)))
            // MDT
            // Mountain Daylight Saving Time, as UTC -6 hours
            .Or(Parse.String("MDT").Select(x => new TimeSpan(-6, 0, 0)))
            // PT
            // Pacific Time, either as PST or PDT, depending on place and time of year
            .Or(Parse.String("PT").Select(x => new TimeSpan(-8, 0, 0)))
            // PST
            // Pacific Standard Time, as UTC -8 hours
            .Or(Parse.String("PST").Select(x => new TimeSpan(-8, 0, 0)))
            // PDT
            // Pacific Daylight Saving Time, as UTC -7 hours
            .Or(Parse.String("PDT").Select(x => new TimeSpan(-7, 0, 0)))
            // AKST
            // Alaska Standard Time, as UTC -9 hours
            .Or(Parse.String("AKST").Select(x => new TimeSpan(-9, 0, 0)))
            // AKDT
            // Alaska Standard Daylight Saving Time, as UTC -8 hours
            .Or(Parse.String("AKDT").Select(x => new TimeSpan(-8, 0, 0)))
            // HST
            // Hawaiian Standard Time, as UTC -10 hours
            .Or(Parse.String("HST").Select(x => new TimeSpan(-10, 0, 0)))
            // WST
            // Western Standard Time, as UTC + 8 hours
            .Or(Parse.String("GMT").Select(x => new TimeSpan(8, 0, 0)))
            // CST
            // Central Standard Time, as UTC + 9.5 hours
            .Or(Parse.String("CST").Select(x => new TimeSpan(9, 30, 0)))
            // EST
            // Eastern Standard/Summer Time, as UTC + 10 hours (+11 hours during summer time)
            .Or(Parse.String("EST").Select(x => new TimeSpan(10, 0, 0)))
            ;

        // CISCO-SYSLOG-RELATIVE-TIME
        internal static readonly Parser<DateTimeOffset> CiscoSyslogRelativeTime =
            from authoritative in (
                from authoritative in Parse.Char('*')
                from whiteSpace in Parse.WhiteSpace.Many().Optional()
                select authoritative
            ).Optional()
            from month in MonthNameAbbreviation
            from ws in Parse.WhiteSpace.Many().Optional()
            from day in Parse.Number
            from ws2 in Parse.WhiteSpace.Many().Optional()
            from hour in Parse.Number
            from colon1 in Parse.Char(':')
            from minute in Parse.Number
            from colon2 in Parse.Char(':')
            from second in Parse.Number
            from millisecond in (
                from period in Parse.Char('.')
                from millisecond in Parse.Number
                select millisecond
            ).Optional()
            from timeZone in (
                from ws4 in Parse.WhiteSpace.AtLeastOnce()
                from timeZone in TimeZone
                select timeZone
            ).Optional()
            select
                new DateTimeOffset(
                    DateTimeOffset.Now.Year,
                    month,
                    int.Parse(day),
                    int.Parse(hour),
                    int.Parse(minute),
                    int.Parse(second),
                    millisecond.IsDefined ? int.Parse(millisecond.Get()) : 0,
                    (timeZone.IsDefined ? timeZone.Get() : new TimeSpan())
                );

        // CISCO-SYSLOG-TIME
        internal static readonly Parser<DateTimeOffset> CiscoSyslogTimeMs =
            from authoritative in (
                from authoritative in Parse.Char('*')
                from whiteSpace in Parse.WhiteSpace.Many().Optional()
                select authoritative
            ).Optional()
            from month in MonthNameAbbreviation
            from day in (
                from whiteSpace in Parse.WhiteSpace.AtLeastOnce()
                from day in Parse.Number
                select day
            )
            from year in (
                from whiteSpace in Parse.WhiteSpace.AtLeastOnce()
                from year in Parse.Number
                select year
            )
            from ws3 in Parse.WhiteSpace.AtLeastOnce()
            from hour in Parse.Number
            from colon1 in Parse.Char(':')
            from minute in Parse.Number
            from colon2 in Parse.Char(':')
            from second in Parse.Number
            from millisecond in (
                from period in Parse.Char('.')
                from millisecond in Parse.Number
                select millisecond
            ).Optional()
            from timeZone in (
                from ws4 in Parse.WhiteSpace.AtLeastOnce()
                from timeZone in TimeZone
                select timeZone
            ).Optional()
            select
                new DateTimeOffset(
                    int.Parse(year),
                    month,
                    int.Parse(day),
                    int.Parse(hour),
                    int.Parse(minute),
                    int.Parse(second),
                    millisecond.IsDefined ? int.Parse(millisecond.Get()) : 0,
                    (timeZone.IsDefined ? timeZone.Get() : new TimeSpan())
                );

        // TODO : Somehow alert the user that these timestamps are absolutely useless
        internal static readonly Parser<DateTimeOffset> CiscoUpTime =
            from days in Parse.Number
            from d in Parse.Char('d')
            from hours in Parse.Number
            from h in Parse.Char('h')
            select
                DateTimeOffset.MinValue;

        // TIMESTAMP = NILVALUE / FULL - DATE "T" FULL-TIME
        internal static readonly Parser<DateTimeOffset> TimeStamp =
            NilValue.Select(x => DateTimeOffset.MinValue)
            .Or(CiscoSyslogRelativeTime)
            .Or(CiscoSyslogTimeMs)
            .Or(CiscoUpTime)
            ;

        internal static readonly Parser<SyslogMessageType> FacilitySeverityMnemonic =
            from ws in Parse.WhiteSpace.Many().Optional()
            from percent in Parse.Char('%')
            from facility in Parse.Regex(@"[A-Z][A-Z0-9_]*")
            from severityLevel in (
                from dash in Parse.Char('-')
                from severityCode in Parse.Number
                select (ESyslogSeverityLevel)int.Parse(severityCode)
            )
            from mnemonic in (
                from dash in Parse.Char('-')
                from mnemonic in Parse.Regex(@"[A-Z][A-Z0-9_]*")
                select mnemonic
            )
            select
                new SyslogMessageType
                {
                    Facility = facility,
                    Severity = severityLevel,
                    Mnemonic = mnemonic,
                };

        // HEADER          = PRI VERSION SP TIMESTAMP SP HOSTNAME
        //                   SP APP-NAME SP PROCID SP MSGID
        internal static readonly Parser<SyslogMessageHeader> Header =
            from pri in Pri
            from version in Version
            from timeStamp in (
                from colon in Parse.Char(':')
                from whiteSpace in Parse.WhiteSpace.Many().Optional()
                from timeStamp in TimeStamp
                select timeStamp
            ).Optional()
            from messageType in (
                from colon in Parse.Char(':')
                from whiteSpace in Parse.WhiteSpace.Many().Optional()
                from messageType in FacilitySeverityMnemonic
                select messageType
            ).Optional()
            from trailingColon in (
                from leadingWhiteSpace in Parse.WhiteSpace.Many().Optional()
                from colon in Parse.Char(':')
                from whiteSpace in Parse.WhiteSpace.Many().Optional()
                select colon
            )
            select new SyslogMessageHeader
            {
                Pri = pri,
                Version = version,
                TimeStamp = timeStamp.IsDefined ? timeStamp.Get() : DateTimeOffset.MinValue,
                MessageType = messageType.IsDefined ? messageType.Get() : null,
            };

        internal static readonly Parser<SyslogMessage> Message =
            from header in Header
            from message in Parse.AnyChar.AtLeastOnce().End()
            select
                new SyslogMessage
                {
                    Header = header,
                    Message = string.Join("", message)
                };
    }
}
