using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class CardCollection
    {

        protected List<Card> cards;
        public List<Card> Cards { get { return cards; } }

        public CardCollection()
        {
        }

        public void ReadFromFile(String path =@".\cartas.csv")
        {
            this.cards = new List<Card>();

            HelperCsv fichero = new HelperCsv(path);
            Card card;

            for (int i = 0; i < fichero.Count; i++)
            {
                card = new Card();

                //Id;Name;Rarity;Fuse;Level;Attack;Defense;Delay;Faction1;Faction2;Faction3;Skill1;Skill2;Skill3;Skill4;Skill5
                card.Id = Convert.ToInt32(fichero[i, "Id"]);
                card.Name = fichero[i, "Name"];
                card.Rarity = fichero[i, "Rarity"];
                card.Fuse = fichero[i, "Fuse"];
                card.Delay = Convert.ToInt32(fichero[i, "Delay"]);
                card.Level = Convert.ToInt32(fichero[i, "Level"]);
                card.Attack = Convert.ToInt32(fichero[i, "Attack"]);
                card.Defense = Convert.ToInt32(fichero[i, "Defense"]);

                if (!String.IsNullOrWhiteSpace(fichero[i, "Faction1"])) card.Factions.Add(fichero[i, "Faction1"]);
                if (!String.IsNullOrWhiteSpace(fichero[i, "Faction2"])) card.Factions.Add(fichero[i, "Faction2"]);
                if (!String.IsNullOrWhiteSpace(fichero[i, "Faction3"])) card.Factions.Add(fichero[i, "Faction3"]);

                if (!String.IsNullOrWhiteSpace(fichero[i, "Skill1"])) card.AddSkillFromText(fichero[i, "Skill1"]);
                if (!String.IsNullOrWhiteSpace(fichero[i, "Skill2"])) card.AddSkillFromText(fichero[i, "Skill2"]);
                if (!String.IsNullOrWhiteSpace(fichero[i, "Skill3"])) card.AddSkillFromText(fichero[i, "Skill3"]);
                if (!String.IsNullOrWhiteSpace(fichero[i, "Skill4"])) card.AddSkillFromText(fichero[i, "Skill4"]);
                if (!String.IsNullOrWhiteSpace(fichero[i, "Skill5"])) card.AddSkillFromText(fichero[i, "Skill5"]);


                this.cards.Add(card);
            }

        }

        public Card GetCard(String name, int level)
        {
            if (level == 0) level = this.cards.Where(x => x.Name.ToLower().Trim() == name.ToLower().Trim()).Max(x => x.Level);

            Card card = this.cards.FirstOrDefault(x => x.Name.ToLower().Trim() == name.ToLower().Trim() && x.Level == level);


            if (card != null)
            {
                return card.Clone();
            }

            return null;
        }
    }
}
