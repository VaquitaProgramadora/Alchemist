using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Alchemist.IngredientsScroller {
    public class IngredientsScrollerController : MonoBehaviour {
        [SerializeField] private IngredientsScrollView ingredientsScrollView; // Referencia al scroll view.

        // Actualiza el contenido del scroll view con los ingredientes y su estado actual.
        public void UpdateScrollViewContent(List<IngredientType> ingredientTypes, int currentIndex) {
            IngredientItemData[] ingredientItemsData = new IngredientItemData[ingredientTypes.Count];

            // Llena el arreglo con los datos de los ingredientes y su estado (pasado o pendiente).
            for (int i = 0; i < ingredientTypes.Count; i++) {
                IngredientType ingredientType = ingredientTypes[i];
                Sprite icon = GetIngredientIcon(ingredientType);
                ingredientItemsData[i] = new IngredientItemData(icon, i < currentIndex);
            }

            // Actualiza el scroll view con los nuevos datos.
            ingredientsScrollView.UpdateData(ingredientItemsData);

            // Selecciona la celda actual o inicial según el índice proporcionado.
            if (currentIndex > 0) {
                ingredientsScrollView.SelectCell(currentIndex - 1);
                ingredientsScrollView.SelectNextCell();
            } else {
                ingredientsScrollView.SelectCell(0);
            }
        }

        // Obtiene el ícono correspondiente a un tipo de ingrediente desde la base de datos.
        private Sprite GetIngredientIcon(IngredientType ingredientType) {
            IngredientsDB.Ingredient ingredient = IngredientsDB.Instance.ingredients.FirstOrDefault(
                ingredient => ingredient.ingredientType == ingredientType);
            return ingredient?.icon; // Devuelve el ícono o null si no se encuentra.
        }
    }
}