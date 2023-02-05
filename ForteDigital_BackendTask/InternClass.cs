using LINQtoCSV;

namespace ForteDigital_BackendTask
{
    /*
       id: integer,
       age: integer,
       name: string,
       email: string,
       internshipStart: date with time,
       internshipEnd: date with time.
    */

    [Serializable]
    public class InternClass
    {
        [CsvColumn(Name = "interns/id", FieldIndex = 1)]
        public int Id { get; private set; }

        [CsvColumn(Name = "interns/name", FieldIndex = 3)]
        public string Name { get; private set; }

        [CsvColumn(Name = "interns/age", FieldIndex = 2)]
        public int Age { get; private set; }

        [CsvColumn(Name = "interns/email", FieldIndex = 4)]
        public string Email { get; private set; }

        public DateTime InternshipStart { get; private set; }

        public DateTime InternshipEnd { get; private set; }

        [CsvColumn(Name = "interns/internshipStart", FieldIndex = 5, OutputFormat = "yyyy-MM-ddTHH:mm:00Z")]
        public string InternshipStartCSV { get; private set; }

        [CsvColumn(Name = "interns/internshipEnd", FieldIndex = 6, OutputFormat = "yyyy-MM-ddTHH:mm:00Z")]
        public string InternshipEndCSV { get; private set; }

        //LINQ-to-CSV can't recognize date with time zone format so I created another ctor with date in string and it works well. (But I'm not sure why.)

        public InternClass()
        {

        }

        public InternClass(int id, string name, int age, string email, string internshipStart, string internshipEnd)
        {
            Id = id;
            Name = name;
            Age = age;
            Email = email;
            InternshipStartCSV = internshipStart;
            InternshipEndCSV = internshipEnd;
        }

        public InternClass(int id, string name, int age, string email, DateTime internshipStart, DateTime internshipEnd)
        {
            Id = id;
            Name = name;
            Age = age;
            Email = email;
            InternshipStart = internshipStart;
            InternshipEnd = internshipEnd;
        }

    }
}
