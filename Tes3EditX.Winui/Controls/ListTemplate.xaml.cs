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
using System.Threading.Channels;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Tes3EditX.Winui.Controls;

public sealed partial class ListTemplate : UserControl
{
    //public event EventHandler<HashSetValueChangedEventArgs>? ValueChanged;

    public ListTemplate()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty ListProperty = DependencyProperty.Register(
        nameof(List),
        typeof(IList),
        typeof(ListTemplate),
        new PropertyMetadata(null, Changed)
    );

    private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // get generic argument
        if (d is FlagsTemplate control)
        {
            control.IsInitialized = false;

        }
    
    }

    public IList List
    {
        get { return (IList)GetValue(ListProperty); }
        set { SetValue(ListProperty, value); }
    }

    public bool IsInitialized { get; set; }
}


public class ListValueChangedEventArgs(IList value) : EventArgs
{
    public IList List { get; set; } = value;
}