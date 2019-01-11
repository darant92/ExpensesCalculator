using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpensesCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            string EmployeesSpendings = FileSteamer.ReadDataFromFile();

            if(EmployeesSpendings == null)
            {
                return;
            }

            CurrencyConverter currencyConverter = new CurrencyConverter();
            IEnumerable<string> convertedEmployeesSpendings = currencyConverter.AggregatedData(EmployeesSpendings);

            if (convertedEmployeesSpendings.Any())
            {
                return;
            }
            
            FileSteamer.WriteDataToFile(convertedEmployeesSpendings);
        }
    }
}
