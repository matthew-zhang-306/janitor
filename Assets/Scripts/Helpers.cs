using UnityEngine;

public static class Helpers {
  
  public static Vector3 ToVector3(this Vector2 vector2, float z = 0) {
    return new Vector3(vector2.x, vector2.y, z);
  }

  public static Vector2 ToVector2(this Vector3 vector3) {
    return new Vector3(vector3.x, vector3.y);
  }


/*
  public static float Mod(this float a, float mod)
  {
      return a - mod * Mathf.Floor(a / mod);
  }


  public static Vector2 FloorToNearest(this Vector2 vector2, float increment) {

  }

  public static Vector2 CeilToNearest(this Vector2 vector2, float increment) {

  }
*/

}