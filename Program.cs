using System;
using System.Collections.Generic;
using System.Linq;

// ───────────────────────────────────────────────
//  STUDENT TRACKING SYSTEM — Mini Project (C#/.NET)
// ───────────────────────────────────────────────

class Student
{
    public int    Id         { get; set; }
    public string Name       { get; set; } = "";
    public string RollNo     { get; set; } = "";
    public string Class      { get; set; } = "";
    public string Section    { get; set; } = "";
    public string Contact    { get; set; } = "";
    public Dictionary<string, double> Marks { get; set; } = new();
    public string Attendance { get; set; } = "0/0";  // Present/Total
    public int    Present    { get; set; } = 0;
    public int    TotalDays  { get; set; } = 0;
    public string EnrolledOn { get; set; } = "";
}

class StudentTracker
{
    static List<Student> students = new();
    static int nextId = 1;

    static readonly string[] Subjects = { "Maths", "Science", "English", "Hindi", "Computer" };

    static void Main()
    {
        Console.Title = "Student Tracking System";
        while (true)
        {
            ShowMenu();
            string choice = Console.ReadLine()?.Trim() ?? "";
            switch (choice)
            {
                case "1": AddStudent();          break;
                case "2": ViewAllStudents();     break;
                case "3": SearchStudent();       break;
                case "4": UpdateStudent();       break;
                case "5": AddMarks();            break;
                case "6": ViewReportCard();      break;
                case "7": MarkAttendance();      break;
                case "8": ViewAttendance();      break;
                case "9": DeleteStudent();       break;
                case "0": Exit();                return;
                default : Warn("  Invalid option. Try again."); break;
            }
        }
    }

    // ─────────────────────────────────────────────
    //  MENU
    // ─────────────────────────────────────────────
    static void ShowMenu()
    {
        Console.Clear();
        Header("   STUDENT TRACKING SYSTEM   ");
        Console.WriteLine("  [1] Add New Student");
        Console.WriteLine("  [2] View All Students");
        Console.WriteLine("  [3] Search Student");
        Console.WriteLine("  [4] Update Student Info");
        Console.WriteLine("  [5] Add / Update Marks");
        Console.WriteLine("  [6] View Report Card");
        Console.WriteLine("  [7] Mark Attendance");
        Console.WriteLine("  [8] View Attendance Summary");
        Console.WriteLine("  [9] Delete Student");
        Console.WriteLine("  [0] Exit");
        Divider();
        Prompt("  Enter choice");
    }

    // ─────────────────────────────────────────────
    //  1. ADD STUDENT
    // ─────────────────────────────────────────────
    static void AddStudent()
    {
        Console.Clear();
        Header("  ADD NEW STUDENT  ");

        Prompt("  Student Name");
        string name = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(name)) { Warn("  Name cannot be empty."); Pause(); return; }

        Prompt("  Roll Number");
        string roll = Console.ReadLine()?.Trim().ToUpper() ?? "";
        if (students.Any(s => s.RollNo == roll)) { Warn("  Roll number already exists!"); Pause(); return; }

        Prompt("  Class (e.g. 10, 11, 12)");
        string cls = Console.ReadLine()?.Trim() ?? "";

        Prompt("  Section (e.g. A, B, C)");
        string sec = Console.ReadLine()?.Trim().ToUpper() ?? "";

        Prompt("  Parent Contact Number");
        string contact = Console.ReadLine()?.Trim() ?? "";

        var student = new Student
        {
            Id         = nextId++,
            Name       = name,
            RollNo     = roll,
            Class      = cls,
            Section    = sec,
            Contact    = contact,
            EnrolledOn = DateTime.Now.ToString("dd-MM-yyyy")
        };

        // Initialize marks for all subjects
        foreach (var sub in Subjects)
            student.Marks[sub] = -1; // -1 = not entered

        students.Add(student);
        Success($"\n  Student '{name}' added successfully! ID: {student.Id:D4}");
        Pause();
    }

    // ─────────────────────────────────────────────
    //  2. VIEW ALL STUDENTS
    // ─────────────────────────────────────────────
    static void ViewAllStudents()
    {
        Console.Clear();
        Header("  ALL STUDENTS  ");

        if (students.Count == 0) { Warn("  No students found."); Pause(); return; }

        Console.WriteLine($"  {"ID",-5} {"Name",-20} {"Roll No",-10} {"Class",-6} {"Sec",-5} {"Contact",-13} {"Enrolled On"}");
        Divider();
        foreach (var s in students)
            Console.WriteLine($"  {s.Id,-5} {s.Name,-20} {s.RollNo,-10} {s.Class,-6} {s.Section,-5} {s.Contact,-13} {s.EnrolledOn}");

        Divider();
        Console.WriteLine($"  Total Students: {students.Count}");
        Pause();
    }

    // ─────────────────────────────────────────────
    //  3. SEARCH STUDENT
    // ─────────────────────────────────────────────
    static void SearchStudent()
    {
        Console.Clear();
        Header("  SEARCH STUDENT  ");
        Console.WriteLine("  Search by: [1] Name   [2] Roll No   [3] Class");
        Prompt("  Choice");
        string opt = Console.ReadLine()?.Trim() ?? "";

        List<Student> results = new();

        if (opt == "1")
        {
            Prompt("  Enter Name");
            string q = Console.ReadLine()?.Trim().ToLower() ?? "";
            results = students.FindAll(s => s.Name.ToLower().Contains(q));
        }
        else if (opt == "2")
        {
            Prompt("  Enter Roll No");
            string q = Console.ReadLine()?.Trim().ToUpper() ?? "";
            results = students.FindAll(s => s.RollNo.Contains(q));
        }
        else if (opt == "3")
        {
            Prompt("  Enter Class");
            string q = Console.ReadLine()?.Trim() ?? "";
            results = students.FindAll(s => s.Class == q);
        }
        else { Warn("  Invalid option."); Pause(); return; }

        if (results.Count == 0) { Warn("  No matching students found."); Pause(); return; }

        Console.WriteLine($"\n  {"ID",-5} {"Name",-20} {"Roll No",-10} {"Class",-6} {"Sec",-5} {"Contact"}");
        Divider();
        foreach (var s in results)
            Console.WriteLine($"  {s.Id,-5} {s.Name,-20} {s.RollNo,-10} {s.Class,-6} {s.Section,-5} {s.Contact}");

        Pause();
    }

    // ─────────────────────────────────────────────
    //  4. UPDATE STUDENT INFO
    // ─────────────────────────────────────────────
    static void UpdateStudent()
    {
        Console.Clear();
        Header("  UPDATE STUDENT INFO  ");

        var s = FindStudentById();
        if (s == null) { Pause(); return; }

        Console.WriteLine($"\n  Current Info: {s.Name} | Roll: {s.RollNo} | Class: {s.Class}{s.Section} | Contact: {s.Contact}");
        Console.WriteLine("  (Press Enter to keep current value)\n");

        Prompt("  New Name");
        string name = Console.ReadLine()?.Trim() ?? "";
        if (!string.IsNullOrEmpty(name)) s.Name = name;

        Prompt("  New Class");
        string cls = Console.ReadLine()?.Trim() ?? "";
        if (!string.IsNullOrEmpty(cls)) s.Class = cls;

        Prompt("  New Section");
        string sec = Console.ReadLine()?.Trim().ToUpper() ?? "";
        if (!string.IsNullOrEmpty(sec)) s.Section = sec;

        Prompt("  New Contact");
        string contact = Console.ReadLine()?.Trim() ?? "";
        if (!string.IsNullOrEmpty(contact)) s.Contact = contact;

        Success("  Student info updated successfully!");
        Pause();
    }

    // ─────────────────────────────────────────────
    //  5. ADD / UPDATE MARKS
    // ─────────────────────────────────────────────
    static void AddMarks()
    {
        Console.Clear();
        Header("  ADD / UPDATE MARKS  ");

        var s = FindStudentById();
        if (s == null) { Pause(); return; }

        Console.WriteLine($"\n  Entering marks for: {s.Name} (Roll: {s.RollNo})");
        Console.WriteLine($"  Maximum Marks per subject: 100\n");

        foreach (var sub in Subjects)
        {
            string current = s.Marks[sub] >= 0 ? $" (current: {s.Marks[sub]})" : "";
            Prompt($"  {sub}{current}");
            string input = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(input)) continue;
            if (double.TryParse(input, out double mark) && mark >= 0 && mark <= 100)
                s.Marks[sub] = mark;
            else
                Warn($"  Invalid marks for {sub}, skipped.");
        }

        Success("\n  Marks saved successfully!");
        Pause();
    }

    // ─────────────────────────────────────────────
    //  6. VIEW REPORT CARD
    // ─────────────────────────────────────────────
    static void ViewReportCard()
    {
        Console.Clear();
        Header("  REPORT CARD  ");

        var s = FindStudentById();
        if (s == null) { Pause(); return; }

        Console.Clear();
        Header("  STUDENT REPORT CARD  ");
        Console.WriteLine($"  Student Name : {s.Name}");
        Console.WriteLine($"  Roll Number  : {s.RollNo}");
        Console.WriteLine($"  Class        : {s.Class} - {s.Section}");
        Console.WriteLine($"  Contact      : {s.Contact}");
        Divider();

        double total = 0;
        int    count = 0;

        Console.WriteLine($"  {"Subject",-25} {"Marks",8} {"Out Of",8} {"Grade",8}");
        Divider();

        foreach (var sub in Subjects)
        {
            double m = s.Marks[sub];
            string marksStr = m >= 0 ? m.ToString("F0") : "N/A";
            string grade    = m >= 0 ? GetGrade(m) : "-";
            Console.WriteLine($"  {sub,-25} {marksStr,8} {"100",8} {grade,8}");
            if (m >= 0) { total += m; count++; }
        }

        Divider();

        if (count > 0)
        {
            double percentage = total / (count * 100) * 100;
            double avg        = total / count;
            string result     = percentage >= 33 ? "PASS" : "FAIL";
            string overallG   = GetGrade(avg);

            Console.WriteLine($"  {"Total Marks",-25} {total,8} {count * 100,8}");
            Console.WriteLine($"  {"Percentage",-25} {percentage,7:F2}%");
            Console.WriteLine($"  {"Overall Grade",-25} {overallG,8}");

            Console.Write($"\n  Result: ");
            if (result == "PASS") Success("PASS ✓");
            else Warn("FAIL ✗");

            // Attendance summary inline
            double attPct = s.TotalDays > 0 ? (double)s.Present / s.TotalDays * 100 : 0;
            Console.WriteLine($"  Attendance   : {s.Present}/{s.TotalDays} days ({attPct:F1}%)");
        }
        else
        {
            Warn("  No marks entered yet.");
        }

        Divider();
        Pause();
    }

    // ─────────────────────────────────────────────
    //  7. MARK ATTENDANCE
    // ─────────────────────────────────────────────
    static void MarkAttendance()
    {
        Console.Clear();
        Header("  MARK ATTENDANCE  ");

        if (students.Count == 0) { Warn("  No students found."); Pause(); return; }

        Console.WriteLine($"  Date: {DateTime.Now:dd-MM-yyyy}\n");
        Console.WriteLine("  Mark each student as Present (P) or Absent (A):\n");

        foreach (var s in students)
        {
            Console.Write($"  {s.Name,-20} (Roll: {s.RollNo}) [P/A]: ");
            string input = Console.ReadLine()?.Trim().ToUpper() ?? "";
            s.TotalDays++;
            if (input == "P") { s.Present++; Success("    Marked: Present"); }
            else               { Warn("    Marked: Absent"); }
        }

        Success("\n  Attendance marked for all students!");
        Pause();
    }

    // ─────────────────────────────────────────────
    //  8. VIEW ATTENDANCE SUMMARY
    // ─────────────────────────────────────────────
    static void ViewAttendance()
    {
        Console.Clear();
        Header("  ATTENDANCE SUMMARY  ");

        if (students.Count == 0) { Warn("  No students found."); Pause(); return; }

        Console.WriteLine($"  {"ID",-5} {"Name",-20} {"Roll No",-10} {"Present",-9} {"Total",-7} {"Percent",-9} {"Status"}");
        Divider();

        foreach (var s in students)
        {
            double pct    = s.TotalDays > 0 ? (double)s.Present / s.TotalDays * 100 : 0;
            string status = pct >= 75 ? "OK" : pct == 0 ? "No Data" : "LOW";
            Console.Write($"  {s.Id,-5} {s.Name,-20} {s.RollNo,-10} {s.Present,-9} {s.TotalDays,-7} {pct,-8:F1}% ");
            if (status == "OK")      Success(status);
            else if (status == "LOW") Warn(status);
            else                     Console.WriteLine(status);
        }

        Divider();
        Console.WriteLine("  * Students below 75% attendance are marked LOW");
        Pause();
    }

    // ─────────────────────────────────────────────
    //  9. DELETE STUDENT
    // ─────────────────────────────────────────────
    static void DeleteStudent()
    {
        Console.Clear();
        Header("  DELETE STUDENT  ");

        var s = FindStudentById();
        if (s == null) { Pause(); return; }

        Console.WriteLine($"\n  Found: [{s.Id}] {s.Name} | Roll: {s.RollNo} | Class: {s.Class}-{s.Section}");
        Prompt("  Confirm delete? (Y/N)");
        if (Console.ReadLine()?.Trim().ToUpper() == "Y")
        {
            students.Remove(s);
            Success("  Student record deleted successfully.");
        }
        else Warn("  Deletion cancelled.");
        Pause();
    }

    // ─────────────────────────────────────────────
    //  EXIT
    // ─────────────────────────────────────────────
    static void Exit()
    {
        Console.Clear();
        Header("  THANK YOU — STUDENT TRACKING SYSTEM  ");
        Console.WriteLine($"\n  Total Students Enrolled : {students.Count}");
        Console.WriteLine("  Exiting system...\n");
    }

    // ─────────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────────
    static Student? FindStudentById()
    {
        Prompt("  Enter Student ID");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Warn("  Invalid ID."); return null; }
        var s = students.Find(x => x.Id == id);
        if (s == null) Warn("  Student not found.");
        return s;
    }

    static string GetGrade(double marks) => marks switch
    {
        >= 90 => "A+",
        >= 80 => "A",
        >= 70 => "B+",
        >= 60 => "B",
        >= 50 => "C",
        >= 33 => "D",
        _     => "F"
    };

    static void Header(string text) { Divider(); Console.WriteLine(text); Divider(); }
    static void Divider()           => Console.WriteLine("  " + new string('-', 65));
    static void Prompt(string msg)  => Console.Write($"{msg}: ");
    static void Warn(string msg)    { Console.ForegroundColor = ConsoleColor.Red;    Console.WriteLine(msg); Console.ResetColor(); }
    static void Success(string msg) { Console.ForegroundColor = ConsoleColor.Green;  Console.WriteLine(msg); Console.ResetColor(); }
    static void Pause()             { Console.WriteLine("\n  Press any key to continue..."); Console.ReadKey(); }
}
