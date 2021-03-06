namespace ImageSelector
{
    public static class InputExtensions
    {
        public static double LimitToRange(this double value, double inclusiveMinimum, double inclusiveMaximum)
        {
            if (value < inclusiveMinimum)
                return inclusiveMinimum;

            if (value > inclusiveMaximum)
                return inclusiveMaximum;

            return value;
        }
    }
}