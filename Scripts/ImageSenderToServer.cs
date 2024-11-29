using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ImageSenderToServer : MonoBehaviour
{
    public string serverURL = "http://127.0.0.1:5000/drowsiness"; // Flask 서버 URL
    public float captureInterval = 2.0f; // 이미지 캡처 간격
    public GameObject avatar; // 아바타 오브젝트

    private Animator animator; // Animator 컴포넌트
    private WebCamTexture webCamTexture;

    void Start()
    {
        // Animator 컴포넌트 가져오기
        animator = avatar.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        // 웹캠 시작
        webCamTexture = new WebCamTexture();
        webCamTexture.Play();

        // 웹캠이 준비될 때까지 대기 후 주기적인 이미지 캡처 및 전송 시작
        StartCoroutine(CheckWebcamReady());
    }

    private IEnumerator CheckWebcamReady()
    {
        // 웹캠이 준비될 때까지 대기
        while (!webCamTexture.isPlaying || webCamTexture.width < 100)
        {
            yield return null;
        }

        Debug.Log("웹캠 시작됨");
        // 이미지 캡처 및 서버 전송 시작
        StartCoroutine(CaptureAndSendImage());
    }

    private IEnumerator CaptureAndSendImage()
    {
        while (true)
        {
            yield return new WaitForSeconds(captureInterval);

            if (webCamTexture.isPlaying && webCamTexture.width > 100)
            {
                // 웹캠 캡처
                Texture2D capturedImage = new Texture2D(webCamTexture.width, webCamTexture.height);
                capturedImage.SetPixels(webCamTexture.GetPixels());
                capturedImage.Apply();

                // 이미지 데이터를 바이트로 변환
                byte[] imageBytes = capturedImage.EncodeToJPG();

                // 서버로 이미지 전송
                StartCoroutine(SendImageToServer(imageBytes));
            }
            else
            {
                Debug.LogError("웹캠 준비 상태 불충분.");
            }
        }
    }

    private IEnumerator SendImageToServer(byte[] imageBytes)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", imageBytes, "webcam.jpg", "image/jpeg");

        using (UnityWebRequest www = UnityWebRequest.Post(serverURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("서버 응답: " + responseText);

                // 서버 응답이 "졸음 상태"일 때 Animator 상태 전환
                if (responseText.Contains("\"drowsiness\": true"))
                {
                    SetAnimatorDrowsyState(true);
                }
                else
                {
                    SetAnimatorDrowsyState(false);
                }
            }
            else
            {
                Debug.LogError("서버 전송 실패: " + www.error);
            }
        }
    }

    private void SetAnimatorDrowsyState(bool isDrowsy)
    {
        if (animator != null)
        {
            animator.SetBool("isDrowsy", isDrowsy);
            Debug.Log($"Animator 상태 전환: isDrowsy = {isDrowsy}");
        }
        else
        {
            Debug.LogError("Animator가 설정되지 않았습니다.");
        }
    }

    void OnDestroy()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}
