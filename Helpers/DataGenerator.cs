﻿using LibraryManagement.Models;
using LibraryManagement.Enums;
using System;
using System.Collections.Generic;

namespace LibraryManagement.Helpers
{
    public static class DataGenerator
    {
        private static Random random = new Random();

        public static List<LoanData> GenerateRandomLoanData(int startYear, int endYear)
        {
            var loanDataList = new List<LoanData>();

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

            return loanDataList;
        }
    }
}
