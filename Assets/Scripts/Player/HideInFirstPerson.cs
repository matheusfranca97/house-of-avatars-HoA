using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HideInFirstPerson : MonoBehaviour
{
    public void CheckIfHide()
    {
        if (PlayerSettingsManager.instance.cameraMode.value == CameraMode.FirstPerson)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
