using System;
using System.Text.RegularExpressions;

namespace ExpensesCalculator
{
    public class EmployeeSpendings : EmployeeExpenses
    {
        public DateTime expensesDate;

        public EmployeeSpendings(Match match) : base(match)
        {
            expensesDate = DateTime.ParseExact(match.Groups[2].Value, "yyyy-MM-dd", null);
        }

        public override string ToString()
        {
            return $"{employee} {expensesDate.ToShortDateString()} {amount} {currency}";
        }
    }
}
