using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RKISApp.Models;
using RKISApp.ViewModels;

namespace RKISApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase _currentViewModel;

        public RKISViewModel RKISViewModel { get; }
        public CoursesViewModel CoursesViewModel { get; }

        public MainWindowViewModel()
        {
            CoursesViewModel = new CoursesViewModel();
            RKISViewModel = new RKISViewModel(CoursesViewModel.Courses);
            CurrentViewModel = RKISViewModel;

            // Синхронизация изменений в курсах
            CoursesViewModel.Courses.CollectionChanged += (s, e) =>
            {
                var newAvailableCourses = new ObservableCollection<Course>(CoursesViewModel.Courses);
                RKISViewModel.AvailableCourses = newAvailableCourses;

                // Обновляем курсы студентов, сохраняя их оригинальные данные
                foreach (var student in RKISViewModel.Students)
                {
                    var updatedCourses = new ObservableCollection<Course>();
                    foreach (var course in student.Courses)
                    {
                        // Если курс всё ещё существует в AvailableCourses, используем его
                        var matchingCourse = newAvailableCourses.FirstOrDefault(c => c.Name == course.Name && c.Description == course.Description);
                        updatedCourses.Add(matchingCourse ?? course); // Сохраняем оригинальный курс, если он удалён
                    }
                    student.Courses = updatedCourses;
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