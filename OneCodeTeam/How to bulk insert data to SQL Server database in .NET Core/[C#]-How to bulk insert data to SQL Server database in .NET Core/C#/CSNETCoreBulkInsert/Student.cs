namespace CSNETCoreBulkInsert
{
    public class Student
    {
        int ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public Student() { }
        public Student(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }
}