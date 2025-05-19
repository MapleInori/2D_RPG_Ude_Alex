using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;

    [SerializeField] private float parallaxEffect;

    private float xPosition;
    private float length;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera");

        // 获取背景的长度
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        // 起始位置
        xPosition = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        // 相机超过背景的距离，算是相机移动的距离
        float distanceMoved = cam.transform.position.x * (1 - parallaxEffect);
        // 如果乘1，相机移动多少，背景就移动多少，这是相对于相机移动的距离
        float distanceToMove = cam.transform.position.x * parallaxEffect;

        // 移动背景图
        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);

        // 三张背景图，回到中心背景图
        // 如果移动距离超出一张背景图的长度，修改起始位置
        if(distanceMoved > xPosition + length)
        {
            xPosition += length;
        }
        else if ( distanceMoved < xPosition - length)
        {
            xPosition -= length;
        }
    }
}
