Character Movement Fundamentals V1.1

Changelog:

Version 1.2

- Fixed a bug that caused visual glitching when viewing controller prefabs in the inspector in newer versions of Unity (2018 and up).
- Added a public 'Add Momentum' function to 'BasicWalkerController', which can be used to add a force to the controller (useful for jump pads, dashing, [...]).
- Added an option for more responsive jumping: By holding the 'jump' key longer, the character will jump higher.
- Added two public functions to 'CameraController'. By calling them from an external script, the camera can be rotated either toward a direction or a specific position (see manual for more details).
- 'SideScroller' controller prefab has been streamlined (code-wise) and is now compatible with the 'GravityFlipper' prefabs.
- Various minor changes to some of the code to improve readability.

Version 1.1

- Replaced old showcase scene with new version (now with a proper controller selection menu).
- Polished controller prefab settings.
- Replaced 'ApplicationControl' and 'ChangeControllerType' scripts with new 'DemoMenu' script, which will now handle switching between different controller prefabs.
- Added 'DisableShadows', 'FPSCounter' and 'PlayerData' scripts, all of which are used in the new showcase scene.
- Decreased scene light intensity to '0.7'.
- Removed all (unnecessary) 'Showcase' controller prefabs, all example scenes now use the controller prefabs included in the projects.
- Added 'MouseCursorLock' script, which provides basic mouse cursor locking functionality. Old (unstable) 'FocusMouse' script was removed.
- Updated user manual to reflect changes.

Version 1.0

- Initial release

