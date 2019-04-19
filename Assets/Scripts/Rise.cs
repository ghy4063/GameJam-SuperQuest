using UnityEngine;

public class Rise : MonoBehaviour {
	private Transform tf;
	private float up=.1f;
	public Transform[] howHigh;
	// Use this for initialization
	void Start () {
		this.tf = this.GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
		this.tf.position = Vector3.MoveTowards (this.tf.position, this.howHigh[0].position, this.up);
	}
}
