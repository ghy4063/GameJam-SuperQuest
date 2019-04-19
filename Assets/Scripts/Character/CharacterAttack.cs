using UnityEngine;

public class CharacterAttack : MonoBehaviour {
	public void Attack(DamageStats stats) {
		foreach (var target in stats.targets) {
			var postDamage = stats.damage - target.Defense;
			//TODO: Elemental effects

			if (postDamage > 0) {
				//TODO: accuracy
				
				target.currentHealth -= postDamage;

				Debug.LogWarning("Damage " + target.name + " for " + postDamage);

				if (target.currentHealth < 0) {
					target.currentHealth = 0;
				}
			}

			//TODO: status effects
		}
	}

	public void Heal(HealStats amount) {
		foreach (var target in amount.targets) {
			var postHeal = amount.heal + target.currentHealth;

			target.currentHealth += postHeal;

			Debug.LogWarning("Healing " + target.name + " for " + postHeal);

			if (target.currentHealth > target.maxHealth) {
				target.currentHealth = target.maxHealth;
			} 
		}
	}

	public void UseHealingItem(CharacterData target, int heal, StatusEffect statusEffect) {
		var itemPostHeal = heal + target.currentHealth;
		target.currentHealth += itemPostHeal;
		if (target.currentHealth	> target.maxHealth) {
			target.currentHealth = target.maxHealth;
		}
	}

}
