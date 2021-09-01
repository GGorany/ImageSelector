using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ImageSelector
{
    internal class ThumbManager
    {
        private readonly ThumbSelect _topLeft, _bottomRight;
        private readonly Canvas _canvas;
        private readonly RectangleManager _rectangleManager;
        private readonly double _thumbSize;

        public ThumbManager(Canvas canvas, RectangleManager rectangleManager)
        {
            //  initizalize
            _canvas = canvas;
            _rectangleManager = rectangleManager;
            _thumbSize = 10;

            //  create thumbs with factory
            _topLeft = ThumbFactory.CreateThumb(ThumbFactory.ThumbPosition.TopLeft, _canvas, _thumbSize);
            _bottomRight = ThumbFactory.CreateThumb(ThumbFactory.ThumbPosition.BottomRight, _canvas, _thumbSize);

            //  subsctibe to mouse events
            _topLeft.DragDelta += new DragDeltaEventHandler(TopLeftDragDeltaEventHandler);
            _topLeft.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PreviewMouseLeftButtonDownGenericHandler);
            _topLeft.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(PreviewMouseLeftButtonUpGenericHandler);

            _bottomRight.DragDelta += new DragDeltaEventHandler(BottomRightDragDeltaEventHandler);
            _bottomRight.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PreviewMouseLeftButtonDownGenericHandler);
            _bottomRight.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(PreviewMouseLeftButtonUpGenericHandler);
        }

        private void BottomRightDragDeltaEventHandler(object sender, DragDeltaEventArgs e)
        {
            ThumbSelect thumb = sender as ThumbSelect;

            double resultThumbLeft = Canvas.GetLeft(thumb) + e.HorizontalChange;

            if (resultThumbLeft > _canvas.ActualWidth)
                resultThumbLeft = _canvas.ActualWidth;

            double thumbResultTop = Canvas.GetTop(thumb) + e.VerticalChange;
            if (thumbResultTop + _thumbSize / 2 > _canvas.ActualHeight)
                thumbResultTop = _canvas.ActualHeight - _thumbSize / 2;

            double resultHeight = thumbResultTop - _rectangleManager.TopLeft.Y + _thumbSize / 2;
            double resultWidth = resultThumbLeft - _rectangleManager.TopLeft.X;

            UpdateRectangeSize(null, null, resultHeight, resultWidth);
        }

        private void TopLeftDragDeltaEventHandler(object sender, DragDeltaEventArgs e)
        {
            ThumbSelect thumb = sender as ThumbSelect;

            double newTop = Canvas.GetTop(thumb) + e.VerticalChange;
            double newLeft = Canvas.GetLeft(thumb) + e.HorizontalChange;

            if (newTop < 0)
                newTop = -_thumbSize / 2;
            if (newLeft < 0)
                newLeft = -_thumbSize / 2;

            double offsetTop = Canvas.GetTop(thumb) - newTop;
            double resultHeight = _rectangleManager.RectangleHeight + offsetTop;
            double resultTop = newTop + _thumbSize / 2;

            double offsetLeft = Canvas.GetLeft(thumb) - newLeft;
            double resultWidth = _rectangleManager.RectangleWidth + offsetLeft;
            double resultLeft = newLeft + _thumbSize / 2;

            UpdateRectangeSize(resultLeft, resultTop, resultHeight, resultWidth);
        }

        /// <summary>
        /// Update (redraw) thumbs positions
        /// </summary>
        public void UpdateThumbsPosition()
        {
            if (_rectangleManager.RectangleHeight > 0 && _rectangleManager.RectangleWidth > 0)
            {
                _topLeft.SetPosition(_rectangleManager.TopLeft.X, _rectangleManager.TopLeft.Y);
                _bottomRight.SetPosition(_rectangleManager.TopLeft.X + _rectangleManager.RectangleWidth, _rectangleManager.TopLeft.Y + _rectangleManager.RectangleHeight);
            }
        }

        /// <summary>
        /// Manage thumbs visibility
        /// </summary>
        /// <param name="isVisble">Set current visibility</param>
        public void ShowThumbs(bool isVisble)
        {
            if (isVisble && _rectangleManager.RectangleHeight > 0 && _rectangleManager.RectangleWidth > 0)
            {
                _topLeft.Visibility = Visibility.Visible;
                _bottomRight.Visibility = Visibility.Visible;

            }
            else
            {
                _topLeft.Visibility = Visibility.Hidden;
                _bottomRight.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Update cropping rectangle
        /// </summary>
        /// <param name="left">Left rectangle coordinate</param>
        /// <param name="top">Top rectangle coordinate</param>
        /// <param name="height">Height of rectangle</param>
        /// <param name="width">Width of rectangle</param>
        private void UpdateRectangeSize(double? left, double? top, double? height, double? width)
        {
            double resultLeft = _rectangleManager.TopLeft.X;
            double resultTop = _rectangleManager.TopLeft.Y;
            double resultHeight = _rectangleManager.RectangleHeight;
            double resultWidth = _rectangleManager.RectangleWidth;

            if (left != null)
                resultLeft = (double)left;
            if (top != null)
                resultTop = (double)top;
            if (height != null)
                resultHeight = (double)height;
            if (width != null)
                resultWidth = (double)width;

            _rectangleManager.UpdateRectangle(resultLeft, resultTop, resultWidth, resultHeight);
            UpdateThumbsPosition();
        }

        private void PreviewMouseLeftButtonDownGenericHandler(object sender, MouseButtonEventArgs e)
        {
            ThumbSelect thumb = sender as ThumbSelect;
            thumb.CaptureMouse();
        }

        private void PreviewMouseLeftButtonUpGenericHandler(object sender, MouseButtonEventArgs e)
        {
            ThumbSelect thumb = sender as ThumbSelect;
            thumb.ReleaseMouseCapture();
        }
    }
}
