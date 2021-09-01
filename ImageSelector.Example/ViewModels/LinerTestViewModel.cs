using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageSelector.Example.ViewModels
{
    public class LinerTestViewModel : BindableBase
    {
        #region Properties
        private string _FileName = string.Empty;
        private BitmapSource _SourceImage = null;
        private Point _StartPoint = new Point(10, 10);
        private Point _EndPoint = new Point(30, 40);
        private int _StartPointX = 0;
        private int _StartPointY = 0;
        private int _EndPointX = 0;
        private int _EndPointY = 0;

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

        public Point StartPoint
        {
            get { return _StartPoint; }
            set
            {
                _StartPoint = value;
                _StartPointX = (int)_StartPoint.X;
                _StartPointY = (int)_StartPoint.Y;
                RaisePropertyChanged("StartPointX");
                RaisePropertyChanged("StartPointY");
            }
        }

        public Point EndPoint
        {
            get { return _EndPoint; }
            set
            {
                _EndPoint = value;
                _EndPointX = (int)_EndPoint.X;
                _EndPointY = (int)_EndPoint.Y;
                RaisePropertyChanged("EndPointX");
                RaisePropertyChanged("EndPointY");
            }
        }

        public int StartPointX
        {
            get { return _StartPointX; }
            set
            {
                _StartPointX = value;
                UpdateStartPoint();
            }
        }

        public int StartPointY
        {
            get { return _StartPointY; }
            set
            {
                _StartPointY = value;
                UpdateStartPoint();
            }
        }

        public int EndPointX
        {
            get { return _EndPointX; }
            set
            {
                _EndPointX = value;
                UpdateEndPoint();
            }
        }

        public int EndPointY
        {
            get { return _EndPointY; }
            set
            {
                _EndPointY = value;
                UpdateEndPoint();
            }
        }
        #endregion

        #region Commands
        private DelegateCommand _OpenFile;

        public DelegateCommand OpenFile => _OpenFile ??= new DelegateCommand(() =>
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".jpg";
            dialog.Filter = "image files (*jpg, *.png) | *.jpg; *.png";

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                this.FileName = dialog.FileName;
                this.SourceImage = new BitmapImage(new Uri(this.FileName));
            }
        });
        #endregion

        #region Constructor
        public LinerTestViewModel()
        {

        }
        #endregion

        #region Private Method
        private void UpdateStartPoint()
        {
            _StartPoint = new Point(StartPointX, StartPointY);
            RaisePropertyChanged("StartPoint");
        }

        private void UpdateEndPoint()
        {
            _EndPoint = new Point(EndPointX, EndPointY);
            RaisePropertyChanged("EndPoint");
        }
        #endregion
    }
}
