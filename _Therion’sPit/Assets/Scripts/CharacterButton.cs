using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    public RawImage rawImage;
    public Texture selectedTexture;
    public int roleIndex;

    private void Awake()
    {
        if (rawImage == null) rawImage = GetComponent<RawImage>();
    }
}