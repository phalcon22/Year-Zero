using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [SerializeField]
    RectTransform img;

    [SerializeField]
    Canvas cnvs;

    [SerializeField]
    Transform ground;

    [SerializeField]
    LayerMask fakeGroundLayer;

    float camTop;
    float camBottom;
    float camLeft;
    float camRight;

    float scaleX;
    float scaleZ;


    [SerializeField] DrawUILine topLine;
    [SerializeField] DrawUILine bottomLine;
    [SerializeField] DrawUILine leftLine;
    [SerializeField] DrawUILine rightLine;

    private void Start()
    {
        scaleX = ground.localScale.x / (img.rect.width * cnvs.scaleFactor);
        scaleZ = ground.localScale.z / (img.rect.height * cnvs.scaleFactor);
        camTop = img.position.y + img.rect.height * cnvs.scaleFactor / 2;
        camBottom = img.position.y - img.rect.height * cnvs.scaleFactor / 2;
        camLeft = img.position.x - img.rect.width * cnvs.scaleFactor / 2;
        camRight = img.position.x + img.rect.width * cnvs.scaleFactor / 2;
        UpdateMinimapSquare();
    }

    public void UpdateMinimapSquare()
    {
        SetSquareSize();
    }

    public bool MouseOnMinimap()
    {
        Vector3 vec = Input.mousePosition;
        return vec[0] >= camLeft && vec[0] <= camRight &&
            vec[1] >= camBottom && vec[1] <= camTop;
    }

    public Vector3 MinimapToWorldSpaceCoords()
    {
        float x = (Input.mousePosition.x - img.rect.width * cnvs.scaleFactor / 2 - camLeft) * scaleX;
        float z = (Input.mousePosition.y - img.rect.height * cnvs.scaleFactor / 2 - camBottom) * scaleZ;

        return new Vector3(x, 0, z);
    }

    public Vector3 MouseWorldSpaceToMinimap()
    {
        float x = 0;
        float z = 0;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
        {
            Vector3 tmp = WorldSpaceToMinimap(hit.point);
            x = tmp.x;
            z = tmp.y;
        }
        else
        {
            Debug.Log("Error");
        }
        return new Vector3(x, z, 0);
    }

    public Vector3 WorldSpaceToMinimap(Vector3 pos)
    {
        float x = (pos.x / scaleX) + img.rect.width * cnvs.scaleFactor / 2 + camLeft;
        float z = (pos.z / scaleZ) + img.rect.height * cnvs.scaleFactor / 2 + camBottom;
        return new Vector3(x, z, 0);
    }

    void SetSquareSize()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0, 1, 0)), out hit, Mathf.Infinity, fakeGroundLayer))
        {
            Vector2 topLeft = new Vector2(hit.point.x, hit.point.z);

            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(1, 1, 0)), out hit, Mathf.Infinity, fakeGroundLayer))
            {
                Vector2 topRight = new Vector2(hit.point.x, hit.point.z);

                if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(1, 0, 0)), out hit, Mathf.Infinity, fakeGroundLayer))
                {
                    Vector2 bottomRight = new Vector2(hit.point.x, hit.point.z);

                    if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0, 0, 0)), out hit, Mathf.Infinity, fakeGroundLayer))
                    {
                        Vector2 bottomLeft = new Vector2(hit.point.x, hit.point.z);

                        Vector2 topLeft2 = WorldSpaceToMinimap(new Vector3(topLeft.x, 0, topLeft.y));
                        Vector2 topRight2 = WorldSpaceToMinimap(new Vector3(topRight.x, 0, topRight.y));
                        Vector2 bottomLeft2 = WorldSpaceToMinimap(new Vector3(bottomLeft.x, 0, bottomLeft.y));
                        Vector2 bottomRight2 = WorldSpaceToMinimap(new Vector3(bottomRight.x, 0, bottomRight.y));

                        float mult = cnvs.scaleFactor;
                        topLine.DrawLine(topLeft2, topRight2, mult);
                        bottomLine.DrawLine(bottomLeft2, bottomRight2, mult);
                        leftLine.DrawLine(topLeft2, bottomLeft2, mult);
                        rightLine.DrawLine(topRight2, bottomRight2, mult);
                    }
                }
            }
        }
    }
}
