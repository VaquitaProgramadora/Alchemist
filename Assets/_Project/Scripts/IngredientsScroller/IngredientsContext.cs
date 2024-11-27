using System;

namespace Alchemist.IngredientsScroller {
    public class IngredientsContext {
        public int SelectedIndex = -1; // Índice de la celda seleccionada actualmente.
        public Action<int> OnCellClicked; // Acción que se ejecuta cuando se hace clic en una celda.
    }
}