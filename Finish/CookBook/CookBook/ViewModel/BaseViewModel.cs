using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace CookBook.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Service

        private HttpClient _httpClient;
        protected HttpClient Client => _httpClient ?? (_httpClient = new HttpClient());

        #endregion
       
        #region Properties

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (_title == value)
                    return;
                _title = value;
                OnPropertyChanged();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value)
                    return;

                _isBusy = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }

        public bool IsNotBusy => !IsBusy;

        #endregion

        #region Constructors

        public BaseViewModel(string title)
        {
            Title = title;
        }
        
        #endregion
        
        #region Property Changes
    
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
