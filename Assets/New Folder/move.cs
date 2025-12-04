using UnityEngine;

using System.Collections.Generic;



[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]

public class move : MonoBehaviour

{

    [Header("이동 세팅")]

    public float moveSpeed = 3f;     // 한 칸 이동 속도

    public float stepSize = 1f;



    [Header("애니메이션 스프라이트")]

    public Sprite[] downSprites;

    public Sprite[] upSprites;

    public Sprite[] leftSprites;

    public Sprite[] rightSprites;





    private SpriteRenderer sr;

    private Rigidbody2D rb;



    private Vector2 targetPos;

    private bool isMoving = false;

    private Vector2 moveDir;



    private float animTimer = 0f;

    private int animIndex = 0;

    private Sprite[] currentAnim;




    private ContactFilter2D filter;

    private readonly List<RaycastHit2D> hits = new();



    void Awake()

    {

        sr = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();





        rb.bodyType = RigidbodyType2D.Dynamic;

        rb.gravityScale = 0f;

        rb.freezeRotation = true;

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;





        filter.useTriggers = false;

        filter.SetLayerMask(Physics2D.DefaultRaycastLayers);

        filter.useLayerMask = true;



        targetPos = rb.position;

        currentAnim = downSprites; 

    }



    void Start()
    {
        Debug.Log("Start 함수 실행 위치: " + transform.position);

        if (TransitionData.hasSpawn)
        {
            Debug.Log("목표 스폰 위치 " + TransitionData.nextSpawnPos + "로 이동");

            rb.position = TransitionData.nextSpawnPos;
            transform.position = TransitionData.nextSpawnPos;
            targetPos = rb.position;

            Debug.Log("이동 후 위치: " + transform.position);

            TransitionData.hasSpawn = false;
        }
    }

    void LateUpdate()
    {
        Debug.Log("최종위치: " + transform.position);
    }

    void Update()
    {
        if (isMoving)
        {
            AnimateStep();
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (vertical != 0)
        {
            moveDir = vertical > 0 ? Vector2.up : Vector2.down;
        }
        else if (horizontal != 0)
        {
            moveDir = horizontal > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            return;
        }

        if (IsBlocked(moveDir, stepSize))
        {
            currentAnim = GetSpriteArray(moveDir);
            if (currentAnim != null && currentAnim.Length > 0)
                sr.sprite = currentAnim[0];

            animIndex = 0;
            animTimer = 0f;
            return;
        }

        targetPos = rb.position + moveDir * stepSize;
        isMoving = true;


        animIndex = 0;
        animTimer = 0f;
        currentAnim = GetSpriteArray(moveDir);
    }

    void FixedUpdate()

    {

        if (!isMoving) return;





        Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, moveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(newPos);



        if ((rb.position - targetPos).sqrMagnitude <= 0.0001f)

        {

            rb.MovePosition(targetPos);

            isMoving = false;

            animIndex = 0;

            animTimer = 0f;

            if (currentAnim != null && currentAnim.Length > 0)

                sr.sprite = currentAnim[0];

        }

    }



    bool IsBlocked(Vector2 dir, float distance)

    {



        int count = rb.Cast(dir, filter, hits, distance);

        hits.Clear();

        return count > 0;

    }



    void AnimateStep()

    {

        if (currentAnim == null || currentAnim.Length == 0) return;



        animTimer += Time.deltaTime;





        if (animTimer >= 0.1f)

        {

            sr.sprite = currentAnim[animIndex];

            animIndex = (animIndex + 1) % currentAnim.Length;

            animTimer = 0f;

        }

    }



    Sprite[] GetSpriteArray(Vector2 dir)

    {

        if (dir == Vector2.up) return upSprites;

        if (dir == Vector2.down) return downSprites;

        if (dir == Vector2.left) return leftSprites;

        return rightSprites;

    }

}