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

  public static void Invoke(this MonoBehaviour mb, System.Action f, float delay)
  {
      mb.StartCoroutine(InvokeRoutine(f, delay));
  }

  private static System.Collections.IEnumerator InvokeRoutine(System.Action f, float delay)
  {
      yield return new WaitForSeconds(delay);
      f();
  }


}


public delegate void EmptyDelegate();