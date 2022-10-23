using UnityEngine;

public class StoreToHome : MonoBehaviour
{
    public GameObject buttonManagerObject;
    private AllButtons buttonManager;

    public GameObject startCanvasObject;
    public GameObject shopCanvasObject;

    private RectTransform startCanvas;
    private RectTransform shopCanvas;

    // Start is called before the first frame update
    void Start()
    {
        buttonManager = buttonManagerObject.GetComponent<AllButtons>();

        startCanvas = startCanvasObject.GetComponent<RectTransform>();
        shopCanvas = shopCanvasObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        startCanvas.localPosition = Vector3.Lerp(startCanvas.localPosition, buttonManager.startCanvasOrignalPos, buttonManager.transitionSpeed * Time.deltaTime);
        shopCanvas.localPosition = Vector3.Lerp(shopCanvas.localPosition, buttonManager.shopCanvasOriginalPos, buttonManager.transitionSpeed * Time.deltaTime);
    }
}
