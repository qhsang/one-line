using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class QHS_Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{

    private enum Ease
    {
        Linear,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        OutBack,
        InBack,
        InOutBack
    }
    #region Setting

    [SerializeField] private Ease easeOut = Ease.OutBack;
    [SerializeField] private Ease easeIn = Ease.InQuad;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Vector3 scale = new Vector3(1.25f, 1.25f, 1f);

    private Button _button;
    private Image[] _childImage;
    private Transform _transform;
    private Vector3 _initialScale, _targetScale;
    private bool _scaleUpDone = true, _scaleDownDone = true;
    private Coroutine _scaleUpCoroutine;
    private Coroutine _scaleDownCoroutine;

    [Space]
    [SerializeField] private bool canHole;
    [SerializeField] private float timeHolding;
    [SerializeField] private UnityEvent startHoldEvent;
    [SerializeField] private UnityEvent endHoldEvent, cancelHoldEvent;
    private bool _isHolding, _isCompleteHold;
    private const string HoldingKey = "holding";
    private CancellationTokenSource _holdingCts;
    [Space]
    #endregion

    #region Texture

    [SerializeField] private bool changeTexture;
    [SerializeField] private bool useInteractable;

    [SerializeField] private bool useChildImage;
    [SerializeField] private Sprite imageChange;
    [SerializeField] private Sprite[] imageA;
    [SerializeField] private Sprite[] imageB;
    [SerializeField] private Sprite[] imageDisable;

    private Sprite _normalImage;
    private bool _isChanging;
    [Space]
    #endregion

    #region Color

    [SerializeField] private bool changeColor;
    [SerializeField] private Color colorChange;
    [Space]
    private Color _normalColor;

    public bool IsHolding { get => _isHolding; set => _isHolding = value; }
    public bool CanHole { get => canHole; set => canHole = value; }

    #endregion

    private void Awake()
    {
        _button = GetComponent<Button>();
        if ((changeTexture && useChildImage) || useInteractable)
        {
            _childImage = GetComponentsInChildren<Image>().Skip(1).ToArray();
            //int i = 0;
            //foreach (var img in _childImage)
            //{
            //    //img.sprite = imageA[i];
            //    i++;
            //}
        }
        if (changeTexture && !useChildImage)
        {
            _normalImage = _button.image.sprite;
        }

        if (changeColor)
        {
            _normalColor = _button.image.color;
        }
        
    }
    void Start()
    {
        _transform = _button.transform;
        _transform.localScale = Vector3.one;
        _initialScale = _transform.localScale;
        _targetScale = MultipleVector(_initialScale, scale);

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_button.interactable)
            return;

        StartHolding();

        if (_scaleDownCoroutine != null)
            StopCoroutine(_scaleDownCoroutine);
        _scaleUpCoroutine = StartCoroutine(ScaleUp());
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_button.interactable)
            return;

        if (!_isCompleteHold)
            CancelHoldEvent();

        if (_scaleUpCoroutine != null)
            StopCoroutine(_scaleUpCoroutine);
        _scaleDownCoroutine = StartCoroutine(ScaleDown());
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (!_button.interactable)
            return;
        if (changeTexture)
        {
            if (useChildImage)
            {
                for (int i = 0; i < _childImage.Length; i++)
                {
                    _childImage[i].sprite = _isChanging ? imageB[i] : imageA[i];
                }
            }
            else
            {
                _button.image.sprite = _isChanging ? imageChange : _normalImage;
            }
        }
        if (changeColor)
        {
            if (useChildImage)
            {
                foreach (var img in _childImage)
                {
                    img.color = _isChanging ? colorChange : _normalColor;
                }
            }
            else
            {
                _button.image.color = _isChanging ? colorChange : _normalColor;

            }
        }
        _isChanging = !_isChanging;

    }

    private async Task StartHoldingAsync()
    {
        if (canHole)
        {
            // Cancel any previous holding operation if still running
            if (_holdingCts != null)
            {
                _holdingCts.Cancel();
                _holdingCts.Dispose();
            }
            
            // Create new cancellation token source
            _holdingCts = new CancellationTokenSource();
            
            _isHolding = true;
            _isCompleteHold = false;
            startHoldEvent?.Invoke();
            
            try
            {
                // Wait for the specified time
                await Task.Delay(Mathf.RoundToInt(timeHolding * 1000), _holdingCts.Token);
                
                // If we get here without cancellation, complete the hold
                _isCompleteHold = true;
                EndHolding();
            }
            catch (TaskCanceledException)
            {
                // Task was canceled, nothing to do here
            }
            catch (Exception ex)
            {
                // Log any other exceptions but don't let them crash the application
                Debug.LogError($"Exception in holding task: {ex.Message}");
            }
        }
    }
    
    private void StartHolding()
    {
        if (canHole)
        {
            StartHoldingAsync();
        }
    }

    private void EndHolding()
    {
        if (canHole)
        {
            _isHolding = false;
            endHoldEvent?.Invoke();
        }
    }

    private void CancelHoldEvent()
    {
        if (canHole)
        {
            _isHolding = false;
            cancelHoldEvent?.Invoke();
            
            // Cancel the task if it's running
            if (_holdingCts != null)
            {
                _holdingCts.Cancel();
                _holdingCts.Dispose();
                _holdingCts = null;
            }
        }
    }


    public void SetInteractable(bool value)
    {
        _button.interactable = value;
        if (useInteractable)
        {
            for (int i = 0; i < _childImage.Length; i++)
            {
                _childImage[i].sprite = _button.interactable ? imageA[i] : imageDisable[i];
            }
        }
    }

    // Implementation of various easing functions to replace DOTween
    private float EaseValue(float t, Ease easeType)
    {
        switch (easeType)
        {
            case Ease.Linear:
                return t;
            case Ease.InQuad:
                return t * t;
            case Ease.OutQuad:
                return t * (2 - t);
            case Ease.InOutQuad:
                return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
            case Ease.InCubic:
                return t * t * t;
            case Ease.OutCubic:
                return 1 + (t - 1) * (t - 1) * (t - 1);
            case Ease.InOutCubic:
                return t < 0.5f ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
            case Ease.OutBack:
                {
                    const float c1 = 1.70158f;
                    const float c3 = c1 + 1;
                    return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
                }
            case Ease.InBack:
                {
                    const float c1 = 1.70158f;
                    const float c3 = c1 + 1;
                    return c3 * t * t * t - c1 * t * t;
                }
            case Ease.InOutBack:
                {
                    const float c1 = 1.70158f;
                    const float c2 = c1 * 1.525f;
                    return t < 0.5f
                        ? (Mathf.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2)) / 2
                        : (Mathf.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
                }
            default:
                return t;
        }
    }

    private IEnumerator ScaleUp()
    {
        _scaleUpDone = false;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float normalizedTime = elapsedTime / duration;
            float easedTime = EaseValue(normalizedTime, easeOut);
            _transform.localScale = Vector3.Lerp(_initialScale, _targetScale, easedTime);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        _transform.localScale = _targetScale;
        _scaleUpDone = true;
    }

    private IEnumerator ScaleDown()
    {
        _scaleDownDone = false;
        float elapsedTime = 0f;
        float scaleDownDuration = duration / 2;
        while (elapsedTime < scaleDownDuration)
        {
            float normalizedTime = elapsedTime / scaleDownDuration;
            float easedTime = EaseValue(normalizedTime, easeIn);
            _transform.localScale = Vector3.Lerp(_targetScale, _initialScale, easedTime);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        _transform.localScale = _initialScale;
        _scaleDownDone = true;
    }

    private void OnDisable()
    {
        // Clean up the cancellation token source if it exists
        if (_holdingCts != null)
        {
            _holdingCts.Cancel();
            _holdingCts.Dispose();
            _holdingCts = null;
        }
        if(_transform == null)
            return;
        _transform.localScale = _initialScale;
    }

#if UNITY_EDITOR
    private void UpdateNewValue()
    {
        if (!Application.isPlaying)
            return;
        _targetScale = MultipleVector(_initialScale, scale);
    }
#endif

    private Vector3 MultipleVector(Vector3 vector, Vector3 multiplier)
    {
        return new Vector3(vector.x * multiplier.x, vector.y * multiplier.y, vector.z * multiplier.z);
    }

}
