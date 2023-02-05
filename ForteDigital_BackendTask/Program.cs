using ForteDigital_BackendTask;
using Newtonsoft.Json.Linq;
using System.Globalization;
using LINQtoCSV;
using System.Formats.Asn1;
using Microsoft.VisualBasic;
using System.Text.Json.Nodes;
using System.IO.Compression;
using System;

class Program
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public static void Main(string[] args)
    {
    #region Instructions
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
    #endregion

    //https://fortedigital.github.io/Back-End-Internship-Task/interns.json
    //https://fortedigital.github.io/Back-End-Internship-Task/interns.csv
    //https://fortedigital.github.io/Back-End-Internship-Task/interns.zip

        Console.WriteLine("Url: ");
        string url = Console.ReadLine();

        ChooseFileFormat(url);

        //Thread.Sleep(8000);
    }

    public static async void ChooseFileFormat(string url)
    {
        if (url is not null)
        {
            try
            {
                switch (url.Substring(url.IndexOf(".", url.Length - 5)))
                {
                    case ".json":
                        Console.WriteLine("json");
                        PrintDataJSON(url);
                        break;

                    case ".csv":
                        Console.WriteLine("Csv");
                        PrintDataCSV(url);
                        break;
                    case ".zip":
                        Console.WriteLine("zup");
                        PrintDataCSV(url);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: Cannot get file." + Environment.NewLine + ex.Message);
            }

        }
    }

    public static void PrintDataJSON(string url)
    {

        foreach (var item in GettingDataJson(url).Result)
        {
            Console.WriteLine(item.Id + " " +
                item.Name + " " +
                item.Age + " " +
                item.Email + " " +
                item.InternshipStart + " " +
                item.InternshipEnd);
        }
    }
    public static void PrintDataCSV(string url)
    {
        foreach (var item in GettingDataCSV(url))
        {
            Console.WriteLine(item.Id + " " +
                item.Name + " " +
                item.Age + " " +
                item.Email + " " +
                item.InternshipStart + " " +
                item.InternshipEnd);
        }
    }

    private static async Task DownloadFile(string url, string name)
    {
        var internsList = new List<InternClass>();

        byte[] fileBytes = await _httpClient.GetByteArrayAsync(url);
        File.WriteAllBytes(name, fileBytes);

        if (name.Contains(".zip"))
        {
            ZipFile.ExtractToDirectory("./interns.zip", "./");
        }
    }


    private static List<InternClass> GettingDataCSV(string url)
    {
        //Waiting for file to be downloaded.
        Task.WaitAll(DownloadFile(url, "interns" + url.Substring(url.IndexOf(".", url.Length - 5))));

        var internsList = new List<InternClass>();

        var csvFileDescription = new CsvFileDescription
        {
            FirstLineHasColumnNames = true,
            IgnoreUnknownColumns = true,
            SeparatorChar = ',',
            UseFieldIndexForReadingData = false
        };

        var csvContext = new CsvContext();
        var interns = csvContext.Read<InternClass>("interns.csv", csvFileDescription);

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

    private static async Task<List<InternClass>> GettingDataJson(string url)
    {
        string data = string.Empty;
        var internsList = new List<InternClass>();

        //Downloading data.
        data = await _httpClient.GetStringAsync(url);


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