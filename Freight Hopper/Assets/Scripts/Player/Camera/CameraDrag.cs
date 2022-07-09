using System.Collections;
using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private Transform cameraHead;
    [SerializeField] private float collidLerpSmoothness = 0.01f;
    [SerializeField] private float collidConstant = 0.02f;

    private void Awake()
    {
        cameraHead = this.transform.GetChild(0);
    }

    public void CollidDrag(Vector3 forceDirection)
    {
        forceDirection.Normalize();
        StartCoroutine(Collid(forceDirection));
    }

    private IEnumerator Collid(Vector3 jumpDirection)
    {
        yield return StartCoroutine(Colliding(jumpDirection));
        yield return StartCoroutine(Colliding(-jumpDirection));
        cameraHead.localPosition = Vector3.zero;
    }

    private IEnumerator Colliding(Vector3 jumpDirection)
    {
        float collidLerpValue = 0;

        while (collidLerpValue <= 1)
        {
            collidLerpValue += collidLerpSmoothness;

            cameraHead.Translate(collidConstant * jumpDirection, Space.Self);

            yield return null;
        }
    }
}