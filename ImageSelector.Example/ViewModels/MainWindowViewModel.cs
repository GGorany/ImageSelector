using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageSelector.Example.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Properties
        private string _FileName = string.Empty;
        private BitmapSource _SourceImage = null;
        private Point _PointTopLeft;
        private Point _PointBottomRight;
        private string _TextTopLeft = string.Empty;
        private string _TextBottomRight = string.Empty;

        public string FileName
        {
            get { return _FileName; }
            set { SetProperty(ref _FileName, value); }
        }

        public BitmapSource SourceImage
        {
            get { return _SourceImage; }
            set { SetProperty(ref _SourceImage, value); }
        }

        public Point PointTopLeft
        {
            get { return _PointTopLeft; }
            set 
            { 
                SetProperty(ref _PointTopLeft, value);
                this.SetTextTopLeft();
            }
        }

        public Point PointBottomRight
        {
            get { return _PointBottomRight; }
            set 
            { 
                SetProperty(ref _PointBottomRight, value);
                this.SetTextBottomRight();
            }
        }

        public string TextTopLeft
        {
            get { return _TextTopLeft; }
            set { SetProperty(ref _TextTopLeft, value); }
        }

        public string TextBottomRight
        {
            get { return _TextBottomRight; }
            set { SetProperty(ref _TextBottomRight, value); }
        }
        #endregion

        #region Commands
        private DelegateCommand _OpenFile;

        public DelegateCommand OpenFile => _OpenFile ??= new DelegateCommand(() =>
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".jpg";
            dialog.Filter = "image files (.jpg)|*.jpg";

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                this.FileName = dialog.FileName;
                this.SourceImage = new BitmapImage(new Uri(this.FileName));
            }
        });
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            this.PointTopLeft = new Point(10, 10);
            this.PointBottomRight = new Point(40, 50);
        }
        #endregion

        #region Private Method
        private void SetTextTopLeft()
        {
            TextTopLeft = $"Top Left   X : {PointTopLeft.X}, Y : {PointTopLeft.Y}";
        }

        private void SetTextBottomRight()
        {
            TextBottomRight = $"Bottom Right   X : {PointBottomRight.X}, Y : {PointBottomRight.Y}";
        }
        #endregion
    }
}
