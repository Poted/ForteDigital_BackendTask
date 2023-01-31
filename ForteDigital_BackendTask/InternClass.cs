using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForteDigital_BackendTask
{
    internal class InternClass
    {
        /*
                id: integer,
                age: integer,
                name: string,
                email: string,
                internshipStart: date with time,
                internshipEnd: date with time.
         */


        public int id { get; set; }
        public string name { get; set; }
        public string _age { get; set; }
        public string _email { get; set; }   //Should be private.
        public DateTime _internshipStart { get; set; }
        public DateTime _internshipEnd { get; set; }
    }
}
