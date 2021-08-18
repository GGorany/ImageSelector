using ImageSelector.ROIs;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageSelector
{
    /// <summary>
    /// Selector.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Selector : UserControl
    {
        private Point mousePosition = new Point(0, 0);
        private ROIDescriptor _lastROIDescriptor = new ROIDescriptor();
        private WriteableBitmap writeableBitmap = null;

        public event EventHandler<ROIValueChangedEventArgs> ROIValueChanged;

        #region DependencyProperties
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source",
            typeof(ImageSource),
            typeof(Selector),
            new PropertyMetadata(default(ImageSource), OnSourceChanged));

        public static readonly DependencyProperty RectProperty = DependencyProperty.Register(
            "Rect",
            typeof(Rectangle),
            typeof(Selector),
            new PropertyMetadata(default(Rectangle), OnRectChanged));

        public static readonly DependencyProperty MagnificationProperty = DependencyProperty.Register(
            "Magnification", 
            typeof(double), 
            typeof(Selector),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender, OnMagnificationChanged));

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public Rectangle Rect
        {
            get { return (Rectangle)GetValue(RectProperty); }
            set { SetValue(RectProperty, value); }
        }

        public double Magnification
        {
            get { return (double)GetValue(MagnificationProperty); }
            set { SetValue(MagnificationProperty, value); }
        }
        #endregion

        #region DependencyProperties ROI
        private static readonly DependencyPropertyKey ROIListPropertyKey = DependencyProperty.RegisterReadOnly(
            "ROIList",
            typeof(ObservableCollection<ROI>),
            typeof(Selector),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ROIListProperty = ROIListPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey GetLastEventDataPropertyKey = DependencyProperty.RegisterReadOnly(
            "GetLastEventData",
            typeof(ROIDescriptor.LastEventData),
            typeof(Selector),
            new PropertyMetadata(null, OnGetLastEventDataChanged));

        public static readonly DependencyProperty GetLastEventDataProperty = GetLastEventDataPropertyKey.DependencyProperty;

        public ObservableCollection<ROI> ROIList
        {
            get { return (ObservableCollection<ROI>)GetValue(ROIListProperty); }
            protected set { SetValue(ROIListPropertyKey, value); }
        }

        public ROIDescriptor.LastEventData GetLastEventData
        {
            get { return (ROIDescriptor.LastEventData)GetValue(GetLastEventDataProperty); }
            protected set { SetValue(GetLastEventDataPropertyKey, value); }
        }
        #endregion

        public Selector()
        {
            InitializeComponent();

            _SourceImage.LayoutTransform = new ScaleTransform();
            _ROI.LayoutTransform = new ScaleTransform();

            _MouseHandler.MouseLeftButtonDown += _MouseHandler_MouseLeftButtonDown;
            _MouseHandler.MouseMove += _MouseHandler_MouseMove;
            _MouseHandler.MouseLeftButtonUp += _MouseHandler_MouseLeftButtonUp;
            _MouseHandler.MouseWheel += _MouseHandler_MouseWheel;

            ROIList = new ObservableCollection<ROI>();
        }

        #region Events
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Selector selector)) return;
            if (e.NewValue is ImageSource newImage)
            {
                selector.writeableBitmap = BitmapFactory.ConvertToPbgra32Format((BitmapSource)newImage);
                selector._SourceImage.Width = newImage.Width;
                selector._SourceImage.Height = newImage.Height;
                selector._SourceImage.Source = selector.writeableBitmap;
            }
            else
            {
                selector.writeableBitmap = null;
                selector._SourceImage.Width = double.NaN;
                selector._SourceImage.Height = double.NaN;
                selector._SourceImage.Source = null;
            }
        }

        private static void OnRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Selector selector)) return;
            //if (selector.CroppingAdorner == default) return;
            //if (e.NewValue is Rectangle rect)
            //{
            //    selector.AdornerCrop(rect);
            //}
            //else
            //{
            //    selector.CroppingAdorner.Crop = Rectangle.Empty;
            //}
        }

        private static void OnMagnificationChanged(DependencyObject d, DependencyPropertyChangedEventArgs magnification)
        {
            if (!(d is Selector selector)) return;
            selector.ApplyMagnification((double)magnification.NewValue);
        }

        private void _MouseHandler_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == _MouseHandler)
            {
                ROIList.Clear();
                StartDrawingRectROI();
            }
        }

        private void _MouseHandler_MouseMove(object sender, MouseEventArgs e)
        {
            ShowMousePosition();
        }

        private void _MouseHandler_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _MouseHandler.ReleaseMouseCapture();
        }

        private void _MouseHandler_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoom_delta = e.Delta > 0 ? .1 : -.1;
            Magnification = (Magnification += zoom_delta).LimitToRange(.1, 10);
            ApplyMagnification(Magnification);
            ShowMousePosition();
            e.Handled = true;
        }

        private static void OnGetLastEventDataChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Selector selector = (Selector)obj;
            ROIDescriptor.LastEventData lastEventData = (ROIDescriptor.LastEventData)args.NewValue;
            ROIDescriptor.LastEventData other = (ROIDescriptor.LastEventData)args.OldValue;
            if (lastEventData.type == EventType.Draw)
            {
                ROIDescriptor lastROIDescriptor = selector._lastROIDescriptor;
                ROIDescriptor previousROIDescriptor = selector.GetROIDescriptor();
                if (!lastEventData.IsChanged(other) || !previousROIDescriptor.IsChanged(lastROIDescriptor))
                {
                    selector._lastROIDescriptor = previousROIDescriptor;
                    selector.OnROIValueChanged(new ROIValueChangedEventArgs(lastEventData, lastROIDescriptor, previousROIDescriptor));
                }
            }
        }

        private void OnGetLastDrawEventUpdated(object sender, ROIDescriptor.LastEventArgs lastEventArgs)
        {
            GetLastEventData = lastEventArgs.data;
        }

        private void OnROIValueChanged(ROIValueChangedEventArgs args)
        {
            this.ROIValueChanged?.Invoke(this, args);
        }
        #endregion

        #region Private Method
        private void ShowMousePosition()
        {
            var point = Mouse.GetPosition(_SourceImage);
            if ((point.X >= 0) && (point.X < _SourceImage.Width) && (point.Y >= 0) && (point.Y < _SourceImage.Height))
            {
                _Position.Text = $"x = {Mouse.GetPosition(_SourceImage).X:N0}; y = {Mouse.GetPosition(_SourceImage).Y:N0}";
            }
        }

        private void ApplyMagnification(double magnification)
        {
            if (_SourceImage != null)
            {
                ScaleTransform obj = (ScaleTransform)_SourceImage.LayoutTransform;
                obj.ScaleX = obj.ScaleY = magnification;
                RenderOptions.SetBitmapScalingMode(_SourceImage, BitmapScalingMode.HighQuality);
                _Zoom.Text = $"{magnification * 100:N0}%";
            }

            if (_ROI != null)
            {
                ScaleTransform obj2 = (ScaleTransform)_ROI.LayoutTransform;
                obj2.ScaleX = obj2.ScaleY = magnification;
            }
        }

        private void StartDrawingRectROI()
        {
            ROIRect rectROI = new ROIRect();
            ROIList.Add(rectROI);
            rectROI.TopLeftPoint = rectROI.BottomRightPoint = mousePosition;
            rectROI.CaptureMouse();
            rectROI.CurrentState = State.DrawingInProgress;
            rectROI.LastROIDrawEvent += OnGetLastDrawEventUpdated;
            SetBinding(MagnificationProperty.Name, rectROI, ROI.MagnificationProperty);
        }

        private ROIDescriptor GetROIDescriptor()
        {
            ROIDescriptor roiDescriptor = new ROIDescriptor();

            if (ROIList.Count == 0)
                return roiDescriptor;

            foreach (ROI item in ROIList)
                roiDescriptor.contours.Add(item.GetROIDescriptorContour());

            return roiDescriptor;
        }

        private void SetBinding(string sourcePathName, DependencyObject targetObject, DependencyProperty targetDp)
        {
            Binding binding = new Binding(sourcePathName);
            binding.Source = this;
            BindingOperations.SetBinding(targetObject, targetDp, binding);
        }

        private void ReCreateBitmap(ImageSource image)
        {
            PixelFormat pixelFormat = PixelFormats.Gray8;
            BitmapPalette palette = null;

            if ((null == writeableBitmap) ||
                (image.Width != writeableBitmap.Width) || 
                (image.Height != writeableBitmap.Height))
            {
                writeableBitmap = new WriteableBitmap((int)image.Width, (int)image.Height, 96, 96, pixelFormat, palette);
            }

        }
        #endregion
    }
}
