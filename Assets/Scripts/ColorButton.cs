using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class ColorButton : MonoBehaviour
{
    private ARPainter painter;
    private Button btn;
    private Image img;

    void Start()
    {
        // On cherche le peintre automatiquement
        painter = FindAnyObjectByType<ARPainter>();
        btn = GetComponent<Button>();
        img = GetComponent<Image>();

        // On dit au bouton : "Quand on clique sur toi, envoie ta couleur au peintre"
        btn.onClick.AddListener(() => {
            if(painter != null)
            {
                painter.SetBrushColor(img.color);
            }
        });
    }
}