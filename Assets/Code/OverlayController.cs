using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class OverlayController : MonoBehaviour
{

    public Sprite character;
    public Sprite background;
    public CharacterAlignment characterAlignment;

    public float cameraSize = 3;
    public float dialogHeightRatio = 0.25f;
    public float paddingRatio = 0.05f;
    public int maxFontSize = 128;


    public string dialogText = "";

    private Camera _camera;
    private SpriteRenderer _characterRenderer;
    private SpriteRenderer _backgroundRenderer;
    private TextMesh _dialogTextMesh;

    public int ChangeHash
    {
        get
        {
            return 13 * character.GetHashCode()
                + 27 * background.GetHashCode()
                + 37 * characterAlignment.GetHashCode()
                + 43 * cameraSize.GetHashCode()
                + 43 * dialogHeightRatio.GetHashCode()
                + 43 * paddingRatio.GetHashCode()
                + 43 * maxFontSize.GetHashCode()
                + 43 * dialogText.GetHashCode()
                ;
        }
    }

    void Start()
    {
        _camera = transform.FindChild("Camera").gameObject.GetComponent<Camera>();
        _characterRenderer = transform.FindChild("Character").gameObject.GetComponent<SpriteRenderer>();
        _backgroundRenderer = transform.FindChild("Background").gameObject.GetComponent<SpriteRenderer>();
        _dialogTextMesh = transform.FindChild("Dialog Text").gameObject.GetComponent<TextMesh>();
    }

    private int _lastChangeHash = 0;
    void Update()
    {

        if (_lastChangeHash != ChangeHash)
        {
            _lastChangeHash = ChangeHash;
            Redraw();
        }
    }

    void Redraw()
    {
        Debug.Log("Redraw");

        _camera.orthographicSize = cameraSize;

        var cameraBottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
        var cameraTopRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.nearClipPlane));
        var cameraWidth = cameraTopRight.x - cameraBottomLeft.x;
        var cameraHeight = cameraTopRight.y - cameraBottomLeft.y;

        var dialogHeight = cameraHeight * dialogHeightRatio;

        // Reduce camera width and height to create padding
        var paddingWidth = cameraWidth * paddingRatio;
        var paddedCameraWidth = cameraWidth * (1.0f - 2 * paddingRatio);
        var paddedCameraHeight = cameraHeight * (1.0f - 2 * paddingRatio);

        // Assume using center alignment on the sprites
        var spriteCenter = character.bounds.center;
        var spriteLeft = character.bounds.min.x;
        var spriteTop = character.bounds.max.y;

        var xFromEdge = spriteCenter.x - spriteLeft;
        var yFromEdge = spriteTop - spriteCenter.y;

        var spriteWidth = xFromEdge * 2.0f + paddingWidth;

        var characterPosition = new Vector3();

        switch (characterAlignment)
        {
            case CharacterAlignment.BottomLeft:
                characterPosition = new Vector3(-0.5f * paddedCameraWidth + xFromEdge, -0.5f * paddedCameraHeight + yFromEdge, 5);
                _dialogTextMesh.anchor = TextAnchor.MiddleLeft;
                _dialogTextMesh.transform.localPosition = characterPosition + new Vector3(spriteWidth, 0, 0);
                break;
            case CharacterAlignment.BottomRight:
                characterPosition = new Vector3(0.5f * paddedCameraWidth - xFromEdge, -0.5f * paddedCameraHeight + yFromEdge, 5);
                _dialogTextMesh.anchor = TextAnchor.MiddleRight;
                _dialogTextMesh.transform.localPosition = characterPosition - new Vector3(spriteWidth, 0, 0);
                break;
            case CharacterAlignment.TopRight:
                characterPosition = new Vector3(0.5f * paddedCameraWidth - xFromEdge, 0.5f * paddedCameraHeight - yFromEdge, 5);
                _dialogTextMesh.anchor = TextAnchor.MiddleRight;
                _dialogTextMesh.transform.localPosition = characterPosition - new Vector3(spriteWidth, 0, 0);
                break;
            case CharacterAlignment.TopLeft:
            default:
                characterPosition = new Vector3(-0.5f * paddedCameraWidth + xFromEdge, 0.5f * paddedCameraHeight - yFromEdge, 5);
                _dialogTextMesh.anchor = TextAnchor.MiddleLeft;
                _dialogTextMesh.transform.localPosition = characterPosition + new Vector3(spriteWidth, 0, 0);
                break;
        }

        // Show sprite
        _characterRenderer.sprite = character;
        _characterRenderer.transform.localPosition = characterPosition;

        // Show background
        _backgroundRenderer.sprite = background;
        _backgroundRenderer.transform.localPosition = new Vector3(0, characterPosition.y, characterPosition.z);

        _backgroundRenderer.transform.localScale = new Vector3(1, 1, 1);
        var bWidth = _backgroundRenderer.bounds.size.x;
        var bWidthScale = cameraWidth / bWidth;
        var bHeight = _backgroundRenderer.bounds.size.y;
        var bHeightScale = dialogHeight / bHeight * 1.1f;
        _backgroundRenderer.transform.localScale = new Vector3(bWidthScale, bHeightScale, 1);


        // Show dialog text using a text mesh
        _dialogTextMesh.text = dialogText;

        var cameraScreenLeft = _camera.WorldToScreenPoint(new Vector3(-paddedCameraWidth * 0.5f, 0, 0));
        var cameraScreenRight = _camera.WorldToScreenPoint(new Vector3(paddedCameraWidth * 0.5f, 0, 0));
        var cameraScreenWidth = cameraScreenRight.x - cameraBottomLeft.x;
        var pixelsPerUnit = cameraScreenWidth / cameraWidth;
        var textWidthPixels = pixelsPerUnit * (paddedCameraWidth - spriteWidth);

        var style = new GUIStyle() { font = _dialogTextMesh.font, fontSize = maxFontSize };
        var content = new GUIContent() { text = dialogText };

        var width = style.CalcSize(content).x;

        // Reduce the font size until it is small enough
        var attempts = 0;
        while (width > textWidthPixels)
        {
            style.fontSize = (int)(style.fontSize * 0.8f);
            width = style.CalcSize(content).x;

            attempts++;

            // Break to avoid infinite loop an killing unity
            if (attempts > 100)
            {
                break;
            }
        }

        // Now use the font size in the text mesh
        _dialogTextMesh.fontSize = (int)(style.fontSize);
        _dialogTextMesh.characterSize = cameraSize * 0.05f;

        Debug.Log("End Redraw");
    }

}

public enum CharacterAlignment
{
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}