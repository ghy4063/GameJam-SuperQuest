using UnityEngine;
using System.Linq;

public class ItemDatabase : MonoBehaviour {
	public static ItemDatabase masterData;

	[SerializeField]
	private ConsumableObject[] globalConsumables;

	private void Awake() {
		if (masterData == null) {
			masterData = this;
			DontDestroyOnLoad(this);
		} else {
			Destroy(this);
		}
	}

	public ConsumableObject ParseItem(string itemName) {
		if (masterData.globalConsumables.Any(particle => particle.itemName == itemName)) {
			var temp = masterData.globalConsumables.First(particle => 
				particle.itemName == itemName);

			return new ConsumableObject (temp.itemName, temp.itemObject);
		} else {
			Debug.LogWarning("No particle effect was found with the name: " + itemName);
			return null;
		}
	}
}

[System.Serializable]
public class ConsumableObject {
	public string itemName;
	public GameObject itemObject;

	internal ConsumableObject(string particleName, GameObject particleObject) {
		this.itemName = particleName;
		this.itemObject = particleObject;
	}
}