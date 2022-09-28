using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private int life = 3;

    public void damage() {
        life--;

        if(life <= 0) {
            Destroy(gameObject);
        }
    }

}
