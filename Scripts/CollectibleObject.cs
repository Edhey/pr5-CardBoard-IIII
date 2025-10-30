using UnityEngine;

/// <summary>
/// Controla el comportamiento de objetos recolectables mediante la vista en Cardboard.
/// </summary>
public class CollectibleObject : MonoBehaviour {
  [SerializeField] CollectionNotifier collectionNotifier;
  private Transform _transform_to_move;
  [SerializeField] private Vector3 _hiddenPosition = new Vector3(0, 0, 0);

  private void OnEnable() {
    collectionNotifier.OnCollectionPointed += OnCollectionPointed;
  }

  private void OnDisable() {
    collectionNotifier.OnCollectionPointed -= OnCollectionPointed;
  }

  /// <summary>
  /// Llamado cuando la cámara empieza a mirar este objeto.
  /// Se recolecta inmediatamente escondiéndose.
  /// </summary>
  public void OnPointerEnter() {
    Debug.Log("Objeto recolectable mirado y recolectado.");
    transform.position = _hiddenPosition;
  }

  private void Update() {
    if (_transform_to_move != null) {
      MoveToTransform(_transform_to_move);
    }
  }

  /// <summary>
  /// Mueve el objeto hacia el transform objetivo.
  /// </summary>
  private void MoveToTransform(Transform targetObject) {
    if (targetObject == null) {
      return;
    }

    const float stopThreshold = 0.1f;
    if (Vector3.Distance(transform.position, targetObject.position) <= stopThreshold) {
      _transform_to_move = null;
      return;
    }

    Vector3 direction = (targetObject.position - transform.position).normalized;
    transform.Translate(
      Time.deltaTime * 10 * direction.normalized,
      Space.World
    );
  }

  /// <summary>
  /// Llamado cuando el notificador es activado.
  /// El objeto aparece y se mueve hacia el jugador.
  /// </summary>
  private void OnCollectionPointed(Transform transform_to_move) {
    _transform_to_move = transform_to_move;
  }
}