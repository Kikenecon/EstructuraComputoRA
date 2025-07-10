using UnityEngine;

namespace AssemblyGame
{
    public abstract class FaultCreator
    {
        public abstract IFault CreateFault(GameObject faultPrefab);

        protected Vector2 GetRandomSpawnPosition()
        {
            float screenWidth = Camera.main.orthographicSize * Camera.main.aspect * 2f;
            float screenHeight = Camera.main.orthographicSize * 2f;
            float offset = 1f;

            int side = Random.Range(0, 4);
            Vector2 spawnPosition = side switch
            {
                0 => new Vector2(-screenWidth / 2 + offset, Random.Range(-screenHeight / 2 + offset, screenHeight / 2 - offset)),
                1 => new Vector2(screenWidth / 2 - offset, Random.Range(-screenHeight / 2 + offset, screenHeight / 2 - offset)),
                2 => new Vector2(Random.Range(-screenWidth / 2 + offset, screenWidth / 2 - offset), screenHeight / 2 - offset),
                3 => new Vector2(Random.Range(-screenWidth / 2 + offset, screenWidth / 2 - offset), -screenHeight / 2 + offset),
                _ => Vector2.zero
            };

            return spawnPosition;
        }
    }
}


//using UnityEngine;

//namespace AssemblyGame
//{
//    public abstract class FaultCreator
//    {
//        public abstract IFault CreateFault(GameObject faultPrefab);

//        protected Vector2 GetRandomSpawnPosition()
//        {
//            Canvas canvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
//            if (canvas == null)
//            {
//                Debug.LogWarning("Canvas no encontrado. Usando dimensiones por defecto.");
//                return Vector2.zero;
//            }

//            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
//            Vector2 canvasSize = canvasRect.sizeDelta;
//            float canvasWidth = canvasSize.x / 2f;
//            float canvasHeight = canvasSize.y / 2f;
//            float offset = 50f;

//            int side = Random.Range(0, 4);
//            Vector2 spawnPosition = side switch
//            {
//                0 => new Vector2(-canvasWidth + offset, Random.Range(-canvasHeight + offset, canvasHeight - offset)),
//                1 => new Vector2(canvasWidth - offset, Random.Range(-canvasHeight + offset, canvasHeight - offset)),
//                2 => new Vector2(Random.Range(-canvasWidth + offset, canvasWidth - offset), canvasHeight - offset),
//                3 => new Vector2(Random.Range(-canvasWidth + offset, canvasWidth - offset), -canvasHeight + offset),
//                _ => Vector2.zero
//            };

//            return spawnPosition;
//        }
//    }
//}


//using UnityEngine;

//namespace AssemblyGame
//{
//    public abstract class FaultCreator
//    {
//        public abstract IFault CreateFault(GameObject faultPrefab);

//        protected Vector2 GetRandomSpawnPosition()
//        {
//            // Usar las dimensiones de la pantalla en espacio 2D
//            float screenWidth = Camera.main.orthographicSize * Camera.main.aspect * 2f;
//            float screenHeight = Camera.main.orthographicSize * 2f;
//            float offset = 1f; // Margen de seguridad

//            int side = Random.Range(0, 4);
//            Vector2 spawnPosition = side switch
//            {
//                0 => new Vector2(-screenWidth / 2 + offset, Random.Range(-screenHeight / 2 + offset, screenHeight / 2 - offset)), // Izquierda
//                1 => new Vector2(screenWidth / 2 - offset, Random.Range(-screenHeight / 2 + offset, screenHeight / 2 - offset)),   // Derecha
//                2 => new Vector2(Random.Range(-screenWidth / 2 + offset, screenWidth / 2 - offset), screenHeight / 2 - offset),   // Arriba
//                3 => new Vector2(Random.Range(-screenWidth / 2 + offset, screenWidth / 2 - offset), -screenHeight / 2 + offset),  // Abajo
//                _ => Vector2.zero
//            };

//            Debug.Log($"Spawn position generated: {spawnPosition} (Screen size: {screenWidth}x{screenHeight})");
//            return spawnPosition;
//        }
//    }
//}