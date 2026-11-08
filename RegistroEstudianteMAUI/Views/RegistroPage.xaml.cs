namespace RegistroEstudianteMAUI.Views;

public partial class RegistroPage : ContentPage
{
    public RegistroPage(ViewModels.RegistroViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void FechaHandlerChanged(object? sender, EventArgs e)
    {
#if WINDOWS
        if (sender is DatePicker { Handler.PlatformView: Microsoft.UI.Xaml.Controls.CalendarDatePicker native })
        {
            native.PlaceholderText = "Selecciona una fecha";
            native.VerticalContentAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center;
            native.HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch;
            native.HorizontalContentAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch;
        }
#endif
    }

    private void CarreraHandlerChanged(object? sender, EventArgs e)
    {
#if WINDOWS
        if (sender is Picker { Handler.PlatformView: Microsoft.UI.Xaml.Controls.ComboBox native } picker)
        {
            // MAUI maps Title to WinUI's Header, which creates a separate row
            // above the selection and leaves the glyph in the lower row.
            native.HeaderTemplate = null;
            native.Header = null;
            native.PlaceholderText = picker.Title;
            native.Height = 56;
            native.MinHeight = 56;
            native.VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch;
            native.VerticalContentAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center;
            native.Padding = new Microsoft.UI.Xaml.Thickness(12, 0, 32, 0);

        }
#endif
    }
}

