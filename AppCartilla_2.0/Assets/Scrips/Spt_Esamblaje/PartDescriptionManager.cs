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
                    { "Cooler", "Este es el disipador que enfría el procesador. Colócalo primero para evitar sobrecalentamiento." },
                    { "CPU", "El cerebro de la PC. Inserta este chip en el zócalo correspondiente." },
                    { "RAM", "La memoria principal. Añade estos módulos para mejorar el rendimiento." }
                }
            },
            {
                2, new Dictionary<string, string>
                {
                    { "CPU", "El procesador clave. Conecta este chip en el zócalo central." },
                    { "RAM", "Módulos de memoria esenciales. Instálalos en las ranuras adecuadas." },
                    { "GraphicsCard", "La tarjeta gráfica. Colócala para gráficos avanzados." },
                    { "Cooler", "El sistema de enfriamiento. Asegúralo sobre el procesador." }
                }
            },
            {
                3, new Dictionary<string, string>
                {
                    { "RAM", "Memoria vital para el sistema. Colócala en las ranuras designadas." },
                    { "PowerSupply", "La fuente de energía. Conéctala para alimentar todo." },
                    { "CPU", "El núcleo del procesamiento. Instala este chip con cuidado." },
                    { "GraphicsCard", "Tarjeta gráfica potente. Fíjala en su slot." },
                    { "Cooler", "Disipador esencial. Asegúralo sobre el CPU." }
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
