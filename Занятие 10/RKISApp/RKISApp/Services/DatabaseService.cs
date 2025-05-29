using System.Collections.ObjectModel;
using System.Linq; // Добавлено для методов LINQ
using Microsoft.EntityFrameworkCore;
using RKISApp.Data;
using RKISApp.Models;

namespace RKISApp.Services
{
    public class DatabaseService
    {
        private readonly AppDbContext _context;

        public DatabaseService()
        {
            _context = new AppDbContext();
            _context.Database.EnsureCreated(); // Создаёт БД, если её нет
        }

        public ObservableCollection<Student> GetStudents()
        {
            return new ObservableCollection<Student>(_context.Students.Include(s => s.Courses).ToList());
        }

        public ObservableCollection<Course> GetCourses()
        {
            return new ObservableCollection<Course>(_context.Courses.ToList());
        }

        public void SaveStudent(Student student)
        {
            var existingStudent = _context.Students.Include(s => s.Courses).FirstOrDefault(s => s.Id == student.Id);
            if (existingStudent != null)
            {
                _context.Entry(existingStudent).CurrentValues.SetValues(student);
                _context.SaveChanges();
            }
            else
            {
                _context.Students.Add(student);
                _context.SaveChanges();
            }
        }

        public void DeleteStudent(int id)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
        }

        public void SaveCourse(Course course)
        {
            var existingCourse = _context.Courses.FirstOrDefault(c => c.Id == course.Id);
            if (existingCourse != null)
            {
                _context.Entry(existingCourse).CurrentValues.SetValues(course);
                _context.SaveChanges();
            }
            else
            {
                _context.Courses.Add(course);
                _context.SaveChanges();
            }
        }

        public void DeleteCourse(int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
            }
        }
    }
}