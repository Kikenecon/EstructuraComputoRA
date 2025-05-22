using UnityEngine;

namespace AssemblyGame
{
    public abstract class FaultCreator
    {
        public abstract IFault CreateFault(GameObject faultPrefab);

        protected Vector2 GetRandomSpawnPosition()
        {
            // Obtener el Canvas de la escena
            Canvas canvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("Canvas no encontrado. Usando dimensiones por defecto.");
                return Vector2.zero;
            }

            // Obtener las dimensiones del Canvas ajustadas por el CanvasScaler
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 canvasSize = canvasRect.sizeDelta;
            float canvasWidth = canvasSize.x / 2f; // Mitad del ancho
            float canvasHeight = canvasSize.y / 2f; // Mitad del alto

            // Ajustar las posiciones para que estén justo dentro de los márgenes del Canvas
            int side = Random.Range(0, 4);
            float offset = 50f; // Margen de seguridad para que no se generen justo en el borde

            Vector2 spawnPosition = side switch
            {
                0 => new Vector2(-canvasWidth + offset, Random.Range(-canvasHeight + offset, canvasHeight - offset)), // Izquierda
                1 => new Vector2(canvasWidth - offset, Random.Range(-canvasHeight + offset, canvasHeight - offset)),   // Derecha
                2 => new Vector2(Random.Range(-canvasWidth + offset, canvasWidth - offset), canvasHeight - offset),   // Arriba
                3 => new Vector2(Random.Range(-canvasWidth + offset, canvasWidth - offset), -canvasHeight + offset),  // Abajo
                _ => Vector2.zero
            };

            Debug.Log($"Spawn position generated: {spawnPosition} (Canvas size: {canvasSize})");
            return spawnPosition;
        }
    }
}