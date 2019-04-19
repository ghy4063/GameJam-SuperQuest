[System.Serializable]
public class UtilityAbility : Ability {
	/// <summary>
	/// Code for the Utility abilites. Used for adding status effects that don't directly hurt the target. 
	/// </summary>

	public string abilityName;
	public StatusEffect statusEffect;
	public int effectAccuracy;

	public override void OnActivate(CharacterData caster, CharacterData[] targets) {
		this.abilityType = AbilityType.Utility;
		var targetingInfo = new UtilityStats (targets, this.effectAccuracy, this.statusEffect);
		caster.SendMessage("Effect", targetingInfo);
	}
}

public class UtilityStats {
	public UtilityStats(CharacterData[] targets, int effectAccuracy, StatusEffect statusEffect) {
		this.targets = targets;
		this.effectAccuracy = effectAccuracy;
		this.statusEffect = statusEffect;
	}

	public CharacterData[] targets;
	public int effectAccuracy;
	public StatusEffect statusEffect;
}
