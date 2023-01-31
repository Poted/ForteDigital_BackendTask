using ForteDigital_BackendTask;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Newtonsoft.Json;  //Nuget package for deserializing JSON in .NET;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Formats.Tar;

class Program
{
    public static void Main(string[] args)
    {
        /* 
            Instructions: 
                1. Count Command
                2. Max Age Command
                3. Download and parse a file
                4: Errors
                5: Keep in mind that you might need to add support for other commands or filter options in the future. 
                Making sure your code is extensible will be a plus.
                6: Try not to over-engineer this task. Focus on functionality rather than input validation.
            
            Plan:
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
         */

        GettingData();

        Console.WriteLine("");

    }

    private static void GettingData()
    {
        string data = string.Empty;
        var internsList = new List<InternClass>();

        //Downloading data.
        using (WebClient client = new WebClient())
        {
            string url = "https://fortedigital.github.io/Back-End-Internship-Task/interns.json";
            data = client.DownloadString(url);
        }

        //Parsing data into objects.
        var JSONData = JObject.Parse(data);
        //string person = (string)JSONData["interns"][2]["name"];

        //Using LINQ to operate on objects.
        var JSONObjects =
            from p in JSONData["interns"]
            select p;
        //select (string)p["name"];

        //Saving data from JSON into an Intern objects
        foreach (var item in JSONObjects)
        {
            internsList.Add(new InternClass(
                    (int)item["id"],
                    (string)item["name"],
                    (int)item["age"],
                    (string)item["email"],
                    DateTime.ParseExact((string)item["internshipStart"], "yyyy-MM-ddTHH:mm+00Z", new CultureInfo("en-US")).ToUniversalTime(),
                    DateTime.ParseExact((string)item["internshipEnd"], "yyyy-MM-ddTHH:mm+00Z", new CultureInfo("en-US")).ToUniversalTime()
                    ));
        }



        foreach (var item in internsList)
            Console.WriteLine(item.Id + " " + item.Name + " " + item.Age + " " + item.Email + " " + item.InternshipStart + " " + item.InternshipEnd);
    }
}