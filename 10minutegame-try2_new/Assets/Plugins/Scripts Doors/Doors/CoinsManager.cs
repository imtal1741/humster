using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;

public class CoinsManager : MonoBehaviour
{
    //References
    [Header("Audio references")]
    public AudioEffects audioEffects;

    [Header("UI references")]
    [SerializeField] TMP_Text coinUIText;
    [SerializeField] bool simpleText;
    [SerializeField] string nameIcon = "coin";
    [SerializeField] bool IconLeftSide = true;
    [SerializeField] GameObject coinsRoot;
    RectTransform canvas;
    [SerializeField] Camera cam;
    [SerializeField] GameObject animatedCoinPrefab;
    [SerializeField] RectTransform target;

    [Header("UI Key references")]
    [SerializeField] GameObject keyRoot;
    [SerializeField] RectTransform animatedKeyObject;
    [SerializeField] RectTransform targetKey;

    [Space]
    [Header("Available coins : (coins to pool)")]
    [SerializeField] int maxCoins;
    Queue<RectTransform> coinsQueue = new Queue<RectTransform>();


    [Space]
    [Header("Animation settings")]
    [SerializeField] [Range(0.5f, 0.9f)] float minAnimDuration;
    [SerializeField] [Range(0.9f, 2f)] float maxAnimDuration;

    [SerializeField] AnimationCurve alphaCurve, sizeCurve;
    [SerializeField] float spread;

    Vector2 targetPosition;


    private int _c = 0;

    int countNow;
    int result;


    public int Coins
    {
        get { return _c; }
        set
        {
            _c = value;

            countNow++;
            if (countNow == maxCoins)
                Coins = result;
            //update UI text whenever "Coins" variable is changed
            if (simpleText)
            {
                coinUIText.text = Coins.ToString();
            }
            else
            {
                if (IconLeftSide)
                {
                    coinUIText.text = "<sprite name=\"" + nameIcon + "\">" + Coins.ToString();
                }
                else
                {
                    coinUIText.text = Coins.ToString() + "<sprite name=\"" + nameIcon + "\">";
                }
            }
        }
    }

    void Awake()
    {
        targetPosition = target.anchoredPosition;
        canvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>();

        //prepare pool
        PrepareCoins();
    }

    void PrepareCoins()
    {
        GameObject coin;
        for (int i = 0; i < maxCoins; i++)
        {
            coin = Instantiate(animatedCoinPrefab);
            coin.transform.SetParent(coinsRoot.transform);
            coin.gameObject.SetActive(false);
            coinsQueue.Enqueue(coin.GetComponent<RectTransform>());
        }
    }

    void Animate(Vector3 collectedCoinPosition, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            //check if there's coins in the pool
            if (coinsQueue.Count > 0)
            {
                //extract a coin from the pool
                RectTransform coin = coinsQueue.Dequeue();
                coin.gameObject.SetActive(true);

                //move coin to the collected coin pos
                Vector2 viewportPosition = cam.WorldToViewportPoint(collectedCoinPosition);
                Vector2 finalPosition = new Vector2(viewportPosition.x * canvas.rect.width - canvas.rect.width, viewportPosition.y * canvas.rect.height - canvas.rect.height);
                coin.anchoredPosition = finalPosition + new Vector2(Random.Range(-spread, spread), Random.Range(-spread, spread));

                //animate coin to target position
                float duration = Random.Range(minAnimDuration, maxAnimDuration);

                coin.GetComponent<Image>().color = new Color32(255, 255, 225, 0);
                coin.localScale = Vector2.zero;
                DOTween.To(() => new Color32(255, 255, 225, 255), x => coin.GetComponent<Image>().color = x, new Color32(255, 255, 225, 0), duration).SetEase(alphaCurve);
                DOTween.To(()=> new Vector2(1, 1), x => coin.localScale = x, Vector2.zero, duration).SetEase(sizeCurve);
                coin.DOAnchorPos(targetPosition, duration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => {
                    //executes whenever coin reach target position
                    coin.gameObject.SetActive(false);
                    coinsQueue.Enqueue(coin);
                    
                    Coins += amount / maxCoins;
                });
            }
        }
    }

    void AnimateOne(Vector3 ContactObjectPosition, RectTransform ContactObjectUI, RectTransform targetPoint, GameObject UI_FinalObject)
    {
        ContactObjectUI.gameObject.SetActive(true);


        Vector2 viewportPosition = cam.WorldToViewportPoint(ContactObjectPosition);
        Vector2 finalPosition = new Vector2(viewportPosition.x * canvas.rect.width - (canvas.rect.width / 2), viewportPosition.y * canvas.rect.height - (canvas.rect.height / 2));
        ContactObjectUI.anchoredPosition = finalPosition;


        float duration = 0.75f;

        ContactObjectUI.localScale = Vector2.zero;
        DOTween.To(() => new Vector2(1, 1), x => ContactObjectUI.localScale = x, Vector2.zero, duration).SetEase(sizeCurve);
        ContactObjectUI.DOAnchorPos(targetPoint.anchoredPosition, duration)
        .SetEase(Ease.InOutQuad)
        .OnComplete(() => {
            //executes whenever coin reach target position
            ContactObjectUI.gameObject.SetActive(false);
            UI_FinalObject.SetActive(true);
        });
    }

    public void AddCoinsSimple(Transform collectedCoin, int amount)
    {
        if (audioEffects)
            audioEffects.PlaySound(audioEffects.coin);

        string resultString = Regex.Match(coinUIText.text, @"\d+").Value;
        _c = int.Parse(resultString);

        countNow = 0;
        result = _c + amount;

        Animate(collectedCoin.position, amount);
    }

    public void AddCoins(Transform collectedCoin, int amount)
    {
        StartCoroutine(AddCoinsIE(collectedCoin, amount));
    }

    public IEnumerator AddCoinsIE(Transform collectedCoin, int amount)
    {
        yield return new WaitForSeconds(0.3f);

        if (audioEffects)
            audioEffects.PlaySound(audioEffects.coin);

        Animate(collectedCoin.position, amount);
        Destroy(collectedCoin.gameObject);
    }


    public void AddKey(Transform ContactObject, GameObject UI_FinalObject)
    {
        StartCoroutine(AddOneIE(ContactObject, animatedKeyObject, targetKey, UI_FinalObject, true));
    }

    public IEnumerator AddOneIE(Transform ContactObject, RectTransform ContactObjectUI, RectTransform targetPoint, GameObject UI_FinalObject, bool isKey)
    {
        yield return new WaitForSeconds(0.2f);

        if (isKey && audioEffects)
            audioEffects.PlaySound(audioEffects.key);

        AnimateOne(ContactObject.position, ContactObjectUI, targetPoint, UI_FinalObject);
        Destroy(ContactObject.gameObject);
    }
}