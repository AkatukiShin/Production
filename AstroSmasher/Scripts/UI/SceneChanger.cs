using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneChanger : MonoBehaviour
{
    private InputAction decisionAction;

    private void Start()
    {
        var playerInput = GetComponent<PlayerInput>();
        decisionAction = playerInput.actions["Decision"]; // InputActionアセットのアクション名と一致させる
        decisionAction.performed += OnDecision; // イベントを登録
        decisionAction.Enable(); // アクションを有効化
    }

    private void OnDestroy()
    {
        decisionAction.performed -= OnDecision; // イベントを解除
    }

    public void OnDecision(InputAction.CallbackContext context)
    {
        if (context.performed) SceneManager.LoadScene("Fujiwara_Title");
    }
}

