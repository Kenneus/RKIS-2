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
            AvailableCourses = courses ?? new ObservableCollection<Course>(); // ���������� ��������
            _allStudents = _dbService.GetStudents();
            Students = new ObservableCollection<Student>(_allStudents); // ���������� ��������
            SelectedCourses = new ObservableCollection<Course>(); // ���������� ��������
            NewStudentName = string.Empty; // ���������� ��������
            FilterText = string.Empty; // ���������� ��������
            _nextId = _allStudents.Any() ? _allStudents.Max(s => s.Id) + 1 : 1;
        }

        [RelayCommand]
        private async Task UploadPhoto()
        {
            if (EditingStudent == null) return;

            var window = new Window();
            var filePickerOptions = new FilePickerOpenOptions
            {
                Title = "�������� ����",
                AllowMultiple = false,
                FileTypeFilter = new[] { new FilePickerFileType("�����������") { Patterns = new[] { "*.jpg", "*.jpeg", "*.png" } } }
            };

            var files = await window.StorageProvider.OpenFilePickerAsync(filePickerOptions);
            if (files.Any())
            {
                var sourcePath = files[0].Path.LocalPath;
                var fileName = $"student_{EditingStudent.Id}.jpg"; // ���������� ��������
                var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", fileName);

                File.Copy(sourcePath, destinationPath, true);
                EditingStudent.PhotoPath = Path.Combine("Images", fileName); // ���������� ��������
                _dbService.SaveStudent(EditingStudent); // ���������� ��������
            }
        }

        [RelayCommand]
        private void AddStudent()
        {
            if (!string.IsNullOrWhiteSpace(NewStudentName) && !Students.Any(s => s.Name == NewStudentName && s.Id != _nextId)) // ���������� ��������
            {
                var student = new Student
                {
                    Id = _nextId++,
                    Name = NewStudentName, // ���������� ��������
                    Courses = new ObservableCollection<Course>(SelectedCourses ?? new ObservableCollection<Course>()), // ���������� ��������
                    PhotoPath = null
                };
                _allStudents.Add(student);
                Students.Add(student); // ���������� ��������
                _dbService.SaveStudent(student);
                ApplyFilterAndSort();
                NewStudentName = string.Empty; // ���������� ��������
                SelectedCourses?.Clear(); // ���������� ��������
            }
        }

        [RelayCommand]
        private void RemoveStudent(Student student)
        {
            if (student != null)
            {
                _allStudents.Remove(student);
                Students.Remove(student); // ���������� ��������
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
                EditingStudent = student; // ���������� ��������
                NewStudentName = student.Name ?? string.Empty; // ���������� ��������
                SelectedCourses = new ObservableCollection<Course>(student.Courses ?? new ObservableCollection<Course>()); // ���������� ��������
                if (student.Courses != null)
                {
                    foreach (var course in student.Courses)
                    {
                        if (course != null && !AvailableCourses.Any(c => c.Name == course.Name && c.Description == course.Description)) // ���������� ��������
                        {
                            AvailableCourses.Add(course); // ���������� ��������
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
                _dbService.SaveStudent(EditingStudent); // ���������� ��������
                EditingStudent.IsEditing = false; // ���������� ��������
                EditingStudent = null; // ���������� ��������
                NewStudentName = string.Empty; // ���������� ��������
                SelectedCourses?.Clear(); // ���������� ��������
                ApplyFilterAndSort();
            }
        }

        [RelayCommand]
        private void CancelEdit()
        {
            if (EditingStudent != null)
            {
                EditingStudent.IsEditing = false; // ���������� ��������
                EditingStudent = null; // ���������� ��������
                NewStudentName = string.Empty; // ���������� ��������
                SelectedCourses?.Clear(); // ���������� ��������
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
                        Name = NewStudentName ?? string.Empty, // ���������� ��������
                        Courses = new ObservableCollection<Course>(SelectedCourses ?? student.Courses ?? new ObservableCollection<Course>()), // ���������� ��������
                        IsEditing = false,
                        PhotoPath = student.PhotoPath
                    };
                    Students[index] = _allStudents[index]; // ���������� ��������
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
            Students = new ObservableCollection<Student>(sortedStudents); // ���������� ��������
        }

        partial void OnFilterTextChanged(string value)
        {
            ApplyFilterAndSort();
        }
    }
}