using System;
using System.Collections.Generic;
using Alchemist.IngredientsScroller;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Alchemist {
    public class GameManager : MonoBehaviour {
        [SerializeField] private float initialTime = 3f; // Tiempo inicial del timer.
        [SerializeField] private float timeGainPerCorrect = 0.3f; // Tiempo ganado por respuesta correcta.
        [SerializeField] private float timeLossPerIncorrect = 0.5f; // Tiempo perdido por respuesta incorrecta.

        [SerializeField]
        private IngredientsScrollerController
            ingredientsScrollerController; // Controlador del scroller de ingredientes.

        [SerializeField] private TMP_Text timerText; // Texto que muestra el tiempo restante.
        [SerializeField] private TMP_Text scoreText, scoreTextPopup; // Texto que muestra el puntaje.
        [SerializeField] private Button restartButton; // Botón para reiniciar el juego.
        [SerializeField] private GameObject scorePopup; // Ventana emergente al finalizar el juego.

        private List<IngredientType> _ingredientList; // Lista de ingredientes a entregar.
        private int _currentIngredientIndex = 0; // Índice del ingrediente actual.
        private float _timer; // Tiempo restante para la entrega.
        private int _playerScore = 0; // Puntaje del jugador.

        public bool IsGameOver { get; private set; } // Estado del juego (true si el jugador perdió).

        public IngredientType CurrentIngredient =>
            _ingredientList != null
                ? _ingredientList[_currentIngredientIndex]
                : IngredientType.Apple; // Ingrediente actual a entregar.

        public static GameManager Instance { get; private set; } // Instancia del GameManager (Singleton).

        private void Awake() {
            InitializeSingleton();
        }

        private void InitializeSingleton() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        private void Start() {
            restartButton.onClick.AddListener(ReloadScene); // Asigna el botón de reinicio.
            InitializeGame();
        }

        // Inicializa la lógica y las variables del juego.
        private void InitializeGame() {
            _ingredientList = new List<IngredientType>();
            _timer = initialTime;

            // Añade los primeros 3 ingredientes de manera aleatoria.
            for (int i = 0; i < 3; i++) {
                AddRandomIngredient();
            }

            ingredientsScrollerController.UpdateScrollViewContent(_ingredientList, _currentIngredientIndex);
            UpdateScoreTexts();
        }

        private void Update() {
            UpdateTimer();
        }

        // Actualiza el tiempo restante.
        private void UpdateTimer() {
            _timer -= Time.deltaTime;
            timerText.text = $"{_timer:0.00}";

            if (_timer > 0) return;

            timerText.text = "0.00";
            GameOver();
        }

        // Verifica si el ingrediente entregado es el correcto.
        public bool CheckIngredient(IngredientType ingredient) {
            if (ingredient == _ingredientList[_currentIngredientIndex]) {
                HandleCorrectIngredient();
                return true;
            } else {
                HandleIncorrectIngredient();
                return false;
            }
        }

        // Maneja la lógica al entregar un ingrediente correcto.
        [Button]
        private void HandleCorrectIngredient() {
            _playerScore += 10; // Incrementa el puntaje.
            UpdateScoreTexts();

            // Incrementa el tiempo, respetando el máximo inicial.
            _timer = Mathf.Min(_timer + timeGainPerCorrect, initialTime);
            timerText.text = $"{_timer:0.00}";

            // Avanza al siguiente ingrediente.
            _currentIngredientIndex++;

            // Añade un nuevo ingrediente aleatorio.
            AddRandomIngredient();
            ingredientsScrollerController.UpdateScrollViewContent(_ingredientList, _currentIngredientIndex);
        }

        private void UpdateScoreTexts() {
            scoreText.text = $"{_playerScore}";
            scoreTextPopup.text = $"{_playerScore}";
        }

        // Maneja la lógica al entregar un ingrediente incorrecto.
        private void HandleIncorrectIngredient() {
            _timer -= timeLossPerIncorrect; // Reduce el tiempo.
            timerText.text = $"{_timer:0.00}";

            if (_timer > 0) return;

            timerText.text = "0.00";
            GameOver();
        }

        // Añade un ingrediente aleatorio a la lista.
        private void AddRandomIngredient() {
            IngredientType randomIngredient =
                (IngredientType)Random.Range(0, System.Enum.GetValues(typeof(IngredientType)).Length);
            _ingredientList.Add(randomIngredient);
        }

        // Termina el juego cuando se queda sin tiempo.
        private void GameOver() {
            IsGameOver = true;
            scorePopup.SetActive(true);
        }

        // Reinicia la escena.
        private void ReloadScene() {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}