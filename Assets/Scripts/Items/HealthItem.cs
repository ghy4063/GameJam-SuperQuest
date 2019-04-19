using UnityEngine;

[System.Serializable]
public class HealthItem : Consumable {
	public float percentHealed = 0.4f;

	/// <summary>
	/// Heals the target for however much the Item is going to have. 
	/// </summary>
	/// <param name="target">Target.</param>
	public override void OnActivate (CharacterData target) {
		var healAmount = target.maxHealth * this.percentHealed;
		target.currentHealth += Mathf.RoundToInt (healAmount);

		if (target.currentHealth > target.maxHealth) {
			target.currentHealth = target.maxHealth;
		}
	}
}
