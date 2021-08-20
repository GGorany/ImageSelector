using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageSelector.Example.ViewModels
{
    public class RectanglerTestViewModel : BindableBase
    {
        #region Properties
        private string _FileName = string.Empty;
        private BitmapSource _SourceImage = null;
        private Point _PointTopLeft = new Point(10, 10);
        private Point _PointBottomRight = new Point(30, 40);
        private int _TopLeftX = 0;
        private int _TopLeftY = 0;
        private int _BottomRightX = 0;
        private int _BottomRightY = 0;

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
                _PointTopLeft = value;
                _TopLeftX = (int)_PointTopLeft.X;
                _TopLeftY = (int)_PointTopLeft.Y;
                RaisePropertyChanged("TopLeftX");
                RaisePropertyChanged("TopLeftY");
            }
        }

        public Point PointBottomRight
        {
            get { return _PointBottomRight; }
            set
            {
                _PointBottomRight = value;
                _BottomRightX = (int)_PointBottomRight.X;
                _BottomRightY = (int)_PointBottomRight.Y;
                RaisePropertyChanged("BottomRightX");
                RaisePropertyChanged("BottomRightY");
            }
        }

        public int TopLeftX
        {
            get { return _TopLeftX; }
            set
            {
                _TopLeftX = value;
                UpdatePointTopLeft();
            }
        }

        public int TopLeftY
        {
            get { return _TopLeftY; }
            set
            {
                _TopLeftY = value;
                UpdatePointTopLeft();
            }
        }

        public int BottomRightX
        {
            get { return _BottomRightX; }
            set
            {
                _BottomRightX = value;
                UpdatePointBottomRight();
            }
        }

        public int BottomRightY
        {
            get { return _BottomRightY; }
            set
            {
                _BottomRightY = value;
                UpdatePointBottomRight();
            }
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
        public RectanglerTestViewModel()
        {

        }
        #endregion

        #region Private Method
        private void UpdatePointTopLeft()
        {
            _PointTopLeft = new Point(TopLeftX, TopLeftY);
            RaisePropertyChanged("PointTopLeft");
        }

        private void UpdatePointBottomRight()
        {
            _PointBottomRight = new Point(BottomRightX, BottomRightY);
            RaisePropertyChanged("PointBottomRight");
        }
        #endregion
    }
}
