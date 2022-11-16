using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private int life = 3;
    
    Color color1, color2, color3;
    List<Color> colors = new List<Color>() { };

    public SpriteRenderer blockColor;
    //int colorIndex = 3;

    void Start() {
        ColorUtility.TryParseHtmlString("#165e16", out color1);
        ColorUtility.TryParseHtmlString("#177519", out color2);
        ColorUtility.TryParseHtmlString("#00ac0b", out color3);

        colors.Add(color1); colors.Add(color1); colors.Add(color2); colors.Add(color3);
 
        blockColor = GetComponent<SpriteRenderer>();

        blockColor.color = colors[life];
    }

    private void Update() {
        
    }

    public void damage() {      
        life--;
        life = Mathf.Max(0, life); // O valor m�ximo � life ou zero, max retorna o maior valor
        blockColor.color = colors[life];

        if (life <= 0) {
            Destroy(gameObject);
        }
    }
}
