using UnityEngine;

public class LightControl : MonoBehaviour {

	float nRand = 0;

	void Update() {
		this.nRand = Random.Range(4f, 5f);
		this.transform.GetComponent<Light>().intensity = this.nRand;
	}
}
