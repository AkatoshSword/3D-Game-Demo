using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackMaskView : MonoBehaviour
{
    public static BlackMaskView Instance;

    public Image image;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        image = transform.GetComponent<Image>();
    }
    //透明度从1到0
    public IEnumerator FadeIn()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        yield return null;
        while (image.color.a > 0) 
        {
            yield return null;
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - Time.deltaTime);
        }
    }

    //透明度从0到1
    public IEnumerator FadeOut()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        yield return null;
        while (image.color.a < 1)
        {
            yield return null;
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
