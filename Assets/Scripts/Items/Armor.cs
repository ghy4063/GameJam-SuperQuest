using UnityEngine;

[System.Serializable]
public class Armor : Gear {
	public int gearHealthBoost;
	public int gearDefense;
	public int gearEvasion;

	public Armor(string name, int healthBoost, int defense, int evasion) {
		this.itemName = name;
		this.gearHealthBoost = healthBoost;
		this.gearDefense = defense;
		this.gearEvasion = evasion;
	}

	/// <summary>
	/// Adding the Gear Defense, Evasion and Health for the armor. 
	/// </summary>
	/// <param name="target">Target.</param>
	public override void OnEquip(CharacterData target) {
		Debug.Log(this.itemName + " has been equipped.");

		target.maxHealth = target.maxHealth + this.gearHealthBoost;
	}

	/// <summary>
	/// removing the added Gear Defense, Evasionn and Health from armor when Unequipt.
	/// </summary>
	/// <param name="target">Target.</param>

	public override void OnUnequip(CharacterData target) {
		Debug.Log(this.itemName + " has been unequipped.");

		target.maxHealth = target.maxHealth - this.gearHealthBoost;
	}
}
