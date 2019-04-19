using UnityEngine;

[System.Serializable]
public class DamageAbility : Ability {
	/// <summary>
	/// Code for the damage,accuracy and elemental type of the Ability.
	/// </summary>
	[Header("Ability Stats")]
	//Used to keep players from spamming abilities.
	public string abilityName = "###";
	public int damage = 10;
	public int accuracy = 100;
	public int abilityCooldown = 0;
	public Element elementType = Element.None;
	public StatusEffect abilityEffect = StatusEffect.None;

	public override void OnActivate(CharacterData caster, CharacterData[] targets) {
		this.abilityType = AbilityType.Damage;
		var targetingInfo = new DamageStats (targets, this.damage, this.elementType, this.accuracy, 
			                            this.abilityEffect);
		caster.SendMessage("Attack", targetingInfo);
	}
}
