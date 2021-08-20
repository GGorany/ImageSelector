using System.Windows;
using System.Windows.Controls;

namespace ImageSelector
{
    public enum OutType : uint
    {
        InArea = 0x00,
        Left = 0x01,
        Top = 0x02,
        Right = 0x04,
        Bottom = 0x08,
        None = 0x10
    }

    public class Helper
    {
        public static uint IsInImageArea(ref Image image, Point point)
        {
            uint returnvalue = 0x00;

            if (image == null)
                return (uint)OutType.None;

            if (point.X < 0)
                returnvalue += (uint)OutType.Left;

            if (point.Y < 0)
                returnvalue += (uint)OutType.Top;

            if (point.X > image.Width)
                returnvalue += (uint)OutType.Right;

            if (point.Y > image.Height)
                returnvalue += (uint)OutType.Bottom;

            return returnvalue;
        }
    }
}
