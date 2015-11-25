using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Result
    {
        public Guid Id { get; set; }
        public Player Attacker { get; set; }
        public Player Defender { get; set; }

        public Result()
        {
            this.Id = new Guid();
        }

        public int WinnerId
        {
            get
            {
                return this.Attacker.Id;
            }
        }

        public Double Wins
        {
            get
            {
                return (double)winCounter / (double)battleCounter;
            }
        }

        public int BattleCount
        {
            get
            {
                return battleCounter;
            }
        }

        public int WinCount
        {
            get
            {
                return winCounter;
            }
        }

        protected int battleCounter = 0;
        protected int winCounter = 0;

        public void Record(Result newResult)
        {
            battleCounter += newResult.BattleCount;
            if (newResult.WinnerId == this.WinnerId)
            {
                winCounter += newResult.WinCount;
            }
        }

        public void RecordLoss(int id)
        {
            battleCounter++;
            if (id != this.WinnerId)
            {
                winCounter++;
            }
        }
    }
}
