using System;
using System.Windows.Input;
using Apache.Ignite.Core;
using Apache.Ignite.Visor.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Apache.Ignite.Visor
{
    internal class ConnectViewModel : ViewModelBase
    {
        private readonly Action<Cluster> _onConnected;
        private bool _isBusy;
        private string _errorMessage;

        public ConnectViewModel(Action<Cluster> onConnected)
        {
            NativeMethods.AllocConsole();

            _onConnected = onConnected;
            ConnectCommand = new RelayCommand(Connect);
            Connect();
        }

        public ICommand ConnectCommand { get; }

        public bool IsBusy
        {
            get { return _isBusy; }
            private set
            {
                _isBusy = value;
                RaisePropertyChanged(nameof(IsBusy));
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            private set
            {
                _errorMessage = value;
                RaisePropertyChanged(nameof(ErrorMessage));
            }
        }

        private async void Connect()
        {
            IsBusy = true;
            try
            {
                var cluster = await Cluster.Connect();
                _onConnected(cluster);
            }
            catch (Exception e)
            {
                ErrorMessage = e.ToString();
            }

            IsBusy = false;
        }
    }
}
