using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Alchemist {
    public class IngredientSpawner : MonoBehaviour {
        [SerializeField] private int startingIngredients = 4;
        [SerializeField] private float timeBetweenSpawns = 1f;
        [SerializeField] [Range(0, 1)] private float possibilityOfSpawningCorrectIngredient = 0.3f;
        [SerializeField] private Vector2 xRange = new Vector2(-5f, 5f);
        [SerializeField] private Vector2 zRange = new Vector2(-5f, 5f);

        private float spawnTimer;

        private void Start() {
            InitializeStartingIngredients();
        }

        private void InitializeStartingIngredients() {
            for (int i = 0; i < startingIngredients; i++) {
                SpawnRandomIngredient();
            }
        }

        private void Update() {
            if (GameManager.Instance.IsGameOver) return;
            UpdateSpawnTimer();
        }

        private void UpdateSpawnTimer() {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= timeBetweenSpawns) {
                SpawnRandomIngredient();
                spawnTimer = 0f;
            }
        }

        private void SpawnRandomIngredient() {
            Vector3 spawnPosition = GetRandomPosition();

            for (int attempts = 0; attempts < 3; attempts++) {
                float randomValue = Random.value;
                // Obtener un ingrediente aleatorio del enum (dando prioridad a los correctos).
                IngredientType randomIngredient = randomValue < possibilityOfSpawningCorrectIngredient
                    ? GameManager.Instance.CurrentIngredient
                    : (IngredientType)Random.Range(0, System.Enum.GetValues(typeof(IngredientType)).Length);

                // Intentar spawnear el ingrediente
                GameObject spawned =
                    IngredientsPool.Instance.SpawnFromPool(randomIngredient, spawnPosition, Quaternion.identity);
                if (spawned != null) {
                    return; // Si logró spawnear, salir del método
                }
            }

            // No se logró spawnear ningún objeto, ignorar este spawn
            Debug.LogWarning("No se pudo spawnear un ingrediente después de 3 intentos.");
        }

        private Vector3 GetRandomPosition() {
            float randomX = Random.Range(xRange.x, xRange.y) + transform.position.x;
            float randomZ = Random.Range(zRange.x, zRange.y) + transform.position.z;
            return new Vector3(randomX, transform.position.y, randomZ);
        }


        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Vector3 center = new Vector3(
                transform.position.x + (xRange.x + xRange.y) / 2,
                transform.position.y,
                transform.position.z + (zRange.x + zRange.y) / 2
            );
            Vector3 size = new Vector3(xRange.y - xRange.x, 0, zRange.y - zRange.x);
            Gizmos.DrawWireCube(center, size);
        }
    }
}