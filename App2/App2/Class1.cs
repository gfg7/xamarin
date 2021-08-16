using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace App2
{
    public class Model
    {
        public int Count { get; set; }
        public string StringProp { get; set; }
        public string ReverseStringProp => new string(StringProp?.ToCharArray().Reverse().ToArray());
    }

    public abstract class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo, IDataErrorInfo
    {
        public bool HasErrors => throw new NotImplementedException();

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void Changed([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            yield return Errors.TryGetValue(propertyName, out string message);
        }

        private Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();

        public string Error => null;

        public string this[string columnName] => throw new NotImplementedException();

        public void ManageError(string propertyName, string message, bool add = true)
        {
            if (Errors.TryGetValue(propertyName, out _))
            {
                Errors.Remove(propertyName);
            }
            if (add)
            {
                Errors.Add(propertyName, message);
            }

        }
    }

    public class ViewModel : BaseViewModel
    {
        private readonly Model model;
        public ICommand Command { protected set; get; }
        public ObservableCollection<string> Strings { get; set; } = new ObservableCollection<string>();

        public bool Less => model.Count < 20;

        public ViewModel()
        {
            model = new Model();
            Command = new Command(Increase);
        }

        public string Count
        {
            get
            {
                return model.Count.ToString();
            }
        }

        public string StringProp
        {
            get => model.StringProp;
            set
            {
                model.StringProp = value;
                Changed(nameof(StringProp));
                Changed(nameof(ReverseStringProp));
            }
        }

        public string ReverseStringProp => model.ReverseStringProp;

        public void Increase()
        {
            model.Count++;
            Changed(nameof(Count));
            Strings.Add(StringProp);
        }

        public new string this[string columnName]
        {
            get
            {
                string message = null;
                bool notValid = true;
                switch (columnName)
                {
                    case nameof(StringProp):
                        notValid = string.Equals(StringProp, ReverseStringProp);
                        message = "mirrored";
                        break;
                    default:
                        break;
                }
                ManageError(columnName, message, notValid);
                return null;
            }
        }

    }
}
