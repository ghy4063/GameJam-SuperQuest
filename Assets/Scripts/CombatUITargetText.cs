using UnityEngine;
using UnityEngine.UI;

public class CombatUITargetText : MonoBehaviour {
	[SerializeField]
	private int enemyIndex;

	private Text text;

	private void Awake() {
		this.text = this.GetComponent<Text>();
	}

	private void Update() {
		if (CombatManager.inCombat) {
			if (this.enemyIndex > CombatInstance.CurrentInstance.enemyList.Count - 1) {
				this.transform.parent.gameObject.SetActive(false);
			} else {
				this.text.text = CombatInstance.CurrentInstance.enemyList [this.enemyIndex].gameObject.name;
			}
		}
	}
}
