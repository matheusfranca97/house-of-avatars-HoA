
using UnityEngine;
using UnityEngine.UI;

public class AvatarOption : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image avatarImage;
    [SerializeField] private float maxOffset;
    [SerializeField] private float minOffsetAdjustmentSpeed;
    [SerializeField] private float offsetAdjustmentFactor;
    [SerializeField] private float distanceOffsetMultiplier;
    [SerializeField] private AnimationCurve sizeOffsetCurve;
    [SerializeField] private AnimationCurve distanceOffsetCurve;
    [SerializeField] private AnimationCurve alphaOffsetCurve;

    private RectTransform rectTransform => transform as RectTransform;
    private AvatarSelectionWheel selectionWheel;
    private int avatarIndex;
    private float targetPosition;
    private float currentPosition;

    public void Initialize(AvatarSelectionWheel selectionWheel, PlayerAvatarData playerAvatarData, int avatarIndex)
    {
        this.selectionWheel = selectionWheel;
        this.avatarIndex = avatarIndex;
        currentPosition = selectionWheel.selectedAvatarIndex.value;
        targetPosition = currentPosition;
        avatarImage.sprite = playerAvatarData.avatarSprite;
        selectionWheel.selectedAvatarIndex.onValueChange += OnValueChanged_SelectionWheel_SelectedAvatarIndex;
        SetPosition();
    }

    private void OnValueChanged_SelectionWheel_SelectedAvatarIndex(short oldValue, short newValue)
    {
        targetPosition = selectionWheel.selectedAvatarIndex.value;
    }

    private void Update()
    {
        if (targetPosition == currentPosition)
            return;

        float difference = targetPosition - currentPosition;
        float moveAmount = (difference * offsetAdjustmentFactor) * Time.deltaTime;
        if (difference < 0)
            moveAmount -= minOffsetAdjustmentSpeed * Time.deltaTime;
        else
            moveAmount += minOffsetAdjustmentSpeed * Time.deltaTime;

        if (Mathf.Abs(moveAmount) > Mathf.Abs(difference))
            moveAmount = difference;

        currentPosition += moveAmount;
        SetPosition();
    }

    private void SetPosition()
    {
        float difference = avatarIndex - currentPosition;
        difference = Mathf.Clamp(difference, -maxOffset, maxOffset);

        if(difference < 0)
        {
            float differenceFactor = difference / -maxOffset;
            float sizeFactor = sizeOffsetCurve.Evaluate(differenceFactor);
            float distance = distanceOffsetCurve.Evaluate(differenceFactor) * distanceOffsetMultiplier;
            float alpha = alphaOffsetCurve.Evaluate(differenceFactor);

            canvasGroup.alpha = alpha;
            rectTransform.anchoredPosition = new Vector2(-distance, 0);
            rectTransform.localScale = new Vector2(sizeFactor, sizeFactor);
        }
        else
        {
            float differenceFactor = difference / maxOffset;
            float sizeFactor = sizeOffsetCurve.Evaluate(differenceFactor);
            float distance = distanceOffsetCurve.Evaluate(differenceFactor) * distanceOffsetMultiplier;
            float alpha = alphaOffsetCurve.Evaluate(differenceFactor);

            canvasGroup.alpha = alpha;
            rectTransform.anchoredPosition = new Vector2(distance, 0);
            rectTransform.localScale = new Vector2(sizeFactor, sizeFactor);
        }
    }

    private void OnDestroy()
    {
        selectionWheel.selectedAvatarIndex.onValueChange -= OnValueChanged_SelectionWheel_SelectedAvatarIndex;
    }
}