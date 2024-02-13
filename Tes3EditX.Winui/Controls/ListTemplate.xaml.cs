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
public sealed partial class ListTemplate : UserControl
{
    //public event EventHandler<HashSetValueChangedEventArgs>? ValueChanged;

    public ListTemplate()
    {
        InitializeComponent();
    }

    [ObservableProperty]
    private ObservableCollection<ItemViewModel> _bindingList = [];

    public static readonly DependencyProperty ListProperty = DependencyProperty.Register(
        nameof(List),
        typeof(Array),
        typeof(ListTemplate),
        new PropertyMetadata(null, Changed)
    );

    private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // get generic argument
        if (d is ListTemplate ctrl)
        {
            ctrl.BindingList.Clear();
            foreach (var item in ctrl.List)
            {
                ctrl.BindingList.Add(new(item));
            }
        }
    
    }

    public Array List
    {
        get { return (Array)GetValue(ListProperty); }
        set { SetValue(ListProperty, value); }
    }

    //public bool IsInitialized { get; set; }

    private void RecordFieldTemplate_ValueChanged(object sender, EventArgs e)
    {
        //if (!IsInitialized)
        //{
        //    return;
        //}

        List = BindingList.Select(x => x.Name).ToArray();
    }
}

public partial class ItemViewModel(object? name) : ObservableObject
{
    [ObservableProperty]
    private object? _name = name;
}

public class ListValueChangedEventArgs(IList value) : EventArgs
{
    public IList List { get; set; } = value;
}