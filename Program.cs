using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

class Program
{
    static void Main()
    {
        bool debugMode = false;

        PrintTitle();
        Console.WriteLine();

        while (true)
        {
            Console.Write("ENTER PLAYER NAME (or '!exit' to quit): ");
            string username = Console.ReadLine();

            // exit cmd
            if (username.ToLower() == "!exit")
            {
                Environment.Exit(0);
            }

            // debug mode toggle
            if (username == "!debug")
            {
                debugMode = !debugMode;

                if (debugMode == true)
                {
                    Console.WriteLine("Debug Mode enabled. Use !debug again to disable.");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Debug Mode disabled.");
                    Console.WriteLine();
                }
            }

            if (username != "!debug")
            {
                string url = $"https://robertsspaceindustries.com/citizens/{username}";
                GetPlayerInfo(url, debugMode).Wait();
            }            
        }
    }

    static async Task GetPlayerInfo(string url, bool debugMode)
    {
        try
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                Console.Write("Fetching data");
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

                if (playerName == null && debugMode == true)
                {
                    Console.WriteLine("DEBUG CONSOLE: Unable to retrieve player name.");
                }

                // prints player name
                Console.WriteLine($"PLAYER NAME: {playerName}");

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

                // debugMode output
                if (enlistedDate == null && debugMode == true)
                {
                    Console.WriteLine("DEBUG CONSOLE: Unable to retrieve enlisted date.");
                }

                // prints enlisted date
                if (enlistedDate == null)
                {
                    Console.WriteLine($"ENLISTED: N/A");
                }
                else
                {
                    Console.WriteLine($"ENLISTED: {enlistedDate}");
                }

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

                if (orgName == null && debugMode == true)
                {
                    Console.WriteLine("DEBUG CONSOLE: Unable to retrieve organization name.");
                }

                // prints org name
                if (orgName == null)
                {
                    Console.WriteLine($"ORG: N/A");
                }
                else
                {
                    Console.WriteLine($"ORG: {orgName}");
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
                        playerBio = playerBio.Trim();
                    }
                    i++;
                }

                if (playerBio == null && debugMode == true)
                {
                    Console.WriteLine("DEBUG CONSOLE: Unable to retrieve player bio.");
                }

                // prints URL to player profile
                Console.WriteLine($"PROFILE URL: {url}");

                // prints player bio
                if (playerBio == null)
                {
                    Console.WriteLine($"PLAYER BIO: N/A");
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine($"PLAYER BIO:");
                    Console.WriteLine(playerBio);
                    Console.WriteLine();
                    Console.WriteLine("--------------------------");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Player not found.");
                Console.WriteLine();
                Console.WriteLine("--------------------------");
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("--------------------------");
            Console.WriteLine();
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
            Thread.Sleep(200); // Wait for half a second
        }
    }

}
