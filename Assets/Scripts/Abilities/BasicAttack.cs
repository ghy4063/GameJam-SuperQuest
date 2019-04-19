[System.Serializable]
public class BasicAttack : Ability {
	private CharacterData data;

	private void Awake() {
		this.data = this.GetComponent<CharacterData>();
	}

	public override void OnActivate(CharacterData caster, CharacterData[] targets) {
		this.abilityType = AbilityType.Damage;
		var targetingInfo = new DamageStats (targets, 
			                            this.data.CurrentWeapon.weaponDamage, 
			                            this.data.CurrentWeapon.weaponElement, 
			                            this.data.CurrentWeapon.weaponAccuracy, 
			                            StatusEffect.None);
		caster.SendMessage("Attack", targetingInfo);
	}
}

public class DamageStats {
	public DamageStats(CharacterData[] targets, int damage, Element attackType, int accuracy, 
		StatusEffect statusEffect) {
		this.targets = targets;
		this.damage = damage;
		this.attackType = attackType;
		this.accuracy = accuracy;
		this.statusEffect = statusEffect;
	}

	public CharacterData[] targets;
	public int damage;
	public Element attackType;
	public int accuracy;
	public StatusEffect statusEffect = StatusEffect.None;
}


