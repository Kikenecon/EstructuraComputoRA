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
                    { "Cooler", "Este es el disipador que enfr�a el procesador. Col�calo primero para evitar sobrecalentamiento." },
                    { "CPU", "El cerebro de la PC. Inserta este chip en el z�calo correspondiente." },
                    { "RAM", "La memoria principal. A�ade estos m�dulos para mejorar el rendimiento." }
                }
            },
            {
                2, new Dictionary<string, string>
                {
                    { "CPU", "El procesador clave. Conecta este chip en el z�calo central." },
                    { "RAM", "M�dulos de memoria esenciales. Inst�lalos en las ranuras adecuadas." },
                    { "GraphicsCard", "La tarjeta gr�fica. Col�cala para gr�ficos avanzados." },
                    { "Cooler", "El sistema de enfriamiento. Aseg�ralo sobre el procesador." }
                }
            },
            {
                3, new Dictionary<string, string>
                {
                    { "RAM", "Memoria vital para el sistema. Col�cala en las ranuras designadas." },
                    { "PowerSupply", "La fuente de energ�a. Con�ctala para alimentar todo." },
                    { "CPU", "El n�cleo del procesamiento. Instala este chip con cuidado." },
                    { "GraphicsCard", "Tarjeta gr�fica potente. F�jala en su slot." },
                    { "Cooler", "Disipador esencial. Aseg�ralo sobre el CPU." }
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
