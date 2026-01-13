using UnityEngine;
using System.Collections.Generic;

public class Laser : MonoBehaviour
{
    public enum LaserDirection
    {   
        Left,
    };

    [SerializeField, Header("レーザーの発射方向")]
    public LaserDirection laserDirection;

    [SerializeField, Header("レーザー発射位置")]
    private List<GameObject> launchPosObjs = new List<GameObject>();

    public LayerMask layerMask;

    private float rayLength = 0.0f;
    private Vector2 startPos;
    private Vector2 direction;

    private void Start()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        if (startPos == null || direction == null) return;
        LaserShot();
    }

    private void LaserShot()
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, 100, layerMask);

        if(hit.collider && (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") || hit.collider.gameObject.layer == LayerMask.NameToLayer("DeadBody")))
        {
            if (rayLength != hit.distance)
            {
                rayLength = hit.distance;
                Vector2 laserScale;

                switch (laserDirection)
                {
                    case LaserDirection.Left:
                        laserScale = launchPosObjs[0].transform.localScale;
                        laserScale.x = hit.distance;
                        laserScale.y = launchPosObjs[0].transform.localScale.y;
                        launchPosObjs[0].transform.localScale = laserScale;
                        break;

                    //case LaserDirection.Right:
                    //    laserScale = launchPosObjs[1].transform.localScale;
                    //    laserScale.x = -hit.distance;
                    //    laserScale.y = launchPosObjs[1].transform.localScale.y;
                    //    launchPosObjs[1].transform.localScale = laserScale;
                    //    break;

                    //case LaserDirection.Down:
                    //    laserScale = launchPosObjs[2].transform.localScale;
                    //    laserScale.x = launchPosObjs[2].transform.localScale.x;
                    //    laserScale.y = -hit.distance;
                    //    launchPosObjs[2].transform.localScale = laserScale;
                    //    break;

                    //case LaserDirection.Up:
                    //    laserScale = launchPosObjs[3].transform.localScale;
                    //    laserScale.x = launchPosObjs[3].transform.localScale.x;
                    //    laserScale.y = hit.distance;
                    //    launchPosObjs[3].transform.localScale = laserScale;
                    //    break;

                    default:
                        laserScale = launchPosObjs[0].transform.localScale;
                        laserScale.x = hit.distance;
                        laserScale.y = launchPosObjs[0].transform.localScale.y;
                        launchPosObjs[0].transform.localScale = laserScale;
                        break;
                }
                

                Debug.DrawRay(startPos, direction, Color.red);

            }
        }
    }

    private void Initialize()
    {
        switch(laserDirection)
        {
            case LaserDirection.Left:
                startPos = launchPosObjs[0].transform.position;
                direction = Vector2.left;
                break;

            //case LaserDirection.Right:
            //    startPos = launchPosObjs[1].transform.position;
            //    direction = Vector2.right;
            //    break;

            //case LaserDirection.Down:
            //    startPos = launchPosObjs[2].transform.position;
            //    direction = Vector2.down;
            //    break;

            //case LaserDirection.Up:
            //    startPos = launchPosObjs[3].transform.position;
            //    break;

            default:
                laserDirection = LaserDirection.Left;
                startPos = launchPosObjs[0].transform.position;
                direction = Vector2.left;
                break;
        }
    }
}
