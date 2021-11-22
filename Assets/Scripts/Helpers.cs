using UnityEngine;

public static class Helpers {
  
  public static float RoundToNearest(this float value, float unit, float offset = 0f) {
      return (Mathf.Round((value - offset) / unit) * unit) + offset;
  }


  public static Vector3 ToVector3(this Vector2 vector2, float z = 0) {
    return new Vector3(vector2.x, vector2.y, z);
  }

  public static Vector2 ToVector2(this Vector3 vector3) {
    return new Vector3(vector3.x, vector3.y);
  }

  public static Vector2Int ToVector2Int(this Vector3Int vector3) {
    return new Vector2Int(vector3.x, vector3.y);
  }

  public static Vector2 RoundToNearest(this Vector2 vector, Vector2 unit) {
    return vector.RoundToNearest(unit, Vector2.zero);
  }

  public static Vector2 RoundToNearest(this Vector2 vector, Vector2 unit, Vector2 offset) {
    return new Vector2(vector.x.RoundToNearest(unit.x, offset.x), vector.y.RoundToNearest(unit.y, offset.y));
  }


  public static float Mod(this float a, float mod)
  {
    return a - mod * Mathf.Floor(a / mod);
  }

  public static int Mod(this int a, int mod)
  {
    return a - mod * Mathf.FloorToInt((float)a / (float)mod);
  }

  public static void Invoke(this MonoBehaviour mb, System.Action f, float delay, bool useUnscaled=false)
  {
    mb.StartCoroutine(InvokeRoutine(f, delay, useUnscaled));
  }

  private static System.Collections.IEnumerator InvokeRoutine(System.Action f, float delay, bool useUnscaled)
  {
    if (useUnscaled) {
      yield return new WaitForSecondsRealtime(delay);
    }
    else {
      yield return new WaitForSeconds(delay);
    }
    
    f();
  }


  public static Color WithAlpha(this Color color, float alpha) {
    return new Color(color.r, color.g, color.b, alpha);
  }


  public static float DBToVolume(float db) {
    return Mathf.Pow(10f, db / 20f);
  }

  public static float VolumeToDB(float volume) {
    return Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
  }

}


public delegate void EmptyDelegate();