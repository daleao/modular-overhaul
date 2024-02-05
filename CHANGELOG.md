# MARGO Changelogs

This file contains a TL;DR of current version changes and hotfixes from across all modules. For the complete changelog, please refer to the individual changelogs of each module, linked [below](#detailed-changelogs).

## Patch 4.2.3 Highlights

* Fixed missing status effect error when CMBT module is not enabled.
* Fixed `margo revalidate` command not available in Release version.
* [PRFS]: Now displays a blue MAX icon for ordinary fish and red MAX icon for crab-pot fish in the collections page.
* [TWX]: Fixed freeze when collecting trash from Crab Pot with full inventory.
* [TWX]: `TrashDoesNotConsumeBait` now applies to fishing rods as well.

<div align=center>- The final update for SDV 1.5.x. -</div>

## Patch 4.2.2 Highlights

Moved GMCM page links moved to the top of the respective page.
* [CMBT]: Fixed Mammonite enchantment sometimes consuming gold instead of rewarding it.
* [PRFS]: Buffed the Prestiged Trapper perk.
* [PRFS]: Added new Prestige profession icons.
* [PRFS]: Added 5th-level professions to Prestige ribbon tooltips.
* [PRFS]: Fixed Luremaster double Crab Pot production not applying correctly.
* [TWX]: Fixed ring materials not being consumed when crafting rings in the Glowstone progression.
* Updated translations.

## Patch 4.2.1 Highlights

* Fixed a GMCM issue in handheld systems.
* Updated translations. Minor typo fixes.

## Minor Release 4.2.x Highlights

* Added new config presets which can be found in the "optionals" folder.
* Moved the Status Effect framework to the Core module, so that other modules can continue to use it even if CMBT is disabled.
* [CMBT]: Added some flavor dialogue to Emily and Mr. Qi dropping some hints about the Infinity Band.
* [PRFS]: Added new Prestige mode options. You can now have Prestige Levels without needing to reset your skills a bunch of times. Or make it even harder by requiring full reset of **all** skills before unlocking Prestige Levels.
* [PRFS]: Overhauled the Oceanographer (Prestiged Conservationist) profession perk to better fit the profession's title.
* [PRFS]: Overhauled the Saboteur (Prestiged Conservationist) profession perk to better fit the profession's title.
* [PRFS]: Replaced the Aquarist profession's icon with that of fish Roe.
* Some adjustments to the API.
* Shaved off several mods from the curated list due to newfound incompatibilities.

**NOTE: This version is incompatible with XP Display. If you want to use that mod then DO NOT update. Please don't bother Shockah or I about this.**

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Major Release 4.x.x Highlights

* **[PRFS]:**
    * Major rework of most Combat professions. I encourage you to go over the detailed changelog for PRFS module.
* **[CMBT]:**
    * Major rework of all Ranged prismatic enchantments. Removed all the boring old enchantments in favor of much better new ones.
    * Reworked some player status effects to be more consistent with the ones applied to monsters. Also tweaked some monster status effects.
    * Added new visuals and sound effects to certain status effects.
    * Several other tweaks and minor improvements. I encourage you to go over the detailed changelog for CMBT module.
* Added, removed and changed several to translation keys.
* Some bug fixes.
* Changed to custom [license](LICENSE).

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Detailed Changelogs

In-depth changelogs for existing modules.

* [Core](Modules/Core/CHANGELOG.md)
* [Professions](Modules/Professions/CHANGELOG.md)
* [Combat](Modules/Combat/CHANGELOG.md)
* [Tools](Modules/Tools/CHANGELOG.md)
* [Ponds](Modules/Ponds/CHANGELOG.md)
* [Taxes](Modules/Taxes/CHANGELOG.md)
* [Tweex](Modules/Tweex/CHANGELOG.md)

<sup><sup>[🔼 Back to top](#margo-changelogs)</sup></sup>

## Legacy Changelogs

Changelogs for modules that have been merged and no longer exist.

* [Weapons](Modules/Combat/resources/legacy/CHANGELOG_WPNZ.md)
* [Slingshots](Modules/Combat/resources/legacy/CHANGELOG_SLNGS.md)
* [Enchantments](Modules/Combat/resources/legacy/CHANGELOG_ENCH.md)
* [Rings](Modules/Combat/resources/legacy/CHANGELOG_RNGS.md)

[🔼 Back to top](#margo-changelogs)