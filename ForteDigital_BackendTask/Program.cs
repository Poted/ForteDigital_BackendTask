using ForteDigital_BackendTask;
using Newtonsoft.Json.Linq;
using System.Globalization;
using LINQtoCSV;
using System.IO.Compression;
using System.Text.RegularExpressions;

class Program
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private static List<int> ages = new List<int>();

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
         */
        #endregion

        //https://fortedigital.github.io/Back-End-Internship-Task/interns.json
        //https://fortedigital.github.io/Back-End-Internship-Task/interns.csv
        //https://fortedigital.github.io/Back-End-Internship-Task/interns.zip


        Console.WriteLine("interns.exe > ");
        string command = Console.ReadLine();
        string url = string.Empty;

        string s = command.Substring(command.LastIndexOf('/') + 1);
        string fileName = string.Empty;
        foreach (char x in s)
        {
            if (char.IsWhiteSpace(x)) 
                break;
            
            fileName += x;
        }


        if (command is not null)
        {
            //Taking url from command using regex.
            var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (Match m in linkParser.Matches(command))
                url = m.Value;
    
            //Download data from API.
            DownloadAPIData(url, fileName);

            //Preparing result from command.
            ageCountAndMaxAge(command);

        }
    }

    private static void ageCountAndMaxAge(string command)
    {
        int result = 0;

        if (command.Contains("count"))
        {
            if (command.Contains("--age"))
            {
                string s = command.Substring(command.IndexOf("--age") + 9);
                string num = string.Empty;
                foreach (char x in s)
                {
                    if (char.IsNumber(x))
                        num += x;
                }

                if (command.Contains("--age-gt"))
                {
                    foreach (int age in ages)
                    {
                        if (Convert.ToInt32(num) < age)
                            result++;
                    }
                }

                else if (command.Contains("--age-lt"))
                {
                    foreach (int age in ages)
                    {
                        if (Convert.ToInt32(num) > age)
                            result++;
                    }
                }
            }

            else if (!command.Contains("--age"))
            {
                foreach (int age in ages)
                    result++;
            }
        }

        else if (command.Contains("max-age"))
        {
            result = ages.Max();
        }

        else Console.WriteLine("Invalid command.");


        Console.WriteLine("output: " + result);
    }

    public static void DownloadAPIData(string url, string fileName)
    {
        if (url is not null)
        {
            try
            {
                switch (url.Substring(url.IndexOf(".", url.Length - 5)))
                {
                    case ".json":
                        SaveDataJSON(url);
                        break;

                    case ".csv":
                        SaveDataCSV(url, fileName);
                        break;

                    case ".zip":
                        SaveDataCSV(url, fileName);
                        break;
                }
            }
            catch
            {
                Console.WriteLine("Error: Cannot get file." /*+ Environment.NewLine + ex.Message*/);
            }
        }
    }

    public static void SaveDataJSON(string url)
    {

        foreach (var item in GettingDataJson(url).Result)
        {
            ages.Add(item.Age);

            //Console.WriteLine(item.Id + " " +
            //    item.Name + " " +
            //    item.Age + " " +
            //    item.Email + " " +
            //    item.InternshipStart + " " +
            //    item.InternshipEnd);
        }
    }

    public static void SaveDataCSV(string url, string fileName)
    {
        foreach (var item in GettingDataCSV(url, fileName))
        {
            ages.Add(item.Age);

            //Console.WriteLine(item.Id + " " +
            //    item.Name + " " +
            //    item.Age + " " +
            //    item.Email + " " +
            //    item.InternshipStart + " " +
            //    item.InternshipEnd);
        }
    }

    private static async Task DownloadFile(string url, string fileName)
    {
        try
        {
            byte[] fileBytes = await _httpClient.GetByteArrayAsync(url);
             File.WriteAllBytes(fileName, fileBytes);

        }
        catch (Exception ex)
        {
            Console.WriteLine("download exc msg: " + ex.Message);
        }


        try
        {
            if (fileName.Contains(".zip"))
            {
                if (File.Exists(fileName.Replace(fileName.Substring(fileName.LastIndexOf('.') + 1), "csv")))
                    File.Delete(fileName.Replace(fileName.Substring(fileName.LastIndexOf('.') + 1), "csv"));
                ZipFile.ExtractToDirectory($"./{fileName}", "./");

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("extracting exc msg: "  + ex.Message);
        }
    }


    private static List<InternClass> GettingDataCSV(string url, string fileName)
    {
        try
        {
            //Waiting for file to be downloaded.
            Task.WaitAll(DownloadFile(url, fileName));
            
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
        catch
        {
            Console.WriteLine("Cannot process the file.");
        }

        return null;
    }

    private static async Task<List<InternClass>> GettingDataJson(string url)
    {
        try
        {
            string data = string.Empty;
            var internsList = new List<InternClass>();

            //Downloading data.
            data = await _httpClient.GetStringAsync(url);


            //Parsing data into objects.
            var JSONData = JObject.Parse(data);

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
        catch 
        {
            Console.WriteLine("Error: Cannot process the file.");
        }

        return null;
    }
}