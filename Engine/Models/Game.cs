using Engine.Flows;
using System;
using System.Activities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Game
    {

        public Guid Id { get; set; }

        protected CardCollection cards;
        public CardCollection Cards { get { return cards; } }

        private List<Player> oponentes;
        public List<Player> Oponentes
        {
            get
            {
                return oponentes;
            }
        }

        public Player ActivePlayer { get; set; }
    
        protected List<Result> resultados;

        protected int turn;

        protected Combat steppedCombat = null;

        public bool IsCombatInProgress
        {
            get
            {
                if (steppedCombat != null)
                {
                    return steppedCombat.IsInProgress;
                }
                return false;
            }
        }

        public Game()
        {

        }

        public Game(List<Player> againstPlayers)
        {
            this.oponentes = againstPlayers;
        }

        public void LoadFromFiles(String cardPath = null, String playersPath = null)
        {

            if (cardPath == null) cardPath = @"Cartas.csv";
            if (playersPath == null) playersPath = @"Players.csv";

            cards = new CardCollection();
            cards.ReadFromFile(cardPath);

            HelperCsv fichero = new HelperCsv(playersPath);
            Player player;
            oponentes = new List<Player>();

            for (int i = 0; i < fichero.Count; i++)
            {
                player = new Player();
                player.LoadFromCsv(fichero, i, this.cards);
                oponentes.Add(player);
            }

        }

        public int OponentCount
        {
            get
            {
                return oponentes.Count;
            }
        }

        public string StateString()
        {
            if (steppedCombat == null) return "No stepped Combat in Progress!";
            return steppedCombat.StateString();
        }


        public List<Result> War(int repeatCombat)
        {
            resultados = new List<Result>();

            if (this.oponentes.Count == 0) return null;

            Console.Clear();
            long elapsed;
            long elapsedWar = DateTime.Now.Ticks;

            foreach (Player attacker in this.oponentes)
            {
                elapsed = DateTime.Now.Ticks;

                foreach (Player defender in this.oponentes)
                {
                    if (attacker.Id == defender.Id) continue;
                    Combat combat = new Combat(attacker.Clone(), defender.Clone(), repeatCombat);
                    resultados.Add(combat.Resolve());
                }


                Console.WriteLine(string.Format("{0}.{1} ({2} battles)({3}ms)", attacker.Id, attacker.Name, repeatCombat * (this.oponentes.Count - 1), (DateTime.Now.Ticks - elapsed) / 10000));

            }

            Console.WriteLine(string.Format("war done in {0} ms", (DateTime.Now.Ticks - elapsedWar) / 10000));

            return resultados;
        }

        public Result FightAll(Player attacker, int repeatCombat)
        {
            Result resultado = new Result();
            resultado.Attacker = attacker;
            
            foreach (Player defender in this.oponentes)
            {
                if (attacker.Id == defender.Id) continue;
                Combat combat = new Combat(attacker.Clone(), defender.Clone(), repeatCombat);
                resultado.Record(combat.Resolve());
            }

            return resultado;
        }


        public void DoTurn()
        {
            if (oponentes.Count < 2) return;

            if (steppedCombat == null)
            {
                return;
            }

            steppedCombat.StepTurn();
        }

        public bool BeginSteppedCombat()
        {
            if (oponentes.Count < 2) return false;

            if (steppedCombat == null)
            {
                steppedCombat = new Combat(oponentes[0], oponentes[1]);
                steppedCombat.InitCombat();
            }

            return true;
        }

 
        public string ResultString()
        {
            StringBuilder texto = new StringBuilder();

            foreach (Result result in this.resultados)
            {
                texto.AppendFormat("{0};{1};{2:%#0.00}", result.Attacker.Name, result.Defender.Name, result.Wins);
                texto.AppendLine();
            }

            return texto.ToString();
        }

        public void Result2CSV(string totalsPath = "Wartotals.cvs", string detailsPath= "Wardetails.cvs")
        {
            Console.WriteLine("");
            Console.WriteLine("Escribiendo a fichero.");

            if (File.Exists(totalsPath)) File.Delete(totalsPath);
            if (File.Exists(detailsPath)) File.Delete(detailsPath);

            StreamWriter total = new StreamWriter(File.OpenWrite(totalsPath));
            StreamWriter detail = new StreamWriter(File.OpenWrite(detailsPath));

            total.WriteLine("Player;battles;wins;wins%;best%;worst%");
            detail.WriteLine("Attacker;Defender;wins%");

            double wins = 0;
            int battles = 0;
            int winsNumber = 0;
            double best = 0;
            double worst = 0;

            try
            {
                foreach (Player attacker in this.oponentes)
                {
                    winsNumber = resultados.Where(x=>x.Attacker.Id==attacker.Id).Sum(s=>s.WinCount);
                    battles = resultados.Where(x => x.Attacker.Id == attacker.Id).Sum(s => s.BattleCount);
                    if (battles > 0)
                    {
                        wins = (double)winsNumber / battles;
                    }
                    else
                    {
                        wins = 0;
                    } 
                    best = resultados.Where(x => x.Attacker.Id == attacker.Id).Max(s=>s.Wins);
                    worst = best = resultados.Where(x => x.Attacker.Id == attacker.Id).Min(s => s.Wins);


                    total.WriteLine("{0};{1};{2};{3:%#0.00};{4:%#0.00};{5:%#0.00}", attacker.Name, battles, winsNumber, ((double)winsNumber) / battles, best, worst);
                }

                foreach (Result result in this.resultados)
                {
                    detail.WriteLine("{0};{1};{2:%#0.00}", result.Attacker.Name, result.Defender.Name, result.Wins);
                }
            }
            catch
            {
                //de momento no hago nada
                Console.WriteLine("Algo ha ido mal escribiendo los ficheros.");
            }
            finally
            {
                total.Close();
                detail.Close();
                Console.WriteLine("Fin.");
            }
        }

    }
}
