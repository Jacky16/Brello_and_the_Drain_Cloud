using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC", menuName = "NPC Data", order = 1)]
public class NPCData : ScriptableObject
{
    public string NPCName;
    public Color NPCColor;
    public Color NPCNameColor;
    public DialogueData dialogue;
}
