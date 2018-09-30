using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level {

    public int heartMaxLife;
    [Range(5, 30)]
    public int potatoMax;
    [Range(1, 5)]
    public int potatoGrowth;

}
