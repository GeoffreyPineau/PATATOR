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
}
