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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Channels;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Tes3EditX.Winui.Controls;

[ObservableObject]
public sealed partial class EnumTemplate : UserControl
{
    public event EventHandler<EnumValueChangedEventArgs>? ValueChanged;

    public EnumTemplate()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty MyEnumProperty = DependencyProperty.Register(
        nameof(MyEnum),
        typeof(object),
        typeof(EnumTemplate),
        new PropertyMetadata(null, Changed)
    );

    public object MyEnum
    {
        get { return (object)GetValue(MyEnumProperty); }
        set { SetValue(MyEnumProperty, value); }
    }

    private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // get generic argument
        if (d is EnumTemplate control)
        {
            control.IsInitialized = false;
            var values = Enum.GetNames(control.MyEnum.GetType()).ToList();
            control.Values = new(values);
            // set selection

            dynamic en = control.MyEnum;
            var valstr = en.ToString();
            control.SelectedItem = valstr;
            control.IsInitialized = true;
        }
    }

    // Holds all values of the enum
    // bound to UI
    [ObservableProperty]
    private ObservableCollection<string> _values = [];

    [ObservableProperty]
    private string? _selectedItem = null;

    public bool IsInitialized { get; set; }

    partial void OnSelectedItemChanged(string? value)
    {
        if (!IsInitialized)
        {
            return;

        }

        if (MyEnum is Enum e)
        {
            if (Enum.TryParse(e.GetType(), value, out var resultEnum))
            {
                dynamic result = resultEnum;
                ValueChanged?.Invoke(this, new(result));
            }
        }        
    }
}

public class EnumValueChangedEventArgs(Enum value) : EventArgs
{
    public Enum Enum { get; set; } = value;
}