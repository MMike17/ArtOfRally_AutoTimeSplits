using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AutoTimeSplits
{
    static class TimeSplitsUI
    {
        private static StageTimerManager runner;
        private static GameObject splitPanel;
        private static CanvasGroup splitPanelGroup;
        private static Text splitText;
        private static Text bestText;
        private static RectTransform bestPanelRect;
        private static RectTransform splitPanelRect;
        private static float fadeSpeed;
        private static bool isRunning;
        private static bool cancelToken;

        public static void Init(StageTimerManager timerManager)
        {
            runner = timerManager;

            Text timerText = Main.GetField<Text, StageTimerManager>(timerManager, "TimeDisplay", System.Reflection.BindingFlags.Instance);
            GameObject model = timerText.transform.parent.gameObject;

            // best time panel
            GameObject bestPanel = GameObject.Instantiate(model, model.transform.parent);
            bestPanel.name = "Best time";
            bestPanelRect = bestPanel.GetComponent<RectTransform>();
            SetBestHeight(Main.settings.bestTimePanelHeight);

            string bestTime = TimeSplitsManager.GetBestTime();
            bestText = bestPanel.GetComponentInChildren<Text>();
            bestText.text = bestTime;
            bestPanel.SetActive(bestTime != null);

            // time split panel
            splitPanel = GameObject.Instantiate(model, model.transform.parent.parent);
            splitPanel.name = "Time split";

            splitPanelRect = splitPanel.GetComponent<RectTransform>();
            splitPanelRect.anchorMin = Vector2.one * 0.48f;
            splitPanelRect.anchorMax = Vector2.one * 0.52f;

            SetSplitsHeight(Main.settings.timeSplitsPanelHeight);
            SetSplitsSize(Main.settings.timeSplitsPanelSize);

            splitPanelGroup = splitPanel.AddComponent<CanvasGroup>();
            splitPanelGroup.alpha = 0;

            splitText = splitPanel.GetComponentInChildren<Text>();
            splitText.alignment = TextAnchor.MiddleCenter;
            splitText.fontStyle = FontStyle.Bold;

            RectTransform splitTextRect = splitText.GetComponent<RectTransform>();
            splitTextRect.anchorMin = Vector2.one * 0.1f;
            splitTextRect.anchorMax = Vector2.one * 0.9f;
            splitTextRect.anchoredPosition = Vector2.zero;

            fadeSpeed = 1 / Main.settings.timeSplitsFadeDuration;
            Main.Log("Spawned time splits UI");
        }

        public static void SetBestHeight(int height)
        {
            if (bestPanelRect != null)
                bestPanelRect.anchoredPosition = new Vector2(0, height);
        }

        public static void SetSplitsHeight(int height)
        {
            if (splitPanelRect != null)
                splitPanelRect.anchoredPosition = new Vector2(0, height);
        }

        public static void SetSplitsSize(float size)
        {
            if (splitPanelRect != null)
                splitPanelRect.localScale = Vector3.one * size;
        }

        public static void ShowSplits(string timeSplit) => runner.StartCoroutine(ResetAnim(timeSplit));

        public static void UpdateBest()
        {
            if (bestText != null)
                bestText.text = TimeSplitsManager.GetBestTime();
        }

        private static IEnumerator ResetAnim(string timeSplit)
        {
            if (isRunning)
            {
                cancelToken = true;
                yield return new WaitUntil(() => !cancelToken);
            }

            isRunning = true;
            yield return ShowSplit(timeSplit);
            isRunning = false;
        }

        private static IEnumerator ShowSplit(string timeSplit)
        {
            splitText.text = timeSplit;
            splitText.color = Settings.GetColor(timeSplit.Contains("-") ? Main.settings.goodSplitColor : Main.settings.badSplitColor);

            yield return FadePanel(true);

            if (cancelToken)
            {
                cancelToken = false;
                yield break;
            }

            yield return new WaitForSeconds(Main.settings.timeSplitsIdleDuration);

            if (cancelToken)
            {
                cancelToken = false;
                yield break;
            }

            yield return FadePanel(false);
        }

        private static IEnumerator FadePanel(bool inOut)
        {
            float target = inOut ? 1 : 0;

            while (splitPanelGroup.alpha != target)
            {
                if (cancelToken)
                {
                    cancelToken = false;
                    yield break;
                }

                splitPanelGroup.alpha = Mathf.MoveTowards(splitPanelGroup.alpha, target, fadeSpeed * Time.deltaTime);
                yield return null;
            }

            splitPanelGroup.alpha = inOut ? 1 : 0;
        }
    }
}
