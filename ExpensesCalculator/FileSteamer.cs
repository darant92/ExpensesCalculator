using System;
using System.Collections.Generic;
using System.IO;

namespace ExpensesCalculator
{
    public static class FileSteamer
    {
        public static string ReadDataFromFile()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string fullPath = Directory.GetParent(workingDirectory).Parent.FullName + "\\expenses.txt";

            try
            {
                return File.ReadAllText(fullPath);
            }
            catch
            {
                Console.WriteLine("File read error");

                return string.Empty;
            }
        }
        public static void WriteDataToFile(IEnumerable<string> payouts)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string fullPath = Directory.GetParent(workingDirectory).Parent.FullName + "\\payout.txt";
            
            try
            {
                File.WriteAllLines(fullPath, payouts);
            }
            catch
            {
                Console.WriteLine("File write error");
            }
        }
    }
}
