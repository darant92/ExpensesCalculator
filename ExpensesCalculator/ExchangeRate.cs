namespace ExpensesCalculator
{
    class ExchangeRate
    {
        public string toCurrency;
        public double rating;

        public ExchangeRate(string toCurrency, double rating)
        {
            this.toCurrency = toCurrency;
            this.rating = rating;
        }
    }
}
