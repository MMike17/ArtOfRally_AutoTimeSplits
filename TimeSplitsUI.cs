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
        private static float fadeSpeed;
        private static bool isRunning;
        private static bool cancelToken;

        public static void Init(StageTimerManager timerManager)
        {
            runner = timerManager;

            Text timerText = Main.GetField<Text, StageTimerManager>(timerManager, "TimeDisplay", System.Reflection.BindingFlags.Instance);
            GameObject model = timerText.transform.parent.gameObject;

            GameObject bestPanel = GameObject.Instantiate(model, model.transform);
            RectTransform bestPanelRect = bestPanel.GetComponent<RectTransform>();
            bestPanelRect.anchoredPosition = new Vector2(0, -60); // TODO : Move Y pos to settings

            string bestTime = TimeSplitsManager.GetBestTime();
            bestPanel.GetComponentInChildren<Text>().text = bestTime;
            bestPanel.SetActive(bestTime != null);

            splitPanel = GameObject.Instantiate(model, model.transform);
            RectTransform splitPanelRect = splitPanel.GetComponent<RectTransform>();
            splitPanelRect.anchorMin = Vector2.one / 2;
            splitPanelRect.anchorMax = Vector2.one / 2;
            splitPanelRect.anchoredPosition = Vector3.zero;
            splitPanelGroup = splitPanel.AddComponent<CanvasGroup>();

            // TODO : Move these to settings
            float fadeDuration = 1;
            fadeSpeed = 1 / fadeDuration;
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

                splitPanelGroup.alpha = Mathf.MoveTowards(splitPanelGroup.alpha, target, fadeSpeed);
                yield return null;
            }

            splitPanelGroup.alpha = inOut ? 1 : 0;
        }
    }
}
