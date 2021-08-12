using Prism.Commands;
using Prism.Mvvm;

namespace ImageSelector.Example.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Properties
        private string _FileName = string.Empty;

        public string FileName
        {
            get { return _FileName; }
            set { SetProperty(ref _FileName, value); }
        }
        #endregion

        #region Commands
        private DelegateCommand _OpenFile;

        public DelegateCommand OpenFile => _OpenFile ??= new DelegateCommand(OnOpenFile);
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {

        }
        #endregion

        #region Private Method
        private void OnOpenFile()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".jpg";
            dialog.Filter = "image files (.jpg)|*.jpg";

            bool? result = dialog.ShowDialog();
            if (result == true)
                this.FileName = dialog.FileName;
        }
        #endregion
    }
}
