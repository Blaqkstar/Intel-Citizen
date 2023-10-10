using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

class Program
{
    static void Main()
    {
        PrintTitle();
        Console.WriteLine();

        while (true)
        {
            Console.Write("ENTER PLAYER NAME (or '!exit' to quit): ");
            string username = Console.ReadLine();

            if (username.ToLower() == "!exit")
            {
                break;
            }

            string url = $"https://robertsspaceindustries.com/citizens/{username}";

            GetPlayerInfo(url).Wait();
        }
    }

    static async Task GetPlayerInfo(string url)
    {
        try
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                Console.Write("Searching");
                for (int x = 0; x < 6; x++)
                {
                    await Task.Delay(333);
                    Console.Write(".");
                }
                Console.WriteLine();
                Console.WriteLine();

                string html = await response.Content.ReadAsStringAsync();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                // finds player name
                int i = 0;
                string playerName = null;
                while (i < 25 && playerName == null)
                {
                    string xpath = "(//div[@class='profile left-col']//div[@class='inner clearfix']//div[@class='info']//p[@class='entry'])[2]//strong[@class='value']";
                    var node = doc.DocumentNode.SelectSingleNode(xpath);
                    if (node != null)
                    {
                        playerName = node.InnerText;
                    }
                    i++;
                }

                if (playerName == null)
                {
                    Console.WriteLine("Unable to retrieve player name.");
                }

                // prints player name
                Console.WriteLine($"Player Name: {playerName}");

                // finds org name
                i = 0;
                string orgName = null;
                while (i < 25 && orgName == null)
                {
                    string xpath = $"//div[@class='main-org right-col visibility-V']//div[@class='info']//p[@class='entry']//a[@class='value data{i}']";
                    var node = doc.DocumentNode.SelectSingleNode(xpath);
                    if (node != null)
                    {
                        orgName = node.InnerText;
                    }
                    i++;
                    //Console.WriteLine(i);
                }

                //if (orgName == null)
                //{
                //    Console.WriteLine("Unable to retrieve organization name.");
                //}

                // prints org name
                if (orgName == null)
                {
                    Console.WriteLine($"Organization: N/A");
                }
                else
                {
                    Console.WriteLine($"Organization: {orgName}");
                }

                // finds enlisted date
                i = 0;
                string enlistedDate = null;
                while (i < 25 && enlistedDate == null)
                {
                    string xpath = $"//div[@class='left-col']//div[@class='inner']//p[@class='entry']//strong[@class='value']";
                    var node = doc.DocumentNode.SelectSingleNode(xpath);
                    if (node != null)
                    {
                        enlistedDate = node.InnerText;
                    }
                    i++;
                }

                //if (enlistedDate == null)
                //{
                //    Console.WriteLine("Unable to retrieve enlisted date.");
                //}

                // prints enlisted date
                if (enlistedDate == null)
                {
                    Console.WriteLine($"Enlisted: N/A");
                }
                else
                {
                    Console.WriteLine($"Enlisted: {enlistedDate}");
                }
                

                // finds player bio
                i = 0;
                string playerBio = null;
                while (i < 25 && playerBio == null)
                {
                    string xpath = "//div[@class='right-col']//div[@class='inner']//div[@class='entry bio']//div[@class='value']";
                    var node = doc.DocumentNode.SelectSingleNode(xpath);
                    if (node != null)
                    {
                        playerBio = node.InnerText;
                    }
                    i++;
                }

                //if (playerBio == null)
                //{
                //    Console.WriteLine("Unable to retrieve player bio.");
                //}

                // prints URL to player profile
                Console.WriteLine($"Profile URL: {url}");

                // prints player bio
                if (playerBio == null)
                {
                    Console.WriteLine($"Player Bio: N/A");
                }
                else
                {
                    Console.WriteLine($"Player Bio: {playerBio}");
                }
            }
            else
            {
                Console.WriteLine("Player not found.");
            }
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static void PrintTitle()
    {
        string[] asciiArt = new string[]
        {
            "  _____       _       _         ___ _ _   _               ",
            "  \\_   \\_ __ | |_ ___| |       / __(_) |_(_)_______ _ __  ",
            "   / /\\/ '_ \\| __/ _ \\ |_____ / /  | | __| |_  / _ \\ '_ \\ ",
            "/\\/ /_ | | | | ||  __/ |_____/ /___| | |_| |/ /  __/ | | |",
            "\\____/ |_| |_|\\__\\___|_|     \\____/|_|\\__|_/___\\___|_| |_|",
            "                                      (C)Blaqkstar 2023"
        };

        foreach (string line in asciiArt)
        {
            Console.WriteLine(line);
            Thread.Sleep(333); // Wait for half a second
        }
    }
}
