using UnityEngine;

public class BurningTreeWarningUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _textContainer;
    [Space]
    [SerializeField] private Transform[] _treesInScene;

    [Header("State")]
    bool _isShowing;

    private void Start()
    {
        TreeScript.OnAnyTreeIgnited += TreeScript_OnAnyTreeIgnited;
        TreeScript.OnAnyTreeExtinguished += TreeScript_OnAnyTreeExtinguished;
    }

    private void TreeScript_OnAnyTreeExtinguished(object sender, System.EventArgs e)
    {
        foreach (Transform transform in _treesInScene)
        {
            if (transform.GetComponent<TreeScript>().IsIgnited()) return;
        }

        ToggleShow(false);

    }

    private void TreeScript_OnAnyTreeIgnited(object sender, System.EventArgs e)
    {
        if (!_isShowing)
        {
            ToggleShow(true);
        }
    }

    private void ToggleShow(bool state)
    {
        switch (state)
        {
            case true:
                _textContainer.gameObject.SetActive(true); return;
            case false:
                _textContainer.gameObject.SetActive(false); return;            
        }
    }
}
