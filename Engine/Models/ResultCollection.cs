using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class ResultCollection
    {

        protected SortedDictionary<int, Result> resultados = new SortedDictionary<int, Result>();

        public List<Result> Results
        {
            get
            {
                return resultados.Values.ToList();
            }
        }

        public int Count
        {
            get
            {
                return resultados.Count;
            }
        }

        public void Add(Result resultado)
        {
            if (resultados.ContainsKey(resultado.Attacker.Id))
            {
                resultados[resultado.Attacker.Id].Record(resultado);
            }
            else
            {
                resultados.Add(resultado.Attacker.Id, resultado);
            }
        }
    }
}
