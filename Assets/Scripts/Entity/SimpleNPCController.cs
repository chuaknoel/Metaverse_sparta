using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SimpleNPCController : MonoBehaviour
{
    // UI ����
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    // ��ȭ ����
    //�ѱ۷� ����;��µ�, ��Ʈ��...������...
    public string dialogue1 = "Nice to meet you. Come closer and press E.";  // ó�� ������ �� ù ���
    public string dialogue2 = "Good, press E again and you can play the mini-game.";  // �� ��° ���
    public string dialogue3 = "Press E to try the mini-game where you shoot enemies.";  // �� ��° ���
    public string dialogue4 = "You're back? Come closer and press E.";  // ��湮 �� ù ���

    // �̴ϰ��� �� �̸�
    public string miniGameSceneName = "MiniGameScene";

    // PlayerPrefs Ű
    private const string HAS_MET_NPC_KEY = "HasMetNPC";
    private bool hasMetNPC = false;

    // ��ȭ ����
    private int dialogueState = 0;

    // �÷��̾� ����
    private GameObject player;
    private SpriteRenderer spriteRenderer;

    // ��ȣ�ۿ� ����
    public float firstDialogueDistance = 4f;   // ù ��ȭ�� ���۵Ǵ� �Ÿ�
    public float interactionDistance = 1.5f;   // EŰ �Է��� ������ �Ÿ�

    void Start()
    {
        // �ʼ� ������Ʈ Ȯ��
        if (dialoguePanel == null || dialogueText == null)
        {
            Debug.LogError("��ȭ UI�� ������� �ʾҽ��ϴ�!");
            return;
        }

        // ���۽� ��ȭ �г� �����
        dialoguePanel.SetActive(false);

        // �÷��̾� ã��
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player �±׸� ���� ������Ʈ�� ã�� �� �����ϴ�!");
        }

        // ��������Ʈ ������ ��������
        spriteRenderer = GetComponent<SpriteRenderer>();

        // NPC�� �̹� �������� Ȯ��
        hasMetNPC = PlayerPrefs.GetInt(HAS_MET_NPC_KEY, 0) > 0;
    }

    void Update()
    {
        if (player == null) return;

        // X, Y ��ǥ�� ����Ͽ� ���� �Ÿ� ���
        float dx = transform.position.x - player.transform.position.x;
        float dy = transform.position.y - player.transform.position.y;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        // ù ��ȭ�� firstDialogueDistance ������ ����
        if (dialogueState == 0 && distance < firstDialogueDistance)
        {
            // �̹� ���� ���� �ִ����� ���� �ٸ� ù ��� ���
            if (hasMetNPC)
            {
                ShowDialogue(dialogue4);  // ��湮 �� ù ���
            }
            else
            {
                ShowDialogue(dialogue1);  // ó�� ������ �� ù ���
            }
            dialogueState = 1;
        }

        // E Ű�� �� ����� interactionDistance �������� �۵�
        if (Input.GetKeyDown(KeyCode.E) && distance < interactionDistance)
        {
            NextDialogue();
        }

        // NPC ȸ��
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = player.transform.position.x < transform.position.x;
        }
    }

    // ��ȭ ǥ��
    void ShowDialogue(string text)
    {
        dialogueText.text = text;
        dialoguePanel.SetActive(true);
    }

    // ���� ��ȭ�� ����
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
                // NPC�� ���� ������ ����
                if (!hasMetNPC)
                {
                    PlayerPrefs.SetInt(HAS_MET_NPC_KEY, 1);
                    PlayerPrefs.Save();
                    hasMetNPC = true;
                }

                // �̴ϰ��� ������ �̵�
                SceneManager.LoadScene(miniGameSceneName);
                break;

            default:
                break;
        }
    }

    // ��ȣ�ۿ� ���� �ð�ȭ
    void OnDrawGizmos()
    {
        // ù ��ȭ ���� ���� (�����)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, firstDialogueDistance);

        // EŰ ��ȣ�ۿ� ���� (�ʷϻ�)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }

    // ����׿�: NPC ���� ��� �ʱ�ȭ (�ν����Ϳ��� ȣ�� ����)
    public void ResetMeetingStatus()
    {
        PlayerPrefs.DeleteKey(HAS_MET_NPC_KEY);
        hasMetNPC = false;
        Debug.Log("NPC ���� ����� �ʱ�ȭ�Ǿ����ϴ�");
    }
}