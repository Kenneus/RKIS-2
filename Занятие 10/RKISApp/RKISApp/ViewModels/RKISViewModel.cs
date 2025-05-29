using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RKISApp.Models;
using System.Windows.Input;

namespace RKISApp.ViewModels
{
    public partial class RKISViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _newStudentName;

        [ObservableProperty]
        private ObservableCollection<Course> _availableCourses;

        [ObservableProperty]
        private ObservableCollection<Course> _selectedCourses;

        [ObservableProperty]
        private ObservableCollection<Student> _students;

        [ObservableProperty]
        private Student? _editingStudent;

        private int _nextId = 1;

        public RKISViewModel(ObservableCollection<Course> courses)
        {
            _availableCourses = courses ?? new ObservableCollection<Course>();
            _students = new ObservableCollection<Student>
            {
                new Student { Id = _nextId++, Name = "Алексей", Courses = new ObservableCollection<Course> { new Course { Name = "Математика", Description = "Основы математики" } } },
                new Student { Id = _nextId++, Name = "Мария", Courses = new ObservableCollection<Course> { new Course { Name = "Физика", Description = "Основы физики" } } }
            };
            _selectedCourses = new ObservableCollection<Course>();
            _newStudentName = string.Empty;
        }

        [RelayCommand]
        private void AddStudent()
        {
            if (!string.IsNullOrWhiteSpace(NewStudentName))
            {
                var student = new Student
                {
                    Id = _nextId++,
                    Name = NewStudentName,
                    Courses = new ObservableCollection<Course>(SelectedCourses)
                };
                Students.Add(student);
                NewStudentName = string.Empty;
                SelectedCourses.Clear();
            }
        }

        [RelayCommand]
        private void RemoveStudent(Student student)
        {
            Students.Remove(student);
        }

        [RelayCommand]
        private void EditStudent(Student student)
        {
            if (student != null)
            {
                if (EditingStudent != null && EditingStudent != student)
                {
                    SaveStudentChanges(EditingStudent);
                }
                EditingStudent = student;
                NewStudentName = student.Name ?? string.Empty;
                SelectedCourses = new ObservableCollection<Course>(student.Courses ?? new ObservableCollection<Course>());
                student.IsEditing = true;
            }
        }

        [RelayCommand]
        private void SaveStudent()
        {
            if (EditingStudent != null)
            {
                SaveStudentChanges(EditingStudent);
                EditingStudent.IsEditing = false;
                EditingStudent = null;
                NewStudentName = string.Empty;
                SelectedCourses.Clear();
            }
        }

        [RelayCommand]
        private void CancelEdit()
        {
            if (EditingStudent != null)
            {
                EditingStudent.IsEditing = false;
                EditingStudent = null;
                NewStudentName = string.Empty;
                SelectedCourses.Clear();
            }
        }

        public ICommand CurrentAddSaveCommand => EditingStudent == null ? AddStudentCommand : SaveStudentCommand;

        private void SaveStudentChanges(Student student)
        {
            if (student != null)
            {
                student.Name = NewStudentName;
                student.Courses = new ObservableCollection<Course>(SelectedCourses);
            }
        }

        public void ExecuteEditStudent(Student student)
        {
            EditStudentCommand.Execute(student);
        }
    }
}