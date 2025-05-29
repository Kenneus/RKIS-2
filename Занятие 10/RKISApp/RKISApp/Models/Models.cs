using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RKISApp.Models
{
    public partial class Student : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        private string? _name;

        [ObservableProperty]
        private ObservableCollection<Course>? _courses;

        [ObservableProperty]
        private bool _isEditing;

        [ObservableProperty]
        private string? _photoPath;

        public Student()
        {
            _name = string.Empty;
            _courses = new ObservableCollection<Course>();
            _photoPath = string.Empty;
        }
    }

    public partial class Course : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private bool _isEditing;

        public ObservableCollection<Student> Students { get; set; }

        public Course()
        {
            _name = string.Empty;
            _description = string.Empty;
            Students = new ObservableCollection<Student>();
            _isEditing = false;
        }
    }
}