using UnityEngine;
using System.Collections;
using System.Linq;

[ExecuteInEditMode]
public class OverlayController : MonoBehaviour
{

    public Sprite character;
    public Sprite background;
    public CharacterAlignment characterAlignment;
    public float dialogHeightRatio = 0.25f;
    public float paddingRatio = 0.05f;

    public float maxCharacterWidthRatio = 0.25f;
    public float maxCharacterHeightRatio = 0.25f;

    public string dialogText = "";

    public GUIStyle fontStyle = GUI.skin.box;

    public int ChangeHash
    {
        get
        {
            // Guarantee valid values
            if (dialogHeightRatio < 0.1f) { dialogHeightRatio = 0.1f; }
            if (dialogHeightRatio > 1f) { dialogHeightRatio = 1f; }

            if (paddingRatio < 0f) { paddingRatio = 0f; }
            if (paddingRatio > dialogHeightRatio * 0.25f) { paddingRatio = dialogHeightRatio * 0.25f; }

            if (maxCharacterWidthRatio < 0.1f + paddingRatio * 2) { maxCharacterWidthRatio = 0.1f + paddingRatio * 2; }
            if (maxCharacterWidthRatio > 0.8f) { maxCharacterWidthRatio = 0.8f; }

            if (maxCharacterHeightRatio < 0.1f + paddingRatio * 2) { maxCharacterHeightRatio = 0.1f + paddingRatio * 2; }
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

    private Texture2D _gBackgroundTexture;
    private Rect _gBackgroundRect;

    private string _gText;
    private Rect _gTextRect;
    private GUIStyle _gTextStyle;
    private int _gTextFontSize;

    void Redraw()
    {
        // Calculate all rects in screen ratios

        var paddingWidth = paddingRatio;
        var paddingHeight = paddingRatio;

        // Calculate character size
        var spriteTextureWidth = character.texture.width;
        var spriteTextureHeight = character.texture.height;

        var spriteWidthHeightRatio = 1.0f * spriteTextureWidth / spriteTextureHeight;
        var maxWidth = Screen.width * (maxCharacterWidthRatio - paddingWidth * 2);
        var maxHeight = Screen.height * (maxCharacterHeightRatio - paddingHeight * 2);

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
        float spriteLeft;
        float spriteTop;
        bool flipSprite;

        float backgroundHeight = dialogHeightRatio;
        float backgroundTop;

        float textHeight = backgroundHeight - paddingHeight * 2;
        float textWidth = 1.0f - spriteWidth - paddingWidth * 3;
        float textTop;
        float textLeft;


        switch (characterAlignment)
        {
            case CharacterAlignment.BottomLeft:
                spriteLeft = paddingWidth;
                spriteTop = 1.0f - (paddingHeight + spriteHeight);
                flipSprite = false;
                backgroundTop = 1.0f - backgroundHeight;
                textLeft = spriteWidth + paddingWidth * 2;
                break;
            case CharacterAlignment.BottomRight:
                spriteLeft = 1.0f - (paddingWidth + spriteWidth);
                spriteTop = 1.0f - (paddingHeight + spriteHeight);
                flipSprite = true;
                backgroundTop = 1.0f - backgroundHeight;
                textLeft = paddingWidth;
                break;
            case CharacterAlignment.TopRight:
                spriteLeft = 1.0f - (paddingWidth + spriteWidth);
                spriteTop = paddingHeight;
                flipSprite = true;
                backgroundTop = 0;
                textLeft = paddingWidth;
                break;
            case CharacterAlignment.TopLeft:
            default:
                spriteLeft = paddingWidth;
                spriteTop = paddingHeight;
                flipSprite = false;
                backgroundTop = 0;
                textLeft = spriteWidth + paddingWidth * 2;
                break;
        }

        textTop = backgroundTop + paddingHeight;

        var textFontSize = FontSizeHelper.CalculateFontSizeToFill(dialogText, Screen.width * textWidth, Screen.height * textHeight, fontStyle);

        // Set to draw at the right time
        _gSpriteTexture = character.texture;
        _gSpriteShouldFlip = flipSprite;
        _gSpriteRect = new Rect(Screen.width * spriteLeft, Screen.height * spriteTop, Screen.width * spriteWidth, Screen.height * spriteHeight);

        _gBackgroundTexture = background.texture;
        _gBackgroundRect = new Rect(0, Screen.height * backgroundTop, Screen.width, Screen.height * backgroundHeight);

        _gText = dialogText;
        _gTextRect = new Rect(Screen.width * textLeft, Screen.height * textTop, Screen.width * textWidth, Screen.height * textHeight);
        _gTextFontSize = textFontSize;
        _gTextStyle = fontStyle;
    }

    void OnGUI()
    {
        UpdateLayout();

        GUI.depth = -10000;

        if (_gBackgroundTexture != null)
        {
            GUI.DrawTexture(_gBackgroundRect, _gBackgroundTexture, ScaleMode.StretchToFill);
        }

        if (_gSpriteTexture != null)
        {
            var aspectRatio = 1.0f * _gSpriteTexture.width / _gSpriteTexture.height;

            GUI.DrawTexture(_gSpriteRect, _gSpriteTexture, ScaleMode.ScaleToFit, true, _gSpriteShouldFlip ? -aspectRatio : aspectRatio);
        }

        if (!string.IsNullOrEmpty(_gText)
            && _gTextStyle != null
            && _gTextRect.width > 0
            && _gTextRect.height > 0
            && _gTextFontSize > 0)
        {
            var style = _gTextStyle;
            style.fontSize = _gTextFontSize;

            GUI.Label(_gTextRect, _gText, style);
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

        var attempts = 0;

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


            if (attempts > 100)
            {
                // Error
                break;
            }

            attempts++;
        }

        var fSize = style.fontSize;

        style.fontSize = oSize;
        return fSize;
    }

}