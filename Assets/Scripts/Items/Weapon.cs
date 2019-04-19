using UnityEngine;

[System.Serializable]
public class Weapon : Gear {
	public Element weaponElement;
	public int weaponDamage;
	public int weaponAccuracy;

	public Weapon(string name, Element element, int damage, int accuracy) {
		this.itemName = name;
		this.weaponElement = element;
		this.weaponDamage = damage;
		this.weaponAccuracy = accuracy;
	}

	public override void OnEquip(CharacterData target) {
		Debug.Log(this.itemName + " has been equipped.");
		//Do Nothing
	}

	public override void OnUnequip(CharacterData target) {
		Debug.Log(this.itemName + " has been unequipped.");
		//Do nothing
	}
}
