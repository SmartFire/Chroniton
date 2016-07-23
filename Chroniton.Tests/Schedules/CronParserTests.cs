﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Chroniton.Schedules.Cron;

namespace Chroniton.Tests.Schedules
{
    public class CronParserTests
    {
        [Fact]
        public void CtorShouldNotFail()
        {
            Assert.NotNull(new CronParser());
        }
    }

    public class ParseTests
    {
        CronParser parserUnderTest;

        void initParser()
        {
            parserUnderTest = new CronParser();
        }

        [Fact]
        public void ShouldReturnFinder()
        {
            CronParser parser = new CronParser();
            var result = parser.Parse("0 1 2 3 JAN ? 2000");
            Assert.NotNull(result);
        }

        [Theory
            , InlineData(null)
            , InlineData("")
            , InlineData("easter egg hint: relativity")
            , InlineData("0 1 2 3 JAN SUN 2000")
            , InlineData("0 1 2 3,4W JAN ? 2000")
            , InlineData("*/13 1 2 3 JAN ? 2000")
            ]
        public void ShouldThrow(string cron)
        {
            initParser();
            Assert.Throws(typeof(CronParsingException), () => parserUnderTest.Parse(cron));
        }


        public class GetNextTests : ParseTests
        {
            [Theory
                // Year
                , InlineData("0 0 0 * * * 2017", "1/1/2017 00:00:00", "1/2/2017 00:00:00")
                , InlineData("0 0 0 * * * 2016", "1/1/2017 00:00:00", null)

                // Month
                , InlineData("0 0 0 * JAN ? *", "1/1/2017 00:00:00", "1/2/2017 00:00:00")
                , InlineData("0 0 0 * JAN ? *", "1/31/2017 00:00:00", "1/1/2018 00:00:00")
                , InlineData("0 0 0 * JAN-FEB ? *", "1/31/2017 00:00:00", "2/1/2017 00:00:00")
                , InlineData("0 0 0 * JAN-FEB ? *", "2/28/2017 00:00:00", "1/1/2018 00:00:00")
                , InlineData("0 0 0 * JAN-FEB ? *", "3/28/2017 00:00:00", "1/1/2018 00:00:00")
                , InlineData("0 0 0 * JAN,MAR ? *", "3/31/2017 00:00:00", "1/1/2018 00:00:00")
                , InlineData("0 0 0 * JAN,MAR ? *", "1/31/2017 00:00:00", "3/1/2017 00:00:00")
                , InlineData("0 0 0 * JAN,MAR ? *", "1/1/2017 00:00:00", "1/2/2017 00:00:00")
                , InlineData("0 0 0 * JAN,MAR ? *", "2/1/2017 00:00:00", "3/1/2017 00:00:00")
                , InlineData("0 0 0 * JAN,MAR ? *", "4/1/2017 00:00:00", "1/1/2018 00:00:00")

                //day of Month
                , InlineData("0 0 0 1 * ? *", "1/1/2017 00:00:00", "2/1/2017 00:00:00")
                , InlineData("0 0 0 2 * ? *", "1/1/2017 00:00:00", "1/2/2017 00:00:00")
                , InlineData("0 0 0 29-31 * ? *", "2/1/2017 00:00:00", "2/28/2017 00:00:00")
                , InlineData("0 0 0 29-31 * ? *", "2/28/2017 00:00:00", "3/29/2017 00:00:00")
                , InlineData("0 0 0 2,3 * ? *", "1/1/2017 00:00:00", "1/2/2017 00:00:00")
                , InlineData("0 0 0 2,15 * ? *", "1/2/2017 00:00:00", "1/15/2017 00:00:00")
                , InlineData("0 0 0 2,15 * ? *", "1/16/2017 00:00:00", "2/2/2017 00:00:00")
                , InlineData("0 0 0 2-15 * ? *", "1/16/2017 00:00:00", "2/2/2017 00:00:00")
                , InlineData("0 0 0 2-15 * ? *", "1/1/2017 00:00:00", "1/2/2017 00:00:00")
                , InlineData("0 0 0 2-15 * ? *", "1/10/2017 00:00:00", "1/11/2017 00:00:00")
                , InlineData("0 0 0 2-15 * ? *", "1/15/2017 00:00:00", "2/2/2017 00:00:00")
                //day of month special characters
                , InlineData("0 0 0 L * ? *", "1/15/2017 00:00:00", "1/31/2017 00:00:00")
                , InlineData("0 0 0 1W * ? *", "3/15/2017 00:00:00", "4/3/2017 00:00:00")
                , InlineData("0 0 0 30W * ? *", "4/15/2017 00:00:00", "4/28/2017 00:00:00")
                , InlineData("0 0 0 2W * ? *", "3/15/2017 00:00:00", "4/3/2017 00:00:00")
                , InlineData("0 0 0 29W * ? *", "4/15/2017 00:00:00", "4/28/2017 00:00:00")
                , InlineData("0 0 0 3W * ? *", "3/15/2017 00:00:00", "4/3/2017 00:00:00")
                , InlineData("0 0 0 28W * ? *", "4/15/2017 00:00:00", "4/28/2017 00:00:00")
                , InlineData("0 0 0 3W * ? *", "4/3/2017 00:00:00", "5/3/2017 00:00:00")
                , InlineData("0 0 0 28W * ? *", "4/28/2017 00:00:00", "5/29/2017 00:00:00")

                //day of week
                , InlineData("0 0 0 ? * MON-FRI *", "1/1/2017 00:00:00", "1/2/2017 00:00:00")//hyphen day of week
                , InlineData("0 0 0 ? * MON-FRI *", "1/2/2017 00:00:00", "1/3/2017 00:00:00")
                , InlineData("0 0 0 ? * MON,FRI *", "1/2/2017 00:00:00", "1/6/2017 00:00:00")// comma day of week
                , InlineData("0 0 0 ? * 1,5 *", "1/2/2017 00:00:00", "1/6/2017 00:00:00")
                , InlineData("0 0 0 ? * 1,4-5 *", "1/2/2017 00:00:00", "1/5/2017 00:00:00")// commma hyphen day of week
                , InlineData("0 0 0 ? * SUN#2 *", "1/31/1998 00:00:00", "2/8/1998 00:00:00")// specific day of week
                , InlineData("0 0 0 ? * 0L *", "1/31/1998 00:00:00", "2/22/1998 00:00:00")//last day of week
                , InlineData("0 0 0 ? * SUN#5 *", "1/31/1998 00:00:00", "2/22/1998 00:00:00")

                // Hour
                , InlineData("0 0 * * * ? *", "1/1/2000 00:00:00", "1/1/2000 01:00:00")
                , InlineData("0 0 0 * * ? *", "1/1/2000 00:00:00", "1/2/2000 00:00:00")
                , InlineData("0 0 1 * * ? *", "1/1/2000 00:00:00", "1/1/2000 01:00:00")
                , InlineData("0 0 1,3 * * ? *", "1/1/2000 00:00:00", "1/1/2000 01:00:00")
                , InlineData("0 0 1,3 * * ? *", "1/1/2000 01:00:00", "1/1/2000 03:00:00")
                , InlineData("0 0 1-3 * * ? *", "1/1/2000 01:00:00", "1/1/2000 02:00:00")
                , InlineData("0 0 1-3,13 * * ? *", "1/1/2000 01:00:00", "1/1/2000 02:00:00")
                , InlineData("0 0 1-3,13 * * ? *", "1/1/2000 03:00:00", "1/1/2000 13:00:00")
                , InlineData("0 0 */2 * * ? *", "1/1/2000 03:00:00", "1/1/2000 4:00:00")
                , InlineData("0 0 */3 * * ? *", "1/1/2000 03:00:00", "1/1/2000 6:00:00")
                , InlineData("0 0 */4 * * ? *", "1/1/2000 03:00:00", "1/1/2000 4:00:00")
                , InlineData("0 0 */4 * * ? *", "1/1/2000 05:00:00", "1/1/2000 8:00:00")
                , InlineData("0 0 */6 * * ? *", "1/1/2000 05:00:00", "1/1/2000 6:00:00")
                , InlineData("0 0 */12 * * ? *", "1/1/2000 05:00:00", "1/1/2000 12:00:00")
                , InlineData("0 0 */12 * * ? *", "1/1/2000 12:00:00", "1/2/2000 00:00:00")

                // Minute    
                , InlineData("0 * * * * ? *", "1/1/2000 12:00:00", "1/1/2000 12:01:00")
                , InlineData("0 0 * * * ? *", "1/1/2000 12:00:00", "1/1/2000 13:00:00")
                , InlineData("0 0-5,10 * * * ? *", "1/1/2000 12:00:00", "1/1/2000 12:01:00")
                , InlineData("0 0-5,10 * * * ? *", "1/1/2000 12:05:00", "1/1/2000 12:10:00")
                , InlineData("0 0-5,10 * * * ? *", "1/1/2000 12:10:00", "1/1/2000 13:00:00")
                , InlineData("0 0,5-10 * * * ? *", "1/1/2000 12:10:00", "1/1/2000 13:00:00")
                , InlineData("0 */2 * * * * *", "1/1/2000 12:10:00", "1/1/2000 12:12:00")
                , InlineData("0 */20 * * * * *", "1/1/2000 12:10:00", "1/1/2000 12:20:00")
                , InlineData("0 */15 * * * * *", "1/1/2000 12:16:00", "1/1/2000 12:30:00")
                , InlineData("0 */30 * * * * *", "1/1/2000 12:16:00", "1/1/2000 12:30:00")

                // Second    
                , InlineData("* * * * * ? *", "1/1/2000 12:00:00", "1/1/2000 12:00:01")
                , InlineData("0 * * * * ? *", "1/1/2000 12:00:00", "1/1/2000 12:01:00")
                , InlineData("0-5,10 * * * * ? *", "1/1/2000 12:00:00", "1/1/2000 12:00:01")
                , InlineData("0-5,10 * * * * ? *", "1/1/2000 12:00:05", "1/1/2000 12:00:10")
                , InlineData("0-5,10 * * * * ? *", "1/1/2000 12:00:10", "1/1/2000 12:01:00")
                , InlineData("0,5-10 * * * * ? *", "1/1/2000 12:10:00", "1/1/2000 12:10:05")
                , InlineData("*/2 * * * * * *", "1/1/2000 12:10:00", "1/1/2000 12:10:02")
                , InlineData("*/20 * * * * * *", "1/1/2000 12:10:00", "1/1/2000 12:10:20")
                , InlineData("*/15 * * * * * *", "1/1/2000 12:16:00", "1/1/2000 12:16:15")
                , InlineData("*/30 * * * * * *", "1/1/2000 12:16:00", "1/1/2000 12:16:30")

                //// some combined
                , InlineData("*/30 15,45 */12 15W 1,3,5,7,9,11 ? *", "1/1/2016 00:00:00", "1/15/2016 00:15:30")
                , InlineData("*/30 15,45 */12 15W 1,3,5,7,9,11 ? *", "5/15/2016 22:15:35", "5/16/2016 00:15:00")
                , InlineData("*/30 15,45 */12 16W 1,3,5,7,9,11 ? *", "5/16/2016 22:15:35", "7/15/2016 00:15:00")

                // worst cases
                , InlineData("0 0 0 ? * SUN *", "1/31/1998 00:00:00", "2/1/1998 00:00:00")
                , InlineData("0 0 0 1 JAN ? *", "1/1/2000 00:00:00", "1/1/2001 00:00:00")
                , InlineData("0 0 0 1 JAN ? 2000", "1/1/2000 00:00:00", null)
                ]
            public void ShouldReturnCorrectly(string input, string startDate, string expectedDate)
            {
                initParser();
                var finder = parserUnderTest.Parse(input);
                var next = finder.GetNext(DateTime.Parse(startDate));
                if (expectedDate == null)
                {
                    Assert.Null(next);
                }
                else
                {
                    Assert.Equal(DateTime.Parse(expectedDate), next);
                }
            }
        }
    }
}
