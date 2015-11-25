using System;
using System.Collections.Generic;

namespace Engine.Models
{
    public enum SkillCat
    {
        Activated,
        Triggered,
        TurnActivated,
        CloseTurn
    }

    public class Skill
    {
        public string Name { get; set; }
        public SkillCat Category{ get; set; }

        protected int skillvalue = 0;
        public int Value
        {
            get
            {
                return this.skillvalue + this.Enhance;
            }
            set
            {
                skillvalue = value;
            }
        }
        public int Aux { get; set; }
        public int Enhance { get; set; }

        public List<string> Modifiers { get; set; }

        public Skill()
        {
            this.Modifiers = new List<string>();
        }

        public void LoadSkillFromText(String skillText)
        {
            String[] elementos = skillText.Split(',');

            this.Name = elementos[0].Trim();
            this.Value = Convert.ToInt32(elementos[1].Trim());
            string name = this.Name.ToLower();
            this.Aux = this.Value;

            elementos = this.Name.Split(' ');

            if (elementos.Length > 1)
            {
                this.Name = elementos[0];

                for (int i = 1; i < elementos.Length; i++)
                {
                    this.Modifiers.Add(elementos[i]);
                }
            }

            this.Category = SkillCategoryByName(this.Name);
        }

        private SkillCat SkillCategoryByName(string name)
        {
            SkillCat cat = SkillCat.Activated;

            switch (name.ToLower())
            {
                case "empower":
                    cat = SkillCat.TurnActivated;
                    break;
                case "poison":
                    cat = SkillCat.Triggered;
                    break;
                default:
                    break;
            }

            return cat;
        }
    }
}