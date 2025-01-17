using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] public Text counter;

    private IEnumerator Start()
    {
        PlayerSettingsManager.instance.authLevel.onValueChange += OnAuthLevelUpdated;
        counter.gameObject.SetActive(false);
        while (true)
        {
            counter.text = $"{Mathf.Round(1f / Time.unscaledDeltaTime)} FPS";
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F2) && PlayerSettingsManager.instance.authLevel.value > PlayerAuthLevel.User)
        {
            counter.gameObject.SetActive(!counter.gameObject.activeInHierarchy);
        }
    }

    private void OnAuthLevelUpdated(PlayerAuthLevel oldValue, PlayerAuthLevel newValue)
    {
        gameObject.SetActive(newValue > PlayerAuthLevel.User);
    }
}