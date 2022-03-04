using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Data", menuName = "Dialogue Data", order =1)]

public class DialogueData : ScriptableObject
{
    [TextArea(4, 4)]
    public List<string> conversationBlock;
}
