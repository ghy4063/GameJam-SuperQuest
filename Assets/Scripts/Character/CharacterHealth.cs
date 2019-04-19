using UnityEngine;

public class CharacterHealth : MonoBehaviour {
	public bool isAI;

	private CharacterData data;

	void Start() {
		this.data = this.GetComponentInParent<CharacterData>();
	}

	void Update() {
		if (this.data.isDead == false) {
			if (this.data.currentHealth <= 0) {
				Debug.LogWarning(this.name + " has died.");
				this.data.isDead = true;
				CombatInstance.CurrentInstance.RemoveCharacterFromInstance(this.data);

				if (this.isAI) {
					Destroy(this.gameObject);
				} else {
					// TODO: play death animation
				}
			}
		}
	}
}
