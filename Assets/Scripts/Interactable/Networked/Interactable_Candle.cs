using Unity.Netcode;
using System.Collections;
using UnityEngine;

public class Interactable_Candle : NetworkInteractable
{
    [SerializeField] private Light candleLight;
    [SerializeField] private ParticleSystem[] activatedParticleSystems;
    private bool lit = true;

    private void Awake()
    {
        if (candleLight != null)
        {
            candleLight.gameObject.SetActive(true);
        }
        foreach (ParticleSystem particleSystem in activatedParticleSystems)
        {
            if (lit)
            {
                particleSystem.Play(true);
            }
            else
            {
                particleSystem.Stop(true);
            }
        }
    }

    protected override void Interact(bool isTriggerPlayer, bool isGuest)
    {
        if (isGuest)
        {
            return;
        }
        lit = !lit;
        if (candleLight != null)
        {
            candleLight.gameObject.SetActive(lit);
        }
        foreach (ParticleSystem particleSystem in activatedParticleSystems)
        {
            if (lit)
            {
                particleSystem.Play(true);
            }
            else
            {
                particleSystem.Stop(true);
            }
        }
    }
}