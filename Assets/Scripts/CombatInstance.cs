using System.Collections.Generic;
using UnityEngine;

public class CombatInstance : MonoBehaviour {
	public static CombatInstance CurrentInstance;
	public static List<CharacterData> turnOrderList = new List<CharacterData> ();

	[HideInInspector]
	public CombatArea currentArea;
	//[HideInInspector]
	public List<CharacterData> alivePlayers = new List<CharacterData> ();
	//[HideInInspector]
	public List<CharacterData> deadPlayers = new List<CharacterData> ();
	//[HideInInspector]
	public List<CharacterData> enemyList = new List<CharacterData> ();
	//[HideInInspector]
	public CharacterData currentCharacter;
	[HideInInspector]
	public Vector3 overworldPosition;
	[HideInInspector]
	public Quaternion overworldRotation;

	[HideInInspector]
	public bool isBossEncounter;

	private int turnIndex;

	private void Awake() {
		Debug.Log("Combat Awake()");

		if (CurrentInstance != null) {
			Destroy(CurrentInstance);
		}

		CurrentInstance = this;
	}

	private void Start() {
		Debug.Log("Combat Start(), populating turn order list...");

		this.currentArea.combatCamera.GetComponent<CombatUITargetButtonList>().EnableTargetingButtons();
		this.currentArea.combatCamera.enabled = true;

		turnOrderList.AddRange(this.alivePlayers);
		turnOrderList.AddRange(this.enemyList);
		//TODO: turn order based on a stat
		/*
		turnOrderList = turnOrderList.OrderByDescending(i => i.moveSpeed).ToList();
		*/

		this.BeginCombat();
	}

	private void Update() {
		if (this.enemyList.Count == 0) {
			this.TestEndCombat();
		}
	}

	private void UpdateTargets() {
		var toBeRemoved = new List<CharacterData> ();

		foreach (var character in turnOrderList) {
			if (character.isDead) {
				toBeRemoved.Add(character);
			}
		}

		if (toBeRemoved.Count > 0) {
			foreach (var character in toBeRemoved) {
				this.RemoveCharacterFromInstance(character);
			}
		}
	}

	public void BeginCombat() {
		//TODO: Re-enable to automatically start turn order
		this.SetTurnIndex(0);
		
		if (this.currentCharacter.GetComponent<CharacterHealth>().isAI) {
			this.currentArea.combatCamera.GetComponent<CombatUI>().combatCanvas.SetActive(false);
			this.currentCharacter.GetComponent<AIController>().TakeCombatTurn();
		} else {
			this.currentArea.combatCamera.GetComponent<CombatUI>().combatCanvas.SetActive(true);
		}
	}

	[ContextMenu("TestEndCombat")]
	public void TestEndCombat() {
		var j = this.enemyList.Count;

		for (var i = 0; i < j; i++) {
			var data = this.enemyList [0];
			this.RemoveCharacterFromInstance(data);
			Destroy(data.gameObject);
		}

		GameManager.GM.activePlayers [0].transform.position = this.overworldPosition;
		GameManager.GM.activePlayers [0].transform.rotation = this.overworldRotation;

		CombatManager.CM.CloseCombatInstance();
	}

	public void NextTurn() {
		this.UpdateTargets();

		if (this.turnIndex + 1 == turnOrderList.Count) {
			this.SetTurnIndex(0);
		} else {
			this.SetTurnIndex(this.turnIndex + 1);
		}

		if (this.currentCharacter.GetComponent<CharacterHealth>().isAI) {
			this.currentArea.combatCamera.GetComponent<CombatUI>().combatCanvas.SetActive(false);
			this.currentCharacter.GetComponent<AIController>().TakeCombatTurn();
		} else {
			this.currentArea.combatCamera.GetComponent<CombatUI>().combatCanvas.SetActive(true);
		}
	}

	public void SetTurnIndex(int index) {
		this.turnIndex = index;
		this.currentCharacter = turnOrderList [this.turnIndex];
		Debug.Log("It is now " + this.currentCharacter.name + "'s turn.");
	}

	public void AddCharacterToInstance(CharacterData characterData) {
		if (characterData.health.isAI) {
			this.enemyList.Add(characterData);
		} else {
			if (this.deadPlayers.Contains(characterData)) {
				this.deadPlayers.Remove(characterData);
			}
			this.alivePlayers.Add(characterData);
		}
		turnOrderList.Add(characterData);
	}

	public void RemoveCharacterFromInstance(CharacterData characterData) {
		if (characterData.health.isAI) {
			this.enemyList.Remove(characterData);
		} else {
			this.alivePlayers.Remove(characterData);
			this.deadPlayers.Add(characterData);
		}
		turnOrderList.Remove(characterData);
	}
}
