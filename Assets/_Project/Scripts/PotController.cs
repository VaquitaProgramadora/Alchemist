using HighlightPlus;
using UnityEngine;
using UnityEngine.Events;

namespace Alchemist {
    // Interfaz para definir el comportamiento del controlador del caldero.
    public interface IPotController {
        // Método que se ejecuta al verificar si el ingrediente es correcto o incorrecto.
        void OnCheckIngredientDrop(bool correctDrop);
    }

    // Controlador del caldero que maneja interacciones y eventos relacionados con los ingredientes.
    public class PotController : MonoBehaviour, IPotController, IInteractable {
        [SerializeField] private HighlightEffect highlightEffect; // Efecto visual para resaltar el objeto.
        [SerializeField] private UnityEvent onIngredientDrop; // Evento cuando se suelta cualquier ingrediente.
        [SerializeField] private UnityEvent onCorrectIngredientDrop; // Evento cuando se suelta un ingrediente correcto.

        [SerializeField]
        private UnityEvent onIncorrectIngredientDrop; // Evento cuando se suelta un ingrediente incorrecto.

        // Método llamado cuando un objeto entra en el rango de interacción.
        public void OnInteractionRangeEnter() {
            highlightEffect.highlighted = true; // Activa el efecto de resaltado.
        }

        // Método llamado cuando un objeto sale del rango de interacción.
        public void OnInteractionRangeExit() {
            highlightEffect.highlighted = false; // Desactiva el efecto de resaltado.
        }

        // Método llamado al interactuar directamente con el caldero.
        public void OnInteraction() {
            onIngredientDrop.Invoke(); // Dispara el evento genérico de soltar ingrediente.
        }

        // Método que se ejecuta para verificar si el ingrediente soltado es correcto o no.
        public void OnCheckIngredientDrop(bool correctDrop) {
            if (correctDrop)
                onCorrectIngredientDrop.Invoke(); // Si es correcto, dispara el evento correspondiente.
            else
                onIncorrectIngredientDrop.Invoke(); // Si es incorrecto, dispara el evento correspondiente.
        }
    }
}