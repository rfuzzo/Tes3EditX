using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
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

        this.PropertyChanged += RecordFieldTemplate_PropertyChanged;   

        
    }

    private void RecordFieldTemplate_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName ==  nameof(ViewModel))
        {

        }
    }

    
    
    public RecordFieldViewModel ViewModel => (RecordFieldViewModel)DataContext;



    //public static readonly DependencyProperty RecordFieldProperty = DependencyProperty.Register(
    //      nameof(RecordField),
    //      typeof(RecordFieldViewModel),
    //      typeof(RecordFieldTemplate),
    //      new PropertyMetadata(null)
    //    );

    //public RecordFieldViewModel RecordField
    //{
    //    get { return (RecordFieldViewModel)GetValue(RecordFieldProperty); }
    //    set { SetValue(RecordFieldProperty, value); }
    //}

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // notify the record/plugin that something changed
        //if (sender is TextBox textBox && RecordField.WrappedField is string str)
        //{
        //    string text = textBox.Text;
        //    if (!string.IsNullOrEmpty(text) && str.Trim('\0') != text)
        //    {
        //        RecordField.WrappedField = text;
        //    }
        //}
    }

    private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {

    }

    public static void OnValueChanged()
    {

    }
}
