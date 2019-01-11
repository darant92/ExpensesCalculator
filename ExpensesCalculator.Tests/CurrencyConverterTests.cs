using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpensesCalculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesCalculator.Tests
{
    [TestClass()]
    public class CurrencyConverterTests
    {
        [TestMethod()]
        public void AggregatedDataTest_TwoCurrencies_IsEqual()
        {
            string input = "JONAS 2013-02-20 333.21 SEK\r\nANTANAS 2017-05-10 300.00 USD\r\nJONAS 2017-05-10 1687.88 USD\r\nBONIFACIJUS 2018-01-04 20.00 SEK\r\nANTANAS 2016-10-22 1.15 GBP";
            string expectedResult = "ANTANAS                  276,97 EURBONIFACIJUS                2,04 EURJONAS                   1551,08 EURJONAS                    135,70 LTL";
            
            CurrencyConverter currencyConverter = new CurrencyConverter();
            IEnumerable<string> convertedEmployeesSpendings = currencyConverter.AggregatedData(input);
            string result = string.Join(string.Empty, convertedEmployeesSpendings);
            
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod()]
        public void AggregatedDataTest_UnknownCurrency_IsFalse()
        {
            string input = "JONAS 2013-02-20 333.21 CCC";

            CurrencyConverter currencyConverter = new CurrencyConverter();
            IEnumerable<string> convertedEmployeesSpendings = currencyConverter.AggregatedData(input);

            Assert.IsFalse(convertedEmployeesSpendings.Any());
        }

        [TestMethod()]
        public void AggregatedDataTest_EarlyYear_IsFalse()
        {
            string input = "JONAS 1990-02-20 333.21 USD";

            CurrencyConverter currencyConverter = new CurrencyConverter();
            IEnumerable<string> convertedEmployeesSpendings = currencyConverter.AggregatedData(input);

            Assert.IsFalse(convertedEmployeesSpendings.Any());
        }

        [TestMethod()]
        public void AggregatedDataTest_BadInput_IsNull()
        {
            string input = "JONAS 1993-06-244 333.21 USD";

            CurrencyConverter currencyConverter = new CurrencyConverter();
            IEnumerable<string> convertedEmployeesSpendings = currencyConverter.AggregatedData(input);

            Assert.IsFalse(convertedEmployeesSpendings.Any());
        }
    }
}