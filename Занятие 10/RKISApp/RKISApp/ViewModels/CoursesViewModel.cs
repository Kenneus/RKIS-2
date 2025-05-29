using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RKISApp.Models;
using RKISApp.Services;

namespace RKISApp.ViewModels
{
    public partial class CoursesViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _newCourseName;

        [ObservableProperty]
        private string _newCourseDescription;

        [ObservableProperty]
        private ObservableCollection<Course> _courses;

        [ObservableProperty]
        private Course? _editingCourse;

        private readonly DatabaseService _dbService;

        public ICommand CurrentAddSaveCourseCommand => EditingCourse is null ? AddCourseCommand : SaveCourseCommand;

        public CoursesViewModel(ObservableCollection<Course> courses, DatabaseService dbService)
        {
            _dbService = dbService;
            Courses = courses ?? _dbService.GetCourses(); // ���������� �������� ������ ����
            NewCourseName = string.Empty; // ���������� ��������
            NewCourseDescription = string.Empty; // ���������� ��������
        }

        [RelayCommand]
        private void AddCourse()
        {
            if (!string.IsNullOrWhiteSpace(NewCourseName)) // ���������� ��������
            {
                var course = new Course
                {
                    Id = Courses.Any() ? Courses.Max(c => c.Id) + 1 : 1, // ���������� ��������
                    Name = NewCourseName, // ���������� ��������
                    Description = NewCourseDescription // ���������� ��������
                };
                Courses.Add(course); // ���������� ��������
                _dbService.SaveCourse(course);
                NewCourseName = string.Empty; // ���������� ��������
                NewCourseDescription = string.Empty; // ���������� ��������
            }
        }

        [RelayCommand]
        private void RemoveCourse(Course course)
        {
            if (course != null)
            {
                Courses.Remove(course); // ���������� ��������
                _dbService.DeleteCourse(course.Id);
            }
        }

        [RelayCommand]
        private void EditCourse(Course course)
        {
            if (course != null)
            {
                if (EditingCourse != null && EditingCourse != course)
                {
                    SaveCourseChanges(EditingCourse);
                }
                EditingCourse = course; // ���������� ��������
                NewCourseName = course.Name ?? string.Empty; // ���������� ��������
                NewCourseDescription = course.Description ?? string.Empty; // ���������� ��������
            }
        }

        [RelayCommand]
        private void SaveCourse()
        {
            if (EditingCourse != null)
            {
                SaveCourseChanges(EditingCourse);
                _dbService.SaveCourse(EditingCourse);
                EditingCourse.IsEditing = false; // ���������� ��������
                EditingCourse = null; // ���������� ��������
                NewCourseName = string.Empty; // ���������� ��������
                NewCourseDescription = string.Empty; // ���������� ��������
            }
        }

        [RelayCommand]
        private void CancelEditCourse()
        {
            if (EditingCourse != null)
            {
                EditingCourse.IsEditing = false; // ���������� ��������
                EditingCourse = null; // ���������� ��������
                NewCourseName = string.Empty; // ���������� ��������
                NewCourseDescription = string.Empty; // ���������� ��������
            }
        }

        private void SaveCourseChanges(Course course)
        {
            if (course != null)
            {
                int index = Courses.IndexOf(course); // ���������� ��������
                if (index >= 0)
                {
                    Courses[index] = new Course
                    {
                        Id = course.Id,
                        Name = NewCourseName ?? string.Empty, // ���������� ��������
                        Description = NewCourseDescription ?? string.Empty, // ���������� ��������
                        IsEditing = false,
                        Students = course.Students
                    };
                }
            }
        }
    }
}