using ForteDigital_BackendTask;
using Newtonsoft.Json.Linq;
using System.Globalization;
using LINQtoCSV;
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

        //Console.WriteLine("Url: ");
        //string url = Console.ReadLine();

        //ChooseFileFormat(url);

        int[] ages = new int[] { 1, 2, 3, 12, 23, 34 };
        int counter = 0;

        //count commands
        string cmd1 = "json count --age-gt 22";                       //2
        string cmd2 = "json count --age-lt 22";                       //4
        string cmd3 = "json count xD";                                //6

        //max-age command
        string cmd4 = "json max-age";                                 //34


        string countCmd = cmd4;
        
        if(countCmd.Contains("count"))
        {
            if(countCmd.Contains("--age"))
            {
                string s = countCmd.Substring(countCmd.IndexOf("--age") + 9);
                string num = string.Empty;
                foreach (char x in s)
                {
                    if (Char.IsNumber(x))
                        num += x;
                    //if (x == '-') break;

                }
                Console.WriteLine("num: " + num);


                if (countCmd.Contains("--age-gt"))
                {
                    foreach (int age in ages)
                    {
                        if (Convert.ToInt32(num) < age)
                            counter++;
                    }
                }

                else if (countCmd.Contains("--age-lt"))
                {
                    foreach (int age in ages)
                    {
                        if (Convert.ToInt32(num) > age)
                            counter++;
                    }
                }
            }
        
            else if (!countCmd.Contains("--age"))
            {
                foreach(int age in ages)
                counter++;
            }
        }

        else if (countCmd.Contains("max-age"))
        {
            counter = ages.Max();
        }

        else Console.WriteLine("Invalid command.");


        //count <url> [ --age-gt | --age-lt age]

        //max-age <url>



        Console.WriteLine("count: " + counter);

        

        //Todo: use name of a file instead of 'interns', make commends and errors handling

        //Thread.Sleep(8000);
    }

    public static void ChooseFileFormat(string url)
    {
        if (url is not null)
        {
            try
            {
                switch (url.Substring(url.IndexOf(".", url.Length - 5)))
                {
                    case ".json":
                        PrintDataJSON(url);
                        break;

                    case ".csv":
                        PrintDataCSV(url);
                        break;

                    case ".zip":
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