using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogNode", menuName = "Dialog System/Dialog Node")]
public class DialogNode : ScriptableObject
{
    [TextArea(3, 5)]
    public string dialogueText;
    public Sprite characterSprite;
    public List<DialogChoice> choices = new List<DialogChoice>();
    public DialogNode nextNode;
}

[System.Serializable]
public struct DialogChoice
{
    public string choiceText;
    public DialogNode nextNode;
}