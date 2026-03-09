using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace Chess
{
    public abstract class Binding : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public static string GetString(string input, string output = "Unknown")
        {
            return string.IsNullOrWhiteSpace(input) ? output : input;
        }
        public static double GetDouble(double input, double min = double.MinValue, double max = double.MaxValue)
        {
            return Math.Max(min, Math.Min(max, double.IsNaN(input) ? 0 : input));
        }
        public static int GetInt(int input, int min = int.MinValue, int max = int.MaxValue)
        {
            return Math.Max(min, Math.Min(max, double.IsNaN(input) ? 0 : input));
        }
        public static long GetLong(long input, long min = long.MinValue, long max = long.MaxValue)
        {
            return Math.Max(min, Math.Min(max, input));
        }
        public static decimal GetDecimal(decimal input, decimal min = decimal.MinValue, decimal max = decimal.MaxValue)
        {
            return Math.Round(Math.Max(min, Math.Min(max, input)), 0);
        }
    }
}
