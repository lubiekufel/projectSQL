namespace projectSQL;

public partial class HelpPage : ContentPage
{
	public HelpPage()
	{
		InitializeComponent();
	}
    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}