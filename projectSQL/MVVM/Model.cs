using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
//using Foundation;
using Microsoft.Data.Sqlite;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace projectSQL.MVVM
{
    public class Contact : INotifyPropertyChanged
    {
        private string name;
        private string surname;
        private string phone;
        private bool isChecked;

        public string Id { get; set; }
        public bool IsChecked
        {
            get { return isChecked; }
            set {
                isChecked = value;
                OnPropertyChanged(nameof(isChecked));
            }
        }
        public string Name
        {
            get { return name; }
            set { 
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string Surname
        {
            get { return surname; }
            set
            {
                surname = value;
                OnPropertyChanged(nameof(Surname));
            }
        }
        public string Phone
        {
            get { return phone; }
            set
            {
                phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        public Contact(string id, string name, string surname, string phone)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Phone = phone;
        }
        public Contact(string name, string surname, string phone)
        {
            Name = name;
            Surname = surname;
            Phone = phone;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class Model : INotifyPropertyChanged
    {
        SqliteConnection connection;
        public int page = 1;
        public int pageMax = 999;
        public List<int> invalidVariables = new List<int>();
        public int limit = 5;
        private ObservableCollection<Contact> _contacts;
        public ObservableCollection<Contact> contacts
        {
            get { return _contacts; }
            set
            {
                _contacts = value;
                OnPropertyChanged(nameof(contacts));
            }
        }
        public Model()
        {
            string dbFileName = "contacts.db";
            string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "contacts.db"
            );
            connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            InitializeDatabase();
            SortContacts();
        }
        private void InitializeDatabase()
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText =
            @"
        CREATE TABLE IF NOT EXISTS contacts (
	        ""id""	INTEGER,
	        ""name""	TEXT,
	        ""surname""	TEXT,
	        ""phone""	TEXT,
	        PRIMARY KEY(""id"" AUTOINCREMENT)
        )
        ";
            cmd.ExecuteNonQuery();
        }
        //contacts = new ObservableCollection<Contact>
        //{
        //    new Contact("John", "Doe", "1234567890"),
        //    new Contact("Jane", "Smith", "0987654321"),
        //    new Contact("Alice", "Johnson", "5555555555"),
        //    new Contact("Bob", "Brown", "1112223333"),
        //    new Contact("Michael", "Davis", "2223334444"),
        //    new Contact("Emily", "Wilson", "3334445555"),
        //    new Contact("David", "Taylor", "4445556666"),
        //    new Contact("Sarah", "Anderson", "5556667777"),
        //    new Contact("James", "Thomas", "6667778888"),
        //    new Contact("Olivia", "Martinez", "7778889999"),
        //    new Contact("Daniel", "Harris", "8889990000"),
        //    new Contact("Sophia", "Clark", "9990001111"),
        //    new Contact("William", "Lewis", "1113335555"),
        //    new Contact("Ava", "Walker", "2224446666"),
        //    new Contact("Ethan", "Hall", "3335557777"),
        //    new Contact("Mia", "Allen", "4446668888"),
        //    new Contact("Alexander", "Young", "5557779999"),
        //    new Contact("Isabella", "Hernandez", "6668880000"),
        //    new Contact("Jacob", "King", "7779991111"),
        //    new Contact("Charlotte", "Wright", "8880002222")

        //};
        //public void SortContacts()
        //{
        //    var command = connection.CreateCommand();
        //    command.CommandText = $"SELECT id, name, surname, phone FROM contacts ORDER BY surname, name LIMIT {limit} OFFSET {(page-1) * limit}";

        //    contacts = new ObservableCollection<Contact>();
        //    var reader = command.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        contacts.Add(new Contact(reader.GetString(0).ToString(), reader.GetString(1).ToString(), reader.GetString(2).ToString(), reader.GetString(3).ToString()));
        //    }
        //    ChangePageMax();
        //}
        public void SortContacts(string input = "")
        {
            var command = connection.CreateCommand();
            if (input != "")
            {
                command.CommandText = $"SELECT id, name, surname, phone FROM contacts WHERE name LIKE $query or surname LIKE $query or phone LIKE $query ORDER BY surname, name";
                command.Parameters.AddWithValue("$query", $"%{input}%");
            }
            else
            {
                command.CommandText = $"SELECT id, name, surname, phone FROM contacts  ORDER BY surname, name LIMIT {limit} OFFSET {(page - 1) * limit}";
            }
            contacts = new ObservableCollection<Contact>();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                contacts.Add(new Contact(reader.GetString(0).ToString(), reader.GetString(1).ToString(), reader.GetString(2).ToString(), reader.GetString(3).ToString()));
            }
            ChangePageMax();
        }
        public async void DeleteContact(Contact contact)
        {
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM contacts WHERE id = $id";
            command.Parameters.AddWithValue("$id", contact.Id);
            command.ExecuteNonQuery();
            contacts.Remove(contact);
            SortContacts();

        }
        public bool VerifyValues(string name, string surname, string phone)
        {
            invalidVariables.Clear();
            if(!Regex.IsMatch(name, "^[A-ZŁŚĆŻŹÓŃ][a-ząćęłńóśźż]+$"))
            {
                invalidVariables.Add(1);
            }
            if(!Regex.IsMatch(surname, "^[A-ZŁŚĆŻŹÓŃ][a-ząćęłńóśźż]+$"))
            {
                invalidVariables.Add(2);
            }
            if(!Regex.IsMatch(phone, "^\\d{9,10}$"))
            {
                invalidVariables.Add(3);
            }
            if (invalidVariables.Count == 0)
                return true;
            return false;

        }
        public bool ModifyContact(Contact contact)
        {

            SqliteCommand command;
            if (!VerifyValues(contact.Name, contact.Surname, contact.Phone))
            {
                
                command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM contacts WHERE id = $id";
                command.Parameters.AddWithValue("$id", contact.Id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    contact.Name = reader.GetString(1).ToString();
                    contact.Surname = reader.GetString(2).ToString();
                    contact.Phone = reader.GetString(3).ToString();
                }
                return false;
            }
            command = connection.CreateCommand();
            command.CommandText = "UPDATE contacts SET name = $name, surname = $surname, phone = $phone WHERE id = $id";
            command.Parameters.AddWithValue("$name", contact.Name);
            command.Parameters.AddWithValue("$surname", contact.Surname);
            command.Parameters.AddWithValue("$phone", contact.Phone);
            command.Parameters.AddWithValue("$id", contact.Id);
            command.ExecuteNonQuery();

            SortContacts();
            return true;
        }
        public bool AddContact(Contact contact)
        {
            if (!VerifyValues(contact.Name, contact.Surname, contact.Phone))
            {
                return false;
            }
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO contacts(name, surname, phone) VALUES ($name, $surname, $phone)";
            command.Parameters.AddWithValue("$name", contact.Name);
            command.Parameters.AddWithValue("$surname", contact.Surname);
            command.Parameters.AddWithValue("$phone", contact.Phone);
            command.ExecuteNonQuery();

            int newId = 0;
            command = connection.CreateCommand();
            command.CommandText = "SELECT MAX(id) FROM contacts";
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                newId = reader.GetInt32(0); 
            }

            var newContact = new Contact(newId.ToString(), contact.Name, contact.Surname, contact.Phone);
            contacts.Add(newContact);

            SortContacts();
            return true;
        }
        public void ChangePageMax()
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM contacts";

            int totalContacts = 0;
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                totalContacts = reader.GetInt32(0) / limit;
            }
            pageMax = totalContacts;
            if (pageMax % limit > 0) { pageMax += 1; }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
