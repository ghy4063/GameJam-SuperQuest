using UnityEngine;

[System.Serializable]
public class Ability : MonoBehaviour {
	[HideInInspector]
	public AbilityType abilityType;
	public TargetingType targetingType = TargetingType.Single;

	public virtual void OnActivate(CharacterData caster, CharacterData[] targets) {
		Debug.Log("Calling virtual function OnActivate().");
	}
}

public enum AbilityType {
	Damage,
	Healing,
	Utility
}

public enum TargetingType {
	Self,
	Single,
	Party,
	RandomSingle
}