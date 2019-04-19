[System.Serializable]
public class HealingAbility : Ability {
	/// <summary>
	/// Healing abilities. Code used to figure out how much the Ability will heal the target.
	/// </summary>
	public string abilityName;
	public int heal;
	public StatusEffect statusEffect;

	public override void OnActivate(CharacterData caster, CharacterData[] targets) {
		this.abilityType = AbilityType.Healing;
		var targetingInfo = new HealStats (targets, this.heal, this.statusEffect);
		caster.SendMessage("Heal", targetingInfo);
	}
}

public class HealStats {
	public HealStats(CharacterData[] targets, int heal, StatusEffect statusEffect) {
		this.targets = targets;
		this.heal = heal;
	}

	public CharacterData[] targets;
	public int heal;
	public StatusEffect statusEffect = StatusEffect.None;
}
