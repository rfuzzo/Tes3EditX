using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections;
using System.Reflection;
using Tes3EditX.Backend.Models;
using TES3Lib.Base;
using TES3Lib.Records;

namespace Tes3EditX.Backend.ViewModels
{
    public partial class RecordFieldViewModel : ObservableObject
    {
        private readonly (Subrecord subrecord, PropertyInfo propertyInfo) _info;

        private readonly bool _isReadonly;

        public RecordFieldViewModel((Subrecord subrecord, PropertyInfo propertyInfo) info, object? wrappedField, string name, bool isReadonly)
        {
            _isReadonly = isReadonly;
            _info = info;
            IsEnabled = !_isReadonly;

            WrappedField = wrappedField;

            Name = name;
            Text = ToString();

        }

        [ObservableProperty]
        private object? _wrappedField;

        [ObservableProperty]
        private bool _isEnabled;

        public string Name { get; }
        public string Text { get; }
        public bool IsConflict { get; set; }

        partial void OnWrappedFieldChanged(object? oldValue, object? newValue)
        {
            if (oldValue != null)
            {
                // reflect up
                _info.propertyInfo.SetValue(_info.subrecord, newValue);
                // notify UI
                WeakReferenceMessenger.Default.Send(new FieldChangedMessage(Name));
            }
        }

        // we display only the text in the normal compare view 
        // and double click opens an editor
        public override string ToString()
        {
            if (WrappedField is null)
            {
                return "NULL";
            }
            else
            {
                if (WrappedField is not string and IEnumerable enumerable)
                {
                    string result = "";
                    foreach (object? item in enumerable)
                    {
                        result += $"{item}|";
                    }
                    string r1 = result.TrimEnd('|');
                    return r1;
                }

                return WrappedField.ToString()!;
            }
        }
    }
}
