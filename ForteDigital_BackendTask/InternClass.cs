using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    internal class InternClass
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Age { get; private set; }
        public string Email { get; private set; }
        public DateTime InternshipStart { get; private set; }
        public DateTime InternshipEnd { get; private set; }

        public InternClass()
        {

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
