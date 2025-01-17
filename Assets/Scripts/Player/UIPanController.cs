using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UIPanController : MonoBehaviour
{
    public static UIPanController instance;

    public float panX = 0;
    public float panY = 0;

    private void Awake()
    {
        instance = this;
    }

    public void OnPanUpStarted()
    {
        panY = -1;
    }

    public void OnPanYEnded()
    {
        panY = 0;
    }

    public void OnPanDownStarted()
    {
        panY = 1;
    }

    public void OnPanLeftStarted()
    {
        panX = -1;
    }

    public void OnPanXEnded()
    {
        panX = 0;
    }

    public void OnPanRightStarted()
    {
        panX = 1;
    }
}