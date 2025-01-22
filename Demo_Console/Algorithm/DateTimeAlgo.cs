namespace Demo_Console.Algorithm;

public static class DateTimeAlgo
{
    public static string[] DaysOfWeek = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    
    public static int[] DaysOfMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
    public static int[] DaysOfMonthLeapYear = [31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

    // Calculation based on today anchor!
    private const int TodayDay = 23;
    private const int TodayMonth = 1;
    private const int TodayYear = 2025;
    private const int TodayDayOfWeekIdx = 4;
    
    public static string GetDayOfWeek(int year, int month, int day)
    {
        try
        {
            new DateTime(year, month, day);
        }
        catch (Exception)
        {
            return "Invalid Date value";
        }

        var distance = 0;
        var isFuture = false;
        var monthIdx = month - 1;
        var todayMonthIdx = TodayMonth - 1;
        
        if(year == TodayYear)
        {
            if (month == TodayMonth) return ExtractDayOfWeekName(day > TodayDay, Math.Abs(TodayDay - day));
            
            var monthToUse = IsLeapYear(year) ? DaysOfMonthLeapYear : DaysOfMonth;
            if (month > TodayMonth)
            {
                for (var i = todayMonthIdx; i < month; i++)
                {
                    if(i == todayMonthIdx) distance += monthToUse[i] - TodayDay;
                    else if (i == monthIdx) distance += day;
                    else distance += monthToUse[i];
                }
                isFuture = true;
            }
            else
            {
                for (var i = monthIdx; i < TodayMonth; i++)
                {
                    if(i == monthIdx) distance += monthToUse[i] - day;
                    else if (i == todayMonthIdx) distance += TodayDay;
                    else distance += monthToUse[i];
                }
                isFuture = false;
            }
            
            return ExtractDayOfWeekName(isFuture, distance);
        }
        
        if (year > TodayYear)
        {
            for (var i = TodayYear + 1; i < year; i++)
            {
                distance += IsLeapYear(i) ? 366 : 365;
            }

            var monthToUse = IsLeapYear(TodayYear) ? DaysOfMonthLeapYear : DaysOfMonth;
            for (var i = todayMonthIdx; i < 12; i++)
            {
                if(i == todayMonthIdx) distance += monthToUse[i] - TodayDay;
                else distance += monthToUse[i];
            }

            monthToUse = IsLeapYear(year) ? DaysOfMonthLeapYear : DaysOfMonth;
            for (var i = 0; i < month; i++)
            {
                if(i == monthIdx) distance += day;
                else distance += monthToUse[i];
            }
            isFuture = true;
        }
        else
        {
            for (var i = year + 1; i < TodayYear; i++)
            {
                distance += IsLeapYear(i) ? 366 : 365;
            }
            
            var monthToUse = IsLeapYear(TodayYear) ? DaysOfMonthLeapYear : DaysOfMonth;
            for (var i = todayMonthIdx; i >= 0 ; i--)
            {
                if(i == todayMonthIdx) distance += TodayDay;
                else distance += monthToUse[i];
            }

            monthToUse = IsLeapYear(year) ? DaysOfMonthLeapYear : DaysOfMonth;
            for (var i = monthIdx; i < 12; i++)
            {
                if(i == monthIdx) distance += monthToUse[i] - day;
                else distance += monthToUse[i];
            }
            isFuture = false;
        }

        return ExtractDayOfWeekName(isFuture, distance);
    }

    private static string ExtractDayOfWeekName(bool isFuture, int distance)
    {
        var distanceCompression = distance % DaysOfWeek.Length;
        return isFuture ? DaysOfWeek[(TodayDayOfWeekIdx + distanceCompression) % DaysOfWeek.Length] :
            TodayDayOfWeekIdx >= distanceCompression ? 
                DaysOfWeek[TodayDayOfWeekIdx - distanceCompression] :
                DaysOfWeek[DaysOfWeek.Length + TodayDayOfWeekIdx - distanceCompression];
    }
    
    public static string GetDayOfWeekRaw(int year, int month, int day)
    {
        if (month is < 1 or > 12) throw new ArgumentException("Month must be in range 1 to 12", nameof(month));
        if (day is < 1 or > 31) throw new ArgumentException("Month must be in range 1 to 12", nameof(month));
        if(month == 2 && day > 29) throw new ArgumentException("February must be in range 1 to 29", nameof(month));
        
        var distance = 0;
        var isFuture = false; 
        
        if(year == TodayYear)
        {
            var monthToUse = IsLeapYear(year) ? DaysOfMonthLeapYear : DaysOfMonth;
            
            if (month == TodayMonth) distance = Math.Abs(TodayDay - day);
            else if (month > TodayMonth)
            {
                for (var i = TodayMonth - 1; i < month; i++)
                {
                    if(i == TodayMonth - 1) distance += monthToUse[i] - TodayDay;
                    else if (i == month - 1) distance += day;
                    else distance += monthToUse[i];
                }
                isFuture = true;
            }
            else
            {
                for (var i = month - 1; i < TodayMonth; i++)
                {
                    if(i == month - 1) distance += monthToUse[i] - day;
                    else if (i == TodayMonth - 1) distance += TodayDay;
                    else distance += monthToUse[i];
                }
            }
        }
        else if (year > TodayYear)
        {
            for (var i = TodayYear + 1; i < year; i++)
            {
                distance += IsLeapYear(i) ? 366 : 365;
            }

            var monthToUse = IsLeapYear(TodayYear) ? DaysOfMonthLeapYear : DaysOfMonth;
            for (var i = TodayMonth - 1; i < 12; i++)
            {
                if(i == TodayMonth - 1) distance += monthToUse[i] - TodayDay;
                else distance += monthToUse[i];
            }

            monthToUse = IsLeapYear(year) ? DaysOfMonthLeapYear : DaysOfMonth;
            for (var i = 0; i < month; i++)
            {
                if(i == month - 1) distance += day;
                else distance += monthToUse[i];
            }
            isFuture = true;
        }
        else
        {
            for (var i = year + 1; i < TodayYear; i++)
            {
                distance += IsLeapYear(i) ? 366 : 365;
            }
            
            var monthToUse = IsLeapYear(TodayYear) ? DaysOfMonthLeapYear : DaysOfMonth;
            for (var i = TodayMonth - 1; i >= 0 ; i--)
            {
                if(i == TodayMonth - 1) distance += TodayDay;
                else distance += monthToUse[i];
            }

            monthToUse = IsLeapYear(year) ? DaysOfMonthLeapYear : DaysOfMonth;
            for (var i = month - 1; i < 12; i++)
            {
                if(i == month - 1) distance += monthToUse[i] - day;
                else distance += monthToUse[i];
            }
        }

        var distanceCompression = distance % 7;

        if (isFuture)
        {
            return DaysOfWeek[(TodayDayOfWeekIdx + distanceCompression) % 7];
        }
        else
        {
            return TodayDayOfWeekIdx >= distanceCompression
                ? DaysOfWeek[TodayDayOfWeekIdx - distanceCompression]
                : DaysOfWeek[DaysOfWeek.Length + TodayDayOfWeekIdx - distanceCompression];
        }
    }

    public static bool IsLeapYear(int year) => year % 100 == 0 ? year % 400 == 0 : year % 4 == 0;
}