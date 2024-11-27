using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Alchemist {
    [CreateAssetMenu(fileName = "IngredientsDB", menuName = "Alchemist/IngredientsDB")]
    public class IngredientsDB : ScriptableObject {
        [Serializable]
        public class Ingredient {
            [EnumToggleButtons] public IngredientType ingredientType; // Tipo de ingrediente.
            public GameObject prefab; // Prefab asociado al ingrediente.
            public Sprite icon; // Icono visual del ingrediente.
        }

        private static IngredientsDB _instance;

        // Singleton que carga la base de datos de ingredientes desde los recursos.
        public static IngredientsDB Instance {
            get {
                if (_instance == null) {
                    _instance = Resources.Load<IngredientsDB>("IngredientsDB");
                }

                return _instance;
            }
        }

        public Ingredient[] ingredients; // Lista de ingredientes disponibles en la base de datos.
    }
}