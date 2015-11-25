using Engine.Flows;
using Engine.Models;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace S3Console
{
    public class TestEngine
    {
        protected Game game;

        protected List<Card> baseDeck;

        protected bool verbose = false;

        public TestEngine()
        {
            //Set initial Decks
            baseDeck = new List<Card>();
            Card card;
            card = new Card() { Attack = 1, Defense = 10, Delay = 0 }; baseDeck.Add(card);
            card = new Card() { Attack = 2, Defense = 5, Delay = 1 }; baseDeck.Add(card);
            card = new Card() { Attack = 3, Defense = 2, Delay = 2 }; baseDeck.Add(card);

            game = new Game();

            Player player;
            player = new Player() { HitPoints = 10, Name = "A", Id = 1 };
            game.PlayerA = player;
            player = new Player() { HitPoints = 10, Name = "B", Id = 2 };
            game.PlayerB = player;
        }


        public virtual void InitBattle()
        {
            game.PlayerA.Init(baseDeck);
            game.PlayerB.Init(baseDeck);

        }

        public Result Run(int veces)
        {
            Result resultado = new Result();
            resultado.Attacker = this.game.PlayerA;

            if (veces < 1)
            {
                return resultado;
            }

            for (int i = 0; i < veces; i++)
            {
                this.InitBattle();
                resultado.Record(this.Run());
            }

            return resultado;
        }


        public Result Run()
        {
           
            int turno = 0;
            StringBuilder sBuilder;
            Player winner = null;

            while (!game.Finished)
            {
                turno++;

                try
                {
                    RunStep();
                }
                catch (DeadPlayerExeption dpex)
                {
                    //Console.WriteLine(string.Format("Perdio el jugador {0}", dpex.DeadPlayer.Id));
                    if (dpex.DeadPlayer.Id == game.PlayerA.Id)
                    {
                        winner = game.PlayerB;
                    }
                    else
                    {
                        winner = game.PlayerA;
                    }
                    break;
                }

                playerA = !playerA;

            }

            return new Result() { Attacker = winner };

            //Console.WriteLine(String.Format("Player A: {0}  PlayerB: {1}", game.PlayerA.HitPoints, game.PlayerB.HitPoints));

        }

        protected bool playerA = true;
        private void RunStep()
        {

            Dictionary<String, Object> parametros = new Dictionary<String, Object>();
            parametros.Add("Game", game);
            parametros.Add("Player", playerA ? game.PlayerA : game.PlayerB);

            WorkflowInvoker wfinvoker = new WorkflowInvoker(new TurnFlow());
            wfinvoker.Invoke(parametros);

            playerA = !playerA;
        }

        public String DebugStep()
        {
            try
            {
                if (!game.Finished) RunStep();
            }
            catch (DeadPlayerExeption dpex)
            {

            }

            return game.StateString();
        }
    }
}
