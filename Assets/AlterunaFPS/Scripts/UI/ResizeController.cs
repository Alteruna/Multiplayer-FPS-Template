using UnityEngine;

[ExecuteInEditMode]
public class ResizeController : MonoBehaviour
{
    public enum ResizeType
    {
        Size,
        Percentage
    }

    public ResizeType currentResizeType = ResizeType.Size;

    // Size-based variables
    public Vector2 maxSize = new Vector2(1600, 800);
    // Percentage-based variables
    public Vector2 percentageOfScreen = new Vector2(0.85f, 0.75f);

    private RectTransform rect;
    private RectTransform parentRect;


    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        parentRect = transform.parent.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (currentResizeType == ResizeType.Percentage)
            ResizeByPercentage();
        else
            ResizeBySize();
    }

    private void ResizeByPercentage()
    {
        var sizePercentage = new Vector2(parentRect.rect.width * percentageOfScreen.x, parentRect.rect.height * percentageOfScreen.y);
        var size = new Vector2(Mathf.Min(sizePercentage.x, parentRect.rect.width), Mathf.Min(sizePercentage.y, parentRect.rect.height));
        rect.sizeDelta = size;
    }

    private void ResizeBySize()
    {
        var size = new Vector2(Mathf.Min(maxSize.x, parentRect.rect.width), Mathf.Min(maxSize.y, parentRect.rect.height));
        rect.sizeDelta = size;
    }

    private void Reset()
    {
        rect = GetComponent<RectTransform>();
    }
}