using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDialogueTrigger : MonoBehaviour
{
    [SerializeField] private int _currentDialogueIndex;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            TriggerNextDialogue();
        }
    }

    private void TriggerNextDialogue() => TutorialGameManager.Instance.StartDialogue(_currentDialogueIndex);

}
