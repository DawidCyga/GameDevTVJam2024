using UnityEngine;

public class TutorialDialogueTrigger : MonoBehaviour
{
    [SerializeField] private int _currentDialogueIndex;

    private bool _hasBenTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasBenTriggered) { return; }
        if (collision.TryGetComponent(out Player player))
        {
            _hasBenTriggered = true;
            TriggerNextDialogue();
        }
    }

    private void TriggerNextDialogue() => TutorialGameManager.Instance.StartDialogue(_currentDialogueIndex);
}
