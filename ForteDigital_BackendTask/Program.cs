using ForteDigital_BackendTask;
using Newtonsoft.Json.Linq;
using System.Globalization;
using LINQtoCSV;
using System.Formats.Asn1;
using Microsoft.VisualBasic;
using System.Text.Json.Nodes;

class Program
{
    private static readonly HttpClient _httpClient = new HttpClient();

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


        //foreach (var item in GettingDataJson().Result)
        //    Console.WriteLine(item.Id + " " + item.Name + " " + item.Age + " " + item.Email + " " + item.InternshipStart + " " + item.InternshipEnd);


        foreach(var item in GettingDataCSV())
        {
            Console.WriteLine(item.Id + " " + item.Name + " " + item.Age + " " + item.Email + " " + item.InternshipStart + " " + item.InternshipEnd);
        }

       
        Thread.Sleep(4000);
    }

    private static async Task DownloadFile()
    {
        string url = @"https://fortedigital.github.io/Back-End-Internship-Task/interns.csv";

        var internsList = new List<InternClass>();

        byte[] fileBytes = await _httpClient.GetByteArrayAsync(url);
        File.WriteAllBytes("MyCsv.csv", fileBytes);
    }

    private static List<InternClass> GettingDataCSV()
    {
        Task.WaitAll(DownloadFile());

        var internsList = new List<InternClass>();

        var csvFileDescription = new CsvFileDescription
        {
            FirstLineHasColumnNames = true,
            IgnoreUnknownColumns = true,
            SeparatorChar = ',',
            UseFieldIndexForReadingData = false
        };

        var csvContext = new CsvContext();
        var interns = csvContext.Read<InternClass>("MyCsv.csv", csvFileDescription);

        foreach (var item in interns)
        {
            internsList.Add(new InternClass(
                    item.Id,
                    item.Name,
                    item.Age,
                    item.Email,
                    DateTime.ParseExact(item.InternshipStartCSV, "yyyy-MM-ddTHH:mm+00Z", new CultureInfo("en-US")).ToUniversalTime(),
                    DateTime.ParseExact(item.InternshipEndCSV, "yyyy-MM-ddTHH:mm+00Z", new CultureInfo("en-US")).ToUniversalTime()
                    ));
        }

        return internsList;

    }



    private static async Task<List<InternClass>> GettingDataJson()
    {
        string data = string.Empty;
        var internsList = new List<InternClass>();

        //Downloading data.
        data = await _httpClient.GetStringAsync(@"https://fortedigital.github.io/Back-End-Internship-Task/interns.json");


        //Parsing data into objects.
        var JSONData = JObject.Parse(data);
        //string person = (string)JSONData["interns"][2]["name"];

        //Using LINQ to operate on objects.
        var JSONObjects =
            from p in JSONData["interns"]
            select p;

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

        return internsList;
    }
}