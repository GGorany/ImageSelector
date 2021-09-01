using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageSelector
{
    internal class SelectingAdorner : Adorner
    {
        //public event EventHandler<DoubleClickEventArgs> OnRectangleDoubleClickEvent;
        public event EventHandler<Rect> OnRectangleSizeEvent;

        private readonly RectangleManager _rectangleManager;
        private readonly OverlayManager _overlayManager;
        private readonly ThumbManager _thumbManager;

        private bool _isMouseLeftButtonDown;
        private readonly VisualCollection _visualCollection;
        private readonly Canvas _canvasOverlay;
        private readonly Canvas _originalCanvas;

        public SelectingAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _visualCollection = new VisualCollection(this);
            _originalCanvas = (Canvas)adornedElement;
            _canvasOverlay = new Canvas();
            _rectangleManager = new RectangleManager(_canvasOverlay);
            _overlayManager = new OverlayManager(_canvasOverlay, _rectangleManager);
            _thumbManager = new ThumbManager(_canvasOverlay, _rectangleManager);
            _visualCollection.Add(_canvasOverlay);

            //add event handlers
            MouseLeftButtonDown += MouseLeftButtonDownEventHandler;
            MouseMove += MouseMoveEventHandler;
            MouseLeftButtonUp += MouseLeftButtonUpEventHandler;
            Loaded += (object sender, RoutedEventArgs args) => Show();
            _originalCanvas.SizeChanged += (object sender, SizeChangedEventArgs e) => Show();
            _rectangleManager.RectangleSizeChanged += (object sender, EventArgs args) => Show();
        }

        public Rect Rect
        {
            get => _rectangleManager.Rect;
            set
            {
                _rectangleManager.Rect = value;
                Show();
            }
        }

        public string Text { get; set; }

        /// <summary>
        /// Get cropping areas as BitmapFrame
        /// </summary>
        /// <returns></returns>
        public BitmapFrame GetCroppedBitmapFrame()
        {
            // 1) get current dpi
            PresentationSource pSource = PresentationSource.FromVisual(Application.Current.MainWindow);
            Matrix m = pSource.CompositionTarget.TransformToDevice;
            double dpiX = m.M11 * 96;
            double dpiY = m.M22 * 96;

            // 2) create RenderTargetBitmap
            RenderTargetBitmap elementBitmap =
                new RenderTargetBitmap((int)_originalCanvas.ActualWidth, (int)_originalCanvas.ActualHeight, dpiX, dpiY,
                    PixelFormats.Pbgra32);

            elementBitmap = new RenderTargetBitmap((int)_originalCanvas.RenderSize.Width,
                (int)_originalCanvas.RenderSize.Height, dpiX, dpiY, PixelFormats.Default);

            //Important
            _originalCanvas.Measure(_originalCanvas.RenderSize);
            _originalCanvas.Arrange(new Rect(_originalCanvas.RenderSize));

            // 3) draw element
            elementBitmap.Render(_originalCanvas);

            if (VisualTreeHelper.GetParent(_originalCanvas) is UIElement parent)
            {
                //Important
                parent.Measure(_originalCanvas.RenderSize);
                parent.Arrange(new Rect(_originalCanvas.RenderSize));
            }

            var crop = new CroppedBitmap(elementBitmap,
                new Int32Rect((int)_rectangleManager.TopLeft.X, (int)_rectangleManager.TopLeft.Y,
                    (int)_rectangleManager.RectangleWidth, (int)_rectangleManager.RectangleHeight));
            return BitmapFrame.Create(crop);
        }

        /// <summary>
        /// Mouse left button down event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseLeftButtonDownEventHandler(object sender, MouseButtonEventArgs e)
        {
            CaptureMouse();
            if (e == null) throw new ArgumentNullException(nameof(e));
            _rectangleManager.MouseLeftButtonDownEventHandler(e);
            _overlayManager.UpdateOverlay();
            if (_rectangleManager.RectangleWidth == 0 && _rectangleManager.RectangleHeight == 0)
            {
                _thumbManager.ShowThumbs(false);
            }
            _isMouseLeftButtonDown = true;
        }

        public void Show()
        {
            if (Rect.IsEmpty)
            {
                _overlayManager.UpdateOverlay();
                _thumbManager.ShowThumbs(false);
                _thumbManager.UpdateThumbsPosition();
            }
            else
            {
                _overlayManager.UpdateOverlay();
                _thumbManager.ShowThumbs(true);
                _thumbManager.UpdateThumbsPosition();
            }
        }

        private void MouseMoveEventHandler(object sender, MouseEventArgs e)
        {
            if (_isMouseLeftButtonDown)
            {
                _rectangleManager.MouseMoveEventHandler(e);
                Show();
            }

            //OnRectangleSizeEvent?.Invoke(sender, _rectangleManager.Rect);
        }

        private void MouseLeftButtonUpEventHandler(object sender, MouseButtonEventArgs e)
        {
            _rectangleManager.MouseLeftButtonUpEventHandler();
            ReleaseMouseCapture();
            _isMouseLeftButtonDown = false;
            OnRectangleSizeEvent?.Invoke(sender, _rectangleManager.Rect);
        }

        // Override the VisualChildrenCount properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount
        {
            get { return _visualCollection.Count; }
        }

        // Override the GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override Visual GetVisualChild(int index)
        {
            return _visualCollection[index];
        }

        // Positions child elements and determines a size
        protected override Size ArrangeOverride(Size size)
        {
            Size finalSize = base.ArrangeOverride(size);
            _canvasOverlay.Arrange(new Rect(0, 0, AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height));
            return finalSize;
        }
    }
}
