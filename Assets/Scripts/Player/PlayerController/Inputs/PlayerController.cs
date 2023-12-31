using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [SerializeField] WallCreator wallCreator;

    [SerializeField] PlayerAnimationController playerAnimController;

    [SerializeField] GameObject effectBarGo;
    [SerializeField] Material effectBarMat;

    public Transform raycastOrigin;
    Vector2 startPos;

    [HideInInspector] public bool isMining;
    [HideInInspector] public bool canPlay;
    [HideInInspector] public bool iscurrentlymining;

    [SerializeField] float distanceToMine = 2f;

    public GameObject targetedObject;

    float spaceBarTimer;

    MiningState miningState;

    [SerializeField] LayerMask blockLayerMask;

    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] Material particleMaterial;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        effectBarGo.SetActive(false);
        startPos = transform.position;
    }
    
    private void Update()
    {
        MiningControl();
    }

    public void ResetPlayer()
    {
        canPlay = false;
        spaceBarTimer = PlayerStats.instance.miningMaxTime.value;
        PlayerStats.instance.depth = 0;
        transform.position = startPos;
        isMining = false;

        playerAnimController.ResetAnim();
    }

    public void Respawn() {
        GameManager.instance.EndParty();
    }

    public void PlayerDeath() {
        iscurrentlymining = false;
        playerAnimController.OnDeath();
        targetedObject = null;

        StopAllCoroutines();
    }

    void MiningControl() {
        if (isMining && canPlay)
        {
            if (spaceBarTimer > 0f)
            {
                spaceBarTimer -= Time.deltaTime;

                if(!effectBarGo.activeSelf)
                    effectBarGo.SetActive(true);
                
                effectBarMat.SetFloat("_HealthAmout", spaceBarTimer / PlayerStats.instance.miningMaxTime.value);

                if (targetedObject == null)
                {
                    RaycastHit2D hit;
                    hit = Physics2D.Raycast(raycastOrigin.position, transform.right, distanceToMine, blockLayerMask);
                    if (hit) targetedObject = hit.transform.gameObject;
                    else
                    {
                        transform.position += Vector3.right * 1 * Time.deltaTime * PlayerStats.instance.walkSpeed.value;
                    }
                }
                else if (!iscurrentlymining)
                {
                    iscurrentlymining = true;
                    StartCoroutine(MiningCoroutine(targetedObject.transform.GetComponent<ObjectData>().objectData));
                }
            }
            else {
                PlayerDeath();
            }
        }
        else if(effectBarGo.activeSelf)
            effectBarGo.SetActive(false);
    }

    public IEnumerator MiningCoroutine(Ore objectData)
    {
        int blocHardness = objectData.hardness;
        int numberOfState = objectData.sprites.Length;

        int durabilityBetweenState = blocHardness / numberOfState;

        if (miningState == null)
        {
            int currentState = numberOfState - 1;

            int remainingDurability = blocHardness;

            miningState = new MiningState(currentState, remainingDurability);
        }

        particleMaterial.SetTexture("_MainTex", objectData.icon.texture);

        while (miningState.remainingDurability > 0)
        {
            if (miningState.remainingDurability <= durabilityBetweenState * miningState.currentState)
            {
                targetedObject.transform.GetComponent<SpriteRenderer>().sprite = objectData.sprites[miningState.currentState];

                miningState.currentState --;
            }

            miningState.remainingDurability -= (int)PlayerStats.instance.miningpower.value;
            yield return new WaitForSeconds(1 / PlayerStats.instance.miningRate.value);

            if (!isMining)
            {
                iscurrentlymining = false;
                yield break;
            }
        }
        miningState = null;
        iscurrentlymining = false;

        inventoryManager.AddItemInInventory(objectData, (int)PlayerStats.instance.miningOreReceived.value);

        GameObject blockBelow = targetedObject.transform.GetComponent<ObjectData>().blockBelow;

        ChangeSpriteOfNearbyBlocks(blockBelow);

        if (!blockBelow) wallCreator.CreateWall();

        Destroy(targetedObject);
        
        if (blockBelow != null)
        {
            targetedObject = blockBelow;
        }
    }

    void ChangeSpriteOfNearbyBlocks(GameObject blockBelow)
    {
        // left block
        RaycastHit2D hit;
        hit = Physics2D.Raycast(targetedObject.transform.position + Vector3.right * Mathf.Abs(targetedObject.transform.lossyScale.x), transform.right, 1, blockLayerMask);



        if (hit)
        {
            hit.transform.GetComponent<SpriteRenderer>().sprite = hit.transform.GetComponent<ObjectData>().objectData.damagedBorder;
        }

        //block bellow
        if (blockBelow != null)
            blockBelow.transform.GetComponent<SpriteRenderer>().sprite = blockBelow.transform.GetComponent<ObjectData>().objectData.damagedCorner;
    }
}

public class MiningState
{
    public int currentState;
    public int remainingDurability;

    public MiningState(int currentState, int remainingDurability)
    {
        this.currentState = currentState;
        this.remainingDurability = remainingDurability;
    }
}
