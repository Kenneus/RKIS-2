using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RKISApp.Models;
using RKISApp.Services;

namespace RKISApp.ViewModels
{
    public partial class RKISViewModel : ViewModelBase, INotifyPropertyChanged
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

        [ObservableProperty]
        private string _filterText = string.Empty;

        private ObservableCollection<Student> _allStudents;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;

        private int _nextId;
        private readonly DatabaseService _dbService;

        public ICommand CurrentAddSaveCommand => EditingStudent is null ? AddStudentCommand : SaveStudentCommand;

        public RKISViewModel(ObservableCollection<Course> courses, DatabaseService dbService)
        {
            _dbService = dbService;
            AvailableCourses = courses ?? new ObservableCollection<Course>(); // Используем свойство
            _allStudents = _dbService.GetStudents();
            Students = new ObservableCollection<Student>(_allStudents); // Используем свойство
            SelectedCourses = new ObservableCollection<Course>(); // Используем свойство
            NewStudentName = string.Empty; // Используем свойство
            FilterText = string.Empty; // Используем свойство
            _nextId = _allStudents.Any() ? _allStudents.Max(s => s.Id) + 1 : 1;
        }

        [RelayCommand]
        private async Task UploadPhoto()
        {
            if (EditingStudent == null) return;

            var window = new Window();
            var filePickerOptions = new FilePickerOpenOptions
            {
                Title = "Выберите фото",
                AllowMultiple = false,
                FileTypeFilter = new[] { new FilePickerFileType("Изображения") { Patterns = new[] { "*.jpg", "*.jpeg", "*.png" } } }
            };

            var files = await window.StorageProvider.OpenFilePickerAsync(filePickerOptions);
            if (files.Any())
            {
                var sourcePath = files[0].Path.LocalPath;
                var fileName = $"student_{EditingStudent.Id}.jpg"; // Используем свойство
                var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", fileName);

                File.Copy(sourcePath, destinationPath, true);
                EditingStudent.PhotoPath = Path.Combine("Images", fileName); // Используем свойство
                _dbService.SaveStudent(EditingStudent); // Используем свойство
            }
        }

        [RelayCommand]
        private void AddStudent()
        {
            if (!string.IsNullOrWhiteSpace(NewStudentName) && !Students.Any(s => s.Name == NewStudentName && s.Id != _nextId)) // Используем свойство
            {
                var student = new Student
                {
                    Id = _nextId++,
                    Name = NewStudentName, // Используем свойство
                    Courses = new ObservableCollection<Course>(SelectedCourses ?? new ObservableCollection<Course>()), // Используем свойство
                    PhotoPath = null
                };
                _allStudents.Add(student);
                Students.Add(student); // Используем свойство
                _dbService.SaveStudent(student);
                ApplyFilterAndSort();
                NewStudentName = string.Empty; // Используем свойство
                SelectedCourses?.Clear(); // Используем свойство
            }
        }

        [RelayCommand]
        private void RemoveStudent(Student student)
        {
            if (student != null)
            {
                _allStudents.Remove(student);
                Students.Remove(student); // Используем свойство
                _dbService.DeleteStudent(student.Id);
                ApplyFilterAndSort();
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
                EditingStudent = student; // Используем свойство
                NewStudentName = student.Name ?? string.Empty; // Используем свойство
                SelectedCourses = new ObservableCollection<Course>(student.Courses ?? new ObservableCollection<Course>()); // Используем свойство
                if (student.Courses != null)
                {
                    foreach (var course in student.Courses)
                    {
                        if (course != null && !AvailableCourses.Any(c => c.Name == course.Name && c.Description == course.Description)) // Используем свойство
                        {
                            AvailableCourses.Add(course); // Используем свойство
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
                _dbService.SaveStudent(EditingStudent); // Используем свойство
                EditingStudent.IsEditing = false; // Используем свойство
                EditingStudent = null; // Используем свойство
                NewStudentName = string.Empty; // Используем свойство
                SelectedCourses?.Clear(); // Используем свойство
                ApplyFilterAndSort();
            }
        }

        [RelayCommand]
        private void CancelEdit()
        {
            if (EditingStudent != null)
            {
                EditingStudent.IsEditing = false; // Используем свойство
                EditingStudent = null; // Используем свойство
                NewStudentName = string.Empty; // Используем свойство
                SelectedCourses?.Clear(); // Используем свойство
                ApplyFilterAndSort();
            }
        }

        [RelayCommand]
        private void SortStudents()
        {
            _sortDirection = _sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            ApplyFilterAndSort();
        }

        private void SaveStudentChanges(Student student)
        {
            if (student != null)
            {
                int index = _allStudents.IndexOf(student);
                if (index >= 0)
                {
                    _allStudents[index] = new Student
                    {
                        Id = student.Id,
                        Name = NewStudentName ?? string.Empty, // Используем свойство
                        Courses = new ObservableCollection<Course>(SelectedCourses ?? student.Courses ?? new ObservableCollection<Course>()), // Используем свойство
                        IsEditing = false,
                        PhotoPath = student.PhotoPath
                    };
                    Students[index] = _allStudents[index]; // Используем свойство
                }
            }
        }

        private void ApplyFilterAndSort()
        {
            var filteredStudents = _allStudents
                .Where(s => string.IsNullOrEmpty(FilterText) || (s.Name ?? string.Empty).Contains(FilterText, StringComparison.OrdinalIgnoreCase))
                .ToList();
            var sortedStudents = _sortDirection == ListSortDirection.Ascending
                ? filteredStudents.OrderBy(s => s.Name ?? string.Empty).ToList()
                : filteredStudents.OrderByDescending(s => s.Name ?? string.Empty).ToList();
            Students = new ObservableCollection<Student>(sortedStudents); // Используем свойство
        }

        partial void OnFilterTextChanged(string value)
        {
            ApplyFilterAndSort();
        }
    }
}