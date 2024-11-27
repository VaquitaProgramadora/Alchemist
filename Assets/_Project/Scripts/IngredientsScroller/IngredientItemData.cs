using UnityEngine;

namespace Alchemist.IngredientsScroller {
    // Clase que representa los datos de un ítem en el scroll de ingredientes.
    public class IngredientItemData {
        public Sprite Icon { get; } // Ícono del ingrediente.
        public bool Completed { get; } // Estado del ingrediente (true si ya fue entregado).

        public IngredientItemData(Sprite icon, bool completed) {
            Icon = icon;
            Completed = completed;
        }
    }
}