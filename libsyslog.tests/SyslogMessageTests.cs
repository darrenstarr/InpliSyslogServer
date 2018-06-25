namespace libsyslog.tests
{
    using System;
    using Xunit;
    using Sprache;

    public class SyslogMessageTests
    {
        [Fact]
        public void Pri()
        {
            var input = " <123>";
            var x = SyslogMessageParser.Pri.Parse(input);
            Assert.Equal(123, x);

            Assert.NotEqual(456, x);
        }

        [Fact]
        public void Version()
        {
            var input = " 992";
            var x = SyslogMessageParser.Version.Parse(input);
            Assert.Equal(992, x);

            Assert.NotEqual(456, x);
        }

        [Fact]
        public void TimeStampNil()
        {
            var input = "-";
            var x = SyslogMessageParser.TimeStamp.Parse(input);
            Assert.Equal(DateTimeOffset.MinValue, x);
        }

        [Fact]
        public void TimeStampNoYear()
        {
            var input = "Jun  4 06:21:04.783";
            var dt = DateTimeOffset.ParseExact("Jun  4 2018 06:21:04.783+00", "MMMdyyyyH:m:s.fffz", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), System.Globalization.DateTimeStyles.AllowWhiteSpaces);
            var x = SyslogMessageParser.TimeStamp.Parse(input);
            Assert.Equal(dt, x);
        }


        [Fact]
        public void HeaderNoDate()
        {
            var input = "  <123> 992: -: %FOO-1-BAR:";
            var x = SyslogMessageParser.Header.Parse(input);
            Assert.NotNull(x);
            Assert.Equal(123, x.Pri);
            Assert.Equal(992, x.Version);
        }

        [Fact]
        public void HeaderCiscoRelativeTime()
        {
            var input = "  <123> 992: Jun  4 06:21:04.783: %FOO-1-BAR:";
            var x = SyslogMessageParser.Header.Parse(input);
            Assert.NotNull(x);
            Assert.Equal(123, x.Pri);
            Assert.Equal(992, x.Version);
        }

        [Fact]
        public void HeaderCiscoUpTime()
        {
            var input = "  <123> 992: 3d21h: %FOO-1-BAR:";
            var x = SyslogMessageParser.Header.Parse(input);
            Assert.NotNull(x);
            Assert.Equal(123, x.Pri);
            Assert.Equal(992, x.Version);
        }

        [Fact]
        public void CiscoTimeYearMsUtc()
        {
            var input = "Jan  5 2018 06:26:36.184 UTC";
            var dt = DateTimeOffset.ParseExact("Jan  5 2018 06:26:36.184+00", "MMMdyyyyH:m:s.fffz", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), System.Globalization.DateTimeStyles.AllowWhiteSpaces);
            var x = SyslogMessageParser.TimeStamp.Parse(input);
            Assert.Equal(dt, x);
        }

        [Fact]
        public void CiscoTimeRelativeNotAuthoritive()
        {
            var input = "*Mar  6 00:00:04 UTC";
            var dt = DateTimeOffset.ParseExact("Mar  6 2018 00:00:04+00", "MMMdyyyyH:m:sz", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), System.Globalization.DateTimeStyles.AllowWhiteSpaces);
            var x = SyslogMessageParser.TimeStamp.Parse(input);
            Assert.Equal(dt, x);
        }

        [Fact]
        public void CiscoTimeYearMsCEST()
        {
            var input = "Jun  5 2018 08:39:48.450 CEST";
            var dt = DateTimeOffset.ParseExact("Jun  5 2018 08:39:48.450+02", "MMMdyyyyH:m:s.fffz", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), System.Globalization.DateTimeStyles.AllowWhiteSpaces);
            var x = SyslogMessageParser.TimeStamp.Parse(input);
            Assert.Equal(dt, x);
        }

        [Fact]
        public void CiscoTimeYearMsNoTimeZone()
        {
            var input = "Jan  5 2018 06:26:36.184";
            var dt = DateTimeOffset.ParseExact("Jan  5 2018 06:26:36.184+00", "MMMdyyyyH:m:s.fffz", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), System.Globalization.DateTimeStyles.AllowWhiteSpaces);
            var x = SyslogMessageParser.TimeStamp.Parse(input);
            Assert.Equal(dt, x);
        }

        [Fact]
        public void ParseMessage()
        {
            var expected = "Process 1, Nbr 10.100.5.7 on Vlan105 from DOWN to DOWN, Neighbor Down: Ignore timer expired";
            var input = "<189>11645: Jun  4 06:28:17.141: %OSPF-5-ADJCHG: Process 1, Nbr 10.100.5.7 on Vlan105 from DOWN to DOWN, Neighbor Down: Ignore timer expired";
            var x = SyslogMessageParser.Message.Parse(input);
            Assert.Equal(expected, x.Message);
        }

        [Fact]
        public void ParseEmptyMessage()
        {
            var x = SyslogMessageParser.Message.Parse("<191>61317:  ");
            Assert.Empty(x.Message);

            x = SyslogMessageParser.Message.Parse("<191>61506:  ");
            Assert.Empty(x.Message);
        }
    }
}
