
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceChatController : MonoBehaviour
{
    public static WorldSpaceChatController instance { private set; get; }

    [SerializeField] private WorldSpaceChat worldSpaceChatPrefab;
    [SerializeField] private RectTransform worldSpaceChatContainer;

    private readonly List<WorldSpaceChat> worldSpaceChatInstances;

    private WorldSpaceChatController()
    {
        worldSpaceChatInstances = new List<WorldSpaceChat>();
    }

    private void Awake()
    {
        instance = this;
    }

    public void CreateWorldSpaceChat(Transform followTransform, string name, string message)
    {
        WorldSpaceChat instance = worldSpaceChatInstances.Find(i => i.followTransform == followTransform);

        if (instance == null)
        {
            instance = GameObject.Instantiate(worldSpaceChatPrefab, worldSpaceChatContainer);
            instance.Initialize(followTransform, name, message);
            worldSpaceChatInstances.Add(instance);
        }
        else
        {
            instance.UpdateText(message);
        }
    }

    public void RemoveWorldSpaceChat(WorldSpaceChat instance)
    {
        worldSpaceChatInstances.Remove(instance);
        GameObject.Destroy(instance.gameObject);
    }
}