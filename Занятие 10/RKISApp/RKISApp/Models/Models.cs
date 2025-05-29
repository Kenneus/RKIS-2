using System.Collections.ObjectModel;

namespace RKISApp.Models
{
    public class Student
    {
        public int Id { get; set; } // ”никальный идентификатор
        public string? Name { get; set; }
        public ObservableCollection<Course>? Courses { get; set; }
        public bool IsEditing { get; set; }
    }

    public class Course
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsEditing { get; set; }
    }
}