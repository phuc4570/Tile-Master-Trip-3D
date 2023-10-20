using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScaleController : MonoBehaviour
{
    [SerializeField] float heightNormal = 1920;
    [SerializeField] float widthNormal = 1080;

    private void Start()
    {
        float percentHeight = heightNormal / getScreenHeight();
        float percentWidth = widthNormal / getScreenWidth();
        transform.localScale = new Vector3(transform.localScale.x / percentWidth, transform.localScale.y, transform.localScale.z / percentHeight);
    }

    public float getScreenHeight()
    {
        return Screen.currentResolution.height;

    }
    public float getScreenWidth()
    {
        return Screen.currentResolution.width;
    }
}
