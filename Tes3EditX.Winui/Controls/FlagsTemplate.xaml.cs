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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Tes3EditX.Backend.Services;
using Tes3EditX.Backend.ViewModels;
using TES3Lib.Base;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Tes3EditX.Winui.Controls;

[ObservableObject]
public partial class FlagsTemplate : UserControl
{
    public FlagsTemplate()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty ListProperty = DependencyProperty.Register(
      nameof(List),
      typeof(IEnumerable),
      typeof(FlagsTemplate),
      new PropertyMetadata(null, Changed)
    );

    public IEnumerable List
    {
        get { return (IEnumerable)GetValue(ListProperty); }
        set { SetValue(ListProperty, value); }
    }

    // Holds all values of the enum
    // bound to UI
    [ObservableProperty]
    private ObservableCollection<object> _flags = [];

    public Type? EnumType { get; set; }
    public bool IsInitialized { get; set; }

    private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // get generic argument
        if (d is FlagsTemplate ctrl)
        {
            ctrl.IsInitialized = false;
            ctrl.EnumType = ctrl.List.GetType().GenericTypeArguments.FirstOrDefault();
            if (ctrl.EnumType is not null)
            {
                var values = Enum.GetValues(ctrl.EnumType);
                var flags = values.Cast<object>().ToList();
                if (flags is not null)
                {
                    if (!ctrl.Flags.ToList().SequenceEqual(flags))
                    {
                        ctrl.Flags = new(flags);
                    }

                    ctrl.listview.SelectedItems.Clear();
                    ctrl.listview.SetValue(ListView.ItemsSourceProperty, ctrl.Flags);
                    foreach (var x in ctrl.List)
                    {
                        if (x is not null)
                        {
                            int i = ctrl.Flags.IndexOf(x);
                            ctrl.listview.SelectRange(new ItemIndexRange(i, 1));
                        }

                    }

                    ctrl.IsInitialized = true;
                }
            }
        }
    }

   

    public event EventHandler<HashSetValueChangedEventArgs>? ValueChanged;

    private void listview_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsInitialized)
        {
            return;
        }

        if (sender is ListView ctrl)
        {
            var selectedItems = ctrl.SelectedItems;
            if (selectedItems is not null)
            {
                // set property
                var type = EnumType;
                if (type is not null)
                {
                    dynamic? hs = Activator.CreateInstance(typeof(HashSet<>).MakeGenericType(type));
                    if (hs is not null)
                    {
                        foreach (var item in Flags)
                        {
                            if (selectedItems.Contains(item))
                            {
                                dynamic enumObject = item;
                                hs.Add(enumObject);
                            }

                        }

                        ValueChanged?.Invoke(this, new(hs));
                    }
                }
            }
        }
    }
}

public class HashSetValueChangedEventArgs(IEnumerable value) : EventArgs
{
    public IEnumerable Hashset { get; set; } = value;
}
