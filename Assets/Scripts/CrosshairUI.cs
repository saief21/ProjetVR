using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    [SerializeField] private float size = 20f;  // Augmenté à 20
    [SerializeField] private Color crosshairColor = Color.green;  // Changé en vert pour plus de visibilité
    
    private void Start()
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            // Créer un sprite en croix
            Texture2D texture = new Texture2D(3, 3);
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    // Créer une croix (+)
                    if (x == 1 || y == 1)
                        texture.SetPixel(x, y, Color.white);
                    else
                        texture.SetPixel(x, y, Color.clear);
                }
            }
            texture.Apply();
            
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 3, 3), new Vector2(0.5f, 0.5f));
            
            image.sprite = sprite;
            image.color = crosshairColor;
            
            RectTransform rect = GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(size, size);
        }
    }
}
