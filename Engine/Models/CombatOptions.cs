using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class CombatOptions
    {

        protected List<Card> defenderCards = new List<Card>();
        public List<Card> DefenderCards
        {
            get
            {
                return defenderCards;
            }
        }

        protected List<Card> attackerCards = new List<Card>();
        public List<Card> AttackerCards
        {
            get
            {
                return attackerCards;
            }
        }


        protected List<Skill> effects = new List<Skill>();
        public List<Skill> BattleEffects { get { return effects; } }


    }
}
