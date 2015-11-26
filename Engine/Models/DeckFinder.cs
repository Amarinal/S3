using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class DeckFinder
    {
        protected const int REPETICIONES = 6000;
        Game simulator;
        protected Player attacker;

        protected List<Player> oponents;

        protected List<Card> baseDeck;
        protected double winRate = -1;
        protected List<Card> cardVault;
        protected List<Player> candidates;
        protected List<List<Card>> vaults;
        protected int repeatCombat= REPETICIONES;

        protected int idCounter = 0;

        public DeckFinder(Game game, Player originalPlayer, List<Player> testOponents, List<Card> inventory, int repeticiones = 0)
        {
            this.simulator = game;

            this.attacker = originalPlayer;
            this.oponents = testOponents;
            this.cardVault = inventory;

            if (repeticiones > 0) repeatCombat = repeticiones;

            attacker.Init();
            this.baseDeck = attacker.Deck.OrderBy(x=>x.Id).ToList();

            if (inventory != null) this.cardVault = inventory.OrderBy(X => X.Id).ToList();
        }

        public Player Search(int maxVariations, int depth, int width)
        {
            int times = -1;
            Player bestPlayer = attacker;
            List<Result> resultados = null;
            Game game = new Game(oponents);
            long elapsed;
            int pos = 0;

            Result result = simulator.FightAll(attacker, repeatCombat);
            this.winRate = result.Wins;


            while (times < width)
            {
                if (winRate > 99.9999) break;

                FindCandidates(maxVariations, depth, pos);

                resultados = new List<Result>();

                foreach(Player attacker in candidates)
                {
                    elapsed = DateTime.Now.Ticks;
                    resultados.Add(game.FightAll(attacker, repeatCombat));
                    Console.WriteLine(string.Format("Tiempo {0}:{1} ms {2:%#0.00}({3:%#0.00})", attacker.Name, (DateTime.Now.Ticks - elapsed) / 10000, resultados[resultados.Count - 1].Wins, resultados.Max(x => x.Wins)));
                }

                resultados = resultados.OrderByDescending(x => x.Wins).ThenByDescending(x => x.Id).ToList();

                if (resultados[0].Attacker.Id != attacker.Id && resultados[0].Wins > winRate)
                {
                    for(int i = 0; i < candidates.Count; i++)
                    {
                        if (resultados[0].Attacker.Id == candidates[i].Id)
                        {
                            pos = i;
                            break;
                        }
                    }

                    attacker = resultados[0].Attacker;
                    winRate = resultados[0].Wins;
                    Console.WriteLine(attacker.ToFullString());
                    times = 0;
                }
                else
                {
                    if (times < 0) times = 0;
                    times++;
                    pos = 0;
                }
            }

            Console.WriteLine(string.Format("Wins: {0:%#0.00}", winRate));
            return attacker;
        }

        public void FindCandidates(int maxVariations, int depth, int vaultPos, string output = null)
        {
            Random random = new Random();
            int variations = 0;
            this.candidates = new List<Player>();
            List<Card> varDeck;

            if (vaults!=null && vaults.Count > vaultPos)
            {
                this.cardVault = vaults[vaultPos].Select(x => x.Clone()).ToList();
            }

            vaults = new List<List<Card>>();

            attacker.Init();
            varDeck = attacker.Deck.Select(x=>x.Clone()).OrderBy(x=>x.Id).ToList();

            candidates.Add(attacker);
            vaults.Add(this.cardVault.Select(x => x.Clone()).OrderBy(x => x.Id).ToList());

            while (candidates.Count < depth)
            {
                variations = random.Next(1, maxVariations + 1);
                int posIn, posOut;

                for(int i = 0; i < variations; i++)
                {
                    posIn = random.Next(cardVault.Count);
                    posOut = random.Next(attacker.Deck.Count);

                    varDeck.Add(cardVault[posIn].Clone());
                    cardVault.Add(varDeck[posOut].Clone());

                    cardVault.RemoveAt(posIn);
                    varDeck.RemoveAt(posOut);

                    cardVault = cardVault.OrderBy(X => X.Id).ToList();
                    varDeck = varDeck.OrderBy(X => X.Id).ToList();
                }

                candidates.Add(new Player(--idCounter,"Samael", varDeck));
                candidates.Add(new Player(--idCounter,"Groc", varDeck));
                candidates.Add(new Player(--idCounter,"Lich", varDeck));

                vaults.Add(cardVault.Select(x => x.Clone()).ToList());
                vaults.Add(cardVault.Select(x => x.Clone()).ToList());
                vaults.Add(cardVault.Select(x => x.Clone()).ToList());
            }

        }

        

    }
}
