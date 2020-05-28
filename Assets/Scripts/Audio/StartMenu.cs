using UnityEngine;

public class StartMenu : MonoBehaviour
{
	#region MonoBehaviour
	void Start()
	{
		AudioManager.Instance?.Play("AnnouncerStart");
		AudioManager.Instance?.Play("StartMenuMusic", 1.75f);
	}
	#endregion
}
