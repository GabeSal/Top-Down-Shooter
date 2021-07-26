using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseExitBehaviour : MonoBehaviour
{
    #region Class Defined Methods
    public void OnCursorExit()
    {
        Button button = GetComponent<Button>();
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

        button.transform.localScale /= 1.15f;
        text.fontSize -= 10;
    }
    #endregion
}
