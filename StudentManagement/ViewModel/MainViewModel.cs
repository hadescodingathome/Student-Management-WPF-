// MainViewModel.cs
using Newtonsoft.Json;
using StudentManagement.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace StudentManagement.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<Student> _studentList;
        private ObservableCollection<Student> _changesList;
        private Student _selectedStudent;
        private string _searchName;
        public string SearchName
        {
            get => _searchName;
            set
            {
                _searchName = value;
                OnPropertyChanged("SearchName");
                PerformSearch();
            }
        }

        private string _searchAddress;
        public string SearchAddress
        {
            get => _searchAddress;
            set
            {
                _searchAddress = value;
                OnPropertyChanged("SearchAddress");
                PerformSearch();
            }
        }

        private string _searchGender;
        public string SearchGender
        {
            get => _searchGender;
            set
            {
                _searchGender = value;
                OnPropertyChanged("SearchGender");
                PerformSearch();
            }
        }

        public ObservableCollection<Student> StudentList
        {
            get => _studentList;
            set
            {
                _studentList = value;
                OnPropertyChanged("StudentList");
            }
        }

        public Student SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                OnPropertyChanged("SelectedStudent");
            }
        }

        public AddWindow AddWindow { get; set; }

        public ICommand Search { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public MainViewModel()
        {
            LoadStudents();
            _changesList = new ObservableCollection<Student>();
            InitializeCommands();
        }

        private void LoadStudents()
        {
            try
            {
                string filePath = @"C:\Users\Admin\OneDrive - Hanoi University of Science and Technology\Desktop\WPF\StudentManagement\StudentManagement\DataResource\StudentInfo.json";

                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    StudentList = JsonConvert.DeserializeObject<ObservableCollection<Student>>(json);
                }
                else
                {
                    StudentList = new ObservableCollection<Student>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải lên học sinh có ID: {ex.Message}");
                StudentList = new ObservableCollection<Student>();
            }
        }

        private void SaveStudents()
        {
            try
            {
                string json = JsonConvert.SerializeObject(StudentList, Formatting.Indented);
                File.WriteAllText(@"C:\Users\Admin\OneDrive - Hanoi University of Science and Technology\Desktop\WPF\StudentManagement\StudentManagement\DataResource\StudentInfo.json", json);
                Console.WriteLine("Thông tin học sinh được lưu thành công.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu học sinh: {ex.Message}");
            }
        }

        private void SaveChanges()
        {
            try
            {
                if (_changesList.Count > 0)
                {
                    foreach (var changedStudent in _changesList)
                    {
                        var existingStudent = StudentList.FirstOrDefault(s => s.Id == changedStudent.Id);
                        if (existingStudent != null)
                        {
                            existingStudent.Name = changedStudent.Name;
                            existingStudent.Gender = changedStudent.Gender;
                            existingStudent.Birthday = changedStudent.Birthday;
                            existingStudent.Address = changedStudent.Address;
                        }
                    }

                    _changesList.Clear();
                    SaveStudents();
                    MessageBox.Show("Các thay đổi đã được lưu thành công.");
                }
                else
                {
                    MessageBox.Show("Không có thay đổi để lưu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu các thay đổi: {ex.Message}");
            }
        }

        private void UpdateSTT()
        {
            for (int i = 0; i < StudentList.Count; i++)
            {
                StudentList[i].STT = i + 1;
            }
        }

        private void OnStudentAdded(object sender, Student newStudent)
        {
            if (StudentList == null)
            {
                StudentList = new ObservableCollection<Student>();
            }

            StudentList.Add(newStudent);
            _changesList.Add(newStudent);
            UpdateSTT();
        }

        private void EditStudent(Student student)
        {
            if (StudentList.Contains(student))
            {
                _changesList.Add(new Student
                {
                    Id = student.Id,
                    Name = student.Name,
                    Gender = student.Gender,
                    Birthday = student.Birthday,
                    Address = student.Address
                });

                MessageBox.Show($"Học sinh có mã: {student.Id} đang được chỉnh sửa.");
            }
            else
            {
                MessageBox.Show($"Không tìm thấy học sinh có mã: {student.Id}.");
            }
        }

        private void DeleteStudent(Student student)
        {
            if (StudentList.Contains(student))
            {
                student.IsDeleted = true;
                _changesList.Add(student);
                StudentList.Remove(student);
                UpdateSTT();
                MessageBox.Show($"Học sinh có mã: {student.Id} đã bị xoá.");

                if (StudentList.Count == 0)
                {
                    SelectedStudent = null;
                }
            }
            else
            {
                MessageBox.Show($"Không tìm thấy học sinh có mã: {student.Id}.");
            }
        }
        private void PerformSearch()
        {
            var filteredStudents = new ObservableCollection<Student>(_studentList);

            if (!string.IsNullOrEmpty(SearchName))
            {
                filteredStudents = new ObservableCollection<Student>(
                    filteredStudents.Where(student => student.Name.ToLower().Contains(SearchName.ToLower()))
                );
            }

            if (!string.IsNullOrEmpty(SearchAddress))
            {
                filteredStudents = new ObservableCollection<Student>(
                    filteredStudents.Where(student => student.Address.ToLower().Contains(SearchAddress.ToLower()))
                );
            }

            if (!string.IsNullOrEmpty(SearchGender))
            {
                var genderEnum = ConvertToGenderEnum(SearchGender);
                filteredStudents = new ObservableCollection<Student>(
                    filteredStudents.Where(student => student.Gender == genderEnum)
                );
            }

            StudentList.Clear();
            foreach (var student in filteredStudents)
            {
                StudentList.Add(student);
            }
        }



        private GenderEnum ConvertToGenderEnum(string genderString)
        {
            if (Enum.TryParse(typeof(GenderEnum), genderString, true, out object gender))
            {
                return (GenderEnum)gender;
            }

            // Return default gender (e.g., GenderEnum.Nam) in case of invalid input
            return GenderEnum.Nam;
        }
        private void InitializeCommands()
        {
            AddCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                AddWindow = new AddWindow();
                var addViewModel = new AddViewModel();
                addViewModel.StudentAdded += OnStudentAdded;
                AddWindow.DataContext = addViewModel;
                AddWindow.ShowDialog();
            });

            EditCommand = new RelayCommand<Student>((p) => p != null, (p) =>
            {
                EditStudent(p);
            });

            DeleteCommand = new RelayCommand<Student>((p) => p != null, (p) =>
            {
                DeleteStudent(p);
            });

            SaveCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                SaveChanges();
                SaveStudents();
            });
            Search = new RelayCommand<object>((p) => true, (p) =>
            {
                PerformSearch();
            });
        }
    }
}
