// --------------------------------------------------
// ReiEditAA2 - StandardDeviation.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace ReiEditAA2.Code
{
    internal class StandardDeviation
    {
        public double Average
        {
            get { return Samples.Average(); }
        }

        public double Deviation
        {
            get
            {
                //return (Maximum - Minimum) / 2;
                return StdDev(Samples);
            }
        }

        public string Key { get; set; }

        public double Maximum
        {
            get { return Samples.Max(); }
        }

        public double Minimum
        {
            get { return Samples.Min(); }
        }

        public IEnumerable<double> Samples { get; set; }

        public static IEnumerable<double> GetSamples(CharacterCollection deviationSource, byte gender, string key)
        {
            return from chr in deviationSource.Characters
                where (byte) chr.Profile.Gender.Value == gender
                let value = (byte) chr.Character.CharAttributes[key].Value
                select Convert.ToDouble(value);
        }

        private static double StdDev(IEnumerable<double> values)
        {
            double ret = 0;

            var doubles = values as double[] ?? values.ToArray();
            if (!doubles.Any())
                return ret;

            double avg = doubles.Average();
            double sum = doubles.Sum(d => Math.Pow(d - avg, 2));
            ret = Math.Sqrt((sum) / doubles.Count());

            return ret;
        }
    }
}