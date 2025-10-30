using UnityEngine;

/// <summary>
/// Objeto que cuando es mirado, activa todos los objetos recolectables para que se dirijan al jugador.
/// </summary>
public class CollectionNotifier : MonoBehaviour {
  /// <summary>
  /// Delegado para notificar cuando el recuperador es activado.
  /// </summary>
  public delegate void CollectionPointed(Transform playerTransform);
  /// <summary>
  /// Evento que se dispara cuando el jugador mira este objeto.
  /// </summary>
  public event CollectionPointed OnCollectionPointed;

  /// <summary>
  /// Material cuando no está siendo observado.
  /// </summary>
  public Material InactiveMaterial;

  /// <summary>
  /// Material cuando está siendo observado.
  /// </summary>
  public Material GazedAtMaterial;

  private Renderer _myRenderer;

  private void Start() {
    _myRenderer = GetComponent<Renderer>();
    SetMaterial(false);
  }

  /// <summary>
  /// Llamado cuando la cámara empieza a mirar este objeto.
  /// </summary>
  public void OnPointerEnter() {
    SetMaterial(true);

    // Notificar a todos los objetos recolectables que deben moverse hacia el jugador
    Camera mainCamera = Camera.main;
    if (mainCamera != null) {
      OnCollectionPointed?.Invoke(mainCamera.transform);
      Debug.Log("¡Recuperador activado! Llamando a todos los objetos recolectables.");
    }
  }

  /// <summary>
  /// Llamado cuando la cámara deja de mirar este objeto.
  /// </summary>
  public void OnPointerExit() {
    SetMaterial(false);
  }


  /// <summary>
  /// Cambia el material según el estado de observación.
  /// </summary>
  private void SetMaterial(bool gazedAt) {
    if (InactiveMaterial != null && GazedAtMaterial != null && _myRenderer != null) {
      _myRenderer.material = gazedAt ? GazedAtMaterial : InactiveMaterial;
    }
  }
}