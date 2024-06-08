using System.Collections;
using TMPro;
using UnityEngine;
using Utilities.Pool.Core;

[RequireComponent(typeof(TMP_Text))]
public class DamageNumberSystem : MonoBehaviour
{
    #region Main settings

    [Header("Main settings")]
    [SerializeField] private float lifeTime;
    private float currentLifeTime;

    [Header("Spawn configuration")]
    [SerializeField] private bool randomSpawn;
    [SerializeField] private float spawnRandomRange;

    [SerializeField] private bool isCritic;
    [SerializeField] private GameObject prefabCritic;

    #endregion

    #region Fade Settings

    [Space]
    [Header("Fade int")]
    [SerializeField] private bool enableFadeIn = true;
    [SerializeField] private float durationFadeIn = 0.2f;
    [SerializeField] private Vector3 offsetFadeIn = new Vector3(0.5f, 0);

    [Header("Fade animation curve")]
    [SerializeField] private AnimationCurve fadeAnimationCurve;
    [SerializeField] private AnimationCurve scaleAnimationCurve;

    [Space]
    [Header("Fade out")]
    [SerializeField] private bool enableFadeOut = true;
    [SerializeField] private float durationFadeOut = 0.2f;

    #endregion

    #region Face camera

    [Header("Face camera")]
    [SerializeField] private bool LookAtCamera;

    #endregion


    #region Cache variables

    //Camera
    private Camera _camera;
    private Transform cameraTransform;

    //FadeIn
    private Vector3 currentPosition;
    private float currentFadeInDuration;
    private Color color;

    //Scale
    private float simulatedScale;
    private Vector3 originalScale;
    private Vector3 finalPosition;
    private float numberScale;
    private float combinationScale;
    private float destructionScale;
    private float renderThroughWallsScale = 0.1f;
    private float lastScaleFactor = 1f;
    private bool firstFrameScale;

    #endregion

    private void OnEnable()
    {
        Invoke(nameof(DisableText), lifeTime);
        currentFadeInDuration = durationFadeIn;
        currentLifeTime = lifeTime;

        if (enableFadeOut)
        {
            StartCoroutine(FadeOutText());
        }
    }

    private void Awake()
    {
        color = GetComponent<TMP_Text>().color;
        _camera = FindAnyObjectByType<Camera>();
        cameraTransform = _camera.GetComponent<Transform>();
    }

    private void OnDisable()
    {
        color = new Color(color.r, color.g, color.b, 1);
        GetComponent<TMP_Text>().color = color;
    }

    private void Update()
    {
        if (enableFadeIn)
        {
            currentFadeInDuration += Time.deltaTime;
            transform.position = Vector3.Lerp(currentPosition, currentPosition + offsetFadeIn, fadeAnimationCurve.Evaluate(currentFadeInDuration));

            float evaluateValue = scaleAnimationCurve.Evaluate(currentFadeInDuration);
            var newScale = new Vector3(evaluateValue, evaluateValue, evaluateValue);

            transform.localScale = Vector3.Lerp(transform.localScale, newScale, currentLifeTime);
        }
        if (LookAtCamera)
        {
            transform.LookAt(transform.position + (transform.position - cameraTransform.position));
        }
    }

    public void Spawn(float newNumber, Vector3 position, GameObject parent)
    {
        var numberGameObject = SpawnNumber(newNumber, parent.transform.position, parent);
    }

    private GameObject SpawnNumber(float number, Vector3 position, GameObject parent)
    {
        GameObject numberGameObject;
        if (isCritic)
        {
            numberGameObject = PoolManager.SpawnObject(prefabCritic, position, Quaternion.identity);
        }
        else
        {
            numberGameObject = PoolManager.SpawnObject(gameObject, position, Quaternion.identity);
        }

        numberGameObject.GetComponent<RectTransform>().SetParent(parent.transform);
        DamageNumberSystem damageNumberPopup = numberGameObject.GetComponent<DamageNumberSystem>();
        damageNumberPopup.SetPosition(position, numberGameObject);


        TMP_Text textNumber = numberGameObject.GetComponent<TMP_Text>();
        textNumber.text = number.ToString();

        numberGameObject.SetActive(true);

        return numberGameObject;
    }

    private Vector3 SetPosition(Vector3 newPosition, GameObject numberGameobject)
    {
        if (randomSpawn)
        {
            Vector3 randomPosition = new Vector3(Random.Range((newPosition.x - spawnRandomRange), (newPosition.x + spawnRandomRange)),
                Random.Range((newPosition.y - spawnRandomRange), (newPosition.y + spawnRandomRange)), 
                Random.Range((newPosition.z - spawnRandomRange), (newPosition.z + spawnRandomRange)));

            numberGameobject.GetComponent<RectTransform>().localPosition = randomPosition;
            currentPosition = randomPosition;
            return randomPosition;
        }
        numberGameobject.GetComponent<RectTransform>().localPosition = newPosition;
        return newPosition;
    }

    private IEnumerator FadeOutText()
    {
        float elapsed = 0.0f;
        yield return new WaitForSeconds(Mathf.Abs(durationFadeOut - lifeTime));
        while (elapsed < durationFadeOut)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(color.a, 0, elapsed / durationFadeOut);
            color = new Color(color.r, color.g, color.b, alpha);
            GetComponent<TMP_Text>().color = color;
            yield return null;
        }
    }

    private void DisableText()
    {
        PoolManager.ReleaseObject(gameObject);
    }
}
