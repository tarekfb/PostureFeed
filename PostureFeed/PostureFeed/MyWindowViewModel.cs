using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostureFeed
{
    public class MyWindowViewModel : ViewModelBase
    {
        private bool _isButton1Visibile = true;
        public bool IsButton1Visible
        {
            get => _isButton1Visibile;
            set
            {
                if (_isButton1Visibile == value)
                    return;

                _isButton1Visibile = value;
                RaisePropertyChanged(nameof(IsButton1Visible));
            }
        }


        RelayCommand _toggleButton1Visbility;
        public RelayCommand ToggleButton1Visibility
        {
            get
            {
                return _toggleButton1Visbility ?? (_toggleButton1Visbility = new RelayCommand(
                    () =>
                    {
                        IsButton1Visible = !IsButton1Visible;
                    }));
            }
        }
    }
}
