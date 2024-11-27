using System;
using UnityEngine;

namespace Alchemist {
    public enum PlayerType {
        A,
        B
    }

    public class CharacterController : MonoBehaviour {
        private static readonly int RunId = Animator.StringToHash("Run");

        [SerializeField] private PlayerType playerType; // Tipo de jugador (A o B).
        [SerializeField] private float moveSpeed = 5f; // Velocidad de movimiento.
        [SerializeField] private float rotationSpeed = 5f; // Velocidad de rotación.
        [SerializeField] private LayerMask groundLayer; // Capa del suelo para raycast.
        [SerializeField] private Transform ingredientHolder; // Transform para sostener ingredientes.

        private ICharacterInput _input;
        private Animator _animator;
        private float _lastTargetAngle; // Último ángulo de rotación.
        private IIngredientController _currentIngredient; // Ingrediente actualmente recogido.
        private IInteractable _currentInteractableInRange; // Objeto interactuable más cercano.

        void Start() {
            InitializeReferences();
        }

        private void InitializeReferences() {
            // Configura los controles según el tipo de jugador.
            _input = playerType == PlayerType.A
                ? new CharacterInput(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.E)
                : new CharacterInput(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow,
                    KeyCode.RightControl);
            _animator = GetComponent<Animator>();
        }

        private void Update() {
            if (GameManager.Instance.IsGameOver) return; // No realizar lógica si el juego terminó.

            UpdateCharacterVelocity();
            PlaceOnFloor();
            UpdateRotation();
            UpdateAnimator();
            UpdateInteraction();
        }

        private void UpdateCharacterVelocity() {
            // Calcula el movimiento basado en los controles.
            float moveX = _input.Right ? 1 : _input.Left ? -1 : 0;
            float moveZ = _input.Forward ? 1 : _input.Backward ? -1 : 0;

            Vector3 move = -Vector3.forward * moveX + Vector3.right * moveZ;
            transform.position += move * (moveSpeed * Time.deltaTime);
        }

        private void PlaceOnFloor() {
            // Ajusta la posición para mantenerse sobre el suelo.
            Vector3 headPosition = transform.position + Vector3.up * 2f;
            if (Physics.Raycast(headPosition, Vector3.down, out RaycastHit hit, 10f, groundLayer)) {
                transform.position = hit.point;
            }
        }

        private void UpdateRotation() {
            // Calcula y aplica la rotación hacia la dirección del movimiento.
            float targetAngle =
                _input.Right || _input.Left || _input.Forward || _input.Backward
                    ? Mathf.Atan2(_input.Right ? 1 : _input.Left ? -1 : 0,
                        _input.Forward ? 1 : _input.Backward ? -1 : 0) * Mathf.Rad2Deg + 90
                    : _lastTargetAngle;

            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            _lastTargetAngle = targetAngle;
        }

        private void UpdateAnimator() {
            // Actualiza la animación dependiendo del movimiento.
            _animator.SetBool(RunId, _input.Right || _input.Left || _input.Forward || _input.Backward);
        }

        private void OnTriggerEnter(Collider other) {
            // Detecta si un objeto interactuable entra en el rango.
            if (!other.TryGetComponent(out IInteractable interactable)) return;

            _currentInteractableInRange?.OnInteractionRangeExit();
            _currentInteractableInRange = interactable;
            _currentInteractableInRange.OnInteractionRangeEnter();
        }

        private void OnTriggerExit(Collider other) {
            // Detecta si un objeto interactuable sale del rango.
            if (!other.TryGetComponent(out IInteractable interactable)) return;
            if (_currentInteractableInRange != interactable) return;
            _currentInteractableInRange.OnInteractionRangeExit();
            _currentInteractableInRange = null;
        }

        private void UpdateInteraction() {
            if (!_input.Interact) return; // No hacer nada si no se presiona el botón de interacción.

            if (_currentIngredient != null) {
                if (_currentInteractableInRange != null) {
                    if (_currentInteractableInRange is IIngredientController ingredientController) {
                        DropIngredient();
                        PickUpIngredient(ingredientController);
                    } else {
                        DropIngredientInPot();
                    }
                } else {
                    DropIngredient();
                }
            } else if (_currentInteractableInRange is IIngredientController ingredientController) {
                PickUpIngredient(ingredientController);
            }
        }

        private void PickUpIngredient(IIngredientController ingredientController) {
            // Recoge un ingrediente y lo asigna al personaje.
            _currentIngredient = ingredientController;
            _currentIngredient.OnPickUp();
            _currentIngredient.Transform.SetParent(ingredientHolder);
            _currentIngredient.Transform.localPosition = Vector3.zero;
            _currentIngredient.Transform.localRotation = Quaternion.Euler(-90, 0, 0);
            _currentInteractableInRange.OnInteraction();
            _currentInteractableInRange = null;
        }

        private void DropIngredient() {
            // Suelta el ingrediente actualmente recogido.
            _currentIngredient.OnDrop();
            _currentIngredient.Transform.SetParent(null);
            _currentIngredient = null;
        }

        private void DropIngredientInPot() {
            // Suelta el ingrediente en el caldero y verifica si es correcto.
            bool correctDrop = GameManager.Instance.CheckIngredient(_currentIngredient.IngredientType);
            IPotController potController = (IPotController)_currentInteractableInRange;
            potController.OnCheckIngredientDrop(correctDrop);

            _currentIngredient.OnDrop();
            _currentIngredient.Transform.SetParent(null);
            _currentIngredient.Destroy();
            _currentIngredient = null;
            _currentInteractableInRange.OnInteraction();
            _currentInteractableInRange = null;
        }
    }
}