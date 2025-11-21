using projectSQL.MVVM;
using System.Threading.Tasks;

namespace projectSQL
{

	public partial class AddContact : ContentPage
	{
		Model model;
        public AddContact(Model model)
		{
            this.model = model;
			InitializeComponent();
        }

		private async void Confirm_Clicked(object sender, EventArgs e)
		{
			string name = nameEntry.Text;
            string surname = surnameEntry.Text;
            string phone = phoneEntry.Text;
            projectSQL.MVVM.Contact contact = new projectSQL.MVVM.Contact(name, surname, phone);
            if (model.AddContact(contact))
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

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}