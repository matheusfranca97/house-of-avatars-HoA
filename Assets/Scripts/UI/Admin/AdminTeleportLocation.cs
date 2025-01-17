using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class AdminTeleportLocation : ScriptableObject
{
    public string locationName;
    public Vector3 position;
    public Quaternion rotation;
}
