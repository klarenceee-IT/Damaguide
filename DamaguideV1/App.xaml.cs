using DamaguideV1.Pages;

namespace DamaguideV1;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // NavigationPage is needed because the app uses Navigation.PushAsync between pages.
        return new Window(new NavigationPage(new HomePage()));
    }
}