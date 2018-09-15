using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {

    public static float Angle(float x, float y)
    {
        float value = Mathf.Atan2(x, y) * Mathf.Rad2Deg;
        if (value < 0) value += 360f;

        return value;
    }
    public static float AngleFull(float x, float y)
    {
        float value = Vector2.SignedAngle(Vector2.right, new Vector2(x, y));

        return value;
    }

    public static Vector3 GetFlooredPosition(Vector3 position)
    {
        return new Vector3(Mathf.Floor(position.x) + .5f, 0, Mathf.Floor(position.z) + .5f);
    }
}
