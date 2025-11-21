namespace projectSQL;

public partial class InfoPage : ContentPage
{
	public InfoPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
		await Navigation.PopAsync();
    }
}