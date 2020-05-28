using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class CanvasOverlay : MonoBehaviour
{
	public float defaultAlpha = 1f, secondsPerCountdown = 1.5f, fadeTime = 0.5f;
	public List<string> countdownStrings = new List<string> { "3", "2", "1", "GO!" };
	public Canvas canvas;
	public CanvasGroup canvasGroup;
	public TextMeshProUGUI text;
	public Image image;

	public IEnumerator FadeIn(float seconds)
	{
		canvasGroup.alpha = 0f;
		canvas.enabled = true;
		float elapsedTime = 0f;
		while (canvasGroup.alpha < 1f)
		{
			elapsedTime += Time.unscaledDeltaTime;
			canvasGroup.alpha = elapsedTime / seconds;
			yield return null;
		}
	}

	public IEnumerator FadeOut(float seconds)
	{
		canvasGroup.alpha = 1f;
		canvas.enabled = true;
		float elapsedTime = 0f;
		while (canvasGroup.alpha > 0f)
		{
			elapsedTime += Time.unscaledDeltaTime;
			canvasGroup.alpha = 1f - (elapsedTime / seconds);
			yield return null;
		}
		canvas.enabled = false;
	}

	public IEnumerator Countdown(Action callback)
	{
		Color color = image.color;
		color.a = defaultAlpha;
		image.color = color;

		float seconds = countdownStrings.Count * secondsPerCountdown;
		float elapsedTime = 0f;

		canvas.enabled = true;

		while (image.color.a > 0f)
		{
			elapsedTime += Time.unscaledDeltaTime;
			color.a = 1f - (elapsedTime / seconds);
			image.color = color;
			text.text = countdownStrings[(int)((elapsedTime - secondsPerCountdown) / secondsPerCountdown)];
			yield return null;
		}
		StartCoroutine(coroutine());
		IEnumerator coroutine()
		{
			yield return new WaitForSecondsRealtime(secondsPerCountdown);
			canvas.enabled = false;
		}
		AudioManager.Instance?.Play("BackgroundMusic");
		callback();
	}
}
