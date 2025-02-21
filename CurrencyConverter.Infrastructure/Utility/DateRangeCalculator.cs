namespace CurrencyConverter.Infrastructure.Utility
{
    public static class DateRangeCalculator
    {
        public static (string StartDate, string EndDate) GetDateRangeForPagination(DateTime baseStartDate, DateTime baseEndDate, int pageSize, int pageNumber)
        {            
            // Calculate the start date for the given page number
            DateTime startDate = baseStartDate.AddDays((pageNumber - 1) * pageSize);

            // Calculate the end date based on the page size
            DateTime endDate = startDate.AddDays(pageSize - 1);
            if (endDate > baseEndDate)
            {
                endDate = baseEndDate;
            }

            return (startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
        }
    }
}
