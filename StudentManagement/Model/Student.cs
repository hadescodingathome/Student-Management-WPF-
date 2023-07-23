// Student.cs
using StudentManagement.ViewModel;
using System;

namespace StudentManagement.Model
{
    public class Student : BaseViewModel
    {
        private static int _currentId = 1;

        public Student()
        {
            Id = _currentId;
            _currentId++;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public GenderEnum Gender { get; set; }
        public DateTime Birthday { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }

        private int _stt;
        public int STT
        {
            get => _stt;
            set
            {
                _stt = value;
                OnPropertyChanged("STT");
            }
        }
    }
}
