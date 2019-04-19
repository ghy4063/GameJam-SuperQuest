using UnityEngine;

public class CharacterMover : MonoBehaviour {
	// For grabing the data
	private CharacterData data;


	// Use this for initialization
	void Start () {
		// Grabs the data
		this.data = this.GetComponentInParent<CharacterData> ();
	}


	public void MoveUp () {
		this.data.tf.position = this.data.tf.position + (Vector3.forward * this.data.moveSpeed);
	}

	public void MoveDown () {
		this.data.tf.position = this.data.tf.position + (Vector3.back * this.data.moveSpeed);
	}

	public void MoveRight () {
		this.data.tf.position = this.data.tf.position + (Vector3.right * this.data.moveSpeed);
	}

	public void MoveLeft () {
		this.data.tf.position = this.data.tf.position + (Vector3.left * this.data.moveSpeed);
	}
}
