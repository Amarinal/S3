using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public class TestEngine
    {
        protected Game game;

        public TestEngine()
        {
            //Set initial Decks
            List<Card> list = new List<Card>();
            Card card;
            card = new Card() { Attack = 1, Defense = 10, Delay = 0 }; list.Add(card);
            card = new Card() { Attack = 2, Defense = 5, Delay = 1 }; list.Add(card);
            card = new Card() { Attack = 3, Defense = 2, Delay = 2 }; list.Add(card);

            game = new Game();

            Player player;
            player = new Player() { HitPoints = 10, Id = "A" };
            foreach (Card carta in list)
            {
                player.Deck.Add(carta.Clone());
            }
            game.PLayerA = player;

            player = new Player() { HitPoints = 10, Id = "B" };
            foreach (Card carta in list)
            {
                player.Deck.Add(carta.Clone());
            }
            game.PLayerA = player;


        }

    }
}
