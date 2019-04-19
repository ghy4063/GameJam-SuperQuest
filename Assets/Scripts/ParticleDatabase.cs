using UnityEngine;
using System.Linq;

public class ParticleDatabase : MonoBehaviour {
	public static ParticleDatabase masterData;

	[SerializeField]
	private ParticleObject[] globalParticles;

	private void Awake() {
		if (masterData == null) {
			masterData = this;
			DontDestroyOnLoad(this);
		} else {
			Destroy(this);
		}
	}

	public static void SpawnParticleEffect(Vector3 spawnPosition, Vector3[] targets, string effectName) {
		if (masterData.globalParticles.Any(particle => particle.particleName == effectName)) {
			var particleObject = masterData.globalParticles.First(particle => 
				particle.particleName == effectName).particleObject;

			foreach (var target in targets) {
				var tempObj = Instantiate(particleObject, spawnPosition, Quaternion.identity);
				tempObj.transform.LookAt(target);
			}
		} else {
			Debug.LogWarning("No particle effect was found with the name: " + effectName);
		}
	}
}

[System.Serializable]
public class ParticleObject {
	public string particleName;
	public GameObject particleObject;

	internal ParticleObject(string particleName, GameObject particleObject) {
		this.particleName = particleName;
		this.particleObject = particleObject;
	}
}
