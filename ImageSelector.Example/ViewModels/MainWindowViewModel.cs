using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace ImageSelector.Example.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Properties
        private string _FileName = string.Empty;
        private BitmapSource _SourceImage = null;
        private Rectangle _Rect;

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

        public Rectangle Rect
        {
            get { return _Rect; }
            set { SetProperty(ref _Rect, value); }
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
            this.Rect = new Rectangle(0, 0, 0, 0);
        }
        #endregion

        #region Private Method

        #endregion
    }
}
