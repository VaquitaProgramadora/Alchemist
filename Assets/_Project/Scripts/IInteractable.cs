namespace Alchemist {
    // Interfaz que define los métodos básicos de interacción.
    public interface IInteractable {
        void OnInteractionRangeEnter(); // Método llamado al entrar en el rango de interacción.
        void OnInteractionRangeExit(); // Método llamado al salir del rango de interacción.
        void OnInteraction(); // Método llamado al interactuar directamente.
    }
}