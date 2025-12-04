using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; 

public enum BattleState { Start, Intro, PlayerAction, EnemyTurn, Busy, Won, Lost, Run }

public class BattleManager : MonoBehaviour
{
    public BattleState state;

    [Header("BattleUnit 스크립트")]
    public BattleUnit playerUnit;
    public BattleUnit enemyUnit;

    [Header("스프라이트")]
    public Image playerSprite;
    public Sprite trainerBackSprite;
    public Image enemySprite;

    [Header("대사창 및 UI 패널")]
    public GameObject dialogBox;
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI nextArrowIcon; 

    [Header("커서 선택")]
    public GameObject actionPanel; 
    public TextMeshProUGUI actionCursor; 
    public Vector2 actionPosFight = new Vector2(-50, 25); 
    public Vector2 actionPosRun = new Vector2(-50, -10);  
    private int currentActionSelection; 

    [Header("임시 테스트용")]
    public PokemonData TEST_PlayerPokemon;

    void Start()
    {
        state = BattleState.Start;
        StartCoroutine(SetupBattle());
    }


    IEnumerator SetupBattle()
    {
        PokemonData wildPokemon = TransitionData.wildPokemon;
        PokemonData playerPokemon = TEST_PlayerPokemon; 
        playerUnit.Setup(playerPokemon);
        enemyUnit.Setup(wildPokemon);
        playerSprite.sprite = trainerBackSprite;
        enemySprite.sprite = wildPokemon.pokemonSprite;

        dialogBox.SetActive(true);
        actionPanel.SetActive(false);
        if (nextArrowIcon != null) nextArrowIcon.gameObject.SetActive(false);

        yield return StartCoroutine(TypeDialogue($"야생 포켓몬 {wildPokemon.pokemonName}이/가 나타났다!"));
        yield return StartCoroutine(TypeDialogue($"골드은/는 {playerPokemon.pokemonName}을(를) 꺼냈다!"));
        playerSprite.sprite = playerPokemon.backSprite;

        state = BattleState.PlayerAction;
        PlayerActionStart();
    }


    void PlayerActionStart()
    {
        dialogText.text = "";
        actionPanel.SetActive(true); 
        if (actionCursor != null)
        {
            actionCursor.gameObject.SetActive(true);
            actionCursor.text = "▶"; 
        }

        currentActionSelection = 0;
        UpdateActionCursor();
    }

    void Update()
    {
        if (state == BattleState.PlayerAction)
        {
  
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentActionSelection = (currentActionSelection == 0) ? 1 : 0; 
                UpdateActionCursor();
            }
         
            else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
            {
                if (currentActionSelection == 0) 
                {
                    state = BattleState.Busy;
                    StartCoroutine(PlayerAttack());
                }
                else // "도망간다" 선택
                {
                    state = BattleState.Run;
                    StartCoroutine(EndBattle());
                }
            }
        }
    }

    
    void UpdateActionCursor()
    {
        if (actionCursor == null) return;
        actionCursor.rectTransform.anchoredPosition = (currentActionSelection == 0) ? actionPosFight : actionPosRun;
    }

 
    IEnumerator PlayerAttack()
    {
        state = BattleState.Busy;
        actionPanel.SetActive(false); 
        yield return StartCoroutine(TypeDialogue($"{playerUnit.pokemon.pokemonName}의 할퀴기!"));

        int damage = Mathf.CeilToInt(enemyUnit.pokemon.maxHp / 2.0f);
        bool isFainted = enemyUnit.TakeDamage(damage);

        if (isFainted)
        {
            state = BattleState.Won;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }


    IEnumerator EnemyTurn()
    {
        state = BattleState.Busy;
        yield return StartCoroutine(TypeDialogue($"{enemyUnit.pokemon.pokemonName}의 몸통박치기!"));

        int damage = Mathf.CeilToInt(playerUnit.pokemon.maxHp / 2.0f);
        bool isFainted = playerUnit.TakeDamage(damage);

        if (isFainted)
        {
            state = BattleState.Lost;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.PlayerAction;
            PlayerActionStart();
        }
    }


    IEnumerator EndBattle()
    {
        state = BattleState.Busy;
        actionPanel.SetActive(false); 

        if (state == BattleState.Won)
        {
            yield return StartCoroutine(TypeDialogue($"{enemyUnit.pokemon.pokemonName}을(를) 쓰러트렸다!"));
            int oldLevel = playerUnit.pokemon.level;
            playerUnit.pokemon.level++;
            yield return StartCoroutine(TypeDialogue($"{playerUnit.pokemon.pokemonName}은(는) 레벨이 {oldLevel}에서 {playerUnit.pokemon.level}(으)로 올랐다!"));
        }
        else if (state == BattleState.Lost)
        {
            yield return StartCoroutine(TypeDialogue("눈앞이 캄캄해졌다..."));
        }
        else if (state == BattleState.Run)
        {
            yield return StartCoroutine(TypeDialogue("무사히 도망쳤다!"));
        }

        SceneManager.LoadScene(TransitionData.returnSceneName);
    }

    // [시나리오 6] 대사창 + 다음 화살표(▼) 로직
    IEnumerator TypeDialogue(string line)
    {
        dialogText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(0.04f);
        }

        if (nextArrowIcon != null) nextArrowIcon.gameObject.SetActive(true);

        yield return new WaitUntil(() => !Input.GetKey(KeyCode.Return));
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

        if (nextArrowIcon != null) nextArrowIcon.gameObject.SetActive(false);
    }
}