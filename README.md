# The SaFrMo Template #

*MIT license - redistribute, modify, sell, etc. to your heart's content!*

The SaFrMo template collects some of the most useful plugins and utility functions I've put together as a Unity developer. These are tools that I find myself importing on just about every project, so making them a public repo seems to be a good way to save myself and others some time in the future.

### How do I use it? ###
1. Create a new Unity project at your target destination.
2. Download the latest .zip of the SaFrMo template (https://bitbucket.org/SaFrMo/template/downloads)
3. Unzip the downloaded file to your new Unity project's root folder.
4. Rename the 'Template' names as necessary.

### What's included? ###
* **iTween**, the fantastic tweening and animation library [(link)](https://www.assetstore.unity3d.com/en/#!/content/84)
* **iTweenHinting**, which enables hinting for iTween methods and parameters [(link)](http://blog.almostlogical.com/2010/09/13/itween-parameter-code-hinting/)
* **Shader snippets**, from the excellent Makin' Stuff Look Good in Unity channel [(link)](https://www.youtube.com/watch?v=T-HXmQAMhG0&index=1&list=PLJ4rOFLQFH4BUVziWikfHvL8TbNGJ6M_f)
* **SaFrMo**, a collection of utility functions I've put together during different projects
* **StringEncryption**, which enables basic string encryption and decyrption given an arbitrary key (**not** meant for security purposes, but instead as a deterrent to modifiying save files, dialogue assets, etc)
* **MenuRefresher**, a utility class to auto-populate UI menus (see file for instructions)
* **Tooltips**, a class to set up tooltips (see Tooltips.cs and Tooltip.cs for instructions)
* **VisualTimer**, a class to set up visual timers (progress bars, etc. - see VisualTimer.cs for instructions)
* **ProximityTrigger**, for events when one object is in range of another (see ProximityTrigger.cs for instructions)
* **Standard2dMover** and **Standard3dMover**, for basic 2D and 3D movement
* **GameObjectHolder**, for when you need a script to hold a reference to a GameObject

### Wish List ###

* Allow count up or count down progress on visual timers
* Generalized RectTransform Follow actions
* Improved creation of UI components (better scaling on RectTransforms, dynamic canvas creation, etc., especially in VisualTimer)

### Wish List - Completed ###
* A utility to create tooltips. Include tooltip text, prefab path or prefab GameObject, target screen position or Transform, and default lifetime for tooltips and/or confirmation/exit button text