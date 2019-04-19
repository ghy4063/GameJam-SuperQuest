using UnityEngine;

[System.Serializable]
public class ReviveItem : Consumable {
	/// <summary>
	/// Adds health target after death, bringing them back
	/// </summary>
	public override void OnActivate(CharacterData target) {
		var healAmount = target.maxHealth * 0.75f;

		target.currentHealth = Mathf.RoundToInt(healAmount);
	}
}
