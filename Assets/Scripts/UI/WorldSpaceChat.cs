
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceChat : MonoBehaviour
{
    [SerializeField] private float timeBeforeFadeout;
    [SerializeField] private float fadeInTime;
    [SerializeField] private float faceOutTime;
    [SerializeField] private Text nameTextField;
    [SerializeField] private Text chatTextField;
    [SerializeField] private CanvasGroup canvasGroup;

    public Transform followTransform { private set; get; }
    private Canvas canvas;

    private RectTransform rectTransform => transform as RectTransform;
    private Coroutine fadeOutRoutine;

    public void Initialize(Transform followTransform, string name, string text)
    {
        this.nameTextField.text = name;
        this.chatTextField.text = text;
        this.followTransform = followTransform;
        canvas = GetComponentInParent<Canvas>();
        StartCoroutine(Routine_FadeIn());
    }

    public void UpdateText(string text)
    {
        this.chatTextField.text = text;

        // Only restart if the coroutine is already running
        if(fadeOutRoutine != null)
        {
            StopCoroutine(fadeOutRoutine);
            fadeOutRoutine = StartCoroutine(Routine_FadeOut());
        }
    }

    private IEnumerator Routine_FadeIn()
    {
        float timeInState = Time.deltaTime;
        while(timeInState < fadeInTime)
        {
            float timeFactor = timeInState / fadeInTime;
            canvasGroup.alpha = timeFactor;
            yield return null;
            timeInState += Time.deltaTime;
        }

        fadeOutRoutine = StartCoroutine(Routine_FadeOut());
    }

    private IEnumerator Routine_FadeOut()
    {
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(timeBeforeFadeout);

        float timeInState = Time.deltaTime;
        while (timeInState < fadeInTime)
        {
            float timeFactor = timeInState / fadeInTime;
            canvasGroup.alpha = 1 - timeFactor;
            yield return null;
            timeInState += Time.deltaTime;
        }

        canvasGroup.alpha = 0;
        WorldSpaceChatController.instance.RemoveWorldSpaceChat(this);
    }

    private void LateUpdate()
    {
        Vector3 difference = (followTransform.position - Camera.main.transform.position).normalized;
        if (Vector3.Dot(Camera.main.transform.forward, difference) <= 0)
        {
            rectTransform.anchoredPosition = new Vector3(-10000,0);
            return;
        }

        Vector2 screenPoint = Camera.main.WorldToScreenPoint(followTransform.position);
        screenPoint.x /= canvas.scaleFactor;
        screenPoint.y /= canvas.scaleFactor;
        rectTransform.anchoredPosition = screenPoint;
    }
}