using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseEnterBehaviour : MonoBehaviour
{
    #region Class Defined Methods
    public void OnCursorEnter()
    {
        Button button = GetComponent<Button>();
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

        button.transform.localScale *= 1.15f;
        text.fontSize += 10;
    }
    #endregion
}
