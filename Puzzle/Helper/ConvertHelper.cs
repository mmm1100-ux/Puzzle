using Enums;

namespace Helper
{
    public static class ConvertHelper
    {
        public static int ToInt(this object obj)
        {
            return System.Convert.ToInt32(obj);
        }

        public static bool ToBoolean(this object obj)
        {
            return System.Convert.ToBoolean(obj);
        }

        public static string Bitmap(this string obj, int? width, long? quality = null, Enum.ImageFormat? format = Enum.ImageFormat.Jpeg)
        {
            var result = "/picture" + obj + "?";

            if (width != null)
            {
                result += "width=" + width + "&";
            }

            if (quality != null)
            {
                result += "quality=" + quality + "&";
            }

            if (format != null)
            {
                result += "format=" + format + "&";
            }

            return result.Trim('?').Trim('&').ToLower();
        }
    }
}
