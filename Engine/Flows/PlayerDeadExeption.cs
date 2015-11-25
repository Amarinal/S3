using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Flows
{
    public class DeadPlayerExeption : Exception
    {
        public Player DeadPlayer { get; set; }

        public DeadPlayerExeption() : base()
        {

        }

        public DeadPlayerExeption(Player player) : base()
        {
            this.DeadPlayer = player;
        }

    }
}
