namespace Demo_Console.Extensions;

public static class RandomExtensions
{
    private static bool _hasSpareGaussian = false;
    private static double _spareGaussian;

    public static double NextGaussian(this Random random, double mean = 0.0, double stdDev = 1.0)
    {
        if (_hasSpareGaussian)
        {
            _hasSpareGaussian = false;
            return _spareGaussian * stdDev + mean;
        }

        _hasSpareGaussian = true;
        double u, v, s;
        do
        {
            u = random.NextDouble() * 2.0 - 1.0;
            v = random.NextDouble() * 2.0 - 1.0;
            s = u * u + v * v;
        }
        while (s is >= 1.0 or 0.0);

        s = Math.Sqrt(-2.0 * Math.Log(s) / s);
        _spareGaussian = v * s;
        return u * s * stdDev + mean;
    }
}