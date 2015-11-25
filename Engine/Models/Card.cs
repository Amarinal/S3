using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string Rarity { get; set; }
        public string Fuse { get; set; }

        public int Attack { get; set; }
        public int AttackModified
        {
            get
            {
                int value = Attack + AttackModifier;
                if (value < 0) value = 0;
                return value;
            }
        }
        public int AttackModifier { get; set; }

        private int defense = int.MinValue;
        public int Defense
        {
            get
            {
                return defense;
            }
            set
            {
                if (defense == int.MinValue)
                {
                    DefenseMax = value;
                }

                if (value > DefenseMax) value = DefenseMax;
                defense = value;
            }
        }
        public int DefenseMax { get; set; }

        public int Delay { get; set; }

        public List<Skill> SkillsActive { get;}

        public List<Skill> Skills { get;}

        public List<String> Factions { get; }

        public Card Oponente { get; set; }

        public int Barrier { get; set; }
        public int Hex { get; set; }
        protected int poison = 0;
        public int Poison
        {
            get
            {
                return poison;
            }
            set
            {
                if (value > poison) poison = value;
            }
        }

        public bool Scorched { get; set; }
        public int Scorch { get; set; }

        public Card()
        {
            this.Skills = new List<Skill>();
            this.SkillsActive = new List<Skill>();

            this.Factions = new List<String>();
        }

        public Card Clone()
        {
            Card carta = new Card();
            

            carta.Attack = this.Attack;
            carta.Defense = this.Defense;
            carta.Name = this.Name;
            carta.Id = this.Id;
            carta.Level = this.Level;
            carta.Fuse = this.Fuse;
            carta.Rarity = this.Rarity;
            carta.Delay = this.Delay;

            Skill nueva;
            foreach (Skill skill in Skills)
            {
                nueva = new Skill();
                nueva.Name = skill.Name;
                nueva.Value = skill.Value;
                nueva.Category = skill.Category;
                nueva.Aux = skill.Aux;

                foreach (string mod in skill.Modifiers)
                {
                    nueva.Modifiers.Add(mod);
                }

                carta.Skills.Add(nueva);
            }

            foreach (String faction in this.Factions)
            {
                carta.Factions.Add(faction);
            }

            return carta;
        }

        public void AddSkillFromText(String skillText)
        {
            Skill skill = new Skill();
            skill.LoadSkillFromText(skillText);

            this.Skills.Add(skill);
        }

        public override string ToString()
        {
            StringBuilder salida = new StringBuilder();

            salida.AppendFormat("{0}.({2},{3},{4}).{1}", Id, Name, AttackModified, Defense, Delay);

            if (Scorch > 0) salida.AppendFormat(" S({1}):{0}", Scorch, Scorched ? "Y" : "N");
            if (Poison > 0) salida.AppendFormat(" P:{0}", Poison);


            return salida.ToString();
        }

        public void TakeSkill(Skill skill)
        {
            int invisibleValue = 0;
            Skill invisible = this.Skills.FirstOrDefault(x => x.Name.ToLower() == "invisibility");

            if (invisible != null) invisibleValue = invisible.Aux;

            switch (skill.Name.ToLower())
            {
                case "barrier":
                    this.Barrier += skill.Value;
                    break;
                case "legion":
                case "empower":
                    this.AttackModifier += skill.Value;
                    break;
                case "heal":
                    this.Defense += skill.Value;
                    break;
                case "bolt":
                    
                    if (invisibleValue > 0)
                    {
                        invisible.Aux--;
                        return;
                    }
                    int bolt = skill.Value;
                    for (int i = Barrier; i > 0; i--)
                    {
                        if (bolt == 0) return;
                        bolt--;
                        Barrier--;
                    }
                    if (bolt>0) this.Defense -= bolt + Hex;
                    break;
                case "freeze":
                    if (invisibleValue > 0)
                    {
                        invisible.Aux--;
                        return;
                    }
                    Delay = 2;
                    break;
                case "hex":
                    if (invisibleValue > 0)
                    {
                        invisible.Aux--;
                        return;
                    }
                    this.Hex += skill.Value;
                    break;
                case "fervor":
                    this.AttackModifier += skill.Value * skill.Aux;
                    break;
                case "weaken":
                    if (invisibleValue > 0)
                    {
                        invisible.Aux--;
                        return;
                    }
                    this.AttackModifier -= skill.Value;
                    if (AttackModified < 0) AttackModifier = -1 * Attack;
                    break;
                case "enhance":
                    string name = skill.Modifiers.FirstOrDefault(x => x.ToLower() != "all");
                    if (name == null) return;

                    foreach(Skill enhancedSkill in Skills.Where(x=>x.Name.ToLower()==name))
                    {
                        if (enhancedSkill.Modifiers.Select(x => x.ToLower()).Contains("all")) continue;
                        enhancedSkill.Enhance = skill.Value;
                        if (name == "invisibility") enhancedSkill.Aux = enhancedSkill.Value;
                    }

                    break;
                default:
                    break;
            }

        }

        public void TakeAttack(Card fromCard)
        {
            int attackValue = fromCard.AttackModified;
            int pierceValue = 0;
            int armorValue = 0;

            if (attackValue == 0) return;

            foreach (Skill skill in fromCard.Skills.Where(x => x.Name.ToLower() == "pierce"))
            {
                pierceValue += skill.Value;
            }

            foreach (Skill skill in this.Skills.Where(x => x.Name.ToLower() == "armor"))
            {
                armorValue += skill.Value;
            }


            if (this.Barrier > 0)
            {
                for (int i = 0; i < Barrier; i++)
                {
                    if (attackValue == 0) break;
                    if (pierceValue == 0)
                    {
                        attackValue--;
                    }
                    else
                    {
                        pierceValue--;
                    }
                    Barrier--;
                }
            }

            //Armor
            if (attackValue > 0)
            {
                for(int j = 0; j < armorValue; j++)
                {
                    if (attackValue == 0) break;
                    if (pierceValue == 0)
                    {
                        attackValue--;
                    }
                    else
                    {
                        pierceValue--;
                    }
                    armorValue--;
                }
            }

            if (attackValue > 0)
            {
                this.defense -= attackValue + Hex;

                //Attacker´s Siphon
                foreach (Skill skill in fromCard.Skills.Where(x => x.Name.ToLower() == "siphon"))
                {
                    fromCard.defense += (attackValue > skill.Value) ? attackValue : skill.Value;
                }
            }

            //Vengance
            foreach (Skill skill in this.Skills.Where(x => x.Name.ToLower() == "vengance"))
            {
                fromCard.defense -= skill.Value;
            }

            if (fromCard.defense < 1) return;

            //Scorch
            if (fromCard.Skills.Where(x => x.Name.ToLower() == "scorch").Count() > 0)
            {
                Scorched = true;
                Scorch += fromCard.Skills.Where(x => x.Name.ToLower() == "scorch").Sum(s => s.Value);
            }

            //Poison
            if (attackValue > 0 && fromCard.Skills.Where(x => x.Name.ToLower() == "poison").Count() > 0) 
            { 
                Poison = fromCard.Skills.Where(x => x.Name.ToLower() == "poison").Max(s => s.Value);
            }


        }
    }
}
