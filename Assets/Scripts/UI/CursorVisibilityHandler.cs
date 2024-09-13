using UnityEngine;

public class CursorVisibilityHandler
{
    public static void SwitchCursorEnabled(bool isEnabled)
    {
        switch (isEnabled)
        {
            case true:
                if (Cursor.lockState != CursorLockMode.None)
                {
                    Cursor.visible = isEnabled;
                    Cursor.lockState = CursorLockMode.None;
                }
                break;
            case false:
                if (Cursor.lockState != CursorLockMode.Locked)
                {
                    Cursor.visible = isEnabled;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                break;
        }
    }
}
