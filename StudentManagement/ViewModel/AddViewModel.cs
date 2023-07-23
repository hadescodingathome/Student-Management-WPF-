using StudentManagement.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace StudentManagement.ViewModel
{
    public class AddViewModel : BaseViewModel
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private GenderEnum _gender;
        public GenderEnum Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        private DateTime _birth = new DateTime (1900,1,1);
        public DateTime Birth
        {
            get => _birth;
            set
            {
                _birth = value;
                OnPropertyChanged();
            }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }


        public event EventHandler<Student> StudentAdded;
        public ICommand SaveCommand { get; set; }

        
        
        public AddViewModel()
        {
            SaveCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                Student student = new Student()
                {
                    Name = Name,
                    Gender = Gender,
                    Address = Address,
                    Birthday = Birth,
                };
                Save();
                StudentAdded?.Invoke(this, student);
            });
        }

        private void Save()
        {
            MessageBox.Show($"Học sinh đã được lưu: \nName: {Name}\nGender: {Gender}\nBirth: {Birth.ToShortDateString()}\nAddress: {Address}");
        }

        private bool CanSave()
        {
            return true; 
        }
    }
}
