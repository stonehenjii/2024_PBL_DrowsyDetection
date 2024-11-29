using UnityEngine;

public class DrowsinessTest : MonoBehaviour
{
    public Animator animator; // 아바타에 연결된 Animator
    private float timer = 0f; // 상태 전환을 위한 타이머
    private bool isDrowsy = false; // 초기 상태: Awake

    void Update()
    {
        // 간단한 타이머로 상태 전환 테스트
        timer += Time.deltaTime;

        if (timer > 3f) // 3초마다 상태 전환
        {
            isDrowsy = !isDrowsy; // 상태 변경
            animator.SetBool("isDrowsy", isDrowsy); // Animator에 전달
            Debug.Log($"Animator 상태 전환: isDrowsy = {isDrowsy}");
            timer = 0f; // 타이머 초기화
        }
    }
}
