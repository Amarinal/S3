using Engine.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Combat
    {
        private const int MAX_TURNS = 100;

        Player Attacker { get; set; }
        Player Defender { get; set; }
        int Repeat { get; set; }

        int turn;

        Card tower = null;

        protected CombatOptions combatOptions;

        public bool IsInProgress
        {
            get
            {
                return (Attacker.IsAlive && Defender.IsAlive);
            }
        }

        public Combat()
        {

        }

        public Combat(Player att, Player def)
        {
            this.Attacker = att;
            this.Defender = def;
        }

        public Combat(Player att, Player def, int repeat) : this(att, def)
        {
            this.Repeat = repeat;
        } 

        public void InitCombat(CombatOptions options = null)
        {
            if (options == null) options = new CombatOptions();
            combatOptions = options;

            Attacker.Init();
            Defender.Init();

            foreach(Card card in combatOptions.DefenderCards) { Defender.Hand.Add(card); }
            foreach (Card card in combatOptions.AttackerCards) { Attacker.Hand.Add(card); }
        }

        public Result Resolve(CombatOptions options=null, int? repeat = null)
        {
            Result result = new Result();
            result.Attacker = Attacker;
            result.Defender = Defender;

            bool isAttackerTurn = true;

            if (repeat == null) repeat = Repeat;

            if (options == null) options = new CombatOptions();
            this.combatOptions = options;

            for (int i = 0; i < repeat; i++)
            {
                turn = 0;
                Attacker.Init();
                Defender.Init();

                foreach (Card card in combatOptions.DefenderCards)
                {
                    Defender.Hand.Add(card.Clone());
                }

                foreach (Card card in combatOptions.AttackerCards)
                {
                    Attacker.Hand.Add(card);
                }

                while (Attacker.IsAlive && Defender.IsAlive)
                {
                    turn++;
                    isAttackerTurn = (turn % 2) == 1;

                    try
                    {
                        if (isAttackerTurn)
                        {
                            DoTurn(Attacker, Defender);
                        }
                        else
                        {
                            DoTurn(Defender, Attacker);
                        }
                    }
                    catch (DeadPlayerExeption dpex)
                    {
                        result.RecordLoss(dpex.DeadPlayer.Id);
                        break;
                    }

                    if (turn > MAX_TURNS)
                    {
                        result.RecordLoss(Attacker.Id);
                        break;
                    }

                }

            }

            return result;
        }



        public void StepTurn()
        {
            turn++;
            bool isAttackerTurn = ((turn % 2) == 1);

            if (!Attacker.IsAlive || !Defender.IsAlive) return;

            try
            {
                if (isAttackerTurn)
                {
                    DoTurn(Attacker, Defender);
                }
                else
                {
                    DoTurn(Defender, Attacker);
                }
            }
            catch (DeadPlayerExeption dpex)
            {
                //Do something?
            }

            //Finishing game if it gets never ending.
            if (turn > MAX_TURNS) Attacker.HitPoints = 0;
        }

        public void DoTurn(Player attacker, Player defender)
        {
            InitTurn(attacker, defender);

            #region Draw a Card
            if (attacker.Deck.Count > 0)
            {
                Random random = new Random();
                int nextCard = random.Next(attacker.Deck.Count);
                attacker.Hand.Add(attacker.Deck[nextCard].Clone());
                attacker.Deck.RemoveAt(nextCard);
            }
            #endregion

            #region Battleeffects
            foreach (Skill skill in combatOptions.BattleEffects)
            {
                foreach (Card card in attacker.CardsForHeroSkill(skill))
                {
                    card.TakeSkill(skill);
                }
            }
            #endregion

            #region Hero Skills
            foreach (Skill skill in attacker.Skills)
            {
                foreach (Card card in attacker.CardsForHeroSkill(skill))
                {
                    card.TakeSkill(skill);
                }
            }
            #endregion

            #region OpenTurn Skills
            foreach (Card card in attacker.Hand.Where(x => x.Delay < 1 && x.Defense > 0 && x.Skills.Where(s => s.Category == SkillCat.TurnActivated).Count() > 0))
            {
                foreach (Skill skill in card.Skills.Where(s => s.Category == SkillCat.TurnActivated))
                {
                    foreach (Card target in this.CardsForSkill(attacker, skill, card))
                    {
                        target.TakeSkill(skill);
                    }
                }
            }

            #endregion

            #region Attack Skills & Attack
            for (int i = 0; i < attacker.Hand.Count; i++)
            {
                if (attacker.Hand[i].Delay > 0) continue;

                DoCardAttack(attacker, defender, i);

                if (attacker.Hand[i].Skills.Any(s => s.Name.ToLower() == "dualstrike"))
                {
                    foreach (Skill skill in attacker.Hand[i].Skills.Where(s => s.Name.ToLower() == "dualstrike"))
                    {
                        if (skill.Aux == skill.Value)
                        {
                            skill.Aux = -1;
                            DoCardAttack(attacker, defender, i);
                        }
                    }
                }
            }
            #endregion

            #region AfterAttack effects & dead recolection
            foreach (Card affectedCard in attacker.Hand)
            {
                if (affectedCard.Scorch > 0)
                {
                    affectedCard.Defense -= affectedCard.Scorch;
                    if (!affectedCard.Scorched) affectedCard.Scorch = 0;
                }

                affectedCard.Defense -= affectedCard.Poison;
            }

            for (int i = 14; i >= 0; i--)
            {
                if (attacker.Hand.Count <= i) continue;
                if (attacker.Hand[i].Defense < 1) attacker.Hand.RemoveAt(i);
            }

            for (int i = 14; i >= 0; i--)
            {
                if (defender.Hand.Count <= i) continue;
                if (defender.Hand[i].Defense < 1) defender.Hand.RemoveAt(i);
            }
            #endregion
        }

        protected void DoCardAttack(Player attacker, Player defender, int pos)
        {
            if (attacker.Hand[pos].Delay > 0) return;

            foreach (Skill skill in attacker.Hand[pos].Skills.Where(s => s.Category == SkillCat.Activated))
            {
                foreach (Card card in this.CardsForSkill(attacker, skill, attacker.Hand[pos]))
                {
                    card.TakeSkill(skill);
                }
            }

            if (defender.Hand.Count > pos)
            {
                defender.Hand[pos].TakeAttack(attacker.Hand[pos]);
            }
            else
            {
                defender.TakeAttack(attacker.Hand[pos]);
            }
        }

        protected void InitTurn(Player attacker, Player defender)
        {
            foreach (Card card in attacker.Hand)
            {
                card.Barrier = 0;
                card.AttackModifier = 0;
                if (card.Delay > 0) card.Delay--;
                card.Hex = 0;
                foreach (Skill skill in card.Skills)
                {
                    skill.Enhance = 0;
                }
            }

            foreach (Card card in defender.Hand)
            {
                card.Scorched = false;
                foreach (Skill skill in card.Skills)
                {
                    skill.Enhance = 0;
                }
                foreach (Skill skill in card.Skills.Where(x => x.Name.ToLower() == "invisibility"))
                {
                    skill.Aux = skill.Value;
                }

            }

        }

        public List<Card> CardsForSkill(Player player, Skill skill, Card fromCard)
        {
            bool targetAttacker = Attacker.Id == player.Id;

            string name = skill.Name.ToLower();

            bool targetShelf = name == "barrier"
                                || name == "empower" || name == "heal"
                                || name == "legion" || name == "fervor";

            targetAttacker = targetAttacker ? targetShelf : !targetShelf;

            return targetAttacker ? Attacker.CardsForSkill(skill, fromCard) : Defender.CardsForSkill(skill, fromCard);
        }


        #region Outputs
        public string StateString()
        {
            StringBuilder state = new StringBuilder();

            int anchoTotal = 80;
            int anchoParcial = (anchoTotal / 2) - 1;

            state.AppendLine(String.Format("Turn:{0} Atacker:{1}", turn, ((turn % 2) > 0) ? Attacker.Name : Defender.Name));

            state.Append(Attacker.ToString().PadRight(anchoParcial));
            state.Append(Defender.ToString().PadRight(anchoParcial));
            state.AppendLine();

            for (int i = 0; i < 15; i++)
            {
                if (i >= Attacker.Hand.Count && i >= Defender.Hand.Count) break;

                if (i < Attacker.Hand.Count) state.Append(Attacker.Hand[i].ToString().PadRight(anchoParcial));
                if (i >= Attacker.Hand.Count) state.Append("-".PadRight(anchoParcial));
                if (i < Defender.Hand.Count) state.Append(Defender.Hand[i].ToString().PadRight(anchoParcial));
                state.AppendLine();

            }

            return state.ToString();
        }
        #endregion
    }
}
