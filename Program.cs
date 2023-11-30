using HtmlAgilityPack;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using SC_Player_Intel_App;

class Program
{
    Collections collections = new Collections();

    static void Main()
    {
        Player player = new Player();
        Program program = new Program(); // allows main to access methods
        Collections collections = new Collections();
        bool debugMode = false;
        bool isRunning = false;

        PrintTitle();
        Console.WriteLine();
        isRunning = true;

        while (isRunning == true)
        {
            Console.Write("ENTER PLAYER NAME (or '/help' for help): ");
            string username = Console.ReadLine();

            program.GetUserInput(username, debugMode);
            program.collections.SearchList.Add(player);
        }
    }

    public void GetUserInput(string username, bool debugMode)
    {
        // exit cmd
        if (username.ToLower() == "/exit")
        {
            Environment.Exit(0);
        }

        // debug mode toggle
        if (username == "/debug")
        {
            ToggleDebug(debugMode);
        }

        // help dialog
        if (username == "/help" || username == "/?")
        {
            PrintHelpInfo();
        }

        // about cmd
        if (username == "/about")
        {
            Console.WriteLine();
            Console.WriteLine("********************************");
            PrintAboutInfo();
            Console.WriteLine();
            Console.WriteLine("********************************");
        }

        // org cmd
        //if (username == "/org")
        //{
        //    Console.WriteLine();
            
        //}

        // player cmd


        // donate cmd
        if (username == "/donate")
        {
            LaunchDonatePage();
        }

        // processes query
        if (username != "/debug" && username != "/help" && username != "/?" && username != "/donate" && username != "/about")
        {
            string url = $"https://robertsspaceindustries.com/citizens/{username}";
            GetPlayerInfo(url, debugMode).Wait();
        }
        Console.WriteLine();
    }

    public async Task GetPlayerInfo(string url, bool debugMode)
    {
        Player player = new Player(); // instantiates new temporary player object

        try
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                Console.Write("Fetching data");
                for (int x = 0; x < 6; x++)
                {
                    await Task.Delay(200);
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
                        player.Name = playerName;
                        player.URL = url;
                    }
                    i++;
                }

                if (playerName == null && debugMode == true)
                {
                    Console.WriteLine("DEBUG CONSOLE: Unable to retrieve player name.");
                }

                // prints player name
                Console.WriteLine($"PLAYER NAME: {player.Name}");

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
                        player.EnlistedDate = enlistedDate;
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
                    Console.WriteLine($"ENLISTED: {player.EnlistedDate}");
                }
                Console.WriteLine();

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
                        player.OrgName = orgName;
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
                    Console.WriteLine($"ORG: {player.OrgName}");
                }

                // finds org URL
                i = 0;
                string orgURL = null;
                while (i < 25 && orgURL == null)
                {
                    //string xpath = $"//div[@class='main-org right-col visibility-V']//div[@class='info']//p[@class='entry']//a[@class='value data{i}']";
                    string xpath = $"//div[@class='main-org right-col visibility-V']//div[@class='info']//p[@class='entry']//a/@href";
                    var node = doc.DocumentNode.SelectSingleNode(xpath);
                    if (node != null)
                    {
                        orgURL = node.GetAttributeValue("href", string.Empty);
                        player.OrgURL = orgURL;
                    }
                    i++;
                    //Console.WriteLine(i);
                }

                if (orgURL == null && debugMode == true)
                {
                    Console.WriteLine("DEBUG CONSOLE: Unable to retrieve organization name.");
                }

                // prints org URL
                if (orgURL == null)
                {
                    Console.WriteLine($"ORG URL: N/A");
                }
                else
                {
                    Console.WriteLine($"ORG URL: http://www.robertsspaceindustries.com{player.OrgURL}");
                }
                Console.WriteLine();

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
                        player.Bio = playerBio;
                    }
                    i++;
                }

                if (playerBio == null && debugMode == true)
                {
                    Console.WriteLine("DEBUG CONSOLE: Unable to retrieve player bio.");
                }

                // prints URL to player profile
                Console.WriteLine($"PROFILE URL: {player.URL}");

                // prints player bio
                if (playerBio == null)
                {
                    Console.WriteLine($"PLAYER BIO: N/A");
                    Console.WriteLine();
                    Console.WriteLine("--------------------------");
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine($"PLAYER BIO:");
                    Console.WriteLine(player.Bio);
                    Console.WriteLine();
                    Console.WriteLine("--------------------------");
                }
            }
            else
            {
                Console.WriteLine("Player not found.");
                Console.WriteLine();
                Console.WriteLine("--------------------------");
            }
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("--------------------------");
        }
    }

    private static void ToggleDebug(bool debugMode)
    {
        debugMode = !debugMode;
        if (debugMode == true)
        {
            Console.WriteLine("Debug Mode enabled. Use /debug again to disable.");
            Console.WriteLine();
            Console.WriteLine("--------------------------");
            Console.WriteLine();
        }
        else if (debugMode == false)
        {
            Console.WriteLine("Debug Mode disabled.");
            Console.WriteLine();
            Console.WriteLine("--------------------------");
            Console.WriteLine();
        }
    }
    private static void PrintHelpInfo()
    {
        Console.WriteLine();
        Console.WriteLine("AVAILABLE COMMANDS:");
        Console.WriteLine("--------------------------");
        Console.WriteLine();
        Console.WriteLine("/org - pulls up org webpage of last-searched player");
        Console.WriteLine("/about - build info");
        Console.WriteLine("/donate - buy me a coffee :)");
        Console.WriteLine("/exit - closes Intel-Citizen");
        Console.WriteLine();
        Console.WriteLine("--------------------------");
        Console.WriteLine();
    }

    private static void PrintAboutInfo()
    {
        string ver = "v0.1.06";
        Console.WriteLine();
        Console.WriteLine("Developed by Blaqkstar, 2023");
        Console.WriteLine($"Version: {ver}");
        Console.WriteLine();
        Console.WriteLine($"What's new with {ver}?");
        Console.WriteLine("- Added output for player org webpage URL (if one exists publicly)");
        Console.WriteLine("- Cleaned up output a little by separating org info block from player bio info block");
        Console.WriteLine("- Created logic and output for /about command");
        Console.WriteLine();
    }

    private static void LaunchDonatePage()
    {
        // STILL NEED TO SET UP DONATION PAGE
        System.Diagnostics.Process.Start("explorer.exe", "http://google.com");
    }
    /*private static void LaunchOrgPage(string orgURL)
    {
        // Launches browser to org page of last-searched player
        System.Diagnostics.Process.Start("explorer.exe", $"http://www.robertsspaceindustries.com{orgURL}");
    }*/
    static void PrintTitle()
    {
        string ver = "v0.1.06";
        string[] asciiArt = new string[]
        {
            "  _____       _       _         ___ _ _   _               ",
            "  \\_   \\_ __ | |_ ___| |       / __(_) |_(_)_______ _ __  ",
            "   / /\\/ '_ \\| __/ _ \\ |_____ / /  | | __| |_  / _ \\ '_ \\ ",
            "/\\/ /_ | | | | ||  __/ |_____/ /___| | |_| |/ /  __/ | | |",
            $"\\____/ |_| |_|\\__\\___|_|     \\____/|_|\\__|_/___\\___|_| |_|{ver}",
        };
        string underline = "==================================================================";

        foreach (char c in underline)
        {
            Console.Write(c);
            Thread.Sleep(1);
        }
        Console.WriteLine();
        foreach (string line in asciiArt)
        {
            Console.WriteLine(line);
            Thread.Sleep(200); // Wait for 200ms
        }
        Console.WriteLine();
        foreach (char c in underline)
        {
            Console.Write(c);
            Thread.Sleep(1);
        }
        Console.WriteLine();

    }

}
