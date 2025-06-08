using System;
using System.Threading;
using System.Threading.Tasks;


using UnityEngine;
using UnityEngine.Events;

// Custom easing enum to replace DOTween's Ease

[RequireComponent(typeof(RectTransform))]
public class QHS_Popup : MonoBehaviour
{
    private RectTransform _panel;

    #region Move varible

    [SerializeField] private bool moveIn;
    [SerializeField] private Vector3 beginPosition;
    [SerializeField] private float durationMove;
    [SerializeField] private AnimationEase easeMove = AnimationEase.Linear;
    private Vector3 _targetPosition;
    private CancellationTokenSource _moveTokenSource;
    private bool _completeMove;

    [SerializeField] private bool moveOut;
    [SerializeField] private Vector3 hidePosition;
    [SerializeField] private float durationHideMove;
    [SerializeField] private AnimationEase easeHideMove = AnimationEase.Linear;
    private CancellationTokenSource _hideMoveTokenSource;
    private bool _completeHideMove;

    #endregion

    #region Scale varible

    [SerializeField] private bool scaleIn;
    [SerializeField] private Vector3 beginScale;
    [SerializeField] private float durationScale;
    [SerializeField] private AnimationEase easeScale = AnimationEase.Linear;
    private Vector3 _targetScale;
    private CancellationTokenSource _scaleTokenSource;
    private bool _completeScale;

    [SerializeField] private bool scaleOut;
    [SerializeField] private Vector3 hideScale;
    [SerializeField] private float durationHideScale;
    [SerializeField] private AnimationEase easeHideScale = AnimationEase.Linear;
    private CancellationTokenSource _hideScaleTokenSource;
    private bool _completeHideScale;

    #endregion

    #region Fade varible

    [SerializeField] private bool fadeIn;
    [SerializeField] private float durationFade;
    private CanvasGroup _canvasGroup;
    private bool _completeFade;
    private CancellationTokenSource _fadeTokenSource;

    [SerializeField] private bool fadeOut;
    [SerializeField] private float durationHideFade;
    private bool _completeHideFade;
    private CancellationTokenSource _hideFadeTokenSource;

    [SerializeField] private UnityEvent onCompleteShow;

    #endregion

    #region Easing Functions
    
    private float EvaluateEase(float t, AnimationEase ease)
    {
        switch (ease)
        {
            case AnimationEase.Linear: 
                return t;
            case AnimationEase.EaseInSine:
                return 1 - Mathf.Cos((t * Mathf.PI) / 2);
            case AnimationEase.EaseOutSine:
                return Mathf.Sin((t * Mathf.PI) / 2);
            case AnimationEase.EaseInOutSine:
                return -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
            case AnimationEase.EaseInQuad:
                return t * t;
            case AnimationEase.EaseOutQuad:
                return 1 - (1 - t) * (1 - t);
            case AnimationEase.EaseInOutQuad:
                return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
            case AnimationEase.EaseInCubic:
                return t * t * t;
            case AnimationEase.EaseOutCubic:
                return 1 - Mathf.Pow(1 - t, 3);
            case AnimationEase.EaseInOutCubic:
                return t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
            case AnimationEase.EaseInBack:
                float c1 = 1.70158f;
                return c1 * t * t * t - c1 * t * t;
            case AnimationEase.EaseOutBack:
                float c2 = 1.70158f;
                return 1 + c2 * Mathf.Pow(t - 1, 3) + c2 * Mathf.Pow(t - 1, 2);
            case AnimationEase.EaseInOutBack:
                float c3 = 1.70158f * 1.525f;
                return t < 0.5
                    ? (Mathf.Pow(2 * t, 2) * ((c3 + 1) * 2 * t - c3)) / 2
                    : (Mathf.Pow(2 * t - 2, 2) * ((c3 + 1) * (t * 2 - 2) + c3) + 2) / 2;
            case AnimationEase.EaseInElastic:
                if (t == 0) return 0;
                if (t == 1) return 1;
                return -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * ((2 * Mathf.PI) / 3));
            case AnimationEase.EaseOutElastic:
                if (t == 0) return 0;
                if (t == 1) return 1;
                return Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * ((2 * Mathf.PI) / 3)) + 1;
            case AnimationEase.EaseInOutElastic:
                if (t == 0) return 0;
                if (t == 1) return 1;
                if (t < 0.5f)
                    return -(Mathf.Pow(2, 20 * t - 10) * Mathf.Sin((20 * t - 11.125f) * ((2 * Mathf.PI) / 4.5f))) / 2;
                return (Mathf.Pow(2, -20 * t + 10) * Mathf.Sin((20 * t - 11.125f) * ((2 * Mathf.PI) / 4.5f))) / 2 + 1;
            case AnimationEase.EaseInBounce:
                return 1 - EaseOutBounce(1 - t);
            case AnimationEase.EaseOutBounce:
                return EaseOutBounce(t);
            case AnimationEase.EaseInOutBounce:
                return t < 0.5
                    ? (1 - EaseOutBounce(1 - 2 * t)) / 2
                    : (1 + EaseOutBounce(2 * t - 1)) / 2;
            default:
                return t;
        }
    }
    
    private float EaseOutBounce(float x)
    {
        float n1 = 7.5625f;
        float d1 = 2.75f;

        if (x < 1 / d1)
            return n1 * x * x;
        else if (x < 2 / d1)
            return n1 * (x -= 1.5f / d1) * x + 0.75f;
        else if (x < 2.5 / d1)
            return n1 * (x -= 2.25f / d1) * x + 0.9375f;
        else
            return n1 * (x -= 2.625f / d1) * x + 0.984375f;
    }
    
    #endregion

    private void Awake()
    {
        _panel = GetComponent<RectTransform>();
        _targetPosition = _panel.anchoredPosition;

        if (fadeIn || fadeOut)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        _targetScale = _panel.localScale;
    }

    private void OnEnable()
    {
        Execute();
    }

    private void Execute()
    {
        if (moveIn)
        {
            DoMove();
        }
        else
        {
            _completeMove = true;
        }
        
        if (scaleIn)
        {
            DoScale();
        }
        else
        {
            _completeScale = true;
        }
        
        if (fadeIn)
        {
            DoFade();
        }
        else
        {
            _completeFade = true;
        }
        
        TriggerEvent();
    }

    private async Task DoMoveAsync(Vector3 from, Vector3 to, float duration, AnimationEase ease, CancellationToken cancellationToken)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration && !cancellationToken.IsCancellationRequested)
        {
            float normalizedTime = elapsedTime / duration;
            float easedTime = EvaluateEase(normalizedTime, ease);
            _panel.anchoredPosition = Vector3.Lerp(from, to, easedTime);
            
            await Task.Yield();
            elapsedTime += Time.deltaTime;
        }
        
        if (!cancellationToken.IsCancellationRequested)
            _panel.anchoredPosition = to;
    }
    
    private async Task DoScaleAsync(Vector3 from, Vector3 to, float duration, AnimationEase ease, CancellationToken cancellationToken)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration && !cancellationToken.IsCancellationRequested)
        {
            float normalizedTime = elapsedTime / duration;
            float easedTime = EvaluateEase(normalizedTime, ease);
            _panel.localScale = Vector3.Lerp(from, to, easedTime);
            
            await Task.Yield();
            elapsedTime += Time.deltaTime;
        }
        
        if (!cancellationToken.IsCancellationRequested)
            _panel.localScale = to;
    }
    
    private async Task DoFadeAsync(float from, float to, float duration, AnimationEase ease, CancellationToken cancellationToken)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration && !cancellationToken.IsCancellationRequested)
        {
            float normalizedTime = elapsedTime / duration;
            float easedTime = EvaluateEase(normalizedTime, ease);
            _canvasGroup.alpha = Mathf.Lerp(from, to, easedTime);
            
            await Task.Yield();
            elapsedTime += Time.deltaTime;
        }
        
        if (!cancellationToken.IsCancellationRequested)
            _canvasGroup.alpha = to;
    }
    
    private async void DoMove()
    {
        _panel.anchoredPosition = beginPosition;
        _completeMove = false;
        
        _moveTokenSource?.Cancel();
        _moveTokenSource = new CancellationTokenSource();
        
        try
        {
            await DoMoveAsync(beginPosition, _targetPosition, durationMove, easeMove, _moveTokenSource.Token);
            if (!_moveTokenSource.Token.IsCancellationRequested)
                _completeMove = true;
        }
        catch (OperationCanceledException)
        {
            // Animation was canceled
        }
    }
    
    private async void DoHideMove()
    {
        _completeHideMove = false;
        
        _hideMoveTokenSource?.Cancel();
        _hideMoveTokenSource = new CancellationTokenSource();
        
        try
        {
            await DoMoveAsync(_panel.anchoredPosition, hidePosition, durationHideMove, easeHideMove, _hideMoveTokenSource.Token);
            if (!_hideMoveTokenSource.Token.IsCancellationRequested)
                _completeHideMove = true;
        }
        catch (OperationCanceledException)
        {
            // Animation was canceled
        }
    }
    
    private async void DoScale()
    {
        _panel.localScale = beginScale;
        _completeScale = false;
        
        _scaleTokenSource?.Cancel();
        _scaleTokenSource = new CancellationTokenSource();
        
        try
        {
            await DoScaleAsync(beginScale, _targetScale, durationScale, easeScale, _scaleTokenSource.Token);
            if (!_scaleTokenSource.Token.IsCancellationRequested)
                _completeScale = true;
        }
        catch (OperationCanceledException)
        {
            // Animation was canceled
        }
    }
    
    private async void DoHideScale()
    {
        _completeHideScale = false;
        
        _hideScaleTokenSource?.Cancel();
        _hideScaleTokenSource = new CancellationTokenSource();
        
        try
        {
            await DoScaleAsync(_panel.localScale, hideScale, durationHideScale, easeHideScale, _hideScaleTokenSource.Token);
            if (!_hideScaleTokenSource.Token.IsCancellationRequested)
                _completeHideScale = true;
        }
        catch (OperationCanceledException)
        {
            // Animation was canceled
        }
    }
    
    private async void DoFade()
    {
        _canvasGroup.alpha = 0;
        _completeFade = false;
        
        _fadeTokenSource?.Cancel();
        _fadeTokenSource = new CancellationTokenSource();
        
        try
        {
            await DoFadeAsync(0, 1, durationFade, AnimationEase.Linear, _fadeTokenSource.Token);
            if (!_fadeTokenSource.Token.IsCancellationRequested)
                _completeFade = true;
        }
        catch (OperationCanceledException)
        {
            // Animation was canceled
        }
    }
    
    private async void DoHideFade()
    {
        _completeHideFade = false;
        
        _hideFadeTokenSource?.Cancel();
        _hideFadeTokenSource = new CancellationTokenSource();
        
        try
        {
            await DoFadeAsync(1, 0, durationHideFade, AnimationEase.Linear, _hideFadeTokenSource.Token);
            if (!_hideFadeTokenSource.Token.IsCancellationRequested)
                _completeHideFade = true;
        }
        catch (OperationCanceledException)
        {
            // Animation was canceled
        }
    }

    private async void TriggerEvent()
    {
        while (!(_completeScale && _completeMove && _completeFade))
        {
            await Task.Yield();
        }
        
        onCompleteShow?.Invoke();
    }

    private async void OnHide(Action callback)
    {
        if (moveOut)
        {
            DoHideMove();
        }
        else
        {
            _completeHideMove = true;
        }
        
        if (scaleOut)
        {
            DoHideScale();
        }
        else
        {
            _completeHideScale = true;
        }
        
        if (fadeOut)
        {
            DoHideFade();
        }
        else
        {
            _completeHideFade = true;
        }
        
        while (!(_completeHideMove && _completeHideScale && _completeHideFade))
        {
            await Task.Yield();
        }
        
        callback?.Invoke();
    }

    private void OnDisable()
    {
        _moveTokenSource?.Cancel();
        _scaleTokenSource?.Cancel();
        _hideMoveTokenSource?.Cancel();
        _hideScaleTokenSource?.Cancel();
        _fadeTokenSource?.Cancel();
        _hideFadeTokenSource?.Cancel();
    }

#if UNITY_EDITOR
    private void TestShow()
    {
        if (!Application.isPlaying)
            return;
            
        _moveTokenSource?.Cancel();
        _scaleTokenSource?.Cancel();
        _fadeTokenSource?.Cancel();
        
        _panel.anchoredPosition = _targetPosition;
        _panel.localScale = _targetScale;
        
        if (fadeIn)
        {
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            _canvasGroup.alpha = 1;
        }
        
        Execute();
    }
    
    private void TestHide()
    {
        if (!Application.isPlaying)
            return;
            
        _hideMoveTokenSource?.Cancel();
        _hideScaleTokenSource?.Cancel();
        _hideFadeTokenSource?.Cancel();
        
        _panel.anchoredPosition = _targetPosition;
        _panel.localScale = _targetScale;
        
        if (fadeIn)
        {
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            _canvasGroup.alpha = 1;
        }

        OnHide(null);
    }
#endif
    private enum AnimationEase
    {
        Linear,
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce
    }
}

