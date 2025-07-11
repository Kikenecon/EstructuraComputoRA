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
                    { "Cooler", "Sin esta pieza, el calor del trabajo se acumularía en el corazón del sistema." },
                    { "CPU", "Este pequeño cuadrado coordina todas las instrucciones que circulan dentro del equipo." },
                    { "RAM", "Aquí se almacenan temporalmente los pensamientos de la máquina mientras trabaja." }
                }
            },
            {
                2, new Dictionary<string, string>
                {
                    { "CPU", "Es el primer componente que debe recibir energía para que todo funcione con lógica." },
                    { "RAM", "Si este módulo no está presente, el sistema olvida lo que hace apenas lo intenta." },
                    { "GraphicsCard", "Permite a la máquina imaginar imágenes, juegos y videos con gran detalle." },
                    { "Cooler", "Aunque no piensa ni procesa, su tarea es vital para que el centro no colapse por calor." }
                }
            },
            {
                3, new Dictionary<string, string>
                {
                    { "RAM", "Se ubica cerca del procesador y actúa como su memoria inmediata." },
                    { "PowerSupply", "Sin esta pieza, ninguna otra recibiría vida ni electricidad." },
                    { "CPU", "Coordina cada proceso, pero es inútil si no está rodeado de los elementos correctos." },
                    { "GraphicsCard", "Traductor visual entre los datos y tu pantalla." },
                    { "Cooler", "Silencioso pero esencial: protege al componente más caliente del sistema." }
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
            return $"Coloca el {part} (descripción no disponible)";
        }
    }
}
