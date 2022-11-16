using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    private Vector2 screenBounds;
    public Block blockPrefab;
    float spaceBetweenBlocks = 0.25f;
    float spaceBetweenRows = 0.1f;

    List<Block> instancedBlocks = new List<Block>();

    [Min(1)]public int blockCols; //propriedade da vari�vel utilizada pelo editor da Unity
    [Min(1)]public int blockRows;


    public void spawnBlocks() {
        destroyAllBlocks();

        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height - 100f));
        
        float screenWidth = screenBounds.x * 2;

        float blockHeight = blockPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
        float blockWidth = ((screenWidth - spaceBetweenBlocks * (blockCols + 1)) / blockCols);

        for (int col = 0; col < blockCols; col++) {

            for (int row = 0; row < blockRows; row++) {
                Vector2 blockPos = new Vector2(-screenBounds.x + spaceBetweenBlocks + col * (blockWidth + spaceBetweenBlocks),
                    screenBounds.y - spaceBetweenBlocks - row * (blockHeight + spaceBetweenRows));

                Block blockInstance = Instantiate(blockPrefab, blockPos, Quaternion.identity);

                instancedBlocks.Add(blockInstance);

                Vector3 blockScale = blockInstance.transform.localScale;

                blockScale.x = blockWidth;

                blockInstance.transform.localScale = blockScale; 
            }

        }

    }

    void destroyAllBlocks() {
        foreach (var item in instancedBlocks) {
            if (item) {
                Destroy(item.gameObject);
            }         
        }
        instancedBlocks.Clear();
    }
}
