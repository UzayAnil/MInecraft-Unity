using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

    public static float Get2DPerlin(Vector2 position, float ofset, float scale){

        return Mathf.PerlinNoise((position.x + 0.1f) * scale + ofset, (position.y + 0.1f) * scale + ofset);
    }

    public static bool Get3DPerlin(Vector3 position, float ofset, float scale, float threashhold){

        float x = ((position.x + ofset + 0.1f) * scale);
        float y = ((position.y + ofset + 0.1f) * scale);
        float z = ((position.z + ofset + 0.1f) * scale);

        float AB = Mathf.PerlinNoise(x, y);
        float BC = Mathf.PerlinNoise(y, z);
        float AC = Mathf.PerlinNoise(x, z);
        float BA = Mathf.PerlinNoise(y, x);
        float CB = Mathf.PerlinNoise(z, y);
        float CA = Mathf.PerlinNoise(z, x);

        float val = (AB + BC + AC + BA + CB + CA) / 6f;

        if (val < threashhold)
            return true;
        else
            return false;
    }

    
}
