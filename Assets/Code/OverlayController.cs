using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class OverlayController : MonoBehaviour
{

    public Sprite character;
    public CharacterAlignment characterAlignment;
    public float paddingRatio = 0.05f;

    private Camera _camera;
    private SpriteRenderer _characterRenderer;

    private Vector3 _characterPosition;

    void Start()
    {
        _camera = gameObject.GetComponent<Camera>();
        _characterRenderer = transform.FindChild("Character").gameObject.GetComponent<SpriteRenderer>();
    }

    private CharacterAlignment _lastCharacterAlignment;
    void Update()
    {
        Debug.Log("Update");

        // Display the character in the correct position if it has changed
        //if (characterAlignment != _lastCharacterAlignment)
        //{
        //    _lastCharacterAlignment = characterAlignment;

        var cameraBottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
        var cameraTopRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.nearClipPlane));
        var cameraWidth = cameraTopRight.x - cameraBottomLeft.x;
        var cameraHeight = cameraTopRight.y - cameraBottomLeft.y;

        // Reduce camera width and height to create padding
        var paddedCameraWidth = cameraWidth * (1.0f - 2 * paddingRatio);
        var paddedCameraHeight = cameraHeight * (1.0f - 2 * paddingRatio);

        // Assume using center alignment on the sprites
        var spriteCenter = character.bounds.center;
        var spriteLeft = character.bounds.min.x;
        var spriteTop = character.bounds.max.y;

        var xFromEdge = spriteCenter.x - spriteLeft;
        var yFromEdge = spriteTop - spriteCenter.y;

        switch (characterAlignment)
        {
            case CharacterAlignment.BottomLeft:
                _characterPosition = new Vector3(-0.5f * paddedCameraWidth + xFromEdge, -0.5f * paddedCameraHeight + yFromEdge, 5);
                break;
            case CharacterAlignment.BottomRight:
                _characterPosition = new Vector3(0.5f * paddedCameraWidth - xFromEdge, -0.5f * paddedCameraHeight + yFromEdge, 5);
                break;
            case CharacterAlignment.TopRight:
                _characterPosition = new Vector3(0.5f * paddedCameraWidth - xFromEdge, 0.5f * paddedCameraHeight - yFromEdge, 5);
                break;
            case CharacterAlignment.TopLeft:
            default:
                _characterPosition = new Vector3(-0.5f * paddedCameraWidth + xFromEdge, 0.5f * paddedCameraHeight - yFromEdge, 5);
                break;
        }

        // Show sprite
        if (_characterRenderer.sprite != character)
        {
            _characterRenderer.sprite = character;
        }

        _characterRenderer.transform.localPosition = _characterPosition;
        //}
    }

}

public enum CharacterAlignment
{
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}