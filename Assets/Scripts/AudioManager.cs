using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {
	public static AudioManager audioManager;
	public static AudioSource musicSource;
	public AudioMixer mixer;

	[Header("Audio Clips")]
	[SerializeField]
	private MusicClip[] musicClips;
	[SerializeField]
	private SoundClip[] sfxClips;

	private static float unmutedMaster;
	private static float unmutedMusic;
	private static float unmutedEffects;

	//TODO add OnLevelLoad events
	private void Awake() {
		if (audioManager == null) {
			audioManager = this;
			DontDestroyOnLoad(this);
			musicSource = this.GetComponent<AudioSource>();
			musicSource.spatialBlend = 0;
			musicSource.loop = true;
		} else {
			Destroy(this.gameObject);
		}
	}

	//Sets decibel level for Master group MasterMixer
	public void SetMasterLevel(float decibelLevel) {
		this.mixer.SetFloat("masterVol", decibelLevel);
	}

	//Sets decibel level for Music group MasterMixer
	public void SetMusicLevel(float decibelLevel) {
		this.mixer.GetFloat("musicVol", out unmutedMusic);
		this.mixer.SetFloat("musicVol", decibelLevel);
	}

	//Sets decibel level for SoundFx group MasterMixer
	public void SetEffectsLevel(float decibelLevel) {
		this.mixer.GetFloat("sfxVol", out unmutedEffects);
		this.mixer.SetFloat("sfxVol", decibelLevel);
	}

	public void MuteMaster(bool isMute) {
		if (isMute) {
			this.mixer.GetFloat("masterVol", out unmutedMaster);
			this.SetMasterLevel(-80);
		} else {
			this.SetMasterLevel(unmutedMaster);
		}
	}

	public void MuteMusic(bool isMute) {
		if (isMute) {
			this.mixer.GetFloat("musicVol", out unmutedMusic);
			this.SetMusicLevel(-80);
		} else {
			this.SetMusicLevel(unmutedMusic);
		}
	}

	public void MuteEffects(bool isMute) {
		if (isMute) {
			this.mixer.GetFloat("sfxVol", out unmutedEffects);
			this.SetEffectsLevel(-80);
		} else {
			this.SetEffectsLevel(unmutedEffects);
		}
	}

	//TODO music track fading on track switch
	public static void PlayMusicTrack(string trackName) {
		if (audioManager.musicClips.Any(clip => clip.soundName == trackName)) {
			musicSource.Stop();
			musicSource.clip = audioManager.musicClips.First(track => track.soundName == trackName).soundAudio;
			musicSource.Play();
		} else {
			Debug.LogWarning("No music track was found with the name: " + trackName);
		}
	}

	public static void PlayMusicTrack(int levelIndex) {
		if (audioManager.musicClips.Any(clip => clip.levelIndex == levelIndex)) {
			musicSource.Stop();
			musicSource.clip = audioManager.musicClips.First(track => track.levelIndex == levelIndex).soundAudio;
			musicSource.Play();
		} else {
			Debug.LogWarning("No default music track was found for the level at index: " + levelIndex);
		}
	}

	//TODO optimize this function
	public static void PlayMusicTrack(int levelIndex, string trackName) {
		if (audioManager.musicClips.Any(clip => clip.soundName == trackName && clip.levelIndex == levelIndex)) {
			musicSource.Stop();
			musicSource.clip = audioManager.musicClips.
				First(track => track.soundName == trackName && track.levelIndex == levelIndex).soundAudio;
			musicSource.Play();
		} else if (audioManager.musicClips.Any(clip => clip.soundName == trackName)) {
			musicSource.Stop();
			musicSource.clip = audioManager.musicClips.First(track => track.soundName == trackName).soundAudio;
			musicSource.Play();
		} else if (audioManager.musicClips.Any(clip => clip.levelIndex == levelIndex)) {
			musicSource.Stop();
			musicSource.clip = audioManager.musicClips.First(track => track.soundName == trackName).soundAudio;
			musicSource.Play();
		} else {
			Debug.LogWarning(
				"No music track was found with the name: " + trackName + " or for level index: " + levelIndex);
		}
	}

	public static void PlaySoundEffect(AudioSource output, string clipName) {
		if (audioManager.sfxClips.Any(clip => clip.particleName == clipName)) {
			output.PlayOneShot(audioManager.sfxClips.First(track => track.particleName == clipName).soundAudio);
		} else {
			Debug.LogWarning("No music track was found with the name: " + clipName);
		}
	}

	public static void PlaySoundEffect(AudioSource output, int trackIndex) {
		output.PlayOneShot(audioManager.sfxClips [trackIndex].soundAudio);
	}
}

[System.Serializable]
public class MusicClip {
	public string soundName;
	//TODO Level checking
	public int levelIndex;
	public AudioClip soundAudio;

	internal MusicClip(string clipName, AudioClip audioFile) {
		this.soundName = clipName;
		this.soundAudio = audioFile;
	}
}

[System.Serializable]
public class SoundClip {
	public string particleName;
	public AudioClip soundAudio;

	internal SoundClip(string clipName, AudioClip audioFile) {
		this.particleName = clipName;
		this.soundAudio = audioFile;
	}
}
