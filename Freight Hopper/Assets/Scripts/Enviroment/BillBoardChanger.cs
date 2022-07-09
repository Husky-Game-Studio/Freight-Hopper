using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BillBoardChanger : MonoBehaviour
{
    private const int billboardCount = 8;
    private const int height = 2048;
    private const int billboardHeight = 132;

    private enum Direction {
        Up,
        Down,
        Left,
        Right,
        RevUp,
        RevDown,
        RevLeft,
        RevRight,
    }
    private Mesh mesh;
    [SerializeField] private Direction rotation;
    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        SwapBillboard();
    }
    
    public void SwapBillboard()
    {
        int billboardSelection = Random.Range(0, billboardCount);
        
        int pixelBottom = height - (billboardCount * billboardHeight);
        int billboardPixelMult = billboardSelection * billboardHeight;
        int pixels = billboardPixelMult + pixelBottom;
        
        float ybillboardPosBot = (float)pixels/ (float)height;
        float yBillboardPosTop = ((float)pixels+ billboardHeight) / (float)height;
        Vector2 botL = new Vector2(0, ybillboardPosBot);
        Vector2 botR = new Vector2(1, ybillboardPosBot);
        Vector2 topL = new Vector2(0, yBillboardPosTop);
        Vector2 topR = new Vector2(1, yBillboardPosTop);
        Vector2[] result;
        switch(rotation)
        {
            case Direction.Up:
                result = new Vector2[] { botR, botL, topL, topR }; //up
                break;
            case Direction.Down:
                result = new Vector2[] { topL, topR, botR, botL }; //down
                break;
            case Direction.Left:
                result = new Vector2[] { botL, topL, topR, botR,    }; //left
                break;
            case Direction.Right:
                result = new Vector2[] { topR, botR, botL, topL,    }; //right
                break;
            case Direction.RevUp:
                result = new Vector2[] {  topR, topL, botL, botR,  }; //Revup
                break;
            case Direction.RevDown:
                result = new Vector2[] {botL, botR, topR, topL   }; //Revdown
                break;
            case Direction.RevLeft:
                result = new Vector2[] { botR, topR, topL, botL,    }; //Revleft
                break;
            default:
                result = new Vector2[] { topL, botL, botR, topR,    }; //Revright
                break;
        }
        //Debug.Log(vectors[0] + " " + vectors[1] + " " + vectors[2] + " " + vectors[3], this.gameObject);
        //Debug.Log("vector count " +  mesh.vertices.Length, this.gameObject);
/*        if(mesh.vertices.Length == 0){
            return;
        }*/
        mesh.SetUVs(0, result);
    }
}