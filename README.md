# Rephidock.AtomicAnimations

[![GitHub License Badge](https://img.shields.io/github/license/Rephidock/Rephidock.AtomicAnimations)](https://github.com/Rephidock/Rephidock.AtomicAnimations/blob/main/LICENSE) [![Nuget Version Badge](https://img.shields.io/nuget/v/Rephidock.AtomicAnimations?logo=nuget)](https://www.nuget.org/packages/Rephidock.AtomicAnimations)

Basic callback-based animations and coroutines written in vanilla C#.


## About

Provides animation 'atoms', which mutate `float` values using callbacks,
and coroutines – animations based on `IEnumerable<T>`, allowing for state and logic.

Does *not* create additional clocks or threads for transparency.
Use `Update(TimeSpan deltaTime)` to provide time flow to animations, runners and queues.


## Contents

| Animation                         | Summary                                                     |
|-----------------------------------|-------------------------------------------------------------|
| (abstract) `.Base.Animation`      | Base class for all animations                               |
| (abstract) `.Base.TimedAnimation` | `Animation` with a defined Duration                         |
| (abstract) `.Base.Ease`           | `TimedAnimation` with defined easing and progress value     |
| `Shift1D`, 2D, 3D, 4D             | Changes 1 to 4 values by adding differences between updates |
| `Move1D`, 2D, 3D, 4D              | Changes 1 to 4 values by setting values directly            |
| `.Waves.WaveEase`                 | Calls an update delegate with a moving Wave (curve)         |
| `.Coroutines.CoroutineAnimation`  | Structures others animations, timing, state and logic       |

| Runner            | Summary                                                                  |
|-------------------|--------------------------------------------------------------------------|
| `AnimationRunner` | Runs animations in parallel. Starts animations the moment they are added |
| `AnimationQueue`  | Runs animations in series. Supports `Lazy<Animation>`                    |

For simplicity, prefer using Shift and Move atoms with delegates.
Inheriting from the base classes is not required, but is useful sometimes.

To control the easing of values use the static methods in the `Easing` class. All easing functions are normalized.

The animations can be run manually or added to an `AnimationRunner` or an `AnimationQueue`.
`AnimationQueue`s and `CoroutineAnimation`s also account for excess time since each atom finishes
for better accuracy when chaining animations together.

Usage example:
```csharp
public float X { get; set; }
public float Y { get; set; }

readonly AnimationRunner animationRunner = new AnimationRunner();

// ...

// In a method that is run every frame
// (commonly Update(float deltaTime) or similar)
animationRunner.Update(TimeSpan.FromSeconds(deltaTime));

// ...

animationRunner.Run( 
    new Shift2D(
        100, 200,
        TimeSpan.FromSeconds(0.5),
        Easing.Linear,
        (x, y) => {
            this.X += x;
            this.Y += y;
        }
    ) 
);
```



### `.Waves` namespace

The `.Waves` namespaces allows for animations that can be interpreted as a moving wave.

Use the `WaveBuilder` to scale and join multiple `EasingCurve`s together, forming a more complex `Wave`. 
The waves do not have to start and end at the same value, and they extend infinitely out of bounds as flat lines.

This example below creates a wave that looks like a bump or hill
with a width of 600 and a height of 1.
```csharp
new WaveBuilder()
    .Add(Easing.QuadOut).To(1).Over(300)
    .Add(Easing.QuadIn).To(0).Over(300)
    .ToWave()
```

The `WaveEase.CreateRunthrough` will create an animation atom that moves a given wave
through a span of known width calling a delegate with a `ShiftedWave` each update.

Waves can be sampled with the `GetValueAt` method.



### `.Coroutines` namespace

The `CoroutineAnimation` allows for building more complex animations. It is based on `IEnumerable<CoroutineYield>`, 
which can hold state and logic if made using a custom iterator/generator.

A `CoroutineYield` is immutable and holds either
- an animation that is to play the moment it is returned or 
- a delay instruction

The `Animation`s can be implicitly cast to `CoroutineYield`s,
while delays are static instances or created through static methods:
- `CoroutineYield.WaitPrevious`: Waits for the previous animation to finish
- `CoroutineYield.Join`: Waits for all previous animations to finish
- `CoroutineYield.Sleep(TimeSpan)`: Waits for a delay of specified time
- `CoroutineYield.SleepUntil(TimeSpan)`: Waits until a timestamp (since the animation has begun)
- `CoroutineYield.SleepUntilTrue(Func<bool>)`: Waits until a condition is satisfied
- `CoroutineYield.Suspend`: Suspends an update without influencing the flow of time

This allows mixing both serial and parallel execution.

Example:
```csharp
public float X { get; set; }
public float Y { get; set; }
public float Rotation { get; set; }

// ...

IEnumerable<CoroutineYield> ExampleCoroutine(float targetY, bool doSpin) {
    
    yield return new Move1D(
        this.Y,
        targetY,
        TimeSpan.FromSeconds(2),
        Easing.Linear,
        y => { this.Y = y; }
    );
    
    if (doSpin) 
    {
        yield return CoroutineYield.Sleep(TimeSpan.FromSeconds(1));

        yield return new Shift1D(
            360,
            TimeSpan.FromSeconds(1),
            Easing.QuadOut,
            r => { this.Rotation += r; }
        );
    }
    
    yield return CoroutineYield.Join;
}
```
