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
using System.Text.Json;
using System.Threading.Channels;
using TES3Lib.Subrecords.Shared;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Tes3EditX.Winui.Controls;

[ObservableObject]
public sealed partial class ListTemplate : UserControl
{
    public event EventHandler<ListValueChangedEventArgs>? ValueChanged;

    public ListTemplate()
    {
        InitializeComponent();
    }

    public Type? ListType { get; set; }

    public bool IsInitialized { get; set; }

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
        if (d is ListTemplate ctrl && !ctrl.IsInitialized)
        {
            ctrl.BindingList.Clear();

            ctrl.ListType = ctrl.List.GetType();

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

    private void RecordFieldTemplate_ValueChanged(object sender, EventArgs e)
    {
        IsInitialized = true;

        if (ListType is not null)
        {
            var json = JsonSerializer.Serialize(BindingList.Select(x => x.Name).ToArray());
            dynamic? val = JsonSerializer.Deserialize(json, ListType);

            if (val != null)
            {
                List = val;
                ValueChanged?.Invoke(this, new(val));
            }
            
        }

        IsInitialized = false;
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