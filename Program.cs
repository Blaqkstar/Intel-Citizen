using HtmlAgilityPack;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using SC_Player_Intel_App;
using System.Net.Mail;

class Program
{
    Collections collections = new Collections();

    static void Main(string[] args)
    {
        Player player = new Player();
        Program program = new Program(); // allows main to access methods
        Collections collections = new Collections();
        bool debugMode = false;
        bool isRunning = false;

        PrintTitle();
        Console.WriteLine();
        isRunning = true;

        // main program loop
        while (isRunning == true)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("ENTER PLAYER NAME (or '/help' for help): ");
            Console.ResetColor();
            string username = Console.ReadLine();
            username = username.Trim();
            program.GetUserInput(username, debugMode);
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

        // list cmd
        if (username == "/list")
        {
            PrintListMenu();
        }


        // donate cmd
        if (username == "/donate")
        {
            LaunchDonatePage();
        }

        // processes query
        if (username != "/debug" && username != "/help" && username != "/?" && username != "/donate" && username != "/about" && username != "/list")
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

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("--------------------------");
                Console.ResetColor();
                Console.WriteLine();
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
                        player.OrgURL = $"http://www.robertsspaceindustries.com{orgURL}";
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
                    Console.WriteLine($"ORG URL: {player.OrgURL}");
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
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("--------------------------");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine($"PLAYER BIO:");
                    Console.WriteLine(player.Bio);
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("--------------------------");
                    Console.ResetColor();
                }
                collections.SearchList.Add(player);
            }
            // no player found
            else
            {
                Console.WriteLine("Player not found.");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("--------------------------");
                Console.ResetColor();
            }
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("--------------------------");
            Console.ResetColor();
        }
    }

    private void PrintListMenu()
    {
        string[] listOptions = new string[] { "[1] Recently Searched", "[2] Target List", "[3] Whitelist", "[4] Export Lists", "[5] Back" };
        List<Player> recentlySearched = collections.SearchList;
        List<Player> targetList = collections.TargetList;
        List<WhiteListedPlayer> whiteList = collections.WhiteList;

        Console.WriteLine();
        string listChoice = "UNDEFINED LIST CHOICE";

        do
        {
            foreach (string s in listOptions)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("CHOOSE A LIST: ");
            Console.ResetColor();
            listChoice = Console.ReadLine();
            Console.WriteLine();
            // user selects to view recent searches
            if (listChoice == "1")
            {
                // if no recent searches found
                if (recentlySearched.Count < 1)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("NO RECENT SEARCHES");
                    Console.ResetColor();
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine();
                    RecentlySearchedListInteraction();
                }
                
            }
            else if (listChoice == "2")
            {
                Console.WriteLine();
                // if targetlist is empty
                if (targetList.Count < 1)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("TARGET LIST IS EMPTY");
                    Console.ResetColor();
                    Console.WriteLine();
                }
                else
                {
                    TargetListInteraction();
                }
            }
            else if (listChoice == "3")
            {
                Console.WriteLine();
                // if whitelist is empty
                if (whiteList.Count < 1)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("WHITELIST IS EMPTY");
                    Console.ResetColor();
                    Console.WriteLine();
                }
                else
                {
                    WhiteListInteraction();
                }
                
            }
            // user selects to export lists
            else if (listChoice == "4")
            {
                // if targetlist is empty
                if (targetList.Count < 1)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("TARGET LIST IS EMPTY");
                    Console.ResetColor();
                    Console.WriteLine();
                }
                else
                {
                    // establishes date and time
                    DateOnly date = DateOnly.FromDateTime(DateTime.Now);
                    string dateString = date.ToString("yyyyMMdd"); // Formats the date as "yyyymmdd"

                    //filepath
                    string currentDirectory = Directory.GetCurrentDirectory();
                    string targetDirectory = Path.Combine(currentDirectory, "Lists", "Target List");
                    Directory.CreateDirectory(targetDirectory); // Creates the directory if it doesn't exist

                    string filePath = Path.Combine(targetDirectory, $"TargetList_{dateString}.csv");

                    // list export
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine($"TARGET LIST - {dateString}");
                        writer.WriteLine("NAME,ENLISTED,URL,ORG NAME,ORG URL,BIO,BOUNTY,NOTES");
                        foreach (Player target in targetList)
                        {
                            writer.WriteLine($"{target.Name},{target.EnlistedDate},{target.URL},{target.OrgName},{target.OrgURL},{target.Bio},{target.Bounty},{target.Notes}");
                        }
                    }
                    // status output
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("TARGET LIST EXPORTED SUCCESSFULLY");
                    Console.WriteLine($"EXPORT LOCATION: {filePath}");
                    Console.ResetColor();
                    Console.WriteLine();
                }
                // if whitelist is empty
                if (whiteList.Count < 1)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("WHITELIST IS EMPTY");
                    Console.WriteLine();
                    Console.ResetColor();
                }
                else
                {
                    // establishes date and time
                    DateOnly date = DateOnly.FromDateTime(DateTime.Now);
                    string dateString = date.ToString("yyyyMMdd"); // Formats the date as "yyyymmdd"

                    //filepath
                    string currentDirectory = Directory.GetCurrentDirectory();
                    string targetDirectory = Path.Combine(currentDirectory, "Lists", "Whitelist");
                    Directory.CreateDirectory(targetDirectory); // Creates the directory if it doesn't exist

                    string filePath = Path.Combine(targetDirectory, $"Whitelist_{dateString}.csv");

                    // list export
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine($"WHITELIST - {dateString}");
                        writer.WriteLine("NAME,URL,ORG NAME,ORG URL,FEE,START DATE,END DATE,NOTES");
                        foreach (WhiteListedPlayer player in whiteList)
                        {
                            writer.WriteLine($"{player.Name},{player.URL},{player.OrgName},{player.OrgURL},{player.Fee},{player.StartDate},{player.EndDate},{player.Notes}");
                        }
                    }
                    // status output
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("WHITELIST EXPORTED SUCCESSFULLY");
                    Console.WriteLine($"EXPORT LOCATION: {filePath}");
                    Console.ResetColor();
                    Console.WriteLine();
                }
            }
            else if (listChoice == "5")
            {
                break;
            }
            else
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("INVALID CHOICE");
                Console.ResetColor();
                Console.WriteLine();
            }
        }
        while (listChoice != "5");

        void RecentlySearchedListInteraction()
        {
            int playerChoice = -1;
            string input;
            bool validInput = false;

            do
            {
                int c = 1;

                Console.WriteLine("RECENT SEARCHES:");
                Console.WriteLine();
                for (int i = recentlySearched.Count - 1; i >= 0; i--)
                {
                    Console.WriteLine($"[{i+1}] {recentlySearched[i].Name}");
                    c++;
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("SELECT A PLAYER (or '/back' to return to previous menu): ");
                Console.ResetColor();
                input = Console.ReadLine();

                if (input == "/back")
                {
                    break;
                }

                playerChoice = int.TryParse(input, out int result) ? result : -1;
            } while (playerChoice <= 0 || playerChoice > recentlySearched.Count);


            if (input != "/back")
            {
                Player selectedPlayer = recentlySearched[playerChoice - 1]; // Uses playerChoice

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("--------------------------");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine($"PLAYER NAME: {selectedPlayer.Name}");
                Console.WriteLine($"ENLISTED: {selectedPlayer.EnlistedDate}");
                if (selectedPlayer.Bounty != null)
                {
                    Console.WriteLine($"BOUNTY: {selectedPlayer.Bounty}");
                }
                else
                {
                    Console.WriteLine($"BOUNTY: N/A");
                }
                Console.WriteLine();
                if (selectedPlayer.OrgName != null)
                {
                    Console.WriteLine($"ORG NAME: {selectedPlayer.OrgName}");
                }
                else
                {
                    Console.WriteLine($"ORG NAME: N/A");
                }
                if (selectedPlayer.OrgURL != null)
                {
                    Console.WriteLine($"ORG URL: {selectedPlayer.OrgURL}");
                }
                else
                {
                    Console.WriteLine($"ORG URL: N/A");
                }
                Console.WriteLine();
                Console.WriteLine($"PROFILE URL: {selectedPlayer.URL}");
                if (selectedPlayer.Bio != null)
                {
                    Console.WriteLine($"PLAYER BIO: {selectedPlayer.Bio}");
                }
                else
                {
                    Console.WriteLine($"PLAYER BIO: N/A");
                }
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("--------------------------");
                Console.ResetColor();
                Console.WriteLine();

                string[] actionStrings = new string[] { "[1] Add Player to Target List", "[2] Add Player to Whitelist", "[3] Remove Player from Search History", "[4] Back"};

                foreach (string s in actionStrings)
                {
                    Console.WriteLine(s);
                }
                Console.WriteLine();

                int choice = -1;

                do
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("CHOOSE ACTION: ");
                    Console.ResetColor();
                    try
                    {
                        choice = int.Parse(Console.ReadLine());
                    }
                    catch (FormatException)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("INVALID INTEGER");
                        Console.ResetColor();
                        choice = 0; // Reset choice
                        continue; // Skip the rest of the loop and start over
                    }
                    Console.WriteLine();
                }
                while (choice <= 0 || choice > actionStrings.Length);

                if (choice != 4)
                {
                    // player added to target list
                    if (choice == 1)
                    {
                        do
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("ADD BOUNTY? (Y/N) ");
                            Console.ResetColor();
                            input = Console.ReadLine();
                            input = input.ToLower();
                            if (input == "y")
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write("ENTER BOUNTY AMOUNT: ");
                                Console.ResetColor();
                                int bountyAmount;
                                if (Int32.TryParse(Console.ReadLine(), out bountyAmount))
                                {
                                    if (bountyAmount > selectedPlayer.Bounty)
                                    {
                                        selectedPlayer.Bounty = bountyAmount;
                                        Console.WriteLine();
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine($"{selectedPlayer.Name} BOUNTY INCREASED TO {bountyAmount} aUEC");
                                        Console.ResetColor();
                                        validInput = true;
                                    }
                                    else if (bountyAmount < selectedPlayer.Bounty)
                                    {
                                        selectedPlayer.Bounty = bountyAmount;
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine($"{selectedPlayer.Name} BOUNTY REDUCED TO {bountyAmount} aUEC");
                                        Console.ResetColor();
                                        validInput = true;
                                    }
                                    else
                                    {
                                        Console.WriteLine();
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine($"NO CHANGES RECORDED TO {selectedPlayer.Name} BOUNTY");
                                        Console.ResetColor();
                                        validInput = true;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine();
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    Console.WriteLine("INVALID INTEGER");
                                    Console.ResetColor();
                                    Console.WriteLine();
                                    continue;
                                }
                            }
                            else if (input == "n")
                            {
                                break;
                            }
                            else continue;
                        } while (validInput == false);

                        // adds player to targetlist
                        targetList.Add(selectedPlayer);
                        selectedPlayer.IsOnTargetList = true;
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"ADDED {selectedPlayer.Name} TO TARGET LIST");
                        Console.ResetColor();
                        Console.WriteLine();
                    }
                    // player added to whitelist
                    else if (choice == 2)
                    {
                        WhiteListedPlayer whiteListedPlayer = new WhiteListedPlayer();
                        validInput = false;

                        // copies attributes from selectedPlayer to WhitelistedPlayer
                        whiteListedPlayer.Name = selectedPlayer.Name;
                        whiteListedPlayer.EnlistedDate = selectedPlayer.EnlistedDate;
                        whiteListedPlayer.URL = selectedPlayer.URL;
                        whiteListedPlayer.OrgName = selectedPlayer.OrgName;
                        whiteListedPlayer.OrgURL = selectedPlayer.OrgURL;
                        whiteListedPlayer.Bio = selectedPlayer.Bio;
                        whiteListedPlayer.Notes = selectedPlayer.Notes;

                        // reason for whitelist
                        do
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("REASON FOR WHITELIST: ");
                            Console.ResetColor();
                            input = Console.ReadLine();
                            if (input.Length < 3)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.WriteLine("WHITELIST REASON MUST BE AT LEAST 3 CHARACTERS LONG");
                                Console.ResetColor();
                                Console.WriteLine();
                                continue;
                            }
                            else if (input.Length > 64)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.WriteLine("WHITELIST REASON CANNOT EXCEED 64 CHARACTERS");
                                Console.ResetColor();
                                Console.WriteLine();
                                continue;
                            }
                            else
                            {
                                whiteListedPlayer.ReasonForWhitelist = input.Trim();
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.WriteLine();
                                Console.WriteLine("WHITELIST REASON SAVED");
                                Console.ResetColor();
                                Console.WriteLine();
                                validInput = true;
                            }
                        } while (!validInput);

                        validInput = false;
                        // sets fee paid
                        do
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("FEE PAID? (Y/N) ");
                            Console.ResetColor();
                            input = Console.ReadLine();
                            input = input.ToLower();
                            if (input == "y")
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write("ENTER AMOUNT PAID: ");
                                Console.ResetColor();
                                int feePaid;
                                if (Int32.TryParse(Console.ReadLine(), out feePaid))
                                {
                                    if (feePaid > whiteListedPlayer.Fee)
                                    {
                                        whiteListedPlayer.Fee = feePaid;
                                        Console.WriteLine();
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine($"{whiteListedPlayer.Name} AMOUNT PAID INCREASED TO {feePaid} aUEC");
                                        Console.ResetColor();
                                        Console.WriteLine();
                                        validInput = true;
                                    }
                                    else if (feePaid < whiteListedPlayer.Fee)
                                    {
                                        whiteListedPlayer.Fee = feePaid;
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine($"{whiteListedPlayer.Name} AMOUNT PAID REDUCED TO {feePaid} aUEC");
                                        Console.ResetColor();
                                        Console.WriteLine();
                                        validInput = true;
                                    }
                                    else
                                    {
                                        Console.WriteLine();
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine($"NO CHANGES RECORDED TO {whiteListedPlayer.Name} FEE");
                                        Console.ResetColor();
                                        Console.WriteLine();
                                        validInput = true;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine();
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    Console.WriteLine("INVALID INTEGER");
                                    Console.ResetColor();
                                    Console.WriteLine();
                                    continue;
                                }
                            }
                            else if (input == "n")
                            {
                                whiteListedPlayer.Fee = 0;
                                break;
                            }
                            else continue;
                        } while (validInput == false);

                        // whitelist start date defaults to the day user adds them to list
                        whiteListedPlayer.StartDate = DateOnly.FromDateTime(DateTime.Now);
                        // whitelist duration defaults to 30 days
                        whiteListedPlayer.EndDate = whiteListedPlayer.StartDate.AddDays(30);

                        collections.WhiteList.Add(whiteListedPlayer);
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"ADDED {whiteListedPlayer.Name} TO WHITELIST");
                        Console.WriteLine();
                        Console.WriteLine($"REASON FOR WHITELIST: {whiteListedPlayer.ReasonForWhitelist}");
                        Console.WriteLine($"WHITELIST START DATE: {whiteListedPlayer.StartDate}");
                        Console.WriteLine($"WHITELIST END DATE: {whiteListedPlayer.EndDate}");
                        Console.ResetColor();
                        Console.WriteLine();
                    }
                    // player removed from search history
                    else if (choice == 3)
                    {
                        recentlySearched.Remove(selectedPlayer);
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"REMOVED {selectedPlayer.Name} FROM SEARCH HISTORY");
                        Console.ResetColor();
                        Console.WriteLine();
                    }
                }
            }
        }

        void TargetListInteraction()
        {
            int playerChoice = -1;
            string input;
            bool validInput = false;

            do
            {
                int c = 1;

                Console.WriteLine("TARGET LIST:");
                Console.WriteLine();
                for (int i = targetList.Count - 1; i >= 0; i--)
                {
                    Console.WriteLine($"[{i + 1}] {targetList[i].Name}");
                    c++;
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("SELECT TARGET (or '/back' to return to previous menu): ");
                Console.ResetColor();
                input = Console.ReadLine();
                input = input.Trim();

                if (input == "/back")
                {
                    break;
                }

                playerChoice = int.TryParse(input, out int result) ? result : -1;
            } while (playerChoice <= 0 || playerChoice > targetList.Count);

            if (input != "/back")
            {
                Player selectedPlayer = targetList[playerChoice - 1]; // Uses playerChoice

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("--------------------------");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine($"TARGET NAME: {selectedPlayer.Name}");
                Console.WriteLine($"ENLISTED: {selectedPlayer.EnlistedDate}");
                if (selectedPlayer.Bounty != null)
                {
                    Console.WriteLine($"BOUNTY: {selectedPlayer.Bounty}");
                }
                else
                {
                    Console.WriteLine($"BOUNTY: N/A");
                }
                Console.WriteLine();
                if (selectedPlayer.OrgName != null)
                {
                    Console.WriteLine($"ORG NAME: {selectedPlayer.OrgName}");
                }
                else
                {
                    Console.WriteLine($"ORG NAME: N/A");
                }
                if (selectedPlayer.OrgURL != null)
                {
                    Console.WriteLine($"ORG URL: {selectedPlayer.OrgURL}");
                }
                else
                {
                    Console.WriteLine($"ORG URL: N/A");
                }
                Console.WriteLine();
                Console.WriteLine($"PROFILE URL: {selectedPlayer.URL}");
                if (selectedPlayer.Notes != null)
                {
                    Console.WriteLine($"TARGET BIO: {selectedPlayer.Notes}");
                }
                else
                {
                    Console.WriteLine($"TARGET NOTES: N/A");
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("--------------------------");
                Console.ResetColor();
                Console.WriteLine();

                string[] actionStrings = new string[] { "[1] Update Target Info", "[2] Remove Target", "[3] Back" };
                string[] updateActionStrings = new string[] { "[1] Update Bounty", "[2] Update Notes", "[3] Back" };

                foreach (string s in actionStrings)
                {
                    Console.WriteLine(s);
                }
                Console.WriteLine();

                int choice = -1;

                do
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("CHOOSE ACTION: ");
                    Console.ResetColor();
                    try
                    {
                        choice = int.Parse(Console.ReadLine());
                    }
                    catch (FormatException)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("INVALID INTEGER");
                        Console.ResetColor();
                        Console.WriteLine();
                        choice = 0; // Reset choice
                        continue; // Skip the rest of the loop and start over
                    }
                    Console.WriteLine();
                }
                while (choice <= 0 || choice > actionStrings.Length);

                // if user does not choose to go back
                while (choice != 3)
                {
                    // User chooses to update target info
                    if (choice == 1)
                    {
                        // gets user choice re: info to update (bounty or notes)
                        do
                        {
                            int updateChoice = -1;
                            do
                            {
                                foreach (string s in updateActionStrings)
                                {
                                    Console.WriteLine(s);
                                }
                                Console.WriteLine();
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write("ITEM TO UPDATE: ");
                                Console.ResetColor();
                                updateChoice = int.Parse(Console.ReadLine());
                                Console.WriteLine();
                            } while (updateChoice <= 0 || updateChoice > updateActionStrings.Length);

                            // user selects to update bounty info
                            if (updateChoice == 1)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write($"UPDATE {selectedPlayer.Name} BOUNTY AMOUNT:");
                                Console.ResetColor();
                                int bountyAmount;
                                if (Int32.TryParse(Console.ReadLine(), out bountyAmount))
                                {
                                    selectedPlayer.Bounty = bountyAmount;
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.WriteLine($"{selectedPlayer.Name} BOUNTY UPDATED TO {bountyAmount} aUEC");
                                    Console.ResetColor();
                                    Console.WriteLine();
                                    validInput = true;
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    Console.WriteLine("INVALID INTEGER");
                                    Console.ResetColor();
                                    Console.WriteLine();
                                    continue;
                                }
                            }
                            // user selects to update target note info
                            else if (updateChoice == 2)
                            {
                                validInput = false;

                                do
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.Write("ENTER TARGET NOTE: ");
                                    Console.ResetColor();
                                    input = Console.ReadLine();
                                    if (input.Length < 3)
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGray;
                                        Console.WriteLine("NOTE LENGTH MUST BE AT LEAST 3 CHARACTERS");
                                        Console.ResetColor();
                                        Console.WriteLine();
                                        continue;
                                    }
                                    else if (input.Length > 2000)
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGray;
                                        Console.WriteLine("NOTE LENGTH CANNOT EXCEED 2000 CHARACTERS");
                                        Console.ResetColor();
                                        Console.WriteLine();
                                        continue;
                                    }
                                    else
                                    {
                                        selectedPlayer.Notes = input.Trim();
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine($"NOTE SAVED TO TARGET {selectedPlayer.Name}");
                                        Console.ResetColor();
                                        Console.WriteLine();
                                        validInput = true;
                                    }
                                } while (!validInput);
                            }
                            else if (updateChoice == 3)
                            {
                                validInput = true;
                                break;
                            }
                            else
                            {
                                Console.WriteLine();
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.WriteLine("INVALID CHOICE");
                                Console.ResetColor();
                                continue;
                            }
                        } while (validInput == false);

                        break;
                    }
                    // user selects to remove target from list
                    else if (choice == 2)
                    {
                        targetList.Remove(selectedPlayer);
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"REMOVED {selectedPlayer.Name} FROM TARGET LIST");
                        Console.ResetColor();
                        Console.WriteLine();
                        break;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("INVALID CHOICE");
                        Console.ResetColor();
                        continue;
                    }
                }
            }
        }
        void WhiteListInteraction()
        {
            int playerChoice = -1;
            string input;
            bool validInput = false;

            do
            {
                int c = 1;

                Console.WriteLine("WHITELIST:");
                Console.WriteLine();
                for (int i = whiteList.Count - 1; i >= 0; i--)
                {
                    Console.WriteLine($"[{i + 1}] {whiteList[i].Name}");
                    c++;
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("SELECT PLAYER (or '/back' to return to previous menu): ");
                Console.ResetColor();
                input = Console.ReadLine();
                Console.WriteLine();
                input = input.Trim();

                if (input == "/back")
                {
                    break;
                }

                playerChoice = int.TryParse(input, out int result) ? result : -1;
            } while (playerChoice <= 0 || playerChoice > whiteList.Count);

            if (input != "/back")
            {
                WhiteListedPlayer selectedPlayer = whiteList[playerChoice - 1]; // Uses playerChoice

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("--------------------------");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine($"PLAYER NAME: {selectedPlayer.Name}");
                Console.WriteLine($"PROFILE URL: {selectedPlayer.URL}");
                if (selectedPlayer.ReasonForWhitelist != null)
                {
                    Console.WriteLine($"REASON FOR WHITELIST: {selectedPlayer.ReasonForWhitelist}");
                }
                else
                {
                    Console.WriteLine($"REASON FOR WHITELIST: N/A");
                }
                Console.WriteLine();
                if (selectedPlayer.StartDate != null)
                {
                    Console.WriteLine($"WHITELIST START DATE: {selectedPlayer.StartDate}");
                }
                else
                {
                    Console.WriteLine($"WHITELIST START DATE: N/A");
                }
                if (selectedPlayer.EndDate != null)
                {
                    Console.WriteLine($"WHITELIST END DATE: {selectedPlayer.EndDate}");
                }
                else
                {
                    Console.WriteLine($"WHITELIST END DATE: N/A");
                }
                if (selectedPlayer.Fee != null)
                {
                    Console.WriteLine($"FEE PAID: {selectedPlayer.Fee}");
                }
                else
                {
                    Console.WriteLine($"FEE PAID: N/A");
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("--------------------------");
                Console.ResetColor();
                Console.WriteLine();

                string[] actionStrings = new string[] { "[1] Update Player Info", "[2] Remove Player", "[3] Back" };
                string[] updateActionStrings = new string[] { "[1] Update Start Date", "[2] Update Duration", "[3] Update Reason for Whitelist", "[4] Update Fee","[5] Back" };

                foreach (string s in actionStrings)
                {
                    Console.WriteLine(s);
                }
                Console.WriteLine();

                int choice = -1;

                do
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("CHOOSE ACTION: ");
                    Console.ResetColor();
                    try
                    {
                        choice = int.Parse(Console.ReadLine());
                    }
                    catch (FormatException)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("INVALID INTEGER");
                        Console.ResetColor();
                        Console.WriteLine();
                        choice = 0; // Reset choice
                        continue; // Skip the rest of the loop and start over
                    }
                    Console.WriteLine();
                }
                while (choice <= 0 || choice > actionStrings.Length);

                // if user does not choose to go back
                while (choice != 3)
                {
                    // User chooses to update player info
                    if (choice == 1)
                    {
                        // gets user choice re: info to update (start date, duration, reason, fee)
                        do
                        {
                            int updateChoice = -1;
                            do
                            {
                                foreach (string s in updateActionStrings)
                                {
                                    Console.WriteLine(s);
                                }
                                Console.WriteLine();
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write("ITEM TO UPDATE: ");
                                Console.ResetColor();
                                updateChoice = int.Parse(Console.ReadLine());
                                Console.WriteLine();
                            } while (updateChoice <= 0 || updateChoice > updateActionStrings.Length);

                            // user selects to update start date
                            if (updateChoice == 1)
                            {
                                do
                                {
                                    validInput = false;
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.Write($"UPDATE {selectedPlayer.Name} WHITELIST START DATE (MM/DD/YYYY): ");
                                    Console.ResetColor();
                                    DateOnly startDate;
                                    if (DateOnly.TryParse(Console.ReadLine(), out startDate))
                                    {
                                        selectedPlayer.StartDate = startDate;
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine($"{selectedPlayer.Name} WHITELIST START DATE UPDATED TO {startDate}");
                                        Console.ResetColor();
                                        Console.WriteLine();
                                        validInput = true;
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGray;
                                        Console.WriteLine("INVALID DATE FORMAT");
                                        Console.ResetColor();
                                        Console.WriteLine();
                                        continue;
                                    }
                                } while (!validInput);
                            }
                            // user selects to update player whitelist duration
                            else if (updateChoice == 2)
                            {
                                validInput = false;

                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write($"UPDATE {selectedPlayer.Name} WHITELIST DURATION: ");
                                Console.ResetColor();
                                int duration;
                                if (int.TryParse(Console.ReadLine(), out duration))
                                {
                                    selectedPlayer.EndDate = selectedPlayer.StartDate.AddDays(duration);
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.WriteLine($"{selectedPlayer.Name} WHITELIST START DATE UPDATED TO {selectedPlayer.EndDate}");
                                    Console.ResetColor();
                                    Console.WriteLine();
                                    validInput = true;
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    Console.WriteLine("INVALID DURATION");
                                    Console.ResetColor();
                                    Console.WriteLine();
                                    continue;
                                }
                            }
                            // user selects to update whitelist reason
                            else if (updateChoice == 3)
                            {
                                // reason for whitelist
                                do
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Console.Write("REASON FOR WHITELIST: ");
                                    Console.ResetColor();
                                    input = Console.ReadLine();
                                    if (input.Length < 3)
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGray;
                                        Console.WriteLine("WHITELIST REASON MUST BE AT LEAST 3 CHARACTERS LONG");
                                        Console.ResetColor();
                                        Console.WriteLine();
                                        continue;
                                    }
                                    else if (input.Length > 64)
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGray;
                                        Console.WriteLine("WHITELIST REASON CANNOT EXCEED 64 CHARACTERS");
                                        Console.ResetColor();
                                        Console.WriteLine();
                                        continue;
                                    }
                                    else
                                    {
                                        selectedPlayer.ReasonForWhitelist = input.Trim();
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine();
                                        Console.WriteLine("WHITELIST REASON SAVED");
                                        Console.ResetColor();
                                        Console.WriteLine();
                                        validInput = true;
                                    }
                                } while (!validInput);
                            }
                            // user selects to update fee
                            else if (updateChoice == 4)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write("ENTER AMOUNT PAID: ");
                                Console.ResetColor();
                                int feePaid;
                                if (Int32.TryParse(Console.ReadLine(), out feePaid))
                                {
                                    if (feePaid > selectedPlayer.Fee)
                                    {
                                        selectedPlayer.Fee = feePaid;
                                        Console.WriteLine();
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine($"{selectedPlayer.Name} AMOUNT PAID INCREASED TO {feePaid} aUEC");
                                        Console.ResetColor();
                                        Console.WriteLine();
                                        validInput = true;
                                    }
                                    else if (feePaid < selectedPlayer.Fee)
                                    {
                                        selectedPlayer.Fee = feePaid;
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine($"{selectedPlayer.Name} AMOUNT PAID REDUCED TO {feePaid} aUEC");
                                        Console.ResetColor();
                                        Console.WriteLine();
                                        validInput = true;
                                    }
                                    else
                                    {
                                        Console.WriteLine();
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.WriteLine($"NO CHANGES RECORDED TO {selectedPlayer.Name} FEE");
                                        Console.ResetColor();
                                        Console.WriteLine();
                                        validInput = true;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine();
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    Console.WriteLine("INVALID INTEGER");
                                    Console.ResetColor();
                                    Console.WriteLine();
                                    continue;
                                }
                            }
                            // user selects to go back
                            else if (updateChoice == 5)
                            {
                                validInput = true;
                                break;
                            }
                            else
                            {
                                Console.WriteLine();
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.WriteLine("INVALID CHOICE");
                                Console.ResetColor();
                                Console.WriteLine();
                                continue;
                            }
                        } while (validInput == false);

                        break;
                    }
                    // user selects to remove player from list
                    else if (choice == 2)
                    {
                        whiteList.Remove(selectedPlayer);
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"REMOVED {selectedPlayer.Name} FROM WHITELIST");
                        Console.ResetColor();
                        Console.WriteLine();
                        break;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("INVALID CHOICE");
                        Console.ResetColor();
                        Console.WriteLine();
                        continue;
                    }
                }
            }
        }
    }

    private static void ToggleDebug(bool debugMode)
    {
        debugMode = !debugMode;
        if (debugMode == true)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Debug Mode enabled. Use '/debug' again to disable.");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("--------------------------");
            Console.WriteLine();
        }
        else if (debugMode == false)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Debug Mode disabled.");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("--------------------------");
            Console.WriteLine();
        }
    }
    private static void PrintHelpInfo()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("AVAILABLE COMMANDS:");
        Console.ResetColor();
        Console.WriteLine("--------------------------");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine();
        Console.WriteLine("/list - interact with a variety of lists");
        Console.WriteLine("/about - build info");
        Console.WriteLine("/donate - buy me a coffee :)");
        Console.WriteLine("/exit - closes Intel-Citizen");
        Console.WriteLine();
        Console.ResetColor();
        Console.WriteLine("--------------------------");
        Console.WriteLine();
    }

    private static void PrintAboutInfo()
    {
        string ver = "v0.2.0";
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Developed by Blaqkstar, 2023");
        Console.WriteLine($"Version: {ver}");
        Console.ResetColor();
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"What's new with {ver}?");
        Console.ResetColor();
        Console.WriteLine("- Completed first iteration whitelist function");
        Console.WriteLine("- Completed first iteration list csv export function");
        Console.WriteLine();
    }

    private static void LaunchDonatePage()
    {
        // STILL NEED TO SET UP DONATION PAGE
        System.Diagnostics.Process.Start("explorer.exe", "http://google.com");
    }
   
    static void PrintTitle()
    {
        string ver = "v0.2.0";
        string[] asciiArt = new string[]
        {
            "  _____       _       _         ___ _ _   _               ",
            "  \\_   \\_ __ | |_ ___| |       / __(_) |_(_)_______ _ __  ",
            "   / /\\/ '_ \\| __/ _ \\ |_____ / /  | | __| |_  / _ \\ '_ \\ ",
            "/\\/ /_ | | | | ||  __/ |_____/ /___| | |_| |/ /  __/ | | |",
            $"\\____/ |_| |_|\\__\\___|_|     \\____/|_|\\__|_/___\\___|_| |_|{ver}",
        };

        string underline = "==================================================================";

        Console.ForegroundColor = ConsoleColor.DarkGray;
        foreach (char c in underline)
        {
            Console.Write(c);
            Thread.Sleep(1);
        }
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        foreach (string line in asciiArt)
        {
            Console.WriteLine(line);
            Thread.Sleep(200); // Wait for 200ms
        }
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        foreach (char c in underline)
        {
            Console.Write(c);
            Thread.Sleep(1);
        }
        Console.ResetColor();
        Console.WriteLine();

    }

}
