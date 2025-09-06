namespace SanjeshP.Mosahebe.Convertor
{
    public static class TextConvertor
    {
        public static string FixTextLower(this string value)
        {
            return value.ToLower().Trim();
        }

        public static string FixTextUpper(this string value)
        {
            return value.ToUpper().Trim();
        }
    }
}
