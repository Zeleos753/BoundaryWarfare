using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectEffect : MonoBehaviour
{
    public float speedX = 0.1f;
    public float speedY = 0.1f;
    private float curX;
    private float curY;
    public Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        curX = rend.material.mainTextureOffset.x;
        curY = rend.material.mainTextureOffset.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        curX += Time.deltaTime * speedX;
        curY += Time.deltaTime * speedY;
        rend.material.SetTextureOffset("_MainTex", new Vector2(curX, curY));
    }
}
