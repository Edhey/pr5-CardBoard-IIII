# pr5-CardBoard-IIII

- **Author**: Himar Edhey Hernández Alonso
- **Subject**: Interfaces Inteligentes
- **Course**: Interfaces Inteligentes
- **Institution**: Universidad de La Laguna

## Description

*Crear el proyecto Cardboard y experimentar y generar una apk para Android.
Crea una escena con Cardboard que tenga objetos que al llegar el jugador a ellos los recolecte con la vista. El jugador se puede desplazar con un mando o con la vista. La escena debe contener un terreno y objetos de algún paquete de la Asset Store.
Elige un objeto en la escena que sirva para recuperar los que se recolectan. Cuando el jugador lo selecciona con la vista los recolectables se dirigen hacie el jugador.*

---

For this project I have used a scene from the Unity Asset Store called Simple Poly City. I used the stadium area as the terrain for the player to navigate. I also used the Saritasa Sports Balls pack to add some collectible objects (as football, basketball, volleyball, tennis ball, etc.) around the stadium area.

With the scene created I implemented two scripts. For the first `CollectionNotifier.cs` I used the `ObjectController` script from the Cardboard SDK as a reference, because I used the Icosahedron object from the Cardboard SDK as the "collector" object. So I modified the script for notify when the player is looking at the collector object.

```csharp
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
```

For the second script `CollectibleObject.cs`, I simply added a listener to the event defined in the first script, so when the player looks at the collector object, the collectible objects will start moving towards the player's position. I also used the `OnPointerEnter` method to immediately hide the collectible object when the player looks at it.

```csharp
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
```

![Cardboard Collector Scene](Resources/pr5-CardBoard.gif)
