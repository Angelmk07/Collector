using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueSettings
{
    public Sprite persona;
    public string name;
    [TextArea] public string dialogue;
}