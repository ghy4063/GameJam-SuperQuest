using UnityEngine;

[System.Serializable]
public class Consumable : Item {
	public virtual void OnActivate(CharacterData target) {
		Debug.Log("Calling virtual function OnActivate().");
	}
}
