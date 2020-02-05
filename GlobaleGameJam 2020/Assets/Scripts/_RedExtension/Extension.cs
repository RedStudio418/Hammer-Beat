using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public static class CoroutineExtension
{
    public static IEnumerator MoveTo(this Transform @this, Vector3 destination, float duration)
    {
        var startPosition = @this.position;
        var timer = new SpecialCountDown(duration);

        while(!timer.isDone)
        {
            @this.position = Vector3.Lerp(startPosition, destination, timer.Progress);
            yield return null;
        }
        @this.position = destination;

        yield break;
    }

    public static IEnumerator SpeedMoveAgent(this NavMeshAgent @this, Vector3 raycastPoint, float speedFactor = 0.6f, Func<bool> additionnalEndCondition=null)
    {
        var startPosition = @this.transform.position;
        SpecialCountDown timer = new SpecialCountDown(speedFactor);
        while(!timer.isDone)
        {

            @this.transform.Translate(Vector3.Lerp(startPosition, raycastPoint, timer.Progress) - @this.transform.position);

            yield return null;

            //if (additionnalEndCondition != null && additionnalEndCondition.Invoke()) break;
        }
        @this.transform.position = raycastPoint;

        yield break;
    }

    public static IEnumerator PlayAndWait(this Animation @this, string name, bool supportLooping = false)
    {
        if (@this.GetClip(name).isLooping && !supportLooping)
        {
            Debug.LogError("You must specify support explicite looping");
            yield break;
        }

        @this.Play(name);
        yield return new WaitWhile(() => @this.IsPlaying(name));
    }
    public static IEnumerator WaitFrameAnd(Action next, int frameToWait = 1)
    {
        for (var i = 0; i < frameToWait; i++) yield return new WaitForEndOfFrame();
        next?.Invoke();
        yield break;
    }
    public static IEnumerator WaitSecondsAnd(float waitDuration, Action next, bool useUnscaledTime=false)
    {
        if (useUnscaledTime) yield return new WaitForSecondsRealtime(waitDuration);
        else yield return new WaitForSeconds(waitDuration);

        next?.Invoke();
        yield break;
    }
    public static IEnumerator SkipFramesAnd(IEnumerator next, int frameCount=1)
    {
        for(var i=0; i<frameCount; i++) yield return null;

        yield return next;
    }
    public static IEnumerator SkipFrames(int frameCount)
    {
        for (var i = 0; i < frameCount; i++) yield return null;
        yield break;
    }

    public static IEnumerator PlayAndWait(this Animation @this)
    {
        @this.Play();
        yield return null;
        yield return new WaitWhile(() => @this.isPlaying);
    }
    public static IEnumerator PlayAndWait(this Animation @this, AnimationClip clip) => @this.PlayAndWait(clip.name);
    public static IEnumerator PlayAndWait(this Animation @this, string name)
    {
        @this.Play(name);
        yield return new WaitWhile(() => @this.IsPlaying(name));
    }

    public static IEnumerator Play(this Animation animation, string clipName, bool useTimeScale, Action onComplete)
    {
        //Debug.Log(&quot; Overwritten Play animation, useTimeScale ? &quot; +useTimeScale);
        //We Don't want to use timeScale, so we have to animate by frame..
        if (!useTimeScale)
        {
            //Debug.Log(&quot; Started this animation!(&quot; +clipName + &quot; ) &quot;);
            AnimationState _currState = animation[clipName];
            bool isPlaying = true;
            float _progressTime = 0F;
            float _timeAtLastFrame = 0F;
            float _timeAtCurrentFrame = 0F;
            float deltaTime = 0F;


            animation.Play(clipName);

            _timeAtLastFrame = Time.realtimeSinceStartup;
            while (isPlaying)
            {
                _timeAtCurrentFrame = Time.realtimeSinceStartup;
                deltaTime = _timeAtCurrentFrame - _timeAtLastFrame;
                _timeAtLastFrame = _timeAtCurrentFrame;

                _progressTime += deltaTime;
                _currState.normalizedTime = _progressTime / _currState.length;
                animation.Sample();

                //Debug.Log(_progressTime);
                if (_progressTime >= _currState.length)
                {
                    //Debug.Log(&quot;Bam! Done animating&quot;);
                    if (_currState.wrapMode != WrapMode.Loop)
                    {
                        //Debug.Log(&quot;Animation is not a loop anim, kill it.&quot;);
                        //_currState.enabled = false;
                        isPlaying = false;
                    }
                    else
                    {
                        //Debug.Log(&quot;Loop anim, continue.&quot;);
                        _progressTime = 0.0f;
                    }
                }

                yield return new WaitForEndOfFrame();
            }
            yield return null;

            //Debug.Log(&quot; Start onComplete&quot;);
            onComplete?.Invoke();
        }
        else
        {
            animation.Play(clipName);
        }
    }
}

public static class Extension {

    private readonly static Func<int, int, int> _classicUnityRandom = 
        (min, max) => UnityEngine.Random.Range(min, max);
    private readonly static Func<System.Random, int, int, int> _systemRandom =
        (random, min, max) => random.Next(min, max);

    public static T PickRandom<T>(this List<T> @this)                                   => PickRandom(@this, _classicUnityRandom);
    public static T PickRandom<T>(this List<T> @this, System.Random random)             => PickRandom(@this, (min, max) => _systemRandom.Invoke(random, min, max));
    public static T PickRandom<T>(this List<T> @this, Func<int, int, int> strategy)     => @this[strategy.Invoke(0, @this.Count)];
    public static T PickRandom<T>(this T[] @this)                                       => @this[_classicUnityRandom(0, @this.Length    )];
    public static float PickRandom(this Vector2 @this)                                  => UnityEngine.Random.Range(@this.x, @this.y);
    public static T PickRandom<T>(this (IEnumerable<T>, int) @this) => @this.Item1.Skip(_classicUnityRandom(0, @this.Item2)).First();

    public static float GetPercentage(float min, float max, float current)
        => (current - min) / (max - min);

    public static string ToFlatString<T>(this IEnumerable<T> @this, bool lineReturn = false)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var el in @this)
        {
            sb.Append(el.ToString());
            sb.Append(" | ");
            if (lineReturn) sb.Append("\n");
        }
        return sb.ToString();
    }

    public static IEnumerable<Transform> ToLinq(this Transform @this)
    {
        return @this.Cast<Transform>();
    }
    
    /// <summary>
    /// Checks if a GameObject has been destroyed.
    /// Source : http://answers.unity.com/answers/1001265/view.html
    /// </summary>
    /// <param name="gameObject">GameObject reference to check for destructedness</param>
    /// <returns>If the game object has been marked as destroyed by UnityEngine</returns>
    public static bool IsDestroyed(this GameObject gameObject)
        => gameObject == null && !ReferenceEquals(gameObject, null);
    public static bool IsDestroyed(this UnityEngine.Object @this)
        => @this == null && !ReferenceEquals(@this, null);

    /// <summary>
    /// /!\ Not a functionnal technique
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> @this, System.Action<T> callback)
    {
        foreach (var el in @this) callback.Invoke(el);
        return @this;
    }

    public static T LastElement<T>(this List<T> @this) => @this[@this.Count - 1];

    public static IEnumerable<Scene> ActiveScenes
    {
        get
        {
            for (var i=0; i< SceneManager.sceneCount; i++)
            {
                yield return SceneManager.GetSceneAt(i);
            }
            yield break;
        }
    }

    public static void SafeDestroy(this UnityEngine.GameObject @this)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) GameObject.DestroyImmediate(@this);
        else
#endif
        {
            GameObject.Destroy(@this);
        }
    }

    public static bool MoveNext<T>(this IEnumerator<T> @this, out T current)
    {
        var movedNext = @this.MoveNext();
        current = movedNext ? @this.Current : default;
        return movedNext;
    }

    public static T NextCurrent<T>(this IEnumerator<T> @this)
    {
        if(!@this.MoveNext()) { throw new Exception("Enumerator returned false to MoveNext. You must check moveNext statut"); }
        return @this.Current;
    }

    public static void Shuffle<T>(this List<T> @this, Func<int,int,int> randomProvider=null)
    {
        if (randomProvider == null) randomProvider = _classicUnityRandom;
        int n = @this.Count;
        while (n > 1)
        {
            n--;
            int k = randomProvider(0, n + 1);
            T value = @this[k];
            @this[k] = @this[n];
            @this[n] = value;
        }
    }

    public static bool IsBetweenFloats(this float current, float a, float b)
    {
        if (a > b) return current <= a && current >= b;
        else return current >= a && current <= b;
    }

}
