using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;

public class NotificationUI : MonoBehaviour
{
    public static NotificationUI instance;

    [Header("Prefabs e Configuração")]
    public GameObject notificationPrefab;
    public Transform notificationParent;

    [Header("Animação")]
    public float showTime = 2f;
    public float moveUp = 30f;
    public float fadeDuration = 0.5f;

    private List<RectTransform> notifications = new List<RectTransform>();

    private void Awake()
    {
        instance = this;
    }

    public static void Show(string msg)
    {
        instance.CreateNotification(msg);
    }

    void CreateNotification(string msg)
    {
        GameObject notif = Instantiate(notificationPrefab, notificationParent);
        RectTransform rect = notif.GetComponent<RectTransform>();
        TMP_Text text = notif.GetComponent<TMP_Text>();

        text.text = msg;
        text.alpha = 0f; // invisível no começo

        notifications.Add(rect);

        // Mover as antigas para cima
        foreach (var n in notifications)
            n.DOAnchorPosY(n.anchoredPosition.y + moveUp, 0.3f);

        // Animação DOTween
        Sequence seq = DOTween.Sequence();
        seq.Append(text.DOFade(1f, fadeDuration));
        seq.AppendInterval(showTime);
        seq.Append(text.DOFade(0f, fadeDuration));
        seq.AppendCallback(() =>
        {
            notifications.Remove(rect);
            Destroy(notif);
        });
    }
}
