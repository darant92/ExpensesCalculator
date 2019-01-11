using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExpensesCalculator
{
    public class CurrencyConverter
    {
        private Dictionary<string, ExchangeRate> exchangeRates = new Dictionary<string, ExchangeRate>();
        private static readonly HttpClient client = new HttpClient();

        public IEnumerable<string> AggregatedData(string text)
        {
            IEnumerable<EmployeeExpenses> employeeSpendings = GetSpendingList(text);

            if(employeeSpendings == null)
            {
                return null;
            }

            List<EmployeeExpenses> employeeExpenses = new List<EmployeeExpenses>();

            foreach (EmployeeSpendings employeeSpending in employeeSpendings)
            {
                if (employeeSpending.expensesDate < DateTime.ParseExact("1993-06-25", "yyyy-MM-dd", null))
                {
                    continue;
                }

                string key = employeeSpending.expensesDate.ToString() + employeeSpending.currency;

                if (!exchangeRates.ContainsKey(key) && !PopulateCurrencyDictionary(employeeSpending))
                {
                    Console.WriteLine($"Error occurred while trying to convert: {employeeSpending.ToString()}");

                    continue;
                }

                employeeSpending.amount *= exchangeRates[key].rating;
                employeeSpending.currency = exchangeRates[key].toCurrency;

                employeeExpenses.Add(employeeSpending);
            }

            return employeeExpenses.GroupBy(i => new { i.employee, i.currency })
                                       .Select(g => new EmployeeExpenses(g.Key.employee,g.Sum(i => i.amount), g.Key.currency))
                                       .OrderBy(x => x.employee)
                                       .OrderBy(x => x.currency)
                                       .Select(x => (string)x);
        }

        private bool PopulateCurrencyDictionary(EmployeeSpendings EmployeeSpending)
        {
            string content = FetchExchangeRatingsAsync(EmployeeSpending).Result;
            XNamespace docNamespace = "{http://www.lb.lt/WebServices/FxRates}";
            IEnumerable<XElement> exchangeRatesXml;

            try
            {
                XDocument doc = XDocument.Parse(content);
                exchangeRatesXml = doc.Elements($"{docNamespace}FxRates");
            }
            catch
            {
                return false;
            }
            
            foreach (XElement element in exchangeRatesXml.Elements($"{docNamespace}FxRate"))
            {
                IEnumerable<XElement> exchangeRateXml = element.Elements($"{docNamespace}CcyAmt");
                string toCurrency = exchangeRateXml.First().Element($"{docNamespace}Ccy").Value;
                double toAmount = double.Parse(exchangeRateXml.First().Element($"{docNamespace}Amt").Value, CultureInfo.InvariantCulture);
                string fromCurrency = exchangeRateXml.Last().Element($"{docNamespace}Ccy").Value;
                double fromAmount = double.Parse(exchangeRateXml.Last().Element($"{docNamespace}Amt").Value, CultureInfo.InvariantCulture);

                exchangeRates[EmployeeSpending.expensesDate.ToString() + fromCurrency] = new ExchangeRate(toCurrency, toAmount / fromAmount);
            }

            return exchangeRates.ContainsKey(EmployeeSpending.expensesDate.ToString() + EmployeeSpending.currency);
        }

        private async Task<string> FetchExchangeRatingsAsync(EmployeeSpendings empployee)
        {
            string date = empployee.expensesDate.ToShortDateString();
            string exchangeType = empployee.expensesDate < DateTime.ParseExact("2015-01-01", "yyyy-MM-dd", null) ? "LT" : "EU";
            Uri url = new Uri($@"http://old.lb.lt/webservices/FxRates/FxRates.asmx/getFxRates?tp={exchangeType}&dt={date}");

            return await client.GetStringAsync(url);
        }

        private static IEnumerable<EmployeeSpendings> GetSpendingList(string text)
        {
            RegexOptions regexOptionsCompiled = RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
            TimeSpan regexTimeOut = new TimeSpan(0, 1, 0);
            string regexPattern = @"(\w.*?)\s*(\d{4}-\d{2}-\d{2})\s*(\d[\d\,\.]*)\s*(\w{3})";
            Regex matchText = new Regex(regexPattern, regexOptionsCompiled, regexTimeOut);

            MatchCollection matches = matchText.Matches(text);

            if (matches.Count < 1 || matches.Cast<Match>().Any(m => m.Success == false))
            {
                return null;
            }

            return matches.Cast<Match>().Select(m => new EmployeeSpendings(m));
        }
    }
}
