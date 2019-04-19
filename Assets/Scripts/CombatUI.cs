using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CombatUI : MonoBehaviour {
	public GameObject combatCanvas;

	private void Awake() {
		this.combatCanvas = this.transform.GetChild(0).gameObject;
	}
}
