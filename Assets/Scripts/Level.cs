using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level {

    [Range(0, 5)]
    public int potatoAmountBonus;
    [Range(0, 1)]
    public int potatoGrowthBonus;

}
