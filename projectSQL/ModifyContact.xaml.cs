using projectSQL.MVVM;

using System.Text.RegularExpressions;
namespace projectSQL
{
	public partial class ModifyContact : ContentPage
	{
        projectSQL.MVVM.Contact contact;
        Model model;
        public ModifyContact(projectSQL.MVVM.Contact contact, Model model)
		{
			InitializeComponent();
            this.model = model;
            BindingContext = contact;
        }

        private async void Confirm_Clicked(object sender, EventArgs e)
        { 
            var contact = BindingContext as projectSQL.MVVM.Contact;
            if (contact != null) 
            {
                //model.ModifyContact(contact);
                if (model.ModifyContact(contact))
                {
                    await Navigation.PopAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("B³¹d", "Dane zosta³y podane niepoprawnie", "OK");
                    if (model.invalidVariables.Contains(1))
                    {
                        nameEntry.Background = Color.FromRgba(255, 0, 0, 80);
                    }
                    else
                    {
                        nameEntry.Background = Color.FromRgb(255, 255, 255);
                    }
                    if (model.invalidVariables.Contains(2))
                    {
                        surnameEntry.Background = Color.FromRgba(255, 0, 0, 80);
                    }
                    else
                    {
                        surnameEntry.Background = Color.FromRgb(255, 255, 255);
                    }
                    if (model.invalidVariables.Contains(3))
                    {
                        phoneEntry.Background = Color.FromRgba(255, 0, 0, 80);
                    }
                    else
                    {
                        phoneEntry.Background = Color.FromRgb(255, 255, 255);
                    }
                }
            }
        }
    }
}