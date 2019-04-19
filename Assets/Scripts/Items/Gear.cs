using UnityEngine;

[System.Serializable]
public class Gear : Item {
	public GameObject visualGameObject;

	public virtual void OnEquip(CharacterData target) {
		Debug.Log("Calling virtual function OnEquip().");
	}

	public virtual void OnUnequip(CharacterData target) {
		Debug.Log("Calling virtual function OnUnequip().");
	}
}
