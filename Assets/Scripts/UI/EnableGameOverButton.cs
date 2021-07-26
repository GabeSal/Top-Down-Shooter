using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnableGameOverButton : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private Button _retryButton;
    [SerializeField]
    private Button _returnToMenuButton;
    #endregion

    #region Private Fields
    private Animator _animator;
    private AnimatorClipInfo[] _animatorClipInfo;
    #endregion

    #region Standard Unity Methods
    private void OnEnable()
    {
        _animator = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        _animatorClipInfo = GetCurrentAnimationClip();
        if (GetAnimationClipName() == "GameOverText")
            StartCoroutine(ShowGameOverButtons());
    }
    #endregion

    #region Class Defined Methods
    private AnimatorClipInfo[] GetCurrentAnimationClip()
    {
        return _animator.GetCurrentAnimatorClipInfo(0);
    }

    private IEnumerator ShowGameOverButtons()
    {
        yield return new WaitForSeconds(_animatorClipInfo[0].clip.length);
        _retryButton.gameObject.SetActive(true);
        _returnToMenuButton.gameObject.SetActive(true);
        this.enabled = false;
    }

    private string GetAnimationClipName()
    {
        return _animatorClipInfo[0].clip.name;
    } 
    #endregion
}
