Screenshots 2 Showcase


# Create Dialog Overlay

## Period 1

### 2014-09-20 5:01-6:17

- Creating Project
- Display Character with positioning
 
### 2014-09-21 4:03-6:19

- Add dialog text
- Add background
- Create prefab

### 2014-09-29 4:33-5:46

- Fix text font size
- Add usable images

### 6:18-6:47

- Scale sprite to fit within height ratio with padding
- Flip sprite for right alignment
- Create sample scene

## Period ?

### 2014-10-14 4:30-6:12

- Render text with OnGUI


### 12:11-12:56
### 13:50-14:33

- Rafactor to use OnGUI for everything
	- Calculate all rects in screen ratios
	- Allow character to go beyond background
		- DialogHeightRatio
		- PaddingRatio
		- MaxCharacterWidthRatio
		- MaxCharacterHeightRatio
	- Set layout fields in UpdateLayout
		- text
		- textRect
		- textFontSize
		- characterImage
		- characterRect
		- backgroundImage
		- backgroundRect
	- DrawLayout at appropriate time
		- DrawLayoutWithOnGUI in OnGUI
			// GUI.depth = guiDepth;
			// GUI.DrawTexture(Rect(10,10,60,60), aTexture, ScaleMode.Stretch);

### 14:34-15:07

- Simplify Overlay Fields
	- Character
	- Background
	- Character Alignment
	- Dialog Height Ratio
	- Padding Ratio
	- Max Character Width Ratio
	- Max Character Height Ratio
	- Dialog Text
	- Font Color
	- Font

### 15:08-15:30

- Add overlay manager game object to switch between overlays

### 2014-10-15 5:30-6:26

- Add freeze button
- Freeze game time during pause
- Change layouts during pause

### 2014-10-16 6:00-7:00
### 8:00-8:26

- Add screenshot button
- Save screenshots

### 8:27-9:14

- Change resolution to actual resolutions

### 9:15-9:23

- Organize Screenshot folder

# TODO



- FUTURE: Choose between text rendering mode
	- onGUI
	- NGUI
	- new GUI