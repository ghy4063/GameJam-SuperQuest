using UnityEngine;

[System.Serializable]
public class CombatArea : MonoBehaviour {
	public AreaType areaType = AreaType.Forest;
	public Camera combatCamera;
	public Transform[] playerSpawns;
	public Transform[] enemySpawns;

	public void Awake() {
		this.combatCamera.enabled = false;
	}
}