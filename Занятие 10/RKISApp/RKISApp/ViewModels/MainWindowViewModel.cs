using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RKISApp.Models;
using RKISApp.Services;
using RKISApp.ViewModels;

namespace RKISApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase _currentViewModel;

        public RKISViewModel RKISViewModel { get; }
        public CoursesViewModel CoursesViewModel { get; }

        private readonly DatabaseService _dbService;

        public MainWindowViewModel()
        {
            _dbService = new DatabaseService();

            // Создаём папку Images, если она не существует
            string imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }

            CoursesViewModel = new CoursesViewModel(_dbService.GetCourses(), _dbService);
            RKISViewModel = new RKISViewModel(_dbService.GetCourses(), _dbService);
            CurrentViewModel = RKISViewModel;

            // Синхронизация изменений в курсах
            CoursesViewModel.Courses.CollectionChanged += (s, e) =>
            {
                var newAvailableCourses = new ObservableCollection<Course>(CoursesViewModel.Courses);
                RKISViewModel.AvailableCourses = newAvailableCourses;

                foreach (var student in RKISViewModel.Students)
                {
                    var updatedCourses = new ObservableCollection<Course>();
                    if (student != null && student.Courses != null) // Проверка на null
                    {
                        foreach (var course in student.Courses)
                        {
                            var matchingCourse = newAvailableCourses.FirstOrDefault(c => c.Name == course.Name && c.Description == course.Description);
                            updatedCourses.Add(matchingCourse ?? course);
                        }
                    }
                    student.Courses = updatedCourses.Any() ? updatedCourses : (student.Courses ?? new ObservableCollection<Course>());
                    _dbService.SaveStudent(student);
                }
            };
        }

        [RelayCommand]
        private void SwitchToStudents()
        {
            if (CurrentViewModel is RKISViewModel rvm && rvm.EditingStudent != null)
            {
                rvm.SaveStudentCommand.Execute(null);
            }
            CurrentViewModel = RKISViewModel;
        }

        [RelayCommand]
        private void SwitchToCourses()
        {
            if (CurrentViewModel is RKISViewModel rvm && rvm.EditingStudent != null)
            {
                rvm.SaveStudentCommand.Execute(null);
            }
            CurrentViewModel = CoursesViewModel;
        }
    }
}