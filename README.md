# NeonWhite-MikeyMode

Pretty much what it says on the tin. Install melonloader & melonPreferences manager; you can then turn the mikey practice mode on/off via the mod preferences menu (default F5, can be rebound).

You need to restart the game to disable the mod, because dealing with "what happens if you try to turn off the mod in the middle of a level" is just way too annoying compared to "lol restart game"

## Installation & Usage

1. Download [MelonLoader](https://github.com/LavaGang/MelonLoader/releases/latest) and install it onto your `Neon White.exe`.
2. Run the game once. This will create required folders; you should see a splash screen if you installed the modloader correctly.
3. Download the **Mono** version of [Melon Preferences Manager](https://github.com/sinai-dev/MelonPreferencesManager/releases/latest), and put the .dlls from that zip into the `mods` folder of your Neon White install (e.g. `SteamLibrary\steamapps\common\Neon White\Mods`)
    * The preferences manager is *required* to use the powertools mod - it is how you turn parts of it on and off. Everything is off by default.
    * The default keybind for the mod preferences menu is F5; you can easily rebind this.
    * The IL2CPP version **WILL NOT WORK**; you **must** download `MelonPreferencesManager.Mono.zip`. 
4. Download the dll from the [Releases page](https://github.com/PandorasFox/NeonWhite-MikeyMode/releases/latest) and drop it in the mods folder

### Additional Notes

You should probably add `--melonloader.hideconsole` to your game launch properties (right click the game in steam -> properties -> launch options at the bottom of that window) to hide the console that melonloader spawns. You really only need that if you're a mod developer; it's a weird default.

![image](https://user-images.githubusercontent.com/3235827/181994781-af470314-9836-49f4-beec-abdf1f9e37ea.png)
