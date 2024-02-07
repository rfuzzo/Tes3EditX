using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES3Lib.Base;
using TES3Lib.Interfaces;

namespace Tes3EditX.Backend.ViewModels
{
    public partial class RecordFieldViewModel : ObservableObject
    {
        public RecordFieldViewModel(object? wrappedField, string name)
        {
            WrappedField = wrappedField;
            Name = name;
            Text = ToString();
        }

        [ObservableProperty]
        private object? _wrappedField;
        public string Name { get; }
        public string Text { get; }
        public bool IsConflict { get; set; }

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
                if ( WrappedField is not string && WrappedField is IEnumerable enumerable)
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
