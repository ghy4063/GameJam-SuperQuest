using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemEvents : MonoBehaviour
{
	public void ExitToDesktop()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	public void LoadLevelByIndex(int index)
	{
		SceneManager.LoadScene(index);
	}
}
