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
        private bool isSquareMode = false;
        private ROIDescriptor _lastROIDescriptor = new ROIDescriptor();

        public event EventHandler<ROIValueChangedEventArgs> ROIValueChanged;

        #region Properties
        private double _Magnification = 1.0;

        public double Magnification
        {
            get { return _Magnification; }
            set
            {
                _Magnification = value;
                ApplyMagnification();
            }
        }
        #endregion

        #region DependencyProperties
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source",
            typeof(ImageSource),
            typeof(Selector),
            new PropertyMetadata(default(ImageSource), OnSourceChanged));

        public static readonly DependencyProperty TopLeftProperty = DependencyProperty.Register(
            "TopLeft",
            typeof(Point),
            typeof(Selector),
            new PropertyMetadata(default(Point), OnTopLeftChanged));

        public static readonly DependencyProperty BottomRightProperty = DependencyProperty.Register(
            "BottomRight",
            typeof(Point),
            typeof(Selector),
            new PropertyMetadata(default(Point), OnBottomRightChanged));

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public Point TopLeft
        {
            get { return (Point)GetValue(TopLeftProperty); }
            set { SetValue(TopLeftProperty, value); }
        }

        public Point BottomRight
        {
            get { return (Point)GetValue(BottomRightProperty); }
            set { SetValue(BottomRightProperty, value); }
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

            _SquareMode.Checked += (s, e) => { isSquareMode = (bool)(s as CheckBox).IsChecked; };
            _SquareMode.Unchecked += (s, e) => { isSquareMode = (bool)(s as CheckBox).IsChecked; };

            ROIList = new ObservableCollection<ROI>();
            _ROI.ItemsSource = ROIList;
        }

        #region Events
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Selector selector)) 
                return;

            if (e.NewValue is ImageSource newImage)
            {
                selector._SourceImage.Source = newImage;
                selector._SourceImage.Width = newImage.Width;
                selector._SourceImage.Height = newImage.Height;
            }
            else
            {
                selector._SourceImage.Source = null;
                selector._SourceImage.Width = double.NaN;
                selector._SourceImage.Height = double.NaN;
            }
        }

        private static void OnTopLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Selector selector))
                return;

            if (selector.ROIList.Count != 1)
                return;

            if (e.NewValue is Point point)
            {
                (selector.ROIList[0] as ROIRect).TopLeftPoint = point;
            }
            else
            {
                (selector.ROIList[0] as ROIRect).TopLeftPoint = new Point(0, 0);
            }
        }

        private static void OnBottomRightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Selector selector))
                return;

            if (selector.ROIList.Count != 1)
                return;

            if (e.NewValue is Point point)
            {
                (selector.ROIList[0] as ROIRect).BottomRightPoint = point;
            }
            else
            {
                (selector.ROIList[0] as ROIRect).BottomRightPoint = new Point(0, 0);
            }
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
            mousePosition = Mouse.GetPosition(_SourceImage);

            ShowMousePosition();
        }

        private void _MouseHandler_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _MouseHandler.ReleaseMouseCapture();
        }

        private void _MouseHandler_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mousePosition = Mouse.GetPosition(_SourceImage);

            double zoom_delta = e.Delta > 0 ? .1 : -.1;
            Magnification = (Magnification += zoom_delta).LimitToRange(.1, 10);
            ApplyMagnification();
            ShowMousePosition();

            // Center Viewer Around Mouse Position
            if (_ScrollViewer != null)
            {
                _ScrollViewer.ScrollToHorizontalOffset(mousePosition.X * Magnification - Mouse.GetPosition(_ScrollViewer).X);
                _ScrollViewer.ScrollToVerticalOffset(mousePosition.Y * Magnification - Mouse.GetPosition(_ScrollViewer).Y);
            }

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
            Point point = Mouse.GetPosition(_SourceImage);
            if ((point.X >= 0) && (point.X < _SourceImage.Width) && (point.Y >= 0) && (point.Y < _SourceImage.Height))
            {
                _Position.Text = $"x = {Mouse.GetPosition(_SourceImage).X:N0}; y = {Mouse.GetPosition(_SourceImage).Y:N0}";
            }
        }

        private void ApplyMagnification()
        {
            if (_SourceImage != null)
            {
                ScaleTransform obj = (ScaleTransform)_SourceImage.LayoutTransform;
                obj.ScaleX = obj.ScaleY = Magnification;
                RenderOptions.SetBitmapScalingMode(_SourceImage, BitmapScalingMode.HighQuality);
                _Zoom.Text = $"{Magnification * 100:N0}%";
            }

            if (_ROI != null)
            {
                ScaleTransform obj2 = (ScaleTransform)_ROI.LayoutTransform;
                obj2.ScaleX = obj2.ScaleY = Magnification;
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
        #endregion
    }
}
