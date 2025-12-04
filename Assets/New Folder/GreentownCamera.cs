using UnityEngine;

public class CameraFollowClamp2D : MonoBehaviour
{
    [Header("따라갈 대상(플레이어)")]
    public Transform target;

    [Header("맵 스프라이트 렌더러")]
    public SpriteRenderer mapRenderer;

    [Header("속도")]
    public float smooth = 5f;

    Camera cam;
    Bounds mapBounds;
    float camHalfHeight;
    float camHalfWidth;

    void Start()
    {
        cam = GetComponent<Camera>();
        mapBounds = mapRenderer.bounds;

        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (target == null || mapRenderer == null) return;


        Vector3 desired = new Vector3(target.position.x, target.position.y, transform.position.z);

        // 2) 카메라가 맵 밖으로 나가지 않게
        float minX = mapBounds.min.x + camHalfWidth;
        float maxX = mapBounds.max.x - camHalfWidth;
        float minY = mapBounds.min.y + camHalfHeight;
        float maxY = mapBounds.max.y - camHalfHeight;

        if (minX > maxX) (minX, maxX) = (maxX, minX);
        if (minY > maxY) (minY, maxY) = (maxY, minY);

        desired.x = Mathf.Clamp(desired.x, minX, maxX);
        desired.y = Mathf.Clamp(desired.y, minY, maxY);

        transform.position = Vector3.Lerp(transform.position, desired, smooth * Time.deltaTime);
    }
}
