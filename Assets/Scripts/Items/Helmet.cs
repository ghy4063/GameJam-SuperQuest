using UnityEngine;

[System.Serializable]
public class Helmet : Gear {
	public int gearHealthBoost;
	public int gearDefense;
	public int gearEvasion;

	public Helmet(string name, int healthBoost, int defense, int evasion) {
		this.itemName = name;
		this.gearHealthBoost = healthBoost;
		this.gearDefense = defense;
		this.gearEvasion = evasion;
	}

	/// <summary>
	/// Adds the amount of Defense,Evasion and Health that each item adds for equipping
	/// </summary>
	/// <param name="target">Target.</param>
	public override void OnEquip(CharacterData target) {
		Debug.Log(this.itemName + " has been equipped.");

		target.maxHealth = target.maxHealth + this.gearHealthBoost;
	}

	/// <summary>
	/// Removes the amount of Defense,Evasion,and Health that each item adds when unequiping. 
	/// Checks to make sure it doesnt go under 0 stats before equipping. 
	/// </summary>
	/// <param name="target">Target.</param>
	public override void OnUnequip(CharacterData target) {
		Debug.Log(this.itemName + " has been unequipped.");

		target.maxHealth = target.maxHealth - this.gearHealthBoost;
	}
}

