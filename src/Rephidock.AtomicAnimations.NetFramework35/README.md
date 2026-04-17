# AtomicAnimations+NetFramework35

[![GitHub License Badge](https://img.shields.io/github/license/Rephidock/Rephidock.AtomicAnimations)](https://github.com/Rephidock/Rephidock.AtomicAnimations/blob/main/LICENSE) 

This is a source clone of AtomicAnimations downgraded to NET Framework 3.5. 
Due to drastic differences (including in dependencies) this needs to be a separate package.

Prefer using the original package when possible.

Following features were changed:
- `Wave` and `CoroutineYield` are no longer records
- Former `init` properties of `ShiftedWave` and `CoroutineYield` can only be set through a constructor or creation methods.
