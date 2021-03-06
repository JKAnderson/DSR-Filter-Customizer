
--| DSR Filter Customizer 1.0
--| https://www.nexusmods.com/darksouls/mods/1435
--| https://github.com/JKAnderson/DSR-Filter-Customizer

A utility to customize the built-in visual filters of Dark Souls: Remastered. Supports multiple profiles, global or localized filter replacement, and realtime editing. 
Requires .NET 4.7.2: https://www.microsoft.com/net/download/thank-you/net472
Windows 10 users should already have this.

Seven sample profiles are included:
	Cheerful - every area is a bit brighter and more saturated
	Gloomy - every area is a bit darker and less saturated
	Negative - inverted colors
	Night Vision - useful to see what you're doing in areas like Tomb of the Giants
	Noir - "gritty" black and white, for the hard-boiled detective type
	TK's Cut - a set of filters for the whole game that I happen to think are nice
	Vanilla - all default filters, for if you just want the modded fade effect

	
--| Installation

Extract the mod folder anywhere you like, then run DSR Filter Customizer.exe.
Once Dark Souls is running, your filters will automatically be applied.


--| Profiles

Click the Clone button to make a copy of an existing profile, or New to create a profile with default values.
There are four profile types:
    Global - one filter will be applied no matter where you are.
    Multiplier - the vanilla filters for each area will be multiplied by one filter you define.
    Detailed - define one or two filters for each different area.
    Full Control - every filter used by the game is available for editing.
Changes will take effect immediately in-game, but remember to click Save to make them permanent before closing the app.


--| Sharing Profiles

Your profiles can be found in the "profiles" folder alongside the .exe. Each one is saved in a separate .xml file, so all you need to do is upload it somewhere, and drop any downloaded profiles in the same folder.


--| Credits

Costura.Fody by Simon Cropp, Cameron MacFarland
https://github.com/Fody/Costura

Octokit by GitHub
https://github.com/octokit/octokit.net

Semver by Max Hauser
https://github.com/maxhauser/semver
