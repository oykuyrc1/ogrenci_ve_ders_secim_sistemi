using System;
using System.Collections.Generic;

namespace ConsoleApp2
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("_");

            // Öğretim Görevlisi oluşturma
            Instructor instructor1 = new Instructor("ÖYKÜ YARICI", 1);
            Instructor instructor2 = new Instructor("DUYGU KIRAN", 2);
            Instructor instructor3 = new Instructor("AREN KESKİN", 3);

            List<Instructor> instructors = new List<Instructor> { instructor1, instructor2, instructor3 };

            // Ders oluşturma
            Course course1 = new Course("Matematik", 3, instructor1, null);
            Course course2 = new Course("Fizik", 4, instructor2, course1); // Fizik dersinin önkoşulu Matematik
            Course course3 = new Course("Kimya", 3, instructor3, course2); // Kimya dersinin önkoşulu Fizik.

            // Derslerin öğretim görevlilerine atanması
            instructor1.Courses.Add(course1);
            instructor2.Courses.Add(course2);
            instructor3.Courses.Add(course3);

            List<Course> courses = new List<Course> { course1, course2, course3 };

            // Öğrenci oluşturma
            Student newStudent = null;

            // Menü gösterme
            bool running = true;
            while (running)
            {
                DisplayMenu();

                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        // Ders seçme işlemi
                        Console.WriteLine("\nÖğretim Görevlisi Seçin:");
                        for (int i = 0; i < instructors.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {instructors[i].Name}");
                        }
                        int instructorChoice = int.Parse(Console.ReadLine()) - 1;

                        Instructor selectedInstructor = instructors[instructorChoice];

                        // Kullanıcıdan ders seçmesini istiyoruz
                        Console.WriteLine("\nSeçebileceğiniz Dersler:");
                        List<Course> availableCourses = selectedInstructor.Courses.Count > 0 ? selectedInstructor.Courses : courses;
                        for (int i = 0; i < availableCourses.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {availableCourses[i].Name} (Kredi: {availableCourses[i].Credit})");
                        }

                        Console.WriteLine("\nBir ders seçin:");
                        int courseChoice = int.Parse(Console.ReadLine()) - 1;
                        Course selectedCourse = availableCourses[courseChoice];

                        // Dersin önkoşulunu kontrol etme
                        if (selectedCourse.Prerequisite != null)
                        {
                            Console.WriteLine($"Bu dersin önkoşulu: {selectedCourse.Prerequisite.Name}");
                            Console.Write("Bu dersi almak için önkoşul dersini tamamladınız mı? (Evet/Hayır): ");
                            string prerequisiteCompleted = Console.ReadLine();
                            if (prerequisiteCompleted.ToLower() != "evet")
                            {
                                Console.WriteLine("Bu derse kaydolamazsınız, lütfen önkoşulu tamamlayın.");
                                continue; // Önkoşul tamamlanmamışsa tekrar menüye dönüyoruz
                            }
                        }

                        // Kullanıcıdan öğrenci bilgilerini istenir (ad soyad)
                        if (newStudent == null)
                        {
                            Console.WriteLine("\nÖğrenci Adı ve Soyadı Girin:");
                            string studentName = Console.ReadLine();
                            newStudent = new Student(studentName, 2001); // Öğrenci ID'sini otomatik olarak 2001 verdik
                        }

                        newStudent.EnrollInCourse(selectedCourse); // Öğrencinin seçilen derse kaydını yapıyoruz

                        // Seçilen öğretim görevlisinin derslerinin gösterilmesi
                        Console.WriteLine("\nSeçilen Öğretim Görevlisi Bilgileri:");
                        selectedInstructor.ShowInfo();

                        // Seçilen dersin bilgilerini gösterme
                        Console.WriteLine("\nSeçilen Ders Bilgileri:");
                        selectedCourse.ShowCourseInfo();

                        // Öğrencinin bilgilerini gösterme
                        Console.WriteLine("\nÖğrencinin Bilgileri:");
                        newStudent.ShowInfo();

                        break;

                    case 2:
                        // Öğrenci bilgisi gösterme
                        if (newStudent != null)
                        {
                            Console.WriteLine("\nÖğrencinin Bilgileri:");
                            newStudent.ShowInfo();
                        }
                        else
                        {
                            Console.WriteLine("\nÖğrenci kaydı bulunamadı.");
                        }
                        break;

                    case 3:
                        // Çıkış
                        Console.WriteLine("Çıkılıyor...");
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Geçersiz seçim! Lütfen tekrar deneyin.");
                        break;
                }
            }
        }

        // Menü ekranı
        public static void DisplayMenu()
        {
            Console.WriteLine("======== Ana Menü ========");
            Console.WriteLine("1. Ders Seç");
            Console.WriteLine("2. Öğrenci Bilgisi");
            Console.WriteLine("3. Çıkış");
            Console.WriteLine("=========================");
        }

        public interface IPerson
        {
            void ShowInfo();   // Bilgi gösterme işlemi
        }

        public abstract class Person : IPerson
        {
            public string Name { get; set; }
            public int ID { get; set; }

            public Person(string name, int id)
            {
                Name = name;
                ID = id;
            }

            // ShowInfo metodu her sınıfta özelleştirilecektir.
            public abstract void ShowInfo();
        }

        public class Student : Person
        {
            public List<Course> EnrolledCourses { get; set; }

            public Student(string name, int id) : base(name, id)
            {
                EnrolledCourses = new List<Course>();
            }

            // ShowInfo metodunu öğrencinin kayıtlı olduğu derslerle özelleştirdik
            public override void ShowInfo()
            {
                Console.WriteLine($"Öğrenci Adı: {Name}, ID: {ID}");
                Console.WriteLine("Kayıtlı Olduğu Dersler:");
                foreach (var course in EnrolledCourses)
                {
                    Console.WriteLine($"- {course.Name} (Kredi: {course.Credit})");
                }
            }

            // Öğrenci bir derse kaydolabilir
            public void EnrollInCourse(Course course)
            {
                EnrolledCourses.Add(course);
                course.AddStudent(this);  // Dersin öğrenci listesine ekler
            }
        }

        public class Instructor : Person
        {
            public List<Course> Courses { get; set; }

            public Instructor(string name, int id) : base(name, id)
            {
                Courses = new List<Course>();
            }

            // ShowInfo metodunu öğretim görevlisinin verdiği derslerle özelleştirdik
            public override void ShowInfo()
            {
                Console.WriteLine($"Öğretim Görevlisi Adı: {Name}, ID: {ID}");
                Console.WriteLine("Verdiği Dersler:");
                foreach (var course in Courses)
                {
                    Console.WriteLine($"- {course.Name} (Kredi: {course.Credit})");
                }
            }
        }

        public class Course
        {
            public string Name { get; set; }
            public int Credit { get; set; }
            public Instructor Instructor { get; set; }
            public List<Student> EnrolledStudents { get; set; }
            public Course Prerequisite { get; set; }  // dersin önkoşuludur
            public Dictionary<Student, int> StudentGrades { get; set; } // Öğrencilerin notlarını tutar

            public Course(string name, int credit, Instructor instructor, Course prerequisite)
            {
                Name = name;
                Credit = credit;
                Instructor = instructor;
                Prerequisite = prerequisite;
                EnrolledStudents = new List<Student>();
                StudentGrades = new Dictionary<Student, int>();
            }

            // Öğrenciyi derse ekler
            public void AddStudent(Student student)
            {
                EnrolledStudents.Add(student);
            }

            // Öğrencinin notunu atar
            public void AssignGradeToStudent(Student student, int grade)
            {
                if (EnrolledStudents.Contains(student))
                {
                    StudentGrades[student] = grade;
                    Console.WriteLine($"{student.Name} için {Name} dersine {grade} notu verildi.");
                }
                else
                {
                    Console.WriteLine("Bu öğrenci bu derse kayıtlı değil.");
                }
            }

            // Dersin öğrencilere ait notlarını gösterir
            public void ShowGrades()
            {
                Console.WriteLine($"Ders: {Name} - Notlar:");
                foreach (var studentGrade in StudentGrades)
                {
                    Console.WriteLine($"{studentGrade.Key.Name}: {studentGrade.Value}");
                }
            }

            internal void ShowCourseInfo()
            {
                throw new NotImplementedException();
            }
        }
    }
}
