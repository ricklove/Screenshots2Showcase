﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class OverlayController : MonoBehaviour
{

    public Sprite character;
    public Sprite background;
    public CharacterAlignment characterAlignment;
    //public float cameraSize = 3;
    public float dialogHeightRatio = 0.25f;
    public float paddingRatio = 0.05f;
    public int maxFontSize = 128;
    //public Font font = null;
    public float fontAdjustment = 1.0f;
    public string dialogText = "";
    private Camera _camera;
    private SpriteRenderer _characterRenderer;
    private SpriteRenderer _backgroundRenderer;
    private TextMesh _dialogTextMesh;

    public int ChangeHash
    {
        get
        {
            return
                    17 * Screen.width.GetHashCode()
                    + 19 * Screen.height.GetHashCode()
                    + 23 * character.GetHashCode()
                    + 27 * background.GetHashCode()
                    + 37 * characterAlignment.GetHashCode()
                //+ 43 * cameraSize.GetHashCode()
                    + 43 * dialogHeightRatio.GetHashCode()
                    + 43 * paddingRatio.GetHashCode()
                    + 43 * maxFontSize.GetHashCode()
                //+ 43 * (font == null ? 1 : font.GetHashCode())
                    + 43 * fontAdjustment.GetHashCode()
                    + 43 * dialogText.GetHashCode();
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

        //_camera.orthographicSize = cameraSize;

        var cameraBottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
        var cameraTopRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.nearClipPlane));
        var cameraViewportWorldWidth = cameraTopRight.x - cameraBottomLeft.x;
        var cameraViewportWorldHeight = cameraTopRight.y - cameraBottomLeft.y;

        var dialogHeight = cameraViewportWorldHeight * dialogHeightRatio;

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


        DrawTextWithTextMesh(cameraViewportWorldWidth, paddedCameraWidth, spriteWidth, characterPosition, characterAlignment);


        Debug.Log("End Redraw");
    }

    private void DrawTextWithTextMesh(float cameraViewportWorldWidth, float paddedCameraViewportWorldWidth, float spriteWidth, Vector3 characterPosition, CharacterAlignment characterAlignment)
    {
        // Show dialog text using a text mesh
        var spriteRadius = spriteWidth / 2.0f;

        switch (characterAlignment)
        {
            case CharacterAlignment.BottomRight:
            case CharacterAlignment.TopRight:
                _dialogTextMesh.anchor = TextAnchor.MiddleRight;
                _dialogTextMesh.transform.localPosition = characterPosition - new Vector3(spriteRadius, 0, 0);
                break;

            case CharacterAlignment.BottomLeft:
            case CharacterAlignment.TopLeft:
            default:
                _dialogTextMesh.anchor = TextAnchor.MiddleLeft;
                _dialogTextMesh.transform.localPosition = characterPosition + new Vector3(spriteRadius, 0, 0);
                break;
        }


        _dialogTextMesh.text = dialogText;

        //if (font != null)
        //{
        //    _dialogTextMesh.font = font;
        //}

        var cameraScreenLeft = _camera.WorldToScreenPoint(_camera.transform.position + new Vector3(-paddedCameraViewportWorldWidth * 0.5f, 0, 0));
        var cameraScreenRight = _camera.WorldToScreenPoint(_camera.transform.position + new Vector3(paddedCameraViewportWorldWidth * 0.5f, 0, 0));
        var cameraScreenWidth = cameraScreenRight.x - cameraScreenLeft.x;
        var pixelsPerUnit = cameraScreenWidth / cameraViewportWorldWidth;
        var textWidthPixels = pixelsPerUnit * (paddedCameraViewportWorldWidth - spriteWidth);

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
        _dialogTextMesh.fontSize = (int)(style.fontSize * fontAdjustment);
        _dialogTextMesh.characterSize = _camera.orthographicSize * 0.05f;

        // Try to fix font corruption bug
        //_dialogTextMesh.font.RequestCharactersInTexture(_dialogTextMesh.text, _dialogTextMesh.fontSize, _dialogTextMesh.fontStyle);
    }

    void DrawText()
    {

    }

}

public enum CharacterAlignment
{
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}