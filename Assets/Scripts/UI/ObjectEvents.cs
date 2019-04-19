using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable InconsistentNaming

public class ObjectEvents : MonoBehaviour {
	[SerializeField, UsedImplicitly]
	private UnityEvent OnEnable;
	[SerializeField, UsedImplicitly]
	private UnityEvent OnDisable;

	public void ToggleGameObject() {
		this.gameObject.SetActive (!this.gameObject.activeSelf);
	}
}
