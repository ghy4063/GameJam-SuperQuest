using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour {
	public static CombatManager CM;

	[Header("Combat Variables")]
	public static bool inCombat = false;
	public CombatArea activeCombatArea;

	private void Awake() {
		if (CM != null) {
			Destroy(CM);
		}
		CM = this;
	}

	/*
	public GameObject[] testEnemyPack;

	[ContextMenu("TestCombat")]
	public void TestCombat() {
		CreateCombatInstance(activeCombatArea, testEnemyPack);
	}
*/

	public void CreateCombatInstance(CombatArea combatArea, GameObject[] enemyPack, bool bossPack) {
		Debug.Log("Creating Combat Instance");

		inCombat = true;
		GameManager.GM.primaryCamera.enabled = false;

		var instanceInfo = this.gameObject.AddComponent<CombatInstance>() as CombatInstance;
		Debug.Log("Combat instance initialized, populating...");

		instanceInfo.currentArea = combatArea;
		instanceInfo.alivePlayers = GameManager.GM.activePlayers;
		instanceInfo.overworldPosition = GameManager.GM.activePlayers [0].transform.position;
		instanceInfo.overworldRotation = GameManager.GM.activePlayers [0].transform.rotation;

		for (var i = 0; i < enemyPack.Length; i++) {
			instanceInfo.enemyList.Add(Instantiate(enemyPack [i], combatArea.enemySpawns [i].position, 
				combatArea.enemySpawns [i].rotation).GetComponent<CharacterData>());
		}

		for (var i = 0; i < instanceInfo.alivePlayers.Count; i++) {
			instanceInfo.alivePlayers [i].transform.position = combatArea.playerSpawns [i].position;
			instanceInfo.alivePlayers [i].transform.rotation = combatArea.playerSpawns [i].rotation;
		}

		instanceInfo.isBossEncounter = bossPack;

		Debug.Log("Combat instance created.");
	}

	public void CloseCombatInstance() {
		CombatInstance.CurrentInstance.currentArea.combatCamera.enabled = false;
		//TODO: some shit transitions
		GameManager.GM.primaryCamera.enabled = true;

		inCombat = false;

		var returnToMenu = CombatInstance.CurrentInstance.isBossEncounter;
		Destroy(CombatInstance.CurrentInstance);

		if (returnToMenu) {
			SceneManager.LoadScene(0);
		}
	}

	public void TestAttackEnemy(int enemyIndex) {
		CombatInstance.CurrentInstance.currentCharacter.abilitiesList [0].OnActivate(
			CombatInstance.CurrentInstance.currentCharacter, 
			new CharacterData[1] { CombatInstance.CurrentInstance.enemyList [enemyIndex] }
		);

		CombatInstance.CurrentInstance.NextTurn();
	}
}
