
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AIChatReponse", order = 1)]
public class AIChatResponse : ScriptableObject
{
    [field: SerializeField] public string[] keywords { private set; get; }
    [field: SerializeField] public int keywordsToMatch { private set; get; }
    [field: SerializeField] public string response { private set; get; }
}