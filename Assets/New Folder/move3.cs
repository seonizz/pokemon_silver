using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // 씬전환
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]

public class move3 : MonoBehaviour
{
    [Header("이동 세팅")]
    public float moveSpeed = 3f;   
    public float stepSize = 1f;

    [Header("애니메이션 스프라이트")]
    public Sprite[] downSprites;
    public Sprite[] upSprites;
    public Sprite[] leftSprites;
    public Sprite[] rightSprites;

    [Header("랜덤 인카운터")]
    public float encounterProbability = 0.1f;
    private bool isInTallGrass = false;     
    private TallGrassZone currentGrassZone; 

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

        currentAnim = leftSprites; 
        if (currentAnim != null && currentAnim.Length > 0)
        {
            sr.sprite = currentAnim[0];
        }
    }

    void Start()
    {
        if (TransitionData.returningFromBattle)
        {
            rb.position = TransitionData.returnPosition;
            transform.position = TransitionData.returnPosition;
            targetPos = rb.position; 

            TransitionData.returningFromBattle = false;
        }

        else if (TransitionData.hasSpawn)
        {
            rb.position = TransitionData.nextSpawnPos;
            transform.position = TransitionData.nextSpawnPos;
            targetPos = rb.position;

            TransitionData.hasSpawn = false;
        }
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

        targetPos = rb.position + moveDir * stepSize;
        animIndex = 0;
        animTimer = 0f;
        isMoving = true;
        currentAnim = GetSpriteArray(moveDir);

        if (IsBlocked(moveDir, stepSize))
        {
            currentAnim = GetSpriteArray(moveDir);
            if (currentAnim != null && currentAnim.Length > 0)
                sr.sprite = currentAnim[0];

            animIndex = 0;
            animTimer = 0f;
            return;
        }
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

            if (isInTallGrass)
            {
                CheckForEncounter();
            }

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

    private void CheckForEncounter()
    {
        if (Random.value <= encounterProbability)
        {
            StartBattle(); 
        }
    }

    private void StartBattle()
    {
        if (currentGrassZone == null) return;

        PokemonData wildPokemon = currentGrassZone.GetRandomPokemon();

        if (wildPokemon != null)
        {
            Debug.Log($"야생의 {wildPokemon.pokemonName}(이)가 나타났다 !!!");

            TransitionData.wildPokemon = wildPokemon;
            TransitionData.returnSceneName = SceneManager.GetActiveScene().name;
            TransitionData.returnPosition = transform.position;
            TransitionData.returningFromBattle = true;
            SceneManager.LoadScene("BattleScene");
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TallGrass"))
        {
            isInTallGrass = true;
            currentGrassZone = other.GetComponent<TallGrassZone>();
            Debug.Log("풀숲에 들어옴");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("TallGrass"))
        {
            isInTallGrass = false;
            currentGrassZone = null;
            Debug.Log("풀숲에서 나옴");
        }
    }
}