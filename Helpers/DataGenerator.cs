using LibraryManagement.Models;
using LibraryManagement.Enums;
using System;
using System.Collections.Generic;
using Serilog;

namespace LibraryManagement.Helpers
{
    public static class DataGenerator
    {
        private static Random random = new Random();

        public static List<LoanData> GenerateRandomLoanData(int startYear, int endYear)
        {
            var loanDataList = new List<LoanData>();

            try
            {
                foreach (Year year in Enum.GetValues(typeof(Year)))
                {
                    if ((int)year < startYear || (int)year > endYear)
                        continue;

                    foreach (Month month in Enum.GetValues(typeof(Month)))
                    {
                        loanDataList.Add(new LoanData
                        {
                            Month = month,
                            Year = year,
                            Loans = random.Next(20, 120)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"-------->An error occurred (DataGenerator): {ex.Message}");
            }

            return loanDataList;
        }
    }
}
