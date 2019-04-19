using UnityEngine;

public class FaceTowardsCamera : MonoBehaviour {
	private Transform tf;

	private void Awake() {
		this.tf = this.GetComponent<Transform>();
	}

	private void Update() {
		if (CombatManager.inCombat) {
			/*
			this.tf.LookAt(new Vector3 (
				CombatInstance.CurrentInstance.currentArea.combatCamera.transform.position.x,
				1,
				CombatInstance.CurrentInstance.currentArea.combatCamera.transform.position.z)
			);*/
			this.tf.LookAt(CombatInstance.CurrentInstance.currentArea.combatCamera.transform);

			//this.tf.rotation = Quaternion.Euler(new Vector3 (tf.rotation.x, -tf.rotation.y, tf.rotation.z));
		}
	}
}
