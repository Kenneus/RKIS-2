using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using RKISApp.ViewModels;
using RKISApp.Models;

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
                RKISViewModel.AvailableCourses = new ObservableCollection<Course>(CoursesViewModel.Courses);
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