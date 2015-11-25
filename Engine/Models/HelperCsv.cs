using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class HelperCsv
    {
        Dictionary<String, int> listaColumnas;
        List<String> columnas;
        List<List<String>> lineas;

        public int Count
        {
            get
            {
                return this.lineas.Count;
            }
        }

        public HelperCsv(String path)
        {
            String linea;
            String[] elementos;
            StreamReader fichero = File.OpenText(path);

            columnas = new List<string>();
            linea = fichero.ReadLine();
            elementos = linea.Split(';');
            foreach (String elemento in elementos)
            {
                columnas.Add(elemento);
            }

            lineas = new List<List<string>>();
            while (!fichero.EndOfStream)
            {
                linea = fichero.ReadLine();
                elementos = linea.Split(';');
                lineas.Add(elementos.ToList<String>());
            }
        }

        public List<String> this[int i]
        {
            get
            {
                return this.lineas[i];
            }
        }

        public String this[int m,int n]
        {
            get
            {
                return this.lineas[m][n];
            }
        }

        public String this [int l, string c]
        {
            get
            {
                if (this.listaColumnas == null)
                {
                    this.listaColumnas = new Dictionary<string, int>();
                    int indice = 0;
                    foreach (String columna in this.columnas)
                    {
                        this.listaColumnas.Add(columna, indice);
                        indice++;
                    }
                }
                int i = this.listaColumnas[c];

                return this.lineas[l][i];

            }
        }

    }


}
