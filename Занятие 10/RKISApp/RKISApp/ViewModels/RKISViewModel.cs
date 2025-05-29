using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RKISApp.Models;

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

        // Свойство для переключения между командами "Добавить" и "Сохранить"
        public ICommand CurrentAddSaveCommand => EditingStudent is null ? AddStudentCommand : SaveStudentCommand;

        public RKISViewModel(ObservableCollection<Course> courses)
        {
            _availableCourses = courses ?? new ObservableCollection<Course>();
            _students = new ObservableCollection<Student>
            {
                new Student { Id = _nextId++, Name = "Алексей", Courses = new() { new Course { Name = "Математика", Description = "Основы математики" } } },
                new Student { Id = _nextId++, Name = "Мария", Courses = new() { new Course { Name = "Физика", Description = "Основы физики" } } }
            };
            _selectedCourses = new ObservableCollection<Course>();
            _newStudentName = string.Empty;
        }

        [RelayCommand]
        private void AddStudent()
        {
            if (!string.IsNullOrWhiteSpace(NewStudentName) && !Students.Any(s => s.Name == NewStudentName && s.Id != _nextId))
            {
                var student = new Student
                {
                    Id = _nextId++,
                    Name = NewStudentName,
                    Courses = new ObservableCollection<Course>(SelectedCourses ?? new ObservableCollection<Course>())
                };
                Students.Add(student);
                NewStudentName = string.Empty;
                SelectedCourses?.Clear();
            }
        }

        [RelayCommand]
        private void RemoveStudent(Student student)
        {
            if (student != null)
            {
                Students.Remove(student);
            }
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
                if (student.Courses != null)
                {
                    foreach (var course in student.Courses)
                    {
                        if (course != null && !AvailableCourses.Any(c => c.Name == course.Name && c.Description == course.Description))
                        {
                            AvailableCourses.Add(course);
                        }
                    }
                }
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
                SelectedCourses?.Clear();
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
                SelectedCourses?.Clear();
            }
        }

        private void SaveStudentChanges(Student student)
        {
            if (student != null)
            {
                int index = Students.IndexOf(student);
                if (index >= 0)
                {
                    Students[index] = new Student
                    {
                        Id = student.Id,
                        Name = NewStudentName ?? string.Empty,
                        Courses = new ObservableCollection<Course>(SelectedCourses ?? student.Courses ?? new ObservableCollection<Course>()),
                        IsEditing = false
                    };
                }
            }
        }

        public void ExecuteEditStudent(Student student)
        {
            EditStudentCommand.Execute(student);
        }
    }
}