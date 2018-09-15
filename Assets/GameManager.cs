using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int columns;
    public int rows;

    private int[,] squaresArray;

    private void Start()
    {
        squaresArray = new int[columns, rows];
    }

    private void OnDrawGizmos()
    {
        squaresArray = new int[columns, rows];

        for (int x = 0; x < squaresArray.GetLength(0); x ++)
        {
            for (int y = 0; y < squaresArray.GetLength(1); y++)
            {
                Gizmos.DrawSphere(new Vector3(x + .5f, 0f, y + .5f), .3f);
            }
        }
    }
}
