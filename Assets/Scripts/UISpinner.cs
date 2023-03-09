using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class UISpinner : MonoBehaviour
{
    [SerializeField] RectTransform rotatingImage;
    [SerializeField] float speed = 5f;
    private void OnEnable()
    {
        rotatingImage.rotation = Quaternion.identity;
    }
    private void OnDisable()
    {
        rotatingImage.rotation = Quaternion.identity;
    }
    private void Update()
    {
        rotatingImage.Rotate(Vector3.forward * speed * Time.unscaledDeltaTime);
    }
}
