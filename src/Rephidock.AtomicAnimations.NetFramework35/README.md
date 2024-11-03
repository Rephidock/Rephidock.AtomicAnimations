# AtomicAnimations+NetFramework35

[![GitHub License Badge](https://img.shields.io/github/license/Rephidock/Rephidock.AtomicAnimations)](https://github.com/Rephidock/Rephidock.AtomicAnimations/blob/main/LICENSE) 

This is a source clone of AtomicAnimations downgraded to NET Framework 3.5. Due to drastic differences (including in dependencies) this needs to be a separate package.

The version of this package mimics the version of the original package.

Following features were changed:
- Removed `record` related features of `Wave`, `CoroutineYield`
- (new) `CoroutineYield` is now `IClonable` to replace absense of `with`
- Protectection regarding `init` properties of `ShiftedWave` and `CoroutineYield` no longer exists
