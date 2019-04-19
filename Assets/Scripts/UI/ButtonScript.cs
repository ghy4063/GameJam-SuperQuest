using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.OnMouseUp ();
	}

	void OnMouseUp () {
		if (Input.GetKey (KeyCode.Mouse0)) {
			SceneManager.LoadScene ("Levels/Forest/Forest", LoadSceneMode.Single);
		}

	}
}
