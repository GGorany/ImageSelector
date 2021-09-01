using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImageSelector
{
    internal class ThumbFactory
    {
        /// <summary>
        /// Available thumbs positions
        /// </summary>
        public enum ThumbPosition
        {
            TopLeft,
            BottomRight,
        }

        /// <summary>
        /// Thumb factory
        /// </summary>
        /// <param name="thumbPosition">Thumb positions</param>
        /// <param name="canvas">Parent UI element that we will attach thumb as child</param>
        /// <param name="size">Size of thumb</param>
        /// <returns></returns>
        public static ThumbSelect CreateThumb(ThumbPosition thumbPosition, Canvas canvas, double size)
        {
            ThumbSelect customThumb = new ThumbSelect(size)
            {
                Cursor = GetCursor(thumbPosition),
                Visibility = Visibility.Hidden
            };
            canvas.Children.Add(customThumb);
            return customThumb;
        }

        /// <summary>
        /// Display proper cursor to corresponding thumb
        /// </summary>
        /// <param name="thumbPosition">Thumb position</param>
        /// <returns></returns>
        private static Cursor GetCursor(ThumbPosition thumbPosition)
        {
            return thumbPosition switch
            {
                (ThumbPosition.TopLeft) => Cursors.SizeNWSE,
                (ThumbPosition.BottomRight) => Cursors.SizeNWSE,
                _ => null,
            };
        }
    }
}
