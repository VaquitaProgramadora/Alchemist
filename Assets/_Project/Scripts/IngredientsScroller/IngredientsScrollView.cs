using System.Collections.Generic;
using EasingCore;
using FancyScrollView;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Alchemist.IngredientsScroller {
    public class IngredientsScrollView : FancyScrollView<IngredientItemData, IngredientsContext> {
        [SerializeField] private Scroller scroller; // Controlador del desplazamiento.
        [SerializeField] private GameObject cellPrefab; // Prefab de cada celda.

        protected override GameObject CellPrefab => cellPrefab;

        protected override void Initialize() {
            base.Initialize();

            // Configura la acción al hacer clic en una celda.
            Context.OnCellClicked = SelectCell;

            // Registra los cambios de valor y selección del scroller.
            scroller.OnValueChanged(UpdatePosition);
            scroller.OnSelectionChanged(UpdateSelection);
        }

        // Actualiza el índice seleccionado cuando cambia la selección.
        private void UpdateSelection(int index) {
            if (Context.SelectedIndex == index) {
                return; // No hacer nada si el índice no cambió.
            }

            Context.SelectedIndex = index;
            Refresh(); // Refresca las celdas con la nueva selección.
            Debug.Log($"Selected index: {index}");
        }

        // Actualiza los datos de la vista con una nueva lista de ingredientes.
        public void UpdateData(IList<IngredientItemData> items) {
            UpdateContents(items);
            scroller.SetTotalCount(items.Count);
        }

        [Button]
        public void SelectNextCell() {
            SelectCell(Context.SelectedIndex + 1); // Selecciona la siguiente celda.
        }

        // Selecciona una celda específica por índice.
        public void SelectCell(int index) {
            if (index < 0 || index >= ItemsSource.Count || index == Context.SelectedIndex) {
                return; // No hace nada si el índice es inválido o ya está seleccionado.
            }

            UpdateSelection(index); // Actualiza la selección.
            scroller.ScrollTo(index, 0.35f, Ease.OutCubic); // Desplaza suavemente al índice.
        }
    }
}