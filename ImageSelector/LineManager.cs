using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageSelector
{
    internal class LineManager
    {
        public event EventHandler LineChanged;

        private readonly Line _line;
        private readonly Line _dashedLine;
        private readonly Canvas _canvas;
        private bool _isDrawing;
        private bool _isDragging;

        private Point _StartPoint = new Point();
        private Point _EndPoint = new Point();

        private Point _mouseStartPoint;
        private Point _mouseLastPoint;

        public Point StartPoint 
        { 
            get => _StartPoint;
            set
            {
                _StartPoint = value;
                UpdateLine(_StartPoint, _EndPoint);
            }
        }

        public Point EndPoint
        {
            get => _EndPoint;
            set
            {
                _EndPoint = value;
                UpdateLine(_StartPoint, _EndPoint);
            }
        }

        public LineManager(Canvas canvasOverlay)
        {
            _canvas = canvasOverlay;
            // intit crop rectangle
            _line = new Line()
            {
                X1 = 0,
                Y1 = 0,
                X2 = 0,
                Y2 = 0,
                Stroke = Brushes.Red,
                StrokeThickness = 1
            };
            //intit second rectangle to fake dashed lines
            _dashedLine = new Line()
            {
                Stroke = Brushes.White,
                StrokeDashArray = new DoubleCollection(new double[] { 4, 4 })
            };
            //add both rectangels, so it will be rendered
            _canvas.Children.Add(_line);
            _canvas.Children.Add(_dashedLine);
            //set intit position on canvas
            Canvas.SetLeft(_line, 0);
            Canvas.SetTop(_line, 0);

            _line.SizeChanged += (sender, args) =>
            {
                LineChanged?.Invoke(sender, args);
            };
        }

        /// <summary>
        /// Event handler for mouse left button
        /// </summary>
        /// <param name="e">Mouse event args</param>
        public void MouseLeftButtonDownEventHandler(MouseButtonEventArgs e)
        {
            _canvas.CaptureMouse();
            //get mouse click point relative to canvas overlay
            Point mouseClickPoint = e.GetPosition(_canvas);

            /*
            //if we click outside of rectengle and rectangle already exist, start recreating
            if ((_rectangle.Height != 0 || _rectangle.Width != 0) &&
                touch == TouchPoint.OutsideRectangle)
            {
                //reset existing rectangle
                UpdateRectangle(0, 0, 0, 0);
                //start drawing
                _isDrawing = true;
            }
            */

            //if rectangle not created - start creating
            if (_line.X1 == 0 && _line.Y1 == 0 && _line.X2 == 0 && _line.Y2 == 0)
            {
                _mouseStartPoint = mouseClickPoint;
                _isDrawing = true;
            }

            /*
            //if rectangle is created and we click inside rectangle - start dragging
            if ((_rectangle.Height != 0 && _rectangle.Width != 0)
                && touch != TouchPoint.OutsideRectangle)
            {
                if (e.ClickCount == 2)
                {
                    OnRectangleDoubleClickEvent(this, EventArgs.Empty);
                    return;
                }
                _isDragging = true;
                _mouseLastPoint = mouseClickPoint;
            }
            */
        }

        /// <summary>
        /// Event handler for mouse move
        /// </summary>
        /// <param name="e">Mouse event args</param>
        public void MouseMoveEventHandler(MouseEventArgs e)
        {
            //get mouse click point relative to canvas overlay
            Point mouseClickPoint = e.GetPosition(_canvas);

            if (_isDrawing)
            {
                double x1 = _mouseStartPoint.X;
                double y1 = _mouseStartPoint.Y;
                double x2 = mouseClickPoint.X;
                double y2 = mouseClickPoint.Y;

                // limit
                if (x1 < 0) x1 = 0;
                if (y1 < 0) y1 = 0;
                if (x2 < 0) x2 = 0;
                if (y2 < 0) y2 = 0;
                if (x1 > _canvas.ActualWidth) x1 = _canvas.ActualWidth;
                if (y1 > _canvas.ActualHeight) y1 = _canvas.ActualHeight;
                if (x2 > _canvas.ActualWidth) x2 = _canvas.ActualWidth;
                if (y2 > _canvas.ActualHeight) y2 = _canvas.ActualHeight;

                UpdateLine(x1, y1, x2, y2);
                return;
            }
            if (_isDragging)
            {
                //see how much the mouse has moved
                double offsetX = mouseClickPoint.X - _mouseLastPoint.X;
                double offsetY = mouseClickPoint.Y - _mouseLastPoint.Y;

                //get the original rectangle parameters
                double left = TopLeft.X;
                double top = TopLeft.Y;
                double width = _rectangle.Width;
                double height = _rectangle.Height;

                left += offsetX;
                top += offsetY;

                //set dragging limits(canvas borders)
                //set bottom limit
                if (top + offsetY + height > _canvas.ActualHeight)
                {
                    top = _canvas.ActualHeight - height;
                }
                //set right limit
                if (left + offsetX + width > _canvas.ActualWidth)
                {
                    left = _canvas.ActualWidth - width;
                }
                //set left limit
                if (left < 0)
                {
                    left = 0;
                }
                //set top limit
                if (top < 0)
                {
                    top = 0;
                }

                // Update the rectangle.
                UpdateRectangle(left, top, width, height);
                _mouseLastPoint = mouseClickPoint;
            }
        }

        /// <summary>
        /// Event handler for mouse left button up
        /// </summary>
        /// <param name="e">Mouse event args</param>
        public void MouseLeftButtonUpEventHandler()
        {
            _isDrawing = false;
            _isDragging = false;
            _canvas.ReleaseMouseCapture();
        }

        public void UpdateLine(double x1, double y1, double x2, double y2)
        {
            //dont use negative value
            if (x1 >= 0 && y1 >= 0 && x2 >= 0 && y2 >= 0)
            {
                Canvas.SetLeft(_line, x1);
                Canvas.SetTop(_line, y1);
                _line.X2 = x2;
                _line.Y2 = y2;
                //we need to update dashed rectangle too
                UpdateDashedLine();
            }
        }

        private void UpdateDashedLine()
        {
            _dashedLine.X2 = _line.X2;
            _dashedLine.Y2 = _line.Y2;
            Canvas.SetLeft(_dashedLine, _line.X1);
            Canvas.SetTop(_dashedLine, _line.Y1);
        }
    }
}
