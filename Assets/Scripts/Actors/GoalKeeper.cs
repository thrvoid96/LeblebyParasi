using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKeeper : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float travellingTime, speed;
    [Header("Left = (-1,0,0) Right = (1,0,0)")]
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private int slowMoveAmount, fastMoveAmount;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(moveRightLeftWithDifferantiatingSpeeds(travellingTime, speed, moveDirection));
    }


    private IEnumerator moveRightLeftWithDifferantiatingSpeeds(float travelTime, float speed, Vector3 moveDir)
    {
        var savedTime = travelTime;
        int slowCount = 0;
        int fastCount = 0;
        bool goingSlow = true;

        while (true)
        {
            if (travelTime <= 0f)
            {
                moveDir = -moveDir;
                travelTime = savedTime;
                rb.velocity = new Vector3(0,0,0);

                if (goingSlow)
                {
                    slowCount++;

                    if (slowCount == slowMoveAmount)
                    {
                        speed *= 2;
                        travelTime /= 2;
                        savedTime /= 2;
                        slowCount = 0;
                        goingSlow = false;
                    }
                }
                else
                {
                    fastCount++;

                    if (fastCount == fastMoveAmount)
                    {
                        speed /= 2;
                        travelTime *= 2;
                        savedTime *= 2;
                        fastCount = 0;
                        goingSlow = true;
                    }
                }

                yield return null;
            }
            else
            {
                rb.AddForce(moveDir * speed * 1000f * Time.deltaTime);
                travelTime -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
