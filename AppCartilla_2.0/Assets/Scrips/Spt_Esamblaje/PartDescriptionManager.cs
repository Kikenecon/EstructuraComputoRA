using UnityEngine;
using System.Collections.Generic;

namespace AssemblyGame
{
    public class PartDescriptionManager : MonoBehaviour
    {
        private static PartDescriptionManager instance = null;

        private Dictionary<int, Dictionary<string, string>> partDescriptions = new Dictionary<int, Dictionary<string, string>>
        {
            {
                1, new Dictionary<string, string>
                {
                    { "Cooler", "Sin esta pieza, el calor del trabajo se acumular�a en el coraz�n del sistema." },
                    { "CPU", "Este peque�o cuadrado coordina todas las instrucciones que circulan dentro del equipo." },
                    { "RAM", "Aqu� se almacenan temporalmente los pensamientos de la m�quina mientras trabaja." }
                }
            },
            {
                2, new Dictionary<string, string>
                {
                    { "CPU", "Es el primer componente que debe recibir energ�a para que todo funcione con l�gica." },
                    { "RAM", "Si este m�dulo no est� presente, el sistema olvida lo que hace apenas lo intenta." },
                    { "GraphicsCard", "Permite a la m�quina imaginar im�genes, juegos y videos con gran detalle." },
                    { "Cooler", "Aunque no piensa ni procesa, su tarea es vital para que el centro no colapse por calor." }
                }
            },
            {
                3, new Dictionary<string, string>
                {
                    { "RAM", "Se ubica cerca del procesador y act�a como su memoria inmediata." },
                    { "PowerSupply", "Sin esta pieza, ninguna otra recibir�a vida ni electricidad." },
                    { "CPU", "Coordina cada proceso, pero es in�til si no est� rodeado de los elementos correctos." },
                    { "GraphicsCard", "Traductor visual entre los datos y tu pantalla." },
                    { "Cooler", "Silencioso pero esencial: protege al componente m�s caliente del sistema." }
                }
            }
        };

        public static PartDescriptionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PartDescriptionManager>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject("PartDescriptionManager");
                        instance = obj.AddComponent<PartDescriptionManager>();
                        DontDestroyOnLoad(obj);
                    }
                }
                return instance;
            }
        }

        public string GetDescription(int level, string part)
        {
            if (partDescriptions.ContainsKey(level) && partDescriptions[level].ContainsKey(part))
            {
                return partDescriptions[level][part];
            }
            return $"Coloca el {part} (descripci�n no disponible)";
        }
    }
}
