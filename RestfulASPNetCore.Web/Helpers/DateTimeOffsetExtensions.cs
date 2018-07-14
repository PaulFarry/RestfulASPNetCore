using System;

namespace RestfulASPNetCore.Web.Helpers
{
    public static class DateTimeOffsetExtensions
    {
        public static int GetCurrentAge(this DateTimeOffset dateTimeOffset, DateTimeOffset? dateOfDeath)
        {
            var calculateTo = DateTime.UtcNow;
            if (dateOfDeath.HasValue)
            {
                calculateTo = dateOfDeath.Value.UtcDateTime;
            }

            int age = calculateTo.Year - dateTimeOffset.Year;
            if (calculateTo < dateTimeOffset.AddYears(age))
            {
                age--;
            }
            return age;
        }
    }
}
