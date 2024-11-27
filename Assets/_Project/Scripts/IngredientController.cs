using System;
using HighlightPlus;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Alchemist {
    // Interfaz que define el comportamiento de los ingredientes.
    public interface IIngredientController {
        Transform Transform { get; } // Transform del ingrediente.
        IngredientType IngredientType { get; } // Tipo del ingrediente.
        void OnPickUp(); // Acción al recoger el ingrediente.
        void OnDrop(); // Acción al soltar el ingrediente.
        void Destroy(); // Devuelve el ingrediente al pool.
    }

    public class IngredientController : MonoBehaviour, IIngredientController, IInteractable {
        [EnumToggleButtons] [SerializeField] private IngredientType ingredientType; // Tipo de ingrediente configurado.
        [SerializeField] private HighlightEffect highlightEffect; // Efecto visual al interactuar.
        [SerializeField] private UnityEvent onPickUp; // Evento al recoger el ingrediente.

        public Transform Transform => transform; // Transform del objeto.
        public IngredientType IngredientType => ingredientType; // Tipo de ingrediente.

        private Rigidbody _rigidbody; // Referencia al Rigidbody del objeto.
        private Collider[] _colliders; // Referencia a todos los colliders del objeto.

        private void Start() {
            InitializeReferences();
        }

        // Inicializa las referencias necesarias.
        private void InitializeReferences() {
            _rigidbody = GetComponent<Rigidbody>();
            _colliders = GetComponents<Collider>();
        }

        // Lógica al recoger el ingrediente.
        public void OnPickUp() {
            highlightEffect.highlighted = false; // Desactiva el efecto de resaltado.
            _rigidbody.isKinematic = true; // Congela la física del objeto.
            foreach (Collider col in _colliders) {
                col.enabled = false; // Desactiva los colliders.
            }
        }

        // Lógica al soltar el ingrediente.
        public void OnDrop() {
            _rigidbody.isKinematic = false; // Reactiva la física del objeto.
            foreach (Collider col in _colliders) {
                col.enabled = true; // Reactiva los colliders.
            }
        }

        // Devuelve el objeto al pool.
        public void Destroy() {
            IngredientsPool.Instance.ReturnToPool(gameObject);
        }

        // Activa el resaltado al entrar en el rango de interacción.
        public void OnInteractionRangeEnter() {
            highlightEffect.highlighted = true;
        }

        // Desactiva el resaltado al salir del rango de interacción.
        public void OnInteractionRangeExit() {
            highlightEffect.highlighted = false;
        }

        // Invoca el evento al interactuar directamente con el ingrediente.
        public void OnInteraction() {
            onPickUp.Invoke();
        }
    }
}