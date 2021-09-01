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
        private Rect _Rectangle = Rect.Empty;
        private int _X = 0;
        private int _Y = 0;
        private int _Width = 0;
        private int _Height = 0;

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

        public Rect Rectangle
        {
            get { return _Rectangle; }
            set
            {
                _Rectangle = value;
                _X = (int)_Rectangle.X;
                _Y = (int)_Rectangle.Y;
                _Width = (int)_Rectangle.Width;
                _Height = (int)_Rectangle.Height;
                RaisePropertyChanged("X");
                RaisePropertyChanged("Y");
                RaisePropertyChanged("Width");
                RaisePropertyChanged("Height");
            }
        }

        public int X
        {
            get { return _X; }
            set
            {
                _X = value;
                UpdateRectangle();
            }
        }

        public int Y
        {
            get { return _Y; }
            set
            {
                _Y = value;
                UpdateRectangle();
            }
        }

        public int Width
        {
            get { return _Width; }
            set
            {
                _Width = value;
                UpdateRectangle();
            }
        }

        public int Height
        {
            get { return _Height; }
            set
            {
                _Height = value;
                UpdateRectangle();
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
        public RectanglerTestViewModel()
        {

        }
        #endregion

        #region Private Method
        private void UpdateRectangle()
        {
            _Rectangle = new Rect(X, Y, Width, Height);
            RaisePropertyChanged("Rectangle");
        }
        #endregion
    }
}
