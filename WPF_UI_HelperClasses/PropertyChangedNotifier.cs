using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

/// <summary>
/// SOLID -> Open/Close principle oder Liskov Substitution principle
/// Fragen was für ein Prinzip! wegen Vererbung
/// 
/// DRY Don`t Repeat Yourself -> durch die Vererbung an die ViewModels muss
/// der INotifyPropertyChanged nicht in jeder Klasse neu implementiert werden.
/// => Zählt für alle Klassen in diesem Projektordner
/// 
/// </summary>

namespace WPF_UI_HelperClasses
{
    public class PropertyChangedNotifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T t_old, T t_new, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(t_old, t_new))
                return false;

            t_old = t_new;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            return true;
        }
    }
}
