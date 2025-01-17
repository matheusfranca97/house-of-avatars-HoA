using System;
using UnityEngine;

public class NPCAnimationRandomizer : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float minTimeBetweenAnimations;
    [SerializeField] private float maxTimeBetweenAnimations;

    private float nextAnimationTime;

    private void Awake()
    {
        nextAnimationTime = Time.time + UnityEngine.Random.Range(minTimeBetweenAnimations, maxTimeBetweenAnimations);
    }

    private void Update()
    {
        if (nextAnimationTime > Time.time)
            return;

        nextAnimationTime = Time.time + UnityEngine.Random.Range(minTimeBetweenAnimations, maxTimeBetweenAnimations);
        NPCAnimationType[] npcAnimations = Enum.GetValues(typeof(NPCAnimationType)) as NPCAnimationType[];
        int randomIndex = UnityEngine.Random.Range(0, npcAnimations.Length);
        NPCAnimationType npcAnimation = npcAnimations[randomIndex];
        string triggerName = npcAnimation.GetIdentifier();
        animator.SetTrigger(triggerName);
    }
}