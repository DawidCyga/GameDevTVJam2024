using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingDialogue : MonoBehaviour
{
    public static TestingDialogue Instance {  get; private set; }

    public event EventHandler<OnTimeToDisplayDialogueEventArgs> OnTimeToStartDialogue;
    public class OnTimeToDisplayDialogueEventArgs { public int DialogueIdentifier { get; set; } }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            OnTimeToStartDialogue?.Invoke(this, new OnTimeToDisplayDialogueEventArgs { DialogueIdentifier = 0});
        }
    }
}
