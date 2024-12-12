using System.Text.Json;

namespace Boinkers
{
    static class Program
    {
        private static ProxyType[]? proxies;
        static List<BoinkersQuery>? LoadQuery()
        {
            try
            {
                var contents = File.ReadAllText(@"data.txt");
                return JsonSerializer.Deserialize<List<BoinkersQuery>>(contents);
            }
            catch { }

            return null;
        }

        static ProxyType[]? LoadProxy()
        {
            try
            {
                var contents = File.ReadAllText(@"proxy.txt");
                return JsonSerializer.Deserialize<ProxyType[]>(contents);
            }
            catch { }

            return null;
        }

        static void Main()
        {
            Console.WriteLine("  ____        _       _                 ____   ___ _____ \r\n | __ )  ___ (_)_ __ | | _____ _ __ ___| __ ) / _ \\_   _|\r\n |  _ \\ / _ \\| | '_ \\| |/ / _ \\ '__/ __|  _ \\| | | || |  \r\n | |_) | (_) | | | | |   <  __/ |  \\__ \\ |_) | |_| || |  \r\n |____/ \\___/|_|_| |_|_|\\_\\___|_|  |___/____/ \\___/ |_|  \r\n                                                         ");
            Console.WriteLine();
            Console.WriteLine("Github: https://github.com/glad-tidings/BoinkersBot");
            Console.WriteLine();
            Console.Write("Select an option:\n1. Run bot\n2. Create session\n> ");
            string? opt = Console.ReadLine();

            var BoinkersQueries = LoadQuery();
            proxies = LoadProxy();

            if (opt != null)
            {
                if (opt == "1")
                {
                    foreach (var Query in BoinkersQueries ?? [])
                    {
                        var BotThread = new Thread(() => BoinkersThread(Query)); BotThread.Start();
                        Thread.Sleep(60000);
                    }
                }
                else
                {
                    foreach (var Query in BoinkersQueries ?? [])
                    {
                        if (!File.Exists(@$"sessions\{Query.Name}.session"))
                        {
                            Console.WriteLine();
                            Console.WriteLine($"Create session for account {Query.Name} ({Query.Phone})");
                            TelegramMiniApp.WebView vw = new(Query.API_ID, Query.API_HASH, Query.Name, Query.Phone, "", "");
                            if (vw.Save_Session().Result)
                                Console.WriteLine("Session created");
                            else
                                Console.WriteLine("Create session failed");
                        }
                    }

                    Environment.Exit(0);
                }
            }

            Console.ReadLine();
        }

        public async static void BoinkersThread(BoinkersQuery Query)
        {
            while (true)
            {
                var RND = new Random();

                try
                {
                    var Bot = new BoinkersBot(Query, proxies ?? []);
                    if (!Bot.HasError)
                    {
                        Log.Show("Boinkers", Query.Name, $"my ip '{Bot.IPAddress}'", ConsoleColor.White);
                        Log.Show("Boinkers", Query.Name, $"login successfully.", ConsoleColor.Green);

                        var Sync = await Bot.BoinkersUserInfo();
                        if (Sync is not null)
                        {
                            double CC = 0d;
                            if (Sync.CurrencyCrypto.HasValue)
                                CC = Sync.CurrencyCrypto.Value;
                            Log.Show("Boinkers", Query.Name, $"synced successfully. B<{Math.Round(CC, 2)}> G<{Sync.CurrencySoft}> L<{Sync.Boinkers.CurrentBoinkerProgression.Id}-{Sync.Boinkers.CurrentBoinkerProgression.Level}> CB<{Sync.Boinkers.CompletedBoinkers}> W<{Sync.GamesEnergy.WheelOfFortune.Energy}> S<{Sync.GamesEnergy.SlotMachine.Energy}> R<{Sync.Rank}>", ConsoleColor.Blue);

                            if (Query.Booster & Sync.Boinkers.Booster.X2.LastTimeFreeOptionClaimed.HasValue)
                            {
                                if (Sync.Boinkers.Booster.X2.LastTimeFreeOptionClaimed.Value.ToLocalTime().AddHours(2d) < DateTime.Now)
                                {
                                    bool boost = await Bot.BoinkersBooster();
                                    if (boost)
                                        Log.Show("Boinkers", Query.Name, $"x2 booster done successfully", ConsoleColor.Green);
                                    else
                                        Log.Show("Boinkers", Query.Name, $"x2 booster failed", ConsoleColor.Red);

                                    Thread.Sleep(3000);
                                }
                            }

                            if (Query.Spin)
                            {
                                if (Sync.GamesEnergy.WheelOfFortune.Energy > 0)
                                {
                                    int spincountRND = RND.Next(Query.SpinCount[0], Query.SpinCount[1]);
                                    if (spincountRND > Sync.GamesEnergy.WheelOfFortune.Energy)
                                        spincountRND = Sync.GamesEnergy.WheelOfFortune.Energy;
                                    for (int I = 1, loopTo = spincountRND; I <= loopTo; I++)
                                    {
                                        var spin = await Bot.BoinkersWheelOfFortune(1);
                                        if (spin is not null)
                                            Log.Show("Boinkers", Query.Name, $"{I}/{spincountRND} wheel spin claimed '{spin.Prize.PrizeValue} {spin.Prize.PrizeTypeName}' successfully", ConsoleColor.Green);
                                        else
                                            Log.Show("Boinkers", Query.Name, $"{I}/{spincountRND} wheel spin failed", ConsoleColor.Red);

                                        int eachspinRND = RND.Next(Query.SpinSleep[0], Query.SpinSleep[1]);
                                        Thread.Sleep(eachspinRND * 1000);
                                    }

                                    Sync = await Bot.BoinkersUserInfo();
                                    Thread.Sleep(1000);
                                }

                                if (Sync.GamesEnergy.SlotMachine.Energy > 0)
                                {
                                    int spincountRND = RND.Next(Query.SpinCount[0], Query.SpinCount[1]);
                                    if (spincountRND > Sync.GamesEnergy.SlotMachine.Energy)
                                        spincountRND = Sync.GamesEnergy.SlotMachine.Energy;
                                    for (int I = 1, loopTo1 = spincountRND; I <= loopTo1; I++)
                                    {
                                        var spin = await Bot.BoinkersSlotMachine(Sync.GamesEnergy.SlotMachine.Energy > 1000 ? 50 : (Sync.GamesEnergy.SlotMachine.Energy > 100 ? 5 : 1));
                                        if (spin is not null)
                                            Log.Show("Boinkers", Query.Name, $"{I}/{spincountRND} slot spin claimed '{spin.Prize.PrizeValue} {(string.IsNullOrEmpty(spin.Prize.PrizeTypeName) ? "Gae" : spin.Prize.PrizeTypeName)}' successfully", ConsoleColor.Green);
                                        else
                                            Log.Show("Boinkers", Query.Name, $"{I}/{spincountRND} slot spin failed", ConsoleColor.Red);

                                        int eachspinRND = RND.Next(Query.SpinSleep[0], Query.SpinSleep[1]);
                                        Thread.Sleep(eachspinRND * 1000);
                                    }
                                }
                            }

                            if (Query.Raffle & Sync.Raffle.MilestoneReached == 0 & Sync.Raffle.Tickets == 0)
                            {
                                bool claimRaffle = await Bot.BoinkersRaffleClaim();
                                if (claimRaffle)
                                    Log.Show("Boinkers", Query.Name, "raffle claimed successfully", ConsoleColor.Green);
                                else
                                    Log.Show("Boinkers", Query.Name, "claim raffle failed", ConsoleColor.Red);
                            }

                            if (Query.Upgrade)
                            {
                                Sync = await Bot.BoinkersUserInfo();
                                Thread.Sleep(1000);

                                int upgCount = (int)Math.Round((double)Sync.CurrencySoft / 20000000d);
                                for (int I = 1, loopTo2 = upgCount; I <= loopTo2; I++)
                                {
                                    bool upgrade = await Bot.BoinkersUpgrade();
                                    if (upgrade)
                                        Log.Show("Boinkers", Query.Name, $"boinker upgraded successfully", ConsoleColor.Green);

                                    Thread.Sleep(7000);
                                }
                            }
                        }
                        else
                            Log.Show("Boinkers", Query.Name, $"synced failed", ConsoleColor.Red);
                        Sync = await Bot.BoinkersUserInfo();
                        if (Sync is not null)
                        {
                            double CC = 0d;
                            if (Sync.CurrencyCrypto.HasValue)
                                CC = Sync.CurrencyCrypto.Value;
                            Log.Show("Boinkers", Query.Name, $"B<{Math.Round(CC, 2)}> G<{Sync.CurrencySoft}> L<{Sync.Boinkers.CurrentBoinkerProgression.Id}-{Sync.Boinkers.CurrentBoinkerProgression.Level}> CB<{Sync.Boinkers.CompletedBoinkers}> W<{Sync.GamesEnergy.WheelOfFortune.Energy}> S<{Sync.GamesEnergy.SlotMachine.Energy}> R<{Sync.Rank}>", ConsoleColor.Blue);
                        }
                    }
                    else
                        Log.Show("Boinkers", Query.Name, $"{Bot.ErrorMessage}", ConsoleColor.Red);
                }
                catch (Exception ex)
                {
                    Log.Show("Boinkers", Query.Name, $"Error: {ex.Message}", ConsoleColor.Red);
                }

                int syncRND = 0;
                if (DateTime.Now.Hour < 8)
                    syncRND = RND.Next(Query.NightSleep[0], Query.NightSleep[1]);
                else
                    syncRND = RND.Next(Query.DaySleep[0], Query.DaySleep[1]);
                Log.Show("Boinkers", Query.Name, $"sync sleep '{Convert.ToInt32(syncRND / 3600d)}h {Convert.ToInt32(syncRND % 3600 / 60d)}m {syncRND % 60}s'", ConsoleColor.Yellow);
                Thread.Sleep(syncRND * 1000);
            }
        }
    }
}