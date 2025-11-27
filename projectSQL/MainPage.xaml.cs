using Microsoft.Maui.ApplicationModel.Communication;
using projectSQL.MVVM;
namespace projectSQL
{
    public partial class MainPage : ContentPage
    {
        private Model model = new Model();
        public MainPage()
        {
            InitializeComponent();
#if ANDROID
            model.limit = 15;
#else
            this.ToolbarItems.Clear();
#endif
            BindingContext = model;

        }

        private async void Modify_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var contact = button?.BindingContext as MVVM.Contact;
            if (contact == null) { return; }
            await Navigation.PushAsync(new ModifyContact(contact, model));
        }

        private async void Delete_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Delete Contact", "Potwierdź usunięcie", "OK", "Anuluj");
            if (!confirm) { return; }

            var button = sender as Button;
            var contact = button?.BindingContext as MVVM.Contact;
            if (contact == null) { return; }

            model.DeleteContact(contact);
        }

        private async void Add_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddContact(model));
        }

        private void PagLeft_Clicked(object sender, EventArgs e)
        {
            SearchEntry.Text = "";
            int number = Convert.ToInt32(PagNumber.Text);
            if (number <= 1) { return; }
            model.page -= 1;
            PagNumber.Text = model.page.ToString();
            model.SortContacts();
        }

        private void PagRight_Clicked(object sender, EventArgs e)
        {
            SearchEntry.Text = "";
            int number = Convert.ToInt32(PagNumber.Text);
            if (number >= model.pageMax) { return; }
            model.page += 1;
            PagNumber.Text = model.page.ToString();
            model.SortContacts();

        }

        private void PagNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchEntry.Text = "";
            if (!int.TryParse(PagNumber.Text, out int number)) return;

            if (number <= 1 && number >= model.pageMax) { return; }
            model.page = number;
            PagNumber.Text = model.page.ToString();
            model.SortContacts();

        }

        private async void MenuItemModify_Clicked(object sender, EventArgs e)
        {
            var button = sender as MenuItem;
            var contact = button?.BindingContext as MVVM.Contact;
            if (contact == null) { return; }
            await Navigation.PushAsync(new ModifyContact(contact, model));
        }
        private async void MenuItemDelete_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Delete Contact", "Potwierdź usunięcie", "OK", "Anuluj");
            if (!confirm) { return; }

            var button = sender as MenuItem;
            var contact = button?.BindingContext as MVVM.Contact;
            if (contact == null) { return; }

            model.DeleteContact(contact);
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input;
            if (SearchEntry == null)
                input = "";
            else 
                input = SearchEntry.Text;
            model.SortContacts(input);
        }

        private async void About_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InfoPage());
        }

        private void DeleteMany_Clicked(object sender, EventArgs e)
        {
            var selectedContacts = model.contacts.Where(c => c.IsChecked).ToList();
            foreach (var contact in selectedContacts)
            {
                model.DeleteContact(contact);
            }
            model.SortContacts();
        }

        private async void Help_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HelpPage());
        }
    }

}
