using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;

namespace Tes3EditX.Backend.ViewModels
{
    public partial class RecordFieldViewModel : ObservableObject
    {
        public RecordFieldViewModel(object? wrappedField, string name, bool isReadonly)
        {
            _isReadonly = isReadonly;

            WrappedField = wrappedField;
            Name = name;
            Text = ToString();
        }

        [ObservableProperty]
        private object? _wrappedField;
        private readonly bool _isReadonly;

        public string Name { get; }
        public string Text { get; }
        public bool IsConflict { get; set; }
        public bool IsEnabled => !_isReadonly;

        partial void OnWrappedFieldChanged(object? oldValue, object? newValue)
        {
            
            if (oldValue is not null)
            {
                // register change

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
                if (WrappedField is not string && WrappedField is IEnumerable enumerable)
                {
                    var result = "";
                    foreach (var item in enumerable)
                    {
                        result += $"{item}|";
                    }
                    var r1 = result.TrimEnd('|');
                    return r1;
                }

                return WrappedField.ToString()!;
            }
        }
    }
}
