using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amity.Models
{
    public class User
    {
        public string Name { get; internal set; }
        public double Similarity { get; internal set; }
        public int Variation { get; internal set; }
    }

    public static class Analyzer
    {
        
        public static async Task<List<User>> Run(IProgress<Tuple<double, string>> progress)
        {
            // TODO !!!
            progress.Report(Tuple.Create(10.0, "Started"));
            await Task.Delay(1000);
            return new List<User>() { new User { Name="Test", Similarity=0.958215478, Variation = 1} };
        }
    }
}
