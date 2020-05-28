using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	public string spritesDirectoryPath = "Resources/Sprites/";
	public TextMeshProUGUI powerUpText, deathCountText;
	public Image image;

	public Color Color { get { return image.color; } set { image.color = value; } }

	[SerializeField]
	private Sprite[] powerUpSprites;

	private PassableLock passableLock = new PassableLock();

	#region MonoBehaviour
	void Start()
	{
		UnconditionallyClearSkillUI();
		deathCountText.text = GameManager.GameMode == GameMode.Survival ? Preferences.lives.ToString() : "0";
	}
	#endregion

	public void SetSkillUI(PowerUp powerup)
	{
		passableLock.PassLockTo(powerup);
		image.enabled = true;
		powerUpText.enabled = true;

		powerUpText.text = powerup.GetType().Name;
		image.sprite = Resources.Load<Sprite>(spritesDirectoryPath + powerup.GetType().Name);
	}

	public void ClearSkillUIFor(PowerUp powerup)
	{
		if (passableLock.LockPassing(powerup))
		{
			UnconditionallyClearSkillUI();
			passableLock.Release();
		}
	}

	public void UnconditionallyClearSkillUI()
	{
		image.enabled = false;
		powerUpText.enabled = false;
	}
}
