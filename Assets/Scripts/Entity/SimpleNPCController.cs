using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SimpleNPCController : MonoBehaviour
{
    // UI 참조
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    // 대화 내용
    //한글로 쓰고싶었는데, 폰트가...깨진다...
    public string dialogue1 = "Nice to meet you. Come closer and press E.";  // 처음 만났을 때 첫 대사
    public string dialogue2 = "Good, press E again and you can play the mini-game.";  // 두 번째 대사
    public string dialogue3 = "Press E to try the mini-game where you shoot enemies.";  // 세 번째 대사
    public string dialogue4 = "You're back? Come closer and press E.";  // 재방문 시 첫 대사

    // 미니게임 씬 이름
    public string miniGameSceneName = "MiniGameScene";

    // PlayerPrefs 키
    private const string HAS_MET_NPC_KEY = "HasMetNPC";
    private bool hasMetNPC = false;

    // 대화 상태
    private int dialogueState = 0;

    // 플레이어 참조
    private GameObject player;
    private SpriteRenderer spriteRenderer;

    // 상호작용 설정
    public float firstDialogueDistance = 4f;   // 첫 대화가 시작되는 거리
    public float interactionDistance = 1.5f;   // E키 입력이 가능한 거리

    void Start()
    {
        // 필수 컴포넌트 확인
        if (dialoguePanel == null || dialogueText == null)
        {
            Debug.LogError("대화 UI가 연결되지 않았습니다!");
            return;
        }

        // 시작시 대화 패널 숨기기
        dialoguePanel.SetActive(false);

        // 플레이어 찾기
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }

        // 스프라이트 렌더러 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>();

        // NPC를 이미 만났는지 확인
        hasMetNPC = PlayerPrefs.GetInt(HAS_MET_NPC_KEY, 0) > 0;
    }

    void Update()
    {
        if (player == null) return;

        // X, Y 좌표만 사용하여 직접 거리 계산
        float dx = transform.position.x - player.transform.position.x;
        float dy = transform.position.y - player.transform.position.y;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        // 첫 대화는 firstDialogueDistance 내에서 시작
        if (dialogueState == 0 && distance < firstDialogueDistance)
        {
            // 이미 만난 적이 있는지에 따라 다른 첫 대사 출력
            if (hasMetNPC)
            {
                ShowDialogue(dialogue4);  // 재방문 시 첫 대사
            }
            else
            {
                ShowDialogue(dialogue1);  // 처음 만났을 때 첫 대사
            }
            dialogueState = 1;
        }

        // E 키는 더 가까운 interactionDistance 내에서만 작동
        if (Input.GetKeyDown(KeyCode.E) && distance < interactionDistance)
        {
            NextDialogue();
        }

        // NPC 회전
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = player.transform.position.x < transform.position.x;
        }
    }

    // 대화 표시
    void ShowDialogue(string text)
    {
        dialogueText.text = text;
        dialoguePanel.SetActive(true);
    }

    // 다음 대화로 진행
    void NextDialogue()
    {
        switch (dialogueState)
        {
            case 1:
                ShowDialogue(dialogue2);
                dialogueState = 2;
                break;

            case 2:
                ShowDialogue(dialogue3);
                dialogueState = 3;
                break;

            case 3:
                // NPC를 만난 것으로 저장
                if (!hasMetNPC)
                {
                    PlayerPrefs.SetInt(HAS_MET_NPC_KEY, 1);
                    PlayerPrefs.Save();
                    hasMetNPC = true;
                }

                // 미니게임 씬으로 이동
                SceneManager.LoadScene(miniGameSceneName);
                break;

            default:
                break;
        }
    }

    // 상호작용 범위 시각화
    void OnDrawGizmos()
    {
        // 첫 대화 시작 범위 (노란색)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, firstDialogueDistance);

        // E키 상호작용 범위 (초록색)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }

    // 디버그용: NPC 만남 기록 초기화 (인스펙터에서 호출 가능)
    public void ResetMeetingStatus()
    {
        PlayerPrefs.DeleteKey(HAS_MET_NPC_KEY);
        hasMetNPC = false;
        Debug.Log("NPC 만남 기록이 초기화되었습니다");
    }
}