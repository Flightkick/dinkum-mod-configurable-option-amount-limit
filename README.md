# Dinkum Configure Option Amount Limits

⚠️Use mods at your own risk. Make sure to back up your save file before using any mods in case something goes wrong.

## Summary
- Did you go all-in on your ultra-mega-super-duper farm?
- Are you buying items (like seeds) per 99, because the counter doesn't go any higher?
- Are you spending more time spamming buttons in dialogue than with actual gameplay?

If you answered any of those questions with yes, this mod may be for you!

This mod makes the limit configurable. By default it is set to `999` which should be enough for most players.  
Additionally, it maps controls to increment/decrement values by 10/100 when held.

Controller:
- Hold `LB` to modify by 10
- Hold `RB` to modify by 100

KBM:
- Hold `1` to modify by 10
- Hold `2` to modify by 100

<img src="https://raw.githubusercontent.com/Flightkick/dinkum-mod-configurable-option-amount-limit/master/.github/assets/img/dinkum-configurableoptionamountlimit.png" alt="Configurable Option Amount Limit Showcase" />

## Installation

This mod is build against `BepInEx 6.0.0-pre.1` so you would need that release or a higher version that's backwards compatible to use it.  
Installation instructions for BepInEx can be found [here.](https://docs.bepinex.dev/articles/user_guide/installation/index.html)

Simply put the DLL in your BepInEx plugins directory.  
The path should look like `Dinkum/BepInEx/plugins/ConfigurableOptionAmountLimit.dll`

Start the game to generate the config file, or play with the default configuration.

## Configuration

You can change the configuration in `Dinkum/BepInEx/config/ConfigurableOptionAmountLimit.cfg`:

Example config:
```properties
## Settings file was created by plugin ConfigurableOptionAmountLimit v1.0.0
## Plugin GUID: ConfigurableOptionAmountLimit

[Options]

## You can disable the mod quickly by editing this value to false.
# Setting type: Boolean
# Default value: true
Enabled = true

## The maximum limit of items that can be bought at once. (Min: 1, Max: 100000)
# Setting type: Int32
# Default value: 999
OptionAmountLimit = 999
```

## Contribution & Support
This was a quick project for me to get familiar with Unity modding but I will not be actively maintaining or supporting the project.  
Updates may or may not be provided. In case of prolonged inactivity, the license should be permissive enough for someone else to take over.

## Acknowledgements
- Thanks to [Octr](https://github.com/Octr/) for providing the [ValueTooltip mod](https://github.com/Octr/ValueTooltip) which I used as a baseline for this project.