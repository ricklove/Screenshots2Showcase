using UnityEngine;
using System.Collections;
using System.Linq;

[ExecuteInEditMode]
public class OverlayController : MonoBehaviour
{

    public Sprite character;
    public Sprite background;
    public CharacterAlignment characterAlignment;
    //public float cameraSize = 3;
    public float dialogHeightRatio = 0.25f;
    public float paddingRatio = 0.05f;

    public float maxCharacterWidthRatio = 0.25f;
    public float maxCharacterHeightRatio = 0.25f;

    //public int maxFontSize = 128;
    //public Font font = null;
    //public float fontAdjustment = 1.0f;
    public string dialogText = "";

    public GUIStyle fontStyle = GUI.skin.box;


    private Camera _camera;
    private SpriteRenderer _characterRenderer;
    private SpriteRenderer _backgroundRenderer;
    private TextMesh _dialogTextMesh;

    public int ChangeHash
    {
        get
        {
            // Guarantee valid values
            if (dialogHeightRatio < 0.1f) { dialogHeightRatio = 0.1f; }
            if (dialogHeightRatio > 1f) { dialogHeightRatio = 1f; }
            if (paddingRatio < 0f) { paddingRatio = 0f; }
            if (paddingRatio > dialogHeightRatio * 0.25f) { paddingRatio = dialogHeightRatio * 0.25f; }

            if (maxCharacterWidthRatio < 0.1f) { maxCharacterWidthRatio = 0.1f; }
            if (maxCharacterWidthRatio > 0.8f) { maxCharacterWidthRatio = 0.8f; }

            if (maxCharacterHeightRatio < 0.1f) { maxCharacterHeightRatio = 0.1f; }
            if (maxCharacterHeightRatio > 0.8f) { maxCharacterHeightRatio = 0.8f; }

            return
                    17 * Screen.width.GetHashCode()
                    + 19 * Screen.height.GetHashCode()
                    + 23 * character.GetHashCode()
                    + 27 * background.GetHashCode()
                    + 37 * characterAlignment.GetHashCode()
                    + 43 * dialogHeightRatio.GetHashCode()
                    + 47 * paddingRatio.GetHashCode()
                    + 61 * maxCharacterWidthRatio.GetHashCode()
                    + 67 * maxCharacterHeightRatio.GetHashCode()
                    + 53 * dialogText.GetHashCode();
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
        UpdateLayout();
    }

    private void UpdateLayout()
    {
        if (_lastChangeHash != ChangeHash)
        {
            _lastChangeHash = ChangeHash;
            Redraw();
        }
    }


    private Texture2D _gSpriteTexture;
    private Rect _gSpriteRect;
    private bool _gSpriteShouldFlip;

    void Redraw()
    {
        // Calculate all rects in screen ratios

        // Calculate character size
        var spriteTextureWidth = character.texture.width;
        var spriteTextureHeight = character.texture.height;

        var spriteWidthHeightRatio = 1.0f * spriteTextureWidth / spriteTextureHeight;
        var maxWidth = Screen.width * maxCharacterWidthRatio;
        var maxHeight = Screen.height * maxCharacterHeightRatio;

        // Try width
        var spriteActualWidth = maxWidth;
        var spriteActualHeight = spriteActualWidth / spriteWidthHeightRatio;

        // Use height if that is too big
        if (spriteActualHeight > maxHeight)
        {
            spriteActualHeight = maxHeight;
            spriteActualWidth = spriteActualHeight * spriteWidthHeightRatio;
        }

        // Convert back to ratio 
        var spriteWidth = spriteActualWidth / Screen.width;
        var spriteHeight = spriteActualHeight / Screen.height;

        // Set sprite left and top
        var paddingWidth = paddingRatio;
        var paddingHeight = paddingRatio;

        float spriteLeft;
        float spriteTop;
        bool flipSprite;

        switch (characterAlignment)
        {
            case CharacterAlignment.BottomLeft:
                spriteLeft = paddingWidth;
                spriteTop = 1.0f - (paddingHeight + spriteHeight);
                flipSprite = false;
                break;
            case CharacterAlignment.BottomRight:
                spriteLeft = 1.0f - (paddingWidth + spriteWidth);
                spriteTop = 1.0f - (paddingHeight + spriteHeight);
                flipSprite = true;
                break;
            case CharacterAlignment.TopRight:
                spriteLeft = 1.0f - (paddingWidth + spriteWidth);
                spriteTop = paddingHeight;
                flipSprite = true;
                break;
            case CharacterAlignment.TopLeft:
            default:
                spriteLeft = paddingWidth;
                spriteTop = paddingHeight;
                flipSprite = false;
                break;
        }

        _gSpriteTexture = character.texture;
        _gSpriteShouldFlip = flipSprite;
        _gSpriteRect = new Rect(Screen.width * spriteLeft, Screen.height * spriteTop, Screen.width * spriteWidth, Screen.height * spriteHeight);



        //- Set layout fields in UpdateLayout
        //    - text
        //    - textRect
        //    - textFontSize
        //    - characterImage
        //    - characterRect
        //    - backgroundImage
        //    - backgroundRect
        //- DrawLayout at appropriate time
        //    - DrawLayoutWithOnGUI in OnGUI
        //        // GUI.depth = guiDepth;
        //        // GUI.DrawTexture(Rect(10,10,60,60), aTexture, ScaleMode.Stretch);
    }

    void RedrawOLD()
    {
        Debug.Log("Redraw");

        //_camera.orthographicSize = cameraSize;

        var cameraBottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
        var cameraTopRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.nearClipPlane));
        var cameraViewportWorldWidth = cameraTopRight.x - cameraBottomLeft.x;
        var cameraViewportWorldHeight = cameraTopRight.y - cameraBottomLeft.y;

        var dialogHeight = cameraViewportWorldHeight * Mathf.Max(0.05f + paddingRatio * 2, dialogHeightRatio);

        // Reduce camera width and height to create padding
        var paddingWidth = cameraViewportWorldWidth * paddingRatio;
        var paddedCameraWidth = cameraViewportWorldWidth * (1.0f - 2 * paddingRatio);
        var paddedCameraHeight = cameraViewportWorldHeight * (1.0f - 2 * paddingRatio);

        // Resize sprite to fit appropriate size
        var spriteAvailableSize = dialogHeight - paddingWidth * 2;


        // Assume using center alignment on the sprites
        var spriteCenter = character.bounds.center;
        var spriteLeft = character.bounds.min.x;
        var spriteTop = character.bounds.max.y;

        var xFromEdge = spriteCenter.x - spriteLeft;
        var yFromEdge = spriteTop - spriteCenter.y;

        var spriteActualWidth = xFromEdge * 2.0f + paddingWidth;
        var spriteScale = spriteAvailableSize / spriteActualWidth;
        var spriteWidth = spriteAvailableSize;
        var spriteRadius = spriteWidth / 2.0f;

        var characterPosition = new Vector3();
        var flipSprite = false;

        switch (characterAlignment)
        {
            case CharacterAlignment.BottomLeft:
                characterPosition = new Vector3(-0.5f * paddedCameraWidth + spriteRadius, -0.5f * paddedCameraHeight + spriteRadius, 5);
                flipSprite = false;
                break;
            case CharacterAlignment.BottomRight:
                characterPosition = new Vector3(0.5f * paddedCameraWidth - spriteRadius, -0.5f * paddedCameraHeight + spriteRadius, 5);
                flipSprite = true;
                break;
            case CharacterAlignment.TopRight:
                characterPosition = new Vector3(0.5f * paddedCameraWidth - spriteRadius, 0.5f * paddedCameraHeight - spriteRadius, 5);
                flipSprite = true;
                break;
            case CharacterAlignment.TopLeft:
            default:
                characterPosition = new Vector3(-0.5f * paddedCameraWidth + spriteRadius, 0.5f * paddedCameraHeight - spriteRadius, 5);
                flipSprite = false;
                break;
        }

        // Show sprite
        _characterRenderer.sprite = character;
        _characterRenderer.transform.localPosition = characterPosition;
        _characterRenderer.transform.localScale = new Vector3(flipSprite ? -spriteScale : spriteScale, spriteScale, spriteScale);

        // Show background
        _backgroundRenderer.sprite = background;
        _backgroundRenderer.transform.localPosition = new Vector3(0, characterPosition.y, characterPosition.z);

        _backgroundRenderer.transform.localScale = new Vector3(1, 1, 1);
        var bWidth = _backgroundRenderer.bounds.size.x;
        var bWidthScale = cameraViewportWorldWidth / bWidth;
        var bHeight = _backgroundRenderer.bounds.size.y;
        var bHeightScale = dialogHeight / bHeight * 1.1f;
        _backgroundRenderer.transform.localScale = new Vector3(bWidthScale, bHeightScale, 1);


        //DrawTextWithTextMesh(cameraViewportWorldWidth, paddedCameraWidth, spriteWidth, characterPosition, characterAlignment);
        DrawText(cameraViewportWorldWidth, cameraViewportWorldHeight, paddedCameraWidth, dialogHeight, spriteWidth, characterPosition, characterAlignment);


        Debug.Log("End Redraw");
    }

    //private void DrawTextWithTextMesh(float cameraViewportWorldWidth, float paddedCameraViewportWorldWidth, float dialogHeight, float characterWidth, Vector3 characterPosition, CharacterAlignment characterAlignment)
    //{
    //    // Show dialog text using a text mesh
    //    var spriteRadius = characterWidth / 2.0f;

    //    switch (characterAlignment)
    //    {
    //        case CharacterAlignment.BottomRight:
    //        case CharacterAlignment.TopRight:
    //            _dialogTextMesh.anchor = TextAnchor.MiddleRight;
    //            _dialogTextMesh.transform.localPosition = characterPosition - new Vector3(spriteRadius, 0, 0);
    //            break;

    //        case CharacterAlignment.BottomLeft:
    //        case CharacterAlignment.TopLeft:
    //        default:
    //            _dialogTextMesh.anchor = TextAnchor.MiddleLeft;
    //            _dialogTextMesh.transform.localPosition = characterPosition + new Vector3(spriteRadius, 0, 0);
    //            break;
    //    }


    //    _dialogTextMesh.text = dialogText;

    //    //if (font != null)
    //    //{
    //    //    _dialogTextMesh.font = font;
    //    //}

    //    var cameraScreenLeft = _camera.WorldToScreenPoint(_camera.transform.position + new Vector3(-paddedCameraViewportWorldWidth * 0.5f, 0, 0));
    //    var cameraScreenRight = _camera.WorldToScreenPoint(_camera.transform.position + new Vector3(paddedCameraViewportWorldWidth * 0.5f, 0, 0));
    //    var cameraScreenWidth = cameraScreenRight.x - cameraScreenLeft.x;
    //    var pixelsPerUnit = cameraScreenWidth / cameraViewportWorldWidth;
    //    var textWidthPixels = pixelsPerUnit * (paddedCameraViewportWorldWidth - characterWidth);

    //    var style = new GUIStyle() { font = _dialogTextMesh.font, fontSize = maxFontSize };
    //    var content = new GUIContent() { text = dialogText };

    //    var width = style.CalcSize(content).x;

    //    // Reduce the font size until it is small enough
    //    var attempts = 0;
    //    while (width > textWidthPixels)
    //    {
    //        style.fontSize = (int)(style.fontSize * 0.8f);
    //        width = style.CalcSize(content).x;

    //        attempts++;

    //        // Break to avoid infinite loop an killing unity
    //        if (attempts > 100)
    //        {
    //            break;
    //        }
    //    }

    //    // Now use the font size in the text mesh
    //    _dialogTextMesh.fontSize = (int)(style.fontSize * fontAdjustment);
    //    _dialogTextMesh.characterSize = _camera.orthographicSize * 0.05f;

    //    // Try to fix font corruption bug
    //    //_dialogTextMesh.font.RequestCharactersInTexture(_dialogTextMesh.text, _dialogTextMesh.fontSize, _dialogTextMesh.fontStyle);
    //}

    void DrawText(float cameraViewportWorldWidth, float cameraViewportWorldHeight, float paddedCameraViewportWorldWidth, float dialogWorldHeight, float characterWidth, Vector3 characterPosition, CharacterAlignment characterAlignment)
    {
        // TEMP: Disable Text Mesh
        _dialogTextMesh.gameObject.SetActive(false);

        if (string.IsNullOrEmpty(dialogText.Trim()))
        {
            _onGUIText = "";
        }

        // Calculate screen rect for text area 
        // (using screen ratio 0 = left, 1.0 = right, 0 = top, 1.0 = bottom)
        var textWorldWidth = paddedCameraViewportWorldWidth - characterWidth;

        var widthRatioPerUnit = 1.0f / cameraViewportWorldWidth;
        var textRatioWidth = textWorldWidth * widthRatioPerUnit;

        var characterRatioWidth = characterWidth * widthRatioPerUnit;


        var heightRatioPerUnit = 1.0f / cameraViewportWorldHeight;
        var dialogRatioHeight = dialogWorldHeight * heightRatioPerUnit;
        var textRatioHeight = dialogRatioHeight - paddingRatio * 2;


        float textRatioLeft;
        float textRatioTop;

        switch (characterAlignment)
        {
            case CharacterAlignment.BottomLeft:
                textRatioLeft = paddingRatio + characterRatioWidth;
                textRatioTop = 1.0f - paddingRatio - textRatioHeight;
                break;
            case CharacterAlignment.BottomRight:
                textRatioLeft = paddingRatio;
                textRatioTop = 1.0f - paddingRatio - textRatioHeight;
                break;
            case CharacterAlignment.TopLeft:
                textRatioLeft = paddingRatio + characterRatioWidth;
                textRatioTop = paddingRatio;
                break;
            case CharacterAlignment.TopRight:
            default:
                textRatioLeft = paddingRatio;
                textRatioTop = paddingRatio;
                break;
        }

        var w = Screen.width;
        var h = Screen.height;
        var textRect = new Rect(textRatioLeft * w, textRatioTop * h, textRatioWidth * w, textRatioHeight * h);

        // Measure font size
        var fontSize = FontSizeHelper.CalculateFontSizeToFill(dialogText, textRect.width, textRect.height, fontStyle);

        // Set local text fields
        _onGUIRect = textRect;
        _onGUIText = dialogText;
        _onGUIFontSize = fontSize;
    }



    private string _onGUIText;
    private Rect _onGUIRect;
    private int _onGUIFontSize;

    void OnGUI()
    {
        UpdateLayout();

        if (!string.IsNullOrEmpty(_onGUIText))
        {
            var style = fontStyle;
            style.fontSize = _onGUIFontSize;

            GUI.Label(_onGUIRect, _onGUIText, style);
        }


        if (_gSpriteTexture != null)
        {
            // TODO: Flip texture 
            GUI.DrawTexture(_gSpriteRect, _gSpriteTexture);
        }
    }

}

public enum CharacterAlignment
{
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}



public static class FontSizeHelper
{
    public static int CalculateFontSizeToFill(string text, float width, float height, GUIStyle style)
    {
        var oSize = style.fontSize;

        style.fontSize = (int)(height * 1.0f);

        // Reduce font size if needed
        var longestWord = text.Split(' ').Where(w => w.Trim().Length > 0).OrderByDescending(w => w.Trim().Length).Select(w => w.Trim()).First();
        longestWord = "w" + longestWord + "w";
        var wContent = new GUIContent(longestWord);
        var content = new GUIContent(text);

        var mSize = style.CalcSize(wContent);
        var mHeight = style.CalcHeight(content, width);

        while ((mSize.x > width * 0.85f)
            || (mHeight > height * 0.85f))
        {
            var diffRatio = mHeight / (height * 0.9f);
            var reduce = 1 / diffRatio;
            var halfReduce = (1 + reduce) / 2;
            var ratio = Mathf.Min(halfReduce, 0.8f);

            style.fontSize = (int)(style.fontSize * ratio);

            mSize = style.CalcSize(wContent);
            mHeight = style.CalcHeight(content, width);
        }

        var fSize = style.fontSize;

        style.fontSize = oSize;
        return fSize;
    }

}