using FancyScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace Alchemist.IngredientsScroller {
    public class IngredientCell : FancyCell<IngredientItemData, IngredientsContext> {
        [SerializeField] private Animator animator; // Animador para efectos de scroll.
        [SerializeField] private Image iconImage; // Imagen del ícono del ingrediente.
        [SerializeField] private GameObject completedIcon; // Indicador visual de completado.

        private float _currentPosition = 0; // Posición actual de la celda.

        private static class AnimatorHash {
            public static readonly int Scroll = Animator.StringToHash("scroll"); // Hash para la animación de scroll.
        }

        // Actualiza el contenido de la celda con los datos proporcionados.
        public override void UpdateContent(IngredientItemData itemData) {
            iconImage.sprite = itemData.Icon;
            completedIcon.SetActive(itemData.Completed); // Muestra el ícono de completado si corresponde.
        }

        // Actualiza la posición de la celda dentro del scroll.
        public override void UpdatePosition(float position) {
            _currentPosition = position;

            if (animator.isActiveAndEnabled) {
                animator.Play(AnimatorHash.Scroll, -1, position); // Reproduce la animación según la posición.
            }

            animator.speed = 0; // Pausa la animación para mantener la posición exacta.
        }

        // Asegura que la posición se actualice al habilitar la celda.
        private void OnEnable() => UpdatePosition(_currentPosition);
    }
}