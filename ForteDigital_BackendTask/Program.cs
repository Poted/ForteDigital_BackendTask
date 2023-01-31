using ForteDigital_BackendTask;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Newtonsoft.Json;  //Nuget package for deserializing JSON in .NET;
using Newtonsoft.Json.Linq;

class Program
{
    public static void Main(string[] args)
    {
        /* 
         1: Create classes for taking data from CSV file (Coma Separeted Values) and Json file.
         2: Make an API downloader.
         3: Create class for separated person and make fields for them named id, name, age etc.
                id: integer,
                age: integer,
                name: string,
                email: string,
                internshipStart: date with time,
                internshipEnd: date with time.
        4:Create class for commands (?)
        

        Instructions: 
        1. Count Command
        2. Max Age Command
        3. Download and parse a file
        4: Errors
        5: Keep in mind that you might need to add support for other commands or filter options in the future. 
            Making sure your code is extensible will be a plus.
        6: Try not to over-engineer this task. Focus on functionality rather than input validation.

         */



        //Downloading data.
        string data = string.Empty;

        using (WebClient client = new WebClient())
        {
            string url = "https://fortedigital.github.io/Back-End-Internship-Task/interns.json";
            data = client.DownloadString(url);
            //Console.WriteLine(data);
        }

        //Parsing data into objects.
        var objects = JObject.Parse(data); 
        string person = (string)objects["interns"][2]["name"];
        Console.WriteLine(person);


        var postTitles =
            from p in objects["interns"]
            select (string)p["name"];

        foreach (var item in postTitles)
        {
            Console.WriteLine(item);
        }
        








        //InternClass intern = new InternClass(data);
        //intern._name = "Steven";
        //Console.WriteLine(intern._name);



    }
}