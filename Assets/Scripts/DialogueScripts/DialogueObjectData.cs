using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DialogueObject", menuName = "DialogueObjectData", order = 1)]
public class DialogueObjectData : ScriptableObject
{
    public string NPCName;
    public Color DialogueObjectColor;
    public Color DialogueObjectNameColor;
    public DialogueData dialogue;
}
