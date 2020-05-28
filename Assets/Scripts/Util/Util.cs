using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public static class Util
{
	public const float _2PI = Mathf.PI * 2, PI_2 = Mathf.PI / 2;

	// T must be Serializable.
	public static T DeepCopy<T>(T t)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		binaryFormatter.Serialize(memoryStream, t);
		memoryStream.Position = 0;
		return (T)binaryFormatter.Deserialize(memoryStream);
	}

	public static int NumElementsInEnum<T>()
	{
		return Enum.GetNames(typeof(T)).Length;
	}

	public static string RemoveAfter(this string str, string sub)
	{
		return str.Substring(0, str.IndexOf(sub));
	}

	public static bool ContainsAny<T>(this ISet<T> set, params T[] args)
	{
		foreach (T t in args)
		{
			if (set.Contains(t))
			{
				return true;
			}
		}
		return false;
	}

	public static bool ContainsAll<T>(this ISet<T> set, params T[] args)
	{
		foreach (T t in args)
		{
			if (!set.Contains(t))
			{
				return false;
			}
		}
		return true;
	}

	public static bool RandomBool()
	{
		return Random.value < 0.5f;
	}

	#region Unity
	public static void DoDestroyOnLoad(GameObject go)
	{
		SceneManager.MoveGameObjectToScene(go, SceneManager.GetActiveScene());
	}

	public static bool GetLocalMouse(this GameObject go, out Vector2 result)
	{
		RectTransform rectTransform = go.transform as RectTransform;
		Vector3 mousePos = rectTransform.InverseTransformPoint(Input.mousePosition);
		result.x = Mathf.Clamp(mousePos.x, rectTransform.rect.min.x, rectTransform.rect.max.x);
		result.y = Mathf.Clamp(mousePos.y, rectTransform.rect.min.y, rectTransform.rect.max.y);
		return rectTransform.rect.Contains(mousePos);
	}

	public static void SetExistingParticlesStartLifetime(this ParticleSystem particleSystem, float startLifetime)
	{
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
		particleSystem.GetParticles(particles);
		for (int i = 0; i < particles.Length; i++)
		{
			particles[i].startLifetime = startLifetime;
		}
		particleSystem.SetParticles(particles);
	}

	public static bool HasSameParent(this GameObject go1, GameObject go2)
	{
		return go1.transform.parent == go2.transform.parent;
	}
	#endregion

	#region Primitives
	public static bool ToBool(this float f)
	{
		return f != 0f;
	}

	public static float ToFloat(this bool b)
	{
		return b ? 1f : 0f;
	}

	public static float ReMap(this float value, float sourceMin, float sourceMax, float destMin, float destMax)
	{
		return (value - sourceMin) / (destMin - sourceMin) * (destMax - sourceMax) + sourceMax;
	}

	public static float ReMap(this int value, int sourceMin, int sourceMax, int destMin, int destMax)
	{
		return ReMap((float)value, (float)sourceMin, (float)sourceMax, (float)destMin, (float)destMax);
	}

	public static float ReMap(this int value, int sourceMin, int sourceMax, float destMin, float destMax)
	{
		return ReMap((float)value, (float)sourceMin, (float)sourceMax, destMin, destMax);
	}
	#endregion

	#region LINQ
	// Returns false if 2 or more elements match the predicate, true otherwise.
	// Sets the out parameter to default or the first element matching the predicate.
	public static bool OneOrNone<T>(List<T> list, out T first, Func<T, bool> f)
	{
		first = default;
		foreach (T t in list)
		{
			if (f(t))
			{
				if (first == default)
				{
					first = t;
				}
				else
				{
					return false;
				}
			}
		}
		return true;
	}

	public static int Max<T>(List<T> list, Func<T, T, int> f)
	{
		int max = f(list[0], list[1]);
		for (int i = 2; i < list.Count; i++)
		{
			max = Math.Max(max, f(list[i - 1], list[i]));
		}
		return max;
	}

	public static float Max<T>(List<T> list, Func<T, T, float> f)
	{
		float max = f(list[0], list[1]);
		for (int i = 2; i < list.Count; i++)
		{
			max = Math.Max(max, f(list[i - 1], list[i]));
		}
		return max;
	}
	#endregion

	#region Color
	// See the following useful Unity builtins:
	// Random.ColorHSV
	// Color.HSVToRGB
	// Color.RGBToHSV

	public static Color RandomColorRGB(float minR, float maxR, float minG, float maxG, float minB, float maxB)
	{
		return new Color(Random.Range(minR, maxR), Random.Range(minG, maxG), Random.Range(minB, maxB));
	}

	public static Color RandomColor()
	{
		return RandomColorRGB(0f, 1f, 0f, 1f, 0f, 1f);
	}

	public static Color InverseHue(this Color color)
	{
		color.RGBToHSV(out float hue, out float saturation, out float value);
		return Color.HSVToRGB(1f - hue, saturation, value);
	}

	public static Color SufficientlyDifferentColorHSV(this Color color, float hueDiff, float satDiff, float valDiff)
	{
		hueDiff = Mathf.Clamp(hueDiff, 0f, 1f) / 2;
		satDiff = Mathf.Clamp(satDiff, 0f, 1f) / 2;
		valDiff = Mathf.Clamp(valDiff, 0f, 1f) / 2;
		return Color.HSVToRGB(Random.Range(0f, color.GetHue() + (RandomBool() ? hueDiff : -hueDiff)),
			                  Random.Range(0f, color.GetHue() + (RandomBool() ? satDiff : -satDiff)),
		                      Random.Range(0f, color.GetHue() + (RandomBool() ? valDiff : -valDiff)));
	}

	public static void RGBToHSV(this Color color, out float H, out float S, out float V)
	{
		Color.RGBToHSV(color, out H, out S, out V);
	}

	public static float GetHue(this Color color)
	{
		color.RGBToHSV(out float hue, out _, out _);
		return hue;
	}

	public static void SetHue(ref this Color color, float hue)
	{
		color = Color.HSVToRGB(hue, color.GetSaturation(), color.GetValue());
	}

	public static float GetSaturation(this Color color)
	{
		color.RGBToHSV(out _, out float saturation, out _);
		return saturation;
	}

	public static void SetSaturation(ref this Color color, float saturation)
	{
		color = Color.HSVToRGB(color.GetHue(), saturation, color.GetValue());
	}

	public static float GetValue(this Color color)
	{
		color.RGBToHSV(out _, out _, out float value);
		return value;
	}

	public static void SetValue(ref this Color color, float value)
	{
		color = Color.HSVToRGB(color.GetHue(), color.GetSaturation(), value);
	}

	public static string ToStringColored(this Color color)
	{
		return color.ToStringColored(color);
	}

	public static string HSVString(this Color color)
	{
		return $"(h: {color.GetHue()}, s: {color.GetSaturation()}, v: {color.GetValue()})";
	}
	#endregion

	#region Vector
	public static Vector2 Midpoint(Vector2 v1, Vector2 v2)
	{
		//return new Vector2((v1.x + v2.x) / 2, (v1.y + v2.y) / 2);
		return Vector2.Lerp(v1, v2, 0.5f);
	}

	public static Vector2 Centroid(this List<Vector2> vectors)
	{
		return vectors.Aggregate((v1, v2) => v1 + v2) / vectors.Count;
	}

	public static Vector3 Centroid(this List<Vector3> vectors)
	{
		return vectors.Aggregate((v1, v2) => v1 + v2) / vectors.Count;
	}

	// Returns the angle in degrees between two points.
	public static float Angle(Vector2 v1, Vector2 v2)
	{
		return Mathf.Atan2(v2.y - v1.y, v2.x - v1.x) * 180f / Mathf.PI;
	}

	public static Vector2 ToVector2(this Vector3 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static Vector3 ToVector3(this Vector2 v, float z = 0f)
	{
		return new Vector3(v.x, v.y, z);
	}

	public static Vector3 SetZ(this Vector3 v, float z = 0f)
	{
		return new Vector3(v.x, v.y, z);
	}

	// Returns start moved towards end by magnitude.
	public static Vector2 MoveTowards(Vector2 start, Vector2 end, float magnitude)
	{
		return start + (end - start).normalized * magnitude;
	}

	public static Vector2 RandomVector(Vector2 min, Vector2 max)
	{
		return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
	}
	#endregion

	#region Text
	public static string Bold(this object o)
	{
		return $"<b>{o.ToString()}</b>";
	}

	public static string Ital(this object o)
	{
		return $"<i>{o.ToString()}</i>";
	}

	public static string Size(this object o, int size)
	{
		return $"<size={size}>{o.ToString()}</size>";
	}

	public static string ToStringColored(this object o, Color color)
	{
		return ToStringColored(o, ColorUtility.ToHtmlStringRGB(color));
	}

	// color must be a 6 character hex string.
	public static string ToStringColored(this object o, string color)
	{
		return $"<color=#{color}>{o.ToString()}</color>";
	}
	#endregion
}
