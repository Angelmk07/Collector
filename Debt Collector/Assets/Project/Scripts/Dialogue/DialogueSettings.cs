using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueSettings
{
    public Sprite persona_01;
    public Color colorPersona_01;
    public Sprite persona_02;
    public Color colorPersona_02;
    public string name;
    [TextArea] public string dialogue;
}