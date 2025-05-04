using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTest : MonoBehaviour
{
    [field: SerializeField] private float[] Angle {get; set;}

    private void OnDrawGizmos()
    {
        for (int i = 0; i < Angle.Length; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, Quaternion.Euler(0, Angle[i], 0) * Vector3.forward * 5 + transform.position);
        }
    }
}
