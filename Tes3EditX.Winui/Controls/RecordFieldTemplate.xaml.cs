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

        Enabled = true;
    }

    public static readonly DependencyProperty WrappedFieldProperty = DependencyProperty.Register(
          nameof(WrappedField),
          typeof(object),
          typeof(RecordFieldTemplate),
          new PropertyMetadata(null, OnChanged)
        );

    private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RecordFieldTemplate ctrl)
        {
        }
    }

    public event EventHandler? ValueChanged;

    public object WrappedField
    {
        get { return (object)GetValue(WrappedFieldProperty); }
        set { SetValue(WrappedFieldProperty, value); }
    }

    public static readonly DependencyProperty EnabledProperty = DependencyProperty.Register(
         nameof(Enabled),
         typeof(bool),
         typeof(RecordFieldTemplate),
         new PropertyMetadata(null)
       );

    public bool Enabled
    {
        get { return (bool)GetValue(EnabledProperty); }
        set { SetValue(EnabledProperty, value); }
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // notify the record/plugin that something changed
        if (sender is TextBox ctrl && WrappedField is string val)
        {
            string text = ctrl.Text;
            if (!string.IsNullOrEmpty(text) && val.Trim('\0') != text)
            {
                WrappedField = text;
                ValueChanged?.Invoke(this, new());
            }
        }
    }

    private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (sender is NumberBox ctrl)
        {
            if (WrappedField is int i )
            {
                int ctrlVal = (int)ctrl.Value;
                if (i != ctrlVal)
                {
                    WrappedField = ctrlVal;
                    ValueChanged?.Invoke(this, new());
                }
            }
            else if (WrappedField is short s)
            {
                short ctrlVal = (short)ctrl.Value;
                if (s != ctrlVal)
                {
                    WrappedField = ctrlVal;
                    ValueChanged?.Invoke(this, new());
                }
            }
            else if (WrappedField is byte b)
            {
                byte ctrlVal = (byte)ctrl.Value;
                if (b != ctrlVal)
                {
                    WrappedField = ctrlVal;
                    ValueChanged?.Invoke(this, new());
                }
            }
            else if (WrappedField is float f)
            {
                float ctrlVal = (float)ctrl.Value;
                if (f != ctrlVal)
                {
                    WrappedField = ctrlVal;
                    ValueChanged?.Invoke(this, new());
                }
            }
        }
    }

    private void FlagsTemplate_ValueChanged(object sender, HashSetValueChangedEventArgs e)
    {
        if (WrappedField is IEnumerable)
        {
            WrappedField = e.Hashset;
            ValueChanged?.Invoke(this, new());
        }
    }

    private void EnumTemplate_ValueChanged(object sender, EnumValueChangedEventArgs e)
    {
        if (WrappedField is Enum)
        {
            WrappedField = e.Enum;
            ValueChanged?.Invoke(this, new());
        }
    }

    private void ListTemplate_ValueChanged(object sender, ListValueChangedEventArgs e)
    {
        if (WrappedField is Array)
        {
            WrappedField = e.List;
            ValueChanged?.Invoke(this, new());
        }
    }
}
