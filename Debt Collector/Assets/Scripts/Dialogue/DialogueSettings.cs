using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueSettings
{
    public Image persona;
    public string name;
    [TextArea] public string dialogue;
}