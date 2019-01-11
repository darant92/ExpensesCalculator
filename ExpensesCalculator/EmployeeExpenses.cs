using System.Globalization;
using System.Text.RegularExpressions;

namespace ExpensesCalculator
{
    public class EmployeeExpenses
    {
        public string employee;
        public double amount;
        public string currency;

        public EmployeeExpenses(Match match)
        {
            employee = match.Groups[1].Value;
            amount = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
            currency = match.Groups[4].Value;
        }

        public EmployeeExpenses(string employee, double amount, string currency)
        {
            this.employee = employee;
            this.amount = amount;
            this.currency = currency;
        }

        public static explicit operator string(EmployeeExpenses employee)
        {
            return $"{employee.employee, -15} {employee.amount.ToString(".00"), 15} {employee.currency, 3}";
        }
    }
}
