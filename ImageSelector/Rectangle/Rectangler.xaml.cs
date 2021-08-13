using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing;

namespace ImageSelector
{
    /// <summary>
    /// ROI.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Rectangler : UserControl
    {
        private double magnification = 0.0;

        #region Properties
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source",
            typeof(ImageSource),
            typeof(Rectangler),
            new PropertyMetadata(default(ImageSource), OnSourceChanged));

        public static readonly DependencyProperty RectProperty = DependencyProperty.Register(
            "Rect",
            typeof(Rectangle),
            typeof(Rectangler),
            new PropertyMetadata(default(Rectangle), OnRectChanged));

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
        #endregion

        public Rectangler()
        {
            InitializeComponent();

            SourceImage.LayoutTransform = new ScaleTransform();

            MouseHandler.MouseWheel += MouseHandler_MouseWheel;
            MouseHandler.MouseMove += MouseHandler_MouseMove;
            MouseHandler.MouseLeftButtonDown += MouseHandler_MouseLeftButtonDown;
            MouseHandler.MouseLeftButtonUp += MouseHandler_MouseLeftButtonUp;
        }

        #region Events
        private void MouseHandler_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoom_delta = e.Delta > 0 ? .1 : -.1;
            magnification = (magnification += zoom_delta).LimitToRange(.1, 10);
            //CenterViewerAroundMouse(MousePosition);
            this.ShowMousePosition();
            e.Handled = true;
        }

        private void MouseHandler_MouseMove(object sender, MouseEventArgs e)
        {
            this.ShowMousePosition();
        }

        private void MouseHandler_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void MouseHandler_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
        }
        #endregion

        #region Private Method
        private void ShowMousePosition()
        {
            var pos = Mouse.GetPosition(this.SourceImage);
            if ((pos.X >= 0) && (pos.X < this.SourceImage.Width) && (pos.Y >= 0) && (pos.Y < this.SourceImage.Height))
            {
                this.TextBox_MousePosition.Text = $"x = {Mouse.GetPosition(this.SourceImage).X:N0}; y = {Mouse.GetPosition(this.SourceImage).Y:N0}";
            }
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Rectangler rectangler)) return;
            if (e.NewValue is ImageSource newImage)
            {
                rectangler.SourceImage.Source = newImage;
                rectangler.SourceImage.Width = newImage.Width;
                rectangler.SourceImage.Height = newImage.Height;
                //cropper.CanvasPanel.UpdateLayout();
                //cropper.AdornerCrop(cropper.Rect);
            }
            else
            {
                rectangler.SourceImage.Source = null;
                rectangler.SourceImage.Width = double.NaN;
                rectangler.SourceImage.Height = double.NaN;
                //cropper.CanvasPanel.UpdateLayout();
                //cropper.CroppingAdorner.Crop = Rectangle.Empty;
            }
        }

        private static void OnRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Rectangler rectangler)) return;
            //if (cropper.CroppingAdorner == default) return;
            //if (e.NewValue is Rectangle rect)
            //{
            //    cropper.AdornerCrop(rect);
            //}
            //else
            //{
            //    cropper.CroppingAdorner.Crop = Rectangle.Empty;
            //}
        }

        private void CanvasPanel_Loaded(object sender, RoutedEventArgs e)
        {
            //if (sender is FrameworkElement visual)
            //{
            //    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(visual);
            //    if (adornerLayer == null)
            //        return;

            //    CroppingAdorner = new CroppingAdorner(visual);
            //    adornerLayer.Add(CroppingAdorner);
            //    AdornerCrop(Rect);
            //    CroppingAdorner.OnRectangleSizeEvent += CroppingAdorner_OnRectangleSizeEvent;
            //}
        }

        #endregion

    }
}
