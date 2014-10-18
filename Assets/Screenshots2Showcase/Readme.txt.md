Screenshots 2 Showcase

# Setup

Import the Package from Unity Asset Store

The package contents are located inside the folder:

Screenshots2Showcase

# Demo

Open Screenshots2Showcase/Demo.scene

The Demo includes a two root game objects:

- Root - an example game
- OverlayManager - the main game object for Screenshots2Showcase

# Run the game in preview

- Hit "FREEZE" to freeze the game time
- Hit "PREV" and "NEXT" to change overlays
- Hit "SCREENSHOT" to take a screenshot 
	- In Preview mode, this will take only a screenshot at the preview resolution
	- Run the game as standalone to enable multiple resolutions
- Hit "PLAY" to unfreeze the game time 

# Run the game in Standalone

- Build and Run the game as Standalone
- Use the same commands as above
- Hit "SCREENSHOT" to take multiple screenshots
	- This will take the screenshots listed in:
		- Code/OverlayManagerController.cs
		- ScreenshotSizeHelper.ScreenShotsSizesText
	- It will take both portrait and orientation screenshots

# Modify the overlays

- Select the OverlayManager in the Scene Hierarchy in Unity
- Use the buttons available in the edit window:
	- "Previous" - Enable previous overlay
	- "Next" - Enable next overlay
	- "Add Empty Overlay" - Add a new overlay to the overlay manager (as the last overlay)
	- "Duplicate Overlay" - Duplicate the current overlay and add it to the overlay manager (as the last overlay)

- Select an Overlay under the OverlayManager
	- Enable a disabled overlay in order to make it current and make it visible
	- Modify the Overlay properties to change the look of the character, background, text, or layout
	- See the documentation for a description of each property:
		-

# Add the OverlayManager to your own game

- Create an empty Game Object at the root of your scene hierarchy
- Name it anything like ("OverlayManager")
- Add the script component "OverlayControllerManager"
- Create Overlays as needed
- Use the above instructions to run your game and create screenshots


# Browse the Script Code

The script Code is located under the Code folder. It is well documented, so feel free to read through it.

# Support

Email me (Rick) for anything:
	- Report a bug
	- Ask for help
	- Request a feature

support@toldpro.com