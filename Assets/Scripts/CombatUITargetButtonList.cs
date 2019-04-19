using UnityEngine;
using UnityEngine.UI;

public class CombatUITargetButtonList : MonoBehaviour {
	public Button[] targetingButtons;

	public void EnableTargetingButtons() {
		foreach (var button in this.targetingButtons) {
			button.gameObject.SetActive(true);
		}
	}
}
