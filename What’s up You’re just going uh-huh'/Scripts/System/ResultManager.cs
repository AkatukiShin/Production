using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections;
using unityroom.Api;

public class ResultManager : MonoBehaviour
{
    [TextArea]
    [SerializeField]
    private string allText =
        "「ふー。沢山喋ったね」" +
        "「今日のあなたに点数をつけるとしたら......」" +
        "「○○点　だと思う！」"; 

    [SerializeField]
    private TextMeshProUGUI resultText;

    [SerializeField]
    private GameObject fadeInPanelObj;

    [SerializeField]
    private GameObject resultObj;
    private Image fadeInPanel;

    private int score;

    private string[] messages;
    private int currentIndex = 0;
    private int scoreStage = 0;

    void Start()
    {
        resultObj.SetActive(true);
        fadeInPanel = fadeInPanelObj.GetComponent<Image>();
        score = ScoreManager.score;
        string processedText = allText.Replace("○○", score.ToString());
        UnityroomApiClient.Instance.SendScore(1, score, ScoreboardWriteMode.HighScoreDesc);

        var mc = Regex.Matches(processedText, "「([^」]+)」");
        messages = new string[mc.Count];
        for (int i = 0; i < mc.Count; i++)
            messages[i] = mc[i].Groups[1].Value;

        if (messages.Length > 0)
            resultText.text = messages[0];

        Debug.Log("paweeeee");
    }

    void Update()
    {
        if (Input.anyKeyDown)
            AdvanceMessage();
    }

    private void AdvanceMessage()
    {
        // 通常メッセージを順に表示
        if (currentIndex < messages.Length - 1)
        {
            currentIndex++;
            resultText.text = messages[currentIndex];
            return;
        }
        // 次にスコア 1 行目・2 行目を表示
        if (scoreStage == 0)
        {
            resultText.text = GetScoreFirstLine();
            scoreStage = 1;
        }
        else if (scoreStage == 1)
        {
            resultText.text = GetScoreSecondLine();
            scoreStage = 2;
        }
        // scoreStage == 2 以降は何もしない or シーン切り替え
        else if (scoreStage == 2)
        {
            resultText.text = GetScoreThirdLine();
            scoreStage = 3;
        }
        else if (scoreStage == 3)
        {
            StartCoroutine(fadeIn());
        }
    }

    private string GetScoreFirstLine()
    {
        if (score == 100) return $"神！話しやすさが神！";
        else if (score >= 80) return $"なにその話術！？時間泥棒なんだけど！";
        else if (score >= 50) return $"え、話しやす～い。センスあるー。";
        else if (score >= 20) return $"ちょいちょい話に引き込まれた！";
        else if (score >= 1) return $"あ、うん……。がんばった感は伝わった！";
        else if (score >= 0) return $"え……？不審者か何か……？";
        else return $"虚無～！";
    }

    private string GetScoreSecondLine()
    {
        if (score == 100) return "相づちのプロすぎん？また喋ろ！";
        else if (score >= 80) return $"いや、マジで感動した。トーク番組出てみたら？";
        else if (score >= 50) return $"ちょっとズレても、喋り方オシャレなのずるい。";
        else if (score >= 20) return $"勢いがある人は全然嫌いじゃないよ。";
        else if (score >= 1) return $"ただ、勢いだけでゴリ押すのは好きじゃないかも……。";
        else if (score >= 0) return $"私、帰ります……。";
        else return $"虚無～！";
    }

    private string GetScoreThirdLine()
    {
        return "またね～。";
    }

    IEnumerator fadeIn()
    {
        yield return new WaitForSeconds(1.5f);
        float duration = 1.0f;           // フェードにかける時間（秒）
        float elapsed = 0f;
        Color c = fadeInPanel.color;

        // アルファを 0 → 1 に補間
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / duration);
            fadeInPanel.color = c;
            yield return null;
        }

        // 完全に黒くなったらシーン遷移
        SceneManager.LoadScene("Title");
    }
}
