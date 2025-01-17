using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class VersionText : MonoBehaviour
{
    private void Start()
    {
        if (TryGetComponent(out Text text))
        {
            text.text = text.text.Replace("{version}", Application.version);
        }
    }
}
