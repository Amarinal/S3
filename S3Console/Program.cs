using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.Write("Comando>");
            String command;
            Boolean quitNow = false;
            string[] elementos;
            long elapsed = 0;
            int veces = 1;
            Player player = null;
            Player defender = null;
            List<Result> resultados = new List<Result>();
            Dictionary<String, String> argumentos;
            string playersPath = null;
            String options = null;
            string attackerName = null;
            string defenderName = null;
            Game game = null;

            while (!quitNow)
            {
                string[] comando;
                string key, value;

                command = Console.ReadLine();
                elementos = command.Split(' ');
                command = elementos[0];
             

                argumentos = new Dictionary<string, string>();
                foreach (String elemento in elementos)
                {
                    key = value = elemento;

                    if (elemento.StartsWith("-"))
                    {
                        key = elemento.Substring(1);

                        if (elemento.Contains(":"))
                        {
                            comando = elemento.Split(':');
                            key = comando[0].Substring(1);
                            value = comando[1];
                        }
                    }

                    argumentos.Add(key, value);
                }

                switch (command.ToLower())
                {
                    case "clear":
                        game = null;
                        player = null;
                        defender = null;
                        resultados = new List<Result>();
                        playersPath = null;
                        options = null;
                        attackerName = null;
                        defenderName = null;
                        break;
                    case "c":
                    case "combat": //c -a:Bambori -d:herres47 -o:t,f

                        if (argumentos.Keys.Contains("a")) attackerName = argumentos["a"];
                        if (argumentos.Keys.Contains("d")) defenderName = argumentos["d"];
                        if (argumentos.Keys.Contains("o")) options = argumentos["o"];



                        if (game == null)
                        {
                            game = new Game();
                            game.LoadFromFiles(null, playersPath);
                            if (!string.IsNullOrWhiteSpace(options)) game.SetOptions(options);

                            player = game.Oponentes.FirstOrDefault(x => x.Name == attackerName);
                            defender = game.Oponentes.FirstOrDefault(x => x.Name == defenderName);

                            game.BeginSteppedCombat(player, defender);
                        }

                        if (game.IsCombatInProgress) game.DoTurn();

                        Console.WriteLine(game.StateString());

                        break;

                    case "war": //war -v:5 -o:default
                        elapsed = DateTime.Now.Ticks;
                        bool writeFiles = false;

                        if (argumentos.Keys.Contains("v")) veces = Convert.ToInt32(argumentos["v"]);
                        if (argumentos.Keys.Contains("p")) playersPath = argumentos["p"];
                        if (argumentos.Keys.Contains("o")) options = argumentos["o"];
                        if (argumentos.Keys.Contains("o")) writeFiles = true;
 
                        game = new Game();
                        game.LoadFromFiles(null, playersPath);
                        if (!string.IsNullOrWhiteSpace(options)) game.SetOptions(options);

                        resultados = game.War(veces);  
                        
                        if (writeFiles)
                        {
                            game.Result2CSV();
                            Console.WriteLine("Done!");
                        }
                        else
                        {
                            Console.WriteLine(game.ResultString());
                        }
                        Console.WriteLine(string.Format("Tiempo total:{0} ms", (DateTime.Now.Ticks - elapsed) / 10000));
                        break;
                    case "sim"://sim -v:50 -dp:16 -w:4 -var:3
                        elapsed = DateTime.Now.Ticks;
                        int depth = 64;
                        int width = 2;
                        int variations = 3;
                        string inventoryPath = "inventory.csv";
                        

                        if (argumentos.Keys.Contains("v")) veces = Convert.ToInt32(argumentos["v"]);
                        if (argumentos.Keys.Contains("dp")) depth = Convert.ToInt32(argumentos["dp"]);
                        if (argumentos.Keys.Contains("w")) width = Convert.ToInt32(argumentos["w"]);
                        if (argumentos.Keys.Contains("var")) variations = Convert.ToInt32(argumentos["var"]);
                        if (argumentos.Keys.Contains("p")) playersPath = argumentos["p"];
                        if (argumentos.Keys.Contains("i")) inventoryPath = argumentos["i"];
                        if (argumentos.Keys.Contains("a")) attackerName = argumentos["a"];
                        if (argumentos.Keys.Contains("o")) options = argumentos["o"];
                        if (argumentos.Keys.Contains("d")) defenderName = argumentos["d"];

                        List<Card> inventory = new List<Card>();

                        game = new Game();
                        game.LoadFromFiles();
                        HelperCsv file = new HelperCsv(inventoryPath);

                        for(int i = 0; i < file.Count; i++)
                        {
                            inventory.Add(game.Cards.GetCard(file[i, "Name"], 0));
                        }

                        player = game.Oponentes.FirstOrDefault(x => x.Name == attackerName);
                        defender = game.Oponentes.FirstOrDefault(x => x.Name == defenderName);

                        if (player != null)
                        {
                            game.Oponentes.Remove(player);

                            if (!string.IsNullOrWhiteSpace(options)) game.SetOptions(options);

                            if (defender != null)
                            {
                                game.Oponentes.Clear();
                                game.Oponentes.Add(defender);
                            }

                            DeckFinder simDeck= new DeckFinder(game, player, game.Oponentes, inventory, veces);

                            player = simDeck.Search(variations, depth, width);

                            Console.WriteLine(player.ToFullString());
                        }

                        Console.WriteLine(string.Format("Tiempo total:{0} ms", (DateTime.Now.Ticks - elapsed) / 10000));



                        break;
                    case "f":
                    case "fight"://f -v:2 -a:Bambori -d:herres47 -o:t,f
                        elapsed = DateTime.Now.Ticks;
                        Result result;
                        defender = null;

                        if (argumentos.Keys.Contains("v")) veces = Convert.ToInt32(argumentos["v"]);
                        if (argumentos.Keys.Contains("a")) attackerName = argumentos["a"];
                        if (argumentos.Keys.Contains("d")) defenderName = argumentos["d"];
                        if (argumentos.Keys.Contains("o")) options = argumentos["o"];

                        game = new Game();
                        game.LoadFromFiles();

                        player = game.Oponentes.FirstOrDefault(x => x.Name == attackerName);
                        defender = game.Oponentes.FirstOrDefault(x => x.Name == defenderName);


                        if (player != null)
                        {
                            game.Oponentes.Remove(player);

                            if (defender != null)
                            {
                                game.Oponentes.Clear();
                                game.Oponentes.Add(defender);
                            }

                            if (!string.IsNullOrWhiteSpace(options)) game.SetOptions(options);

                            result = game.FightAll(player, veces);
                            Console.WriteLine(string.Format("WinRatio: {0:%#0.00}. Battles:{1}", result.Wins, result.BattleCount));
                        }

                        Console.WriteLine(string.Format("Tiempo total:{0} ms", (DateTime.Now.Ticks - elapsed) / 10000));
                        break;
                    case "quit":
                        quitNow = true;
                        break;

                    default:
                        //Console.WriteLine("Unknown Command " + command);
                        break;
                }
            }
        }
    }
}
