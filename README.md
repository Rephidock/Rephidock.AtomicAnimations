# Rephidock.AtomicAnimations

[![GitHub Licence Badge](https://img.shields.io/github/license/Rephidock/Rephidock.AtomicAnimations)](https://github.com/Rephidock/Rephidock.AtomicAnimations/blob/main/LICENSE) [![Nuget Version Badge](https://img.shields.io/nuget/v/Rephidock.AtomicAnimations?logo=nuget)](https://www.nuget.org/packages/Rephidock.AtomicAnimations)

Low-level animations written in vanilla C#.

### Features

- Easing functions (for normalized time and values) in the `Easing` class. `EasingCurve` delegate included.
- Animation 'atoms' running on a single thread and controlled with `StartAndUpdate(TimeSpan)` and `Update(TimeSpan)`.
- `AnimationRunner` that runs given animations in parallel when they are added.
- `AnimationQueue` that runs given animations in series when they are added.
- `Shift*` atoms, with an adder delegate, stacking together with other `Shift*` atoms.
- `Move*` atoms, with a setter delegate, overwriting other atoms.
- `Waves.WaveEase` atom for animations that can be interpreted as a moving wave.
- `Coroutines.CoroutineAnimation`, which mimicks Untiy's coroutines.
  - Allows for running multiple animations as a sequence or in parallel or a mix of both.
  - Allows for waiting for a duration, until a timestamp, or until a condition is satisfied.
  - Accounts for excess time since each atom finishes.





