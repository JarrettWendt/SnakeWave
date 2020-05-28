using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
	public Color Color
	{
		get { return knobImage.color; }
		set
		{
			knobImage.color = value;
			knob.transform.localPosition = XYFromColor(value); ;
			onValueChange?.Invoke(value);
		}
	}

	public Action<Color> onValueChange;

	private GameObject panel, knob;
	private Texture2D texture;
	private Image knobImage;

	#region MonoBehaviour
	void Awake()
	{
		panel = transform.Find("Panel").gameObject;
		knob = panel.transform.Find("Knob").gameObject;
		knobImage = knob.GetComponent<Image>();

		RectTransform rectTransform = panel.transform as RectTransform;
		texture = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height);
		panel.GetComponent<RawImage>().texture = texture;

		// Color calls XYFromColor which depends on texture being set.
		Color = (knobImage.color == default) ? Util.RandomColor() : knobImage.color;
	}

	private void Start()
	{
		for (int x = 0; x < texture.width; x++)
		{
			for (int y = 0; y < texture.height; y++)
			{
				texture.SetPixel(x, y, ColorFromXY(x, y));
			}
		}
		texture.Apply();
	}

	void Update()
	{
		if (Input.GetMouseButton(0) && panel.GetLocalMouse(out Vector2 mousePos))
		{
			Color = ColorFromXY(mousePos);
		}
	}
	#endregion

	private Color ColorFromXY(int x, int y)
	{
		return ColorFromXY(new Vector2(x, y));
	}

	private Color ColorFromXY(Vector2 v)
	{
		float halfHeight = texture.height / 2f;
		Color color = Color.HSVToRGB(v.x / texture.width, 1f, 1f);
		if (v.y > halfHeight)
		{
			color.SetSaturation(2f - v.y / halfHeight);
		}
		else
		{
			color.SetValue(v.y / halfHeight);
		}
		return color;
	}

	private Vector2 XYFromColor(Color color)
	{
		return new Vector2(
			x: color.GetHue() * texture.width,
			y: (color.GetValue() < 1f) ? color.GetValue() * texture.height / 2f : (2f - color.GetSaturation()) * texture.height / 2f);
	}
}
