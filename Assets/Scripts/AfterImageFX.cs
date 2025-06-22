using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageFX : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float looseRate;

    public void SetupAfterImage(float _looseRate,Sprite _spriteImage)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        looseRate = _looseRate;
        spriteRenderer.sprite = _spriteImage;
    }

    private void Update()
    {
        float alpha = spriteRenderer.color.a - looseRate * Time.deltaTime ;

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b,alpha);

        if(spriteRenderer.color.a <=0)
        {
            Destroy(gameObject);
        }
    }
}
