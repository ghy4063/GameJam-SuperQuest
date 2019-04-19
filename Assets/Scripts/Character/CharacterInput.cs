using UnityEngine;

public class CharacterInput : MonoBehaviour {
	private CharacterData data;

	[Header("Movement Controles")]
	public KeyCode upKey = KeyCode.W;
	public KeyCode downKey = KeyCode.S;
	public KeyCode leftKey = KeyCode.A;
	public KeyCode rightKey = KeyCode.D;
	public KeyCode interactionKey = KeyCode.F;

	[HideInInspector]
	public bool interactActive;

	void Start() {
		this.data = this.GetComponentInParent<CharacterData>();	
	}

	void Update() {
		if (CombatManager.inCombat == false) {
			if (Input.GetKey(this.upKey)) {
				this.data.gameObject.SendMessage("MoveUp");
			}

			if (Input.GetKey(this.downKey)) {
				this.data.gameObject.SendMessage("MoveDown");
			}

			if (Input.GetKey(this.rightKey)) {
				this.data.gameObject.SendMessage("MoveRight");
			}

			if (Input.GetKey(this.leftKey)) {
				this.data.gameObject.SendMessage("MoveLeft");
			}
		}
	}

	void OnCollisionEnter(Collision other) {
		// If the player collides with this object
		if (other.gameObject.CompareTag("Enemy")) {
			//TODO: transition between scenes.
			CombatManager.CM.CreateCombatInstance(
				CombatManager.CM.activeCombatArea, 
				other.gameObject.GetComponent<AIController>().PackData,
				other.gameObject.GetComponent<AIController>().isBoss
			);
			Destroy(other.gameObject);
		}

	}

	void OnCollisionStay(Collision other) {
		// If the player collides with this object
		if (other.gameObject.CompareTag("InteractibleObject")) {
			this.interactActive = true;
			if (Input.GetKey(this.interactionKey)) {
				if (this.interactActive == true) {
					// TODO: Have the character do something when there is something to interacte with and destroy the interactive object
				}
			}
		}
	}

	void OnCollisionExit(Collision other) {
		// If the player exits collision wiht this game object
		if (other.gameObject.CompareTag("InteractibleObject")) {
			// Deactivate the interactive key
			other.gameObject.GetComponent<CharacterInput>().interactActive = false;
		}
	}

}
