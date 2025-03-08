using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// TODO : Add splits color

namespace AutoTimeSplits
{
    static class TimeSplitsUI
    {
        private static StageTimerManager runner;
        private static GameObject splitPanel;
        private static CanvasGroup splitPanelGroup;
        private static Text splitText;
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
            RectTransform bestPanelRect = bestPanel.GetComponent<RectTransform>();
            bestPanelRect.anchoredPosition = new Vector2(0, -60); // TODO : Move Y pos to settings

            string bestTime = TimeSplitsManager.GetBestTime();
            bestPanel.GetComponentInChildren<Text>().text = bestTime;
            bestPanel.SetActive(bestTime != null);

            // time split panel
            splitPanel = GameObject.Instantiate(model, model.transform.parent.parent);
            splitPanel.name = "Time split";

            // TODO : Add position settings
            RectTransform splitPanelRect = splitPanel.GetComponent<RectTransform>();
            splitPanelRect.anchorMin = Vector2.one * 0.48f;
            splitPanelRect.anchorMax = Vector2.one * 0.52f;
            splitPanelRect.anchoredPosition = Vector3.zero;

            splitPanelGroup = splitPanel.AddComponent<CanvasGroup>();
            splitPanelGroup.alpha = 0;

            splitText = splitPanel.GetComponentInChildren<Text>();
            splitText.alignment = TextAnchor.MiddleCenter;
            splitText.fontStyle = FontStyle.Bold;

            RectTransform splitTextRect = splitText.GetComponent<RectTransform>();
            splitTextRect.anchorMin = Vector2.one * 0.1f;
            splitTextRect.anchorMax = Vector2.one * 0.9f;
            splitTextRect.anchoredPosition = Vector3.zero;

            // TODO : Move these to settings
            float fadeDuration = 1;
            fadeSpeed = 1 / fadeDuration;

            Main.Log("Spawned time splits UI");
        }

        public static void ShowSplits(string timeSplit) => runner.StartCoroutine(ResetAnim(timeSplit));

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

            // TODO : Move these to settings
            float idleDuration = 3;

            yield return FadePanel(true);

            if (cancelToken)
            {
                cancelToken = false;
                yield break;
            }

            yield return new WaitForSeconds(idleDuration);

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
