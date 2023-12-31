using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;
    [SerializeField] bool isMainMenu = false;
    public PlayerController controller;

    [SerializeField] GameObject tombPrefab;
    [SerializeField] CircleWipeController circleWipeController;
    GameObject tomb;

    [SerializeField] ParticleSystem breakingParticle;
    [SerializeField] ParticleSystem deathParticle;

    public float timeBetweenEachAnim = 10f;

    public float timer = 0f;

    void Start() {
        animator = GetComponent<Animator>();
    }

    public void ResetAnim() {
        timer = 0f;
        animator.SetBool("IsDeath", false);
        animator.SetTrigger("Spawn");
    }

    //Called in death annimation
    public void FadeInEndScreen() {
        circleWipeController.FadeIn(GameManager.instance.endFadeOffset, 2f);
    }

    public void ReactivePlayMode() {
        PlayerController.instance.canPlay = true;
    }

    void Update() {
        timer += Time.deltaTime;

        if(timer >= timeBetweenEachAnim) {
            DoIdleAnim();
            timer = 0f;
        }

        if (!isMainMenu && controller.targetedObject != null)
            animator.SetFloat("MiningSpeed", PlayerStats.instance.miningRate.value);
    }

    public void ActiveBreakingParticules()
    {
        if (PlayerController.instance.iscurrentlymining)
            breakingParticle.Play();
    }

    public void ActiveDeathParticules()
    {
        if (PlayerController.instance.iscurrentlymining)
            deathParticle.Play();
    }

    public void DoIdleAnim()
    {
        int randomIndex = Random.Range(0, 50);
        if (randomIndex < 30)
        {
            animator.SetInteger("indexIdle", 1);
        }
        else if (randomIndex < 35)
        {
            animator.SetInteger("indexIdle", 2);
        }
        else
            animator.SetInteger("indexIdle", 3);


        animator.SetTrigger("StartIdle");

    }

    public void DoMiningAnim()
    {
        int randomIndex = Random.Range(0, 50);
        if (randomIndex < 25)
        {
            animator.SetInteger("IndexMining", 0);
        }
        else
        {
            animator.SetInteger("IndexMining", 1);
        }


        animator.SetTrigger("StartMining");

    }

    public void OnDeath() {
        animator.SetBool("IsDeath", true);
        animator.SetBool("isMining", false);
        WakeUp();
    }

    public void WakeUp() {
        animator.SetBool("IsSleeping", false);
        animator.ResetTrigger("StartIdle");
    }

    public void PlaceTomb()
    {
        if (tomb == null)
            tomb = Instantiate(tombPrefab);
        tomb.transform.position = new Vector3(transform.position.x, tomb.transform.position.y);
    }

    public void ActiveIsSleeping()
    {
        animator.SetBool("IsSleeping", true);
    }
}
