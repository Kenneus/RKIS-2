using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RKISApp.Models;
using System.Windows.Input;

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

        public CoursesViewModel()
        {
            _courses = new ObservableCollection<Course>
            {
                new Course { Name = "Математика", Description = "Основы математики" },
                new Course { Name = "Физика", Description = "Основы физики" }
            };
            _newCourseName = string.Empty;
            _newCourseDescription = string.Empty;
        }

        [RelayCommand]
        private void AddCourse()
        {
            if (!string.IsNullOrWhiteSpace(NewCourseName) && !string.IsNullOrWhiteSpace(NewCourseDescription))
            {
                Courses.Add(new Course { Name = NewCourseName, Description = NewCourseDescription });
                NewCourseName = string.Empty;
                NewCourseDescription = string.Empty;
            }
        }

        [RelayCommand]
        private void RemoveCourse(Course course)
        {
            Courses.Remove(course);
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
                EditingCourse = course;
                NewCourseName = course.Name ?? string.Empty;
                NewCourseDescription = course.Description ?? string.Empty;
            }
        }

        [RelayCommand]
        private void SaveCourse()
        {
            if (EditingCourse != null)
            {
                EditingCourse.Name = NewCourseName;
                EditingCourse.Description = NewCourseDescription;
                EditingCourse.IsEditing = false;
                EditingCourse = null;
                NewCourseName = string.Empty;
                NewCourseDescription = string.Empty;
            }
        }

        [RelayCommand]
        private void CancelEditCourse()
        {
            if (EditingCourse != null)
            {
                EditingCourse.IsEditing = false;
                EditingCourse = null;
                NewCourseName = string.Empty;
                NewCourseDescription = string.Empty;
            }
        }

        public ICommand CurrentAddSaveCourseCommand => EditingCourse == null ? AddCourseCommand : SaveCourseCommand;

        private void SaveCourseChanges(Course course)
        {
            course.Name = NewCourseName;
            course.Description = NewCourseDescription;
            course.IsEditing = false;
        }
    }
}