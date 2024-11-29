using UnityEngine;

public class DrowsinessTest : MonoBehaviour
{
    public Animator animator; // �ƹ�Ÿ�� ����� Animator
    private float timer = 0f; // ���� ��ȯ�� ���� Ÿ�̸�
    private bool isDrowsy = false; // �ʱ� ����: Awake

    void Update()
    {
        // ������ Ÿ�̸ӷ� ���� ��ȯ �׽�Ʈ
        timer += Time.deltaTime;

        if (timer > 3f) // 3�ʸ��� ���� ��ȯ
        {
            isDrowsy = !isDrowsy; // ���� ����
            animator.SetBool("isDrowsy", isDrowsy); // Animator�� ����
            Debug.Log($"Animator ���� ��ȯ: isDrowsy = {isDrowsy}");
            timer = 0f; // Ÿ�̸� �ʱ�ȭ
        }
    }
}
