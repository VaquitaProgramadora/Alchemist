using UnityEngine;

namespace Alchemist {
    // Interfaz para definir los controles básicos del personaje.
    public interface ICharacterInput {
        bool Forward { get; } // Movimiento hacia adelante.
        bool Backward { get; } // Movimiento hacia atrás.
        bool Left { get; } // Movimiento hacia la izquierda.
        bool Right { get; } // Movimiento hacia la derecha.
        bool Interact { get; } // Acción de interacción.
    }

    // Clase que implementa los controles del personaje usando teclas configurables.
    public class CharacterInput : ICharacterInput {
        private readonly KeyCode _forwardKey; // Tecla para moverse hacia adelante.
        private readonly KeyCode _backwardKey; // Tecla para moverse hacia atrás.
        private readonly KeyCode _leftKey; // Tecla para moverse hacia la izquierda.
        private readonly KeyCode _rightKey; // Tecla para moverse hacia la derecha.
        private readonly KeyCode _interactKey; // Tecla para interactuar.

        public bool Forward => Input.GetKey(_forwardKey); // Verifica si se presiona la tecla de avanzar.
        public bool Backward => Input.GetKey(_backwardKey); // Verifica si se presiona la tecla de retroceder.
        public bool Left => Input.GetKey(_leftKey); // Verifica si se presiona la tecla de moverse a la izquierda.
        public bool Right => Input.GetKey(_rightKey); // Verifica si se presiona la tecla de moverse a la derecha.
        public bool Interact => Input.GetKeyDown(_interactKey); // Verifica si se presionó la tecla de interacción.

        // Constructor que asigna las teclas configurables.
        public CharacterInput(KeyCode forwardKey, KeyCode backwardKey, KeyCode leftKey, KeyCode rightKey,
            KeyCode interactKey) {
            _forwardKey = forwardKey;
            _backwardKey = backwardKey;
            _leftKey = leftKey;
            _rightKey = rightKey;
            _interactKey = interactKey;
        }
    }
}