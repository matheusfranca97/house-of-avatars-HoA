
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance { private set; get; }
    [SerializeField] private float showLoadingScreenTime;
    [SerializeField] private float hideLoadingScreenTime;
    [SerializeField] private CanvasGroup loadingScreenCanvasGroup;
    [SerializeField] private Image loadingProgressImage;
    [SerializeField] private Text loadingProgressText;

    private void Awake()
    {
        instance = this;
        loadingScreenCanvasGroup.Hide();
        SetLoadingProgress(0.4f);
    }

    public IEnumerator Routine_ShowLoadingScreen()
    {
        loadingScreenCanvasGroup.interactable = true;
        loadingScreenCanvasGroup.blocksRaycasts = true;
        float timeInState = Time.deltaTime;
        while (timeInState < showLoadingScreenTime)
        {
            float timeFactor = timeInState / showLoadingScreenTime;
            loadingScreenCanvasGroup.alpha = timeFactor;
            yield return null;
            timeInState += Time.deltaTime;
        }
        loadingScreenCanvasGroup.alpha = 1;
    }

    public IEnumerator Routine_HideLoadingScreen()
    {
        float timeInState = Time.deltaTime;
        while (timeInState < showLoadingScreenTime)
        {
            float timeFactor = timeInState / showLoadingScreenTime;
            loadingScreenCanvasGroup.alpha = 1 - timeFactor;
            yield return null;
            timeInState += Time.deltaTime;
        }
        loadingScreenCanvasGroup.alpha = 0;
        loadingScreenCanvasGroup.interactable = false;
        loadingScreenCanvasGroup.blocksRaycasts = false;
    }

    public void SetLoadingProgress(float progress)
    {
        loadingProgressImage.fillAmount = progress;
        int progressPercentage = (int)(progress * 100f);
        loadingProgressText.text = string.Format("{0}%", progressPercentage);
    }
}