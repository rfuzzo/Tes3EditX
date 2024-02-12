using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Tes3EditX.Backend.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Tes3EditX.Winui.Controls;

[ObservableObject]
public sealed partial class RecordFieldTemplate : UserControl
{
    public RecordFieldTemplate()
    {
        InitializeComponent();        
    }
    
    public static readonly DependencyProperty RecordFieldProperty = DependencyProperty.Register(
          nameof(RecordField),
          typeof(RecordFieldViewModel),
          typeof(RecordFieldTemplate),
          new PropertyMetadata(null)
        );

    public RecordFieldViewModel RecordField
    {
        get { return (RecordFieldViewModel)GetValue(RecordFieldProperty); }
        set { SetValue(RecordFieldProperty, value); }
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // notify the record/plugin that something changed
        if (sender is TextBox ctrl && RecordField.WrappedField is string val)
        {
            string text = ctrl.Text;
            if (!string.IsNullOrEmpty(text) && val.Trim('\0') != text)
            {
                RecordField.WrappedField = text;
            }
        }
    }

    private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (sender is NumberBox ctrl)
        {
            if (RecordField.WrappedField is int i )
            {
                int ctrlVal = (int)ctrl.Value;
                if (i != ctrlVal)
                {
                    RecordField.WrappedField = ctrlVal;
                }
            }
            else if (RecordField.WrappedField is short s)
            {
                short ctrlVal = (short)ctrl.Value;
                if (s != ctrlVal)
                {
                    RecordField.WrappedField = ctrlVal;
                }
            }
            else if (RecordField.WrappedField is byte b)
            {
                byte ctrlVal = (byte)ctrl.Value;
                if (b != ctrlVal)
                {
                    RecordField.WrappedField = ctrlVal;
                }
            }
            else if (RecordField.WrappedField is float f)
            {
                float ctrlVal = (float)ctrl.Value;
                if (f != ctrlVal)
                {
                    RecordField.WrappedField = ctrlVal;
                }
            }
        }
    }

    private void FlagsTemplate_ValueChanged(object sender, HashSetValueChangedEventArgs e)
    {
        if (RecordField.WrappedField is IEnumerable)
        {
            RecordField.WrappedField = e.Hashset;
        }
    }

    private void EnumTemplate_ValueChanged(object sender, EnumValueChangedEventArgs e)
    {
        if (RecordField.WrappedField is Enum)
        {
            RecordField.WrappedField = e.Enum;
        }
    }
}
