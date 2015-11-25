using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3Console
{
    public class RandomEngine: TestEngine
    {

        List<Card> deckA;
        List<Card> deckB;


        public RandomEngine(String path=null, String playersPath = null)
        {

            game.LoadFromFiles(path, playersPath);   


        }



        public override void InitBattle()
        {
            game.PlayerA.Init();
            game.PlayerB.Init();
        }

    }
}
