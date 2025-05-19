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

        // ��ȡ�����ĳ���
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        // ��ʼλ��
        xPosition = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        // ������������ľ��룬��������ƶ��ľ���
        float distanceMoved = cam.transform.position.x * (1 - parallaxEffect);
        // �����1������ƶ����٣��������ƶ����٣��������������ƶ��ľ���
        float distanceToMove = cam.transform.position.x * parallaxEffect;

        // �ƶ�����ͼ
        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);

        // ���ű���ͼ���ص����ı���ͼ
        // ����ƶ����볬��һ�ű���ͼ�ĳ��ȣ��޸���ʼλ��
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
