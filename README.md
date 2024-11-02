# Rephidock.AtomicAnimations

[![GitHub License Badge](https://img.shields.io/github/license/Rephidock/Rephidock.AtomicAnimations)](https://github.com/Rephidock/Rephidock.AtomicAnimations/blob/main/LICENSE) [![Nuget Version Badge](https://img.shields.io/nuget/v/Rephidock.AtomicAnimations?logo=nuget)](https://www.nuget.org/packages/Rephidock.AtomicAnimations)

Basic callback-based user-controlled animations and coroutines written in vanilla C#.



## Quick summary

The package provides:
- Animation 'atoms', which mutate `float` values via addition or overwriting using callbacks.
- `AnimationRunner` and `AnimationQueue` to run given animations.
- Waves for animations that can be interpreted as moving waves (curves).
- Coroutines – animations based on `IEnumerable<T>`, allowing for state and logic.

This package does *not* create additional clocks or threads to be transparent about control flow. Use the `Update(TimeSpan deltaTime)` to provide time flow to animations, runners and queues.

Additionally queues and coroutines account for excess time since each atom finishes for better accuracy when chaining animations together.



## Contents

The package provides the following animations out of the box:

| Animation                        | Summary                                                     |
| -------------------------------- | ----------------------------------------------------------- |
| `Shift1D`, 2D, 3D, 4D            | Changes 1 to 4 values by adding differences between updates |
| `Move1D`, 2D, 3D, 4D             | Changes 1 to 4 values by setting values directly            |
| `.Waves.WaveEase`                | Calls an update delegate with a moving Wave (curve)         |
| `.Coroutines.CoroutineAnimation` | Structures others animations, timing, state and logic       |

To control the easing of values use the static methods in the `Easing` class. All easing functions are normalized.
`EasingCurve` delegate is included.


Use a queue or a runner to execute multiple animations.
Animations can be added to both during their runtime (fire-and-forget).

| Runner            | Summary                                                                 |
| ----------------- | ----------------------------------------------------------------------- |
| `AnimationRunner` | Runs animations in parallel. Stats animations the moment they are added |
| `AnimationQueue`  | Runs animations in series. Supports `Lazy<Animation>`                   |


For creating animations from scratch you can use the following classes in the `.Base` namespace:

| Abstract Class   | Summary                                                 |
| ---------------- | ------------------------------------------------------- |
| `Animation`      | Base class for all animations                           |
| `TimedAnimation` | `Animation` with a known Duration                       |
| `Ease`           | `TimedAnimation` with defined easing and progress value |


### `.Waves` namespace

The `.Waves` namespaces allows for animations that can be interpreted as a moving wave.

Use the `WaveBuilder` to scale and join multiple `EasingCurve`s together, forming a more complex `Wave`. The waves do not have to start and end at the same value and extend infinitely out of bounds as flat lines.

The `WaveEase.CreateRunthrough` will create an animation atom that moves a given wave through a span of known width calling a delegate with a `ShiftedWave` each update.


### `.Coroutines` namespace

The `CoroutineAnimation` allows for building more complex animations. It is based on `IEnumerable<CoroutineYield>`, which can hold state and logic if made using a custom iterator/generator.

A single `CoroutineYield` holds either
- an animation that is to play the moment it is returned or 
- a separate delay instruction

The following delays are possible:
- (static) `CoroutineYield.WaitPrevious`: Waiting for the previous animation to finish
- (static) `CoroutineYield.Join`: Waiting for all previous animations to finish
- (static) `CoroutineYield.Sleep`: Waiting for a delay of specified time
- `CoroutineYield.WaitUntil`: Waiting until a timestamp (since the animation has begun)
- `CoroutineYield.WaitUntilPredicate`: Waiting until a condition is satisfied
- (static) `CoroutineYield.Suspend`: Suspending an update without influencing the flow of time

This allows mixing both serial and parallel execution.
