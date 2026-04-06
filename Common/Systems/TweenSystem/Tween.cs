using EverlastingOverhaul.Common.Graphics.AnimationSystems;
using EverlastingOverhaul.Common.Utils;
using Humanizer.DateTimeHumanizeStrategy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Terraria;
using Terraria.ModLoader;
namespace EverlastingOverhaul.Common.Systems.TweenSystem;


public enum TweenEaseType : byte
{

    None = 0,
    InSine = 1,
    OutSine = 2,
    InExpo = 3,
    OutExpo = 4,
    OutBack = 5,
}

public enum TweenState : byte 
{
    Paused, Running, Stopped, Finished
}


/// <summary>
/// A tweener almost exactly the same as godot's, call <see cref="TweenProperty(TweenCache{T}[])"/> inside anything like setdefualts hooks and assign its value to anything you want in an update hook
/// </summary>
/// <typeparam name="T"></typeparam>
public class Tween<T> : ITween where T : struct
{
    public List<TweenCache<T>> cache = [];
    public int currentDuration = 0;
    public T currentProgress;
    private float currentProgressPercentage = 0;
    public Tween<T> onFinishTween = null;
    public TweenState state;
    private float endDuration = 0;
    public Action<Tween<T>> onFinsihed;
    public delegate T lerpFunction(T value1, T value2, float amount);
    public lerpFunction lerp;
    public bool Active { get; set; }

    public Tween(lerpFunction lerpFunc)
    {
        ModContent.GetInstance<TweenRunner>().tweens.Add(this);
        lerp = lerpFunc;
        Active = true;
    }

    public static implicit operator T(Tween<T> tween) 
    {
        return tween.currentProgress;
    }

    public Tween<T> TweenProperty(params TweenCache<T>[] cache)
    {
        this.cache = [.. cache];
        currentDuration = 0;
        state = TweenState.Running;
        return this;


    }
    public void Pause() => state = TweenState.Paused;
    public void Resume() => state = TweenState.Running;
    public void Update()
    {
        if (state == TweenState.Paused || state == TweenState.Stopped)
            return;

        var tween = cache[0];
        endDuration = tween.Duration;

        if (currentDuration < endDuration)
            currentDuration++;

        switch (tween.EaseType)
        {
            case TweenEaseType.None:
                currentProgressPercentage = currentDuration / (float)endDuration;
                break;
            case TweenEaseType.InSine:
                currentProgressPercentage = ModUtils.InSine(currentDuration / (float)endDuration);
                break;
            case TweenEaseType.OutSine:
                currentProgressPercentage = ModUtils.OutSine(currentDuration / (float)endDuration);
                break;
            case TweenEaseType.InExpo:
                currentProgressPercentage = ModUtils.InExpo(currentDuration / (float)endDuration, 11f);
                break;
            case TweenEaseType.OutExpo:
                currentProgressPercentage = ModUtils.OutExpo(currentDuration / (float)endDuration, 11f);
                break;
        }
        if (tween.Pingpong)
            Terraria.Utils.PingPongFrom01To010(currentProgressPercentage);
        currentProgress = lerp(tween.Start, tween.End, currentProgressPercentage);
        if (currentDuration == endDuration)
        {
            this.cache.RemoveAt(0);
            currentDuration = 0;
            if (this.cache.Count == 0) 
            {
                state = TweenState.Stopped;
                Active = false;
            }

        }
    }
}
