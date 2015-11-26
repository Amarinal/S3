using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Flows;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }

        protected List<Card> deckOriginal;
        protected List<Card> deck;
        public List<Card> Deck { get { return deck; } }
        protected List<Card> hand;
        public List<Card> Hand { get { return hand; } }

        public List<Skill> Skills { get; set; }

        protected string hero;
        public String Hero
        {
            get
            {
                return hero;
            }
        }

        protected int maxHitPoints = 0;
        protected int hitPoints = 0;
        public int HitPoints
        {
            get
            {
                return hitPoints;
            }
            set
            {
                hitPoints = value;
                if (maxHitPoints == 0)
                {
                    maxHitPoints = hitPoints;
                }
            }
        }

        public bool IsAlive
        {
            get
            {
                if (this.hitPoints < 1) return false;
                return true;
            }
        }

        public Player()
        {
            this.Skills = new List<Skill>();
        }

        public Player(int id, String heroName, List<Card> newDeck)
        {
            this.deckOriginal = newDeck.OrderBy(x=>x.Id).ToList();
            SetHero(heroName);
            this.Id = id;
            this.Name = "Var" + id.ToString();
        }

        public void Init()
        {
            Init(deckOriginal);
        }

        protected void Init(List<Card> initDeck)
        {
            this.HitPoints = this.maxHitPoints;
            this.deck = new List<Card>();
            this.hand = new List<Card>();

            foreach (Card item in initDeck)
            {
                this.deck.Add(item.Clone());
            }
        }

        public void RandomDeck(CardCollection cards, int cardNumber=15)
        {
            Random random = new Random();
            deckOriginal = new List<Card>();

            for (int i = 0; i < cardNumber; i++)
            {
                deckOriginal.Add(cards.Cards[random.Next(0, cards.Cards.Count - 1)].Clone());
            }

            this.Init();
        }

        public void LoadFromCsv (HelperCsv csv, int line, CardCollection cards)
        {
            this.deckOriginal = new List<Card>();
            this.Id = line;
            this.Name = csv[line, "Name"];
            //this.HitPoints = Convert.ToInt32(csv[line, "Hitpoints"]);
            Card card = null;

            string[] carta;

            for(int i = 1; i < 16; i++)
            {
                carta = csv[line, "Card" + i.ToString()].Split(';');

                int level = 0;

                if (carta.Length > 1) level = Convert.ToInt32(carta[1]);

                card = cards.GetCard(carta[0], level);
                if (card != null) this.deckOriginal.Add(card);
            }

            this.deckOriginal = deckOriginal.OrderBy(x => x.Id).ToList();

            string hero = csv[line, "Hero"];
            SetHero(hero);

        }

        public void SetHero (string heroName)
        {
            Skill skill = null;
            this.Skills = new List<Skill>();
            this.maxHitPoints = 0;
            hero = heroName;

            switch (heroName)
            {
                case "Groc":
                    this.HitPoints = 53;

                    skill = new Skill();
                    skill.LoadSkillFromText("Enhance All invisibility,1");
                    Skills.Add(skill);
                    skill = new Skill();
                    skill.LoadSkillFromText("Barrier,2");
                    Skills.Add(skill);
                    skill = new Skill();
                    skill.LoadSkillFromText("Empower All,1");
                    Skills.Add(skill);

                    break;
                case "Lich":
                    this.HitPoints = 48;

                    skill = new Skill();
                    skill.LoadSkillFromText("Bolt All,1");
                    Skills.Add(skill);
                    skill = new Skill();
                    skill.LoadSkillFromText("Enhance Poison,1");
                    Skills.Add(skill);
                    skill = new Skill();
                    skill.LoadSkillFromText("Enhance All Siphon,1");
                    Skills.Add(skill);

                    break;
                case "Ursurio":
                    this.HitPoints = 42;

                    skill = new Skill();
                    skill.LoadSkillFromText("Empower Wyld,2");
                    Skills.Add(skill);
                    skill = new Skill();
                    skill.LoadSkillFromText("Empower Wyld,2");
                    Skills.Add(skill);

                    break;
                case "Samael":
                    this.HitPoints = 49;

                    skill = new Skill();
                    skill.LoadSkillFromText("Enhance All Armor,1");
                    Skills.Add(skill);
                    skill = new Skill();
                    skill.LoadSkillFromText("Enhance Vengance,2");
                    Skills.Add(skill);
                    skill = new Skill();
                    skill.LoadSkillFromText("Weaken All,1");
                    Skills.Add(skill);

                    break;
                case "Decim":
                    this.HitPoints = 49;

                    skill = new Skill();
                    skill.LoadSkillFromText("Enhance All Vengance,1");
                    Skills.Add(skill);
                    skill = new Skill();
                    skill.LoadSkillFromText("Bolt,1");
                    Skills.Add(skill);
                    skill = new Skill();
                    skill.LoadSkillFromText("Bolt,1");
                    Skills.Add(skill);

                    break;
                default:
                    LoadFromText(heroName);
                    break;
            }
        }

        public void LoadFromText(string heroString)
        {
            //Hitpoints;Skill;Skill...



            try
            {
                string[] parts = heroString.Split('-');
                Skill skill = null;
                this.HitPoints = Convert.ToInt32(parts[0]);

                for (int i = 1; i < parts.Length; i++)
                {
                    skill = new Skill();
                    skill.LoadSkillFromText(parts[i]);
                    Skills.Add(skill);
                }
            }
            catch (Exception ex)
            {
                this.maxHitPoints = 0;
                this.HitPoints = 50;
                Skills.Clear();
            }

        }

        public override string ToString()
        {
            return String.Format("{0}.{1}", HitPoints, Name);
        }

        public Card RandomHandCard(bool activeCardOnly = true)
        {
            Card card=null;
            
            List<int> lista = new List<int>();

            for (int i = 0; i < hand.Count; i++)
            {
                lista.Add(i);
                if (activeCardOnly && hand[i].Delay > 0) lista.Remove(lista.Count - 1);
            }

            if (lista.Count > 0)
            {
                Random random = new Random();
                card = hand[lista[random.Next(0, lista.Count - 1)]];
            }

            return card;
        }

        public List<Card> CardsForSkill(Skill skill, Card fromCard=null)
        {
            int targetDelay = 0;
            int fromCardIndex = -1;
            string skillname = skill.Name.ToLower();
            List<Card> activeList = new List<Card>();
            List<Card> factionList = new List<Card>();
            List<Card> list = new List<Card>();

            switch (skillname)
            {
                case "armor":
                    targetDelay = -1;
                    break;
                case "freeze":
                    targetDelay = 1;
                    if (skill.Aux < skill.Value)
                    {
                        skill.Aux++;
                        return list;
                    }
                    break;
                case "dualstrike":
                    if (skill.Aux < skill.Value)
                    {
                        skill.Aux++;       
                    }
                    return list;
                case "weaken":
                    targetDelay = 1;
                    break;
                case "hex":
                case "bolt":
                case "barrier":
                    targetDelay = 5;
                    break;
                case "enhance":
                case "legion":
                case "empowerment":
                case "heal":
                default:
                    targetDelay = 0;
                    break;
            }

            if (skillname == "fervor" || skillname == "legion")
            {

                if (!skill.Modifiers.Select(x => x.ToLower()).Contains("all")) skill.Modifiers.Add("all");

                for (int i = 0; i < hand.Count; i++)
                {
                    if (fromCard != null && hand[i] == fromCard)
                    {
                        fromCardIndex = i;
                        if (i > 0)
                        {
                            activeList.Add(hand[i - 1]);
                            factionList.Add(hand[i - 1]);
                        }

                        if (i < hand.Count - 1)
                        {
                            activeList.Add(hand[i + 1]);
                            factionList.Add(hand[i + 1]);
                        }

                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < hand.Count; i++)
                {

                    if (hand[i].Defense > 0 && hand[i].Delay <= targetDelay)
                    {

                        activeList.Add(hand[i]);
                        factionList.Add(hand[i]);
                    }
                }
            }

            foreach (String modifier in skill.Modifiers)
            {
                if (modifier.ToLower() == "all") continue;

                foreach (Card card in activeList)
                {
                    if (!card.Factions.Select(x => x.ToLower()).Contains(modifier.ToLower()))
                    {
                        factionList.Remove(card);
                    }
                }
            }

            //Heal can only target damaged cards
            if (skillname == "Heal") factionList = factionList.Where(x => x.Defense < x.DefenseMax).ToList();

            //Weaken can only target cards with attack>0
            if (skillname == "weaken") factionList = factionList.Where(x => x.AttackModified > 0).ToList();

            //legion only affect active cards
            if (skillname == "legion")
            {
                foreach (Card card in activeList)
                {
                    if (card.Delay > 0 || card.Defense < 1) factionList.Remove(card);
                }  
            }

            //fervor only affect my card
            if (skillname == "fervor")
            {
                skill.Aux = factionList.Count;
                factionList = new List<Card>();
                factionList.Add(fromCard);
            }


            if (skill.Modifiers.Select(x => x.ToLower()).Contains("all"))
            {
                list = factionList;
            }
            else
            {
                if (factionList.Count > 0)
                {
                    Random random = new Random();
                    list.Add(factionList[random.Next(factionList.Count)]);
                }
            }

            return list;
        }

        public List<Card> CardsForHeroSkill(Skill skill)
        {
            //int targetDelay = 0;
            //int fromCardIndex = -1;
            string skillname = skill.Name.ToLower();
            //List<Card> activeList = new List<Card>();
            //List<Card> factionList = new List<Card>();
            List<Card> list = new List<Card>();

            switch (skillname)
            {
                case "bolt":
                case "empower":
                case "barrier":
                    list = CardsForSkill(skill, null);
                    break;
                case "enhance":
                    foreach(Card card in hand)
                    {
                        if(card.Defense>0 && card.Delay < 1)
                        {
                            foreach(String skillName in skill.Modifiers)
                            {
                                if (card.Skills.Where(s=>s.Name.ToLower().Contains(skillName.ToLower())).Count()>0)
                                {
                                    list.Add(card);
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }


            return list;
        }

        public void TakeAttack(Card fromCard)
        {
            this.hitPoints -= fromCard.AttackModified;

            if (this.hitPoints < 1)
            {
                throw new DeadPlayerExeption(this);
            }

            //Vengance
            foreach (Skill skill in this.Skills.Where(x => x.Name.ToLower() == "vengance"))
            {
                fromCard.Defense -= skill.Value;
            }
        }

        public void ChangeDeck(List<Card> newDeck)
        {
            if (this.hand.Count > 0) throw new Exception("Player not in default state!");

            this.deckOriginal = newDeck;

            this.Init();
        }

        public Player Clone()
        {
            
            Player newPlayer = new Player(Id, Hero,deckOriginal.Select(x=>x.Clone()).ToList());

            newPlayer.Name = Name;

            return newPlayer;
        }

        public String ToFullString()
        {
            StringBuilder cadena = new StringBuilder();

            cadena.AppendFormat("({0}).{1}.{2} {3}", Id, HitPoints, Name, hero);
            cadena.AppendLine();

            foreach(Card card in deckOriginal)
            {
                cadena.AppendLine(card.ToString());
            }

            return cadena.ToString();
        }
    }
}
