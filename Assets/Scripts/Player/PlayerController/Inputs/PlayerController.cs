using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public Transform raycastOrigin;

    public bool isMining;

    public RaycastHit2D targetedObject;

    float spaceBarTimer;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        ResetSpaceBarTimer();
    }

    public void ResetSpaceBarTimer()
    {
        //spaceBarTimer = 
    }

    private void Update()
    {
        if (isMining)
        {
            if (spaceBarTimer > 0f)
            {
                spaceBarTimer -= Time.deltaTime;
                if (!targetedObject)
                {
                    targetedObject = Physics2D.Raycast(raycastOrigin.position, transform.forward, 1);
                }
                else
                {

                }
            }
        }
    }
}