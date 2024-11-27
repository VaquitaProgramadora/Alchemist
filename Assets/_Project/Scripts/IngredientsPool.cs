using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Alchemist {
    public class IngredientsPool : MonoBehaviour {
        [System.Serializable]
        public class Pool {
            [EnumToggleButtons] public IngredientType ingredientType; // Tipo de ingrediente.
            public int initialSize = 5; // Número inicial de instancias preinstanciadas.
        }

        [SerializeField] private List<Pool> pools; // Lista de pools configurados en el inspector.
        [SerializeField] private int maxObjects = 5; // Máximo global de objetos activos.

        private Dictionary<GameObject, Queue<GameObject>>
            _poolDictionary; // Diccionario que guarda los objetos disponibles.

        private HashSet<GameObject> _activeObjects; // Conjunto de objetos actualmente activos.

        public static IngredientsPool Instance { get; private set; }

        private void Awake() {
            InitializeSingleton();
            InitializePools();
        }

        // Configura el singleton de este componente.
        private void InitializeSingleton() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        // Inicializa los pools de ingredientes a partir de los datos configurados.
        private void InitializePools() {
            _poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
            _activeObjects = new HashSet<GameObject>();

            foreach (Pool pool in pools) {
                Queue<GameObject> objectQueue = new();
                GameObject prefab = IngredientsDB.Instance.ingredients.FirstOrDefault(
                    ingredient => ingredient.ingredientType == pool.ingredientType)?.prefab;
                if (prefab == null) {
                    Debug.LogError($"No se encontró prefab para el tipo de ingrediente: {pool.ingredientType}");
                    continue;
                }

                for (int i = 0; i < pool.initialSize; i++) {
                    GameObject obj = Instantiate(prefab);
                    obj.SetActive(false);
                    objectQueue.Enqueue(obj);
                }

                _poolDictionary.Add(prefab, objectQueue);
            }
        }

        // Spawnea un ingrediente desde el pool. Si no hay disponible, devuelve null.
        public GameObject SpawnFromPool(IngredientType ingredientType, Vector3 position, Quaternion rotation) {
            Pool pool = pools.Find(p => p.ingredientType == ingredientType);

            if (pool == null) {
                Debug.LogError($"No existe un pool para el tipo de ingrediente: {ingredientType}");
                return null;
            }

            GameObject prefab = IngredientsDB.Instance.ingredients.FirstOrDefault(
                ingredient => ingredient.ingredientType == pool.ingredientType)?.prefab;
            if (prefab == null) {
                Debug.LogError($"No se encontró prefab para el tipo de ingrediente: {pool.ingredientType}");
                return null;
            }

            Queue<GameObject> objectQueue = _poolDictionary[prefab];
            GameObject obj;
            if (objectQueue.Count > 0) {
                obj = objectQueue.Dequeue();
            } else {
                Debug.LogWarning($"El pool de {pool.ingredientType} está vacío.");
                return null;
            }

            obj.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            _activeObjects.Add(obj);

            return obj;
        }

        // Devuelve un objeto al pool al que pertenece.
        public void ReturnToPool(GameObject obj) {
            if (!_activeObjects.Contains(obj)) {
                Debug.LogError($"El objeto {obj.name} no forma parte del pool activo.");
                return;
            }

            obj.SetActive(false);
            _activeObjects.Remove(obj);

            foreach (var kvp in _poolDictionary) {
                if (kvp.Key.name == obj.name.Replace("(Clone)", "").Trim()) {
                    kvp.Value.Enqueue(obj);
                    return;
                }
            }

            Debug.LogError($"El objeto {obj.name} no pertenece a ningún pool registrado.");
            Destroy(obj); // En caso de error, destruye el objeto como último recurso.
        }
    }
}