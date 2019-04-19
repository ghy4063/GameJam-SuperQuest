using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager GM;
	public static CharacterData primaryCharacter;

	public Camera primaryCamera;
	public GameObject[] playerPrefabs;

	public List<Item> consumablesInventory;
	//[HideInInspector]
	public List<CharacterData> playerData = new List<CharacterData> ();
	//[HideInInspector]
	public List<CharacterData> activePlayers = new List<CharacterData> ();

	private void Awake() {
		if (GM != null) {
			Destroy(this);
		} else {
			GM = this;
		}

		DontDestroyOnLoad(this);
	}

	private void Start() {
		foreach (var player in this.playerPrefabs) {
			var newPlayer = Instantiate(player, new Vector3 (0, -3), Quaternion.identity);
			this.playerData.Add(newPlayer.GetComponent<CharacterData>());
		}

		this.activePlayers.Add(this.playerData [0]);
		this.activePlayers.Add(this.playerData [1]);
		this.activePlayers.Add(this.playerData [2]);
		//RandomSelectCharacters();
	}

	private void RandomSelectCharacters() {
		int firstIndex;
		int secondIndex;
		int thirdIndex;

		firstIndex = Random.Range(0, 4);
		secondIndex = Random.Range(0, 4);
		thirdIndex = Random.Range(0, 4);

		while (secondIndex == firstIndex) {
			secondIndex = Random.Range(0, 4);
		}

		while (thirdIndex == firstIndex || thirdIndex == secondIndex) {
			thirdIndex = Random.Range(0, 4);
		}

		this.activePlayers.Add(this.playerData [firstIndex]);
		this.activePlayers.Add(this.playerData [secondIndex]);
		this.activePlayers.Add(this.playerData [thirdIndex]);

		//TODO: place movement/primary character
		primaryCharacter = this.activePlayers [0];
	}
}
