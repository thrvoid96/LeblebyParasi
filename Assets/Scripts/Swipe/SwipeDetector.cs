using System;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

    [SerializeField]
    private bool detectSwipeOnlyAfterRelease = false;

    [SerializeField]
    private float minDistanceForSwipe = 20f;

    private bool touchDisabled;

    public static event Action<SwipeData> OnSwipe = delegate { };

    private void OnEnable()
    {
        EventManager.StartListening(Events.CoinTravelling, disableTouch);
        EventManager.StartListening(Events.LevelWon, disableTouch);
        EventManager.StartListening(Events.LevelLost, disableTouch);        
        EventManager.StartListening(Events.CoinStopped, enableTouch);
    }


    private void OnDisable()
    {
        EventManager.StopListening(Events.CoinTravelling, disableTouch);
        EventManager.StopListening(Events.LevelWon, disableTouch);
        EventManager.StopListening(Events.LevelLost, disableTouch);        
        EventManager.StopListening(Events.CoinStopped, enableTouch);
    }

    private void disableTouch(EventParam param)
    {
        touchDisabled = true;
    }
    private void enableTouch(EventParam param)
    {
        touchDisabled = false;
    }

    private void Update()
    {
        if (!touchDisabled)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    fingerUpPosition = touch.position;
                    fingerDownPosition = touch.position;

                    DetectSwipe();

                    var eventParam = new EventParam
                    {
                        fingerDownPos = fingerDownPosition,
                    };

                    EventManager.TriggerEvent(Events.ScreenTouched, eventParam);

                }

                if (!detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
                {
                    fingerDownPosition = touch.position;
                    DetectSwipe();
                }

                if (!detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Stationary)
                {
                    fingerDownPosition = touch.position;
                    fingerUpPosition = fingerDownPosition;
                    DetectSwipe();
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    fingerDownPosition = touch.position;
                    DetectSwipe();

                    var eventParam = new EventParam
                    {
                        fingerDownPos = fingerDownPosition,
                    };

                    EventManager.TriggerEvent(Events.ScreenUnTouched, eventParam);

                    fingerUpPosition = fingerDownPosition;
                }
            }
        }
    }

    private void DetectSwipe()
    {
        if (SwipeDistanceCheckMet())
        {
            if (IsVerticalSwipe())
            {
                var direction = fingerDownPosition.y - fingerUpPosition.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                SendSwipe(direction);
            }
            else
            {
                var direction = fingerDownPosition.x - fingerUpPosition.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;

                SendSwipe(direction);
            }
        }
    }

    private bool IsVerticalSwipe()
    {
        return VerticalMovementDistance() > HorizontalMovementDistance();
    }

    private bool SwipeDistanceCheckMet()
    {
        return VerticalMovementDistance() >= minDistanceForSwipe || HorizontalMovementDistance() >= minDistanceForSwipe;
    }

    private float VerticalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y);
    }

    private float HorizontalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x);
    }

    private void SendSwipe(SwipeDirection direction)
    {
        SwipeData swipeData = new SwipeData()
        {
            Direction = direction,
            fingerDownPos = fingerDownPosition,
            fingerUpPos = fingerUpPosition,
            DistanceX = Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x),
            DirectionVector = fingerDownPosition - fingerUpPosition
        };
        OnSwipe(swipeData);
    }
}

public struct SwipeData
{
    public Vector2 fingerDownPos;
    public Vector2 fingerUpPos;
    public Vector2 DirectionVector;
    public float DistanceX;
    public SwipeDirection Direction;
}

public enum SwipeDirection
{
    Up,
    Down,
    Left,
    Right
}