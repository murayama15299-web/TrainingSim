using UnityEngine;
using TMPro;
using System.Collections;

public class TrainingManager : MonoBehaviour
{
    //ステータス
    public int speed = 0;
    public int stamina = 0;
    public int power = 0;
    public int guts = 0;
    public int wisdom = 0;

    //体力
    public int energy = 100;

    //ターン
    public int turn = 1;
    public int maxTurn = 30;

    //UI
    public TextMeshProUGUI statusText;   //左上：ステータス
    public TextMeshProUGUI resultText;   //上中央：結果表示

    Coroutine resultRoutine;

    void Start()
    {
        UpdateStatusText();
        ShowResult("準備OK", Color.white, 40);
    }

    //ボタンから呼ばれる処理
    public void SpeedTraining() { DoTraining(ref speed, "スピード"); }
    public void StaminaTraining() { DoTraining(ref stamina, "スタミナ"); }
    public void PowerTraining() { DoTraining(ref power, "パワー"); }
    public void GutsTraining() { DoTraining(ref guts, "根性"); }
    public void WisdomTraining() { DoTraining(ref wisdom, "賢さ"); }

    //お休み（体力回復）
    public void Rest()
    {
        if (!CanAct()) return;

        energy += 30;
        if (energy > 100) energy = 100;

        ShowResult("お休み：体力が回復した！", new Color(0.2f, 1f, 0.4f), 44);

        NextTurn();
        UpdateStatusText();
    }

    //トレーニング共通処理
    void DoTraining(ref int statusValue, string label)
    {
        if (!CanAct()) return;

        if (!CanTrain())
        {
            ShowResult("体力が足りません", Color.gray, 36);
            return;
        }

        int rand = Random.Range(0, 100);

        if (rand < 20)
        {
            //大成功
            statusValue += 20;
            energy -= 5;
            ShowResult(label + "：大成功！", new Color(1f, 0.8f, 0.2f), 52);
        }
        else if (rand < 80)
        {
            //成功
            statusValue += 10;
            energy -= 10;
            ShowResult(label + "：成功", new Color(0.3f, 0.6f, 1f), 44);
        }
        else
        {
            //失敗
            energy -= 20;
            ShowResult(label + "：失敗…", Color.red, 44);
        }

        NextTurn();
        UpdateStatusText();
    }

    //行動可能チェック
    bool CanAct()
    {
        return turn <= maxTurn;
    }

    //ターン進行
    void NextTurn()
    {
        turn++;

        if (turn > maxTurn)
        {
            ShowFinalResult();
        }
    }

    //体力チェック
    bool CanTrain()
    {
        return energy > 0;
    }

    //ステータス表示更新
    void UpdateStatusText()
    {
        if (statusText == null) return;

        statusText.text =
            "ターン : " + turn + "/" + maxTurn + "\n\n" +
            "スピード : " + speed + "\n" +
            "スタミナ : " + stamina + "\n" +
            "パワー : " + power + "\n" +
            "根性 : " + guts + "\n" +
            "賢さ : " + wisdom + "\n\n" +
            "体力 : " + energy;
    }

    //結果表示
    void ShowResult(string message, Color color, float size)
    {
        if (resultText == null) return;

        if (resultRoutine != null)
            StopCoroutine(resultRoutine);

        resultText.text = message;
        resultText.color = new Color(color.r, color.g, color.b, 1f);
        resultText.fontSize = size;

        resultRoutine = StartCoroutine(FadeOutResult());
    }

    IEnumerator FadeOutResult()
    {
        yield return new WaitForSeconds(1.5f);

        float time = 0f;
        float duration = 0.5f;
        Color c = resultText.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            resultText.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        resultText.color = new Color(c.r, c.g, c.b, 0f);
    }

    //最終評価
    void ShowFinalResult()
    {
        int total = speed + stamina + power + guts + wisdom;

        string rank;
        if (total >= 500) rank = "S";
        else if (total >= 400) rank = "A";
        else if (total >= 300) rank = "B";
        else if (total >= 200) rank = "C";
        else rank = "D";

        if (resultRoutine != null)
            StopCoroutine(resultRoutine);

        resultText.text =
            "育成終了！\n" +
            "合計 : " + total + "\n" +
            "評価 : " + rank;

        resultText.color = Color.white;
        resultText.fontSize = 54;
    }
}
