using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SetMainCamera : MonoBehaviour {
	private void Awake() {
		GameManager.GM.primaryCamera = this.GetComponent<Camera>();
	}
}
