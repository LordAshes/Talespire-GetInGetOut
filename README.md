# Get In / Get Out Plugin

This unofficial TaleSpire plugin allows raising of event triggers when minis enter or leave
an area. Combined with the Relay Plugin, this can be used to trigger various effects to occurances
on entry or leaving of an area.

Lord Ashes' Talespire Plugins (and their source code) are always available for free but if you really want to
make a donation to the chocolate fund to keep the work going, you can do so using the following link:

Donate: http://198.91.243.185/TalespireDonate/Donate.php

Video Demo: https://youtu.be/UbCBg2Nj3C0

## Change Log

```
2.0.0: Fix for BR HF Integration update
1.0.0: Initial release
```

## Install

Install using R2ModMan or similar. The plugin is then available for any parent plugin to use it.

## Usage

On its own this plugin is only one piece of the entire solution for making effects occur when minis enter
or leave designated areas. While different uses of this plugin may exist, the typical uses combines the
HideVolumesMenu plugin, the RelayPlugin and some effect plugins like EAR, Icons, States or Light plugin.

By breaking up the soluton in this way, the plugin can remain felxible and not locked down to a specific
set of effects.

### How Does It Work

This plugins uses the Hide Volumes from the Hide Volumes Menu plugin to determine the different areas and
the names of these areas. When a mini enters one of these areas, a StatMessaging key, "AssetLocation", is
changed, on the corresponding mini, to indicate the mini's location. When a mini leaves an area, the
StatMessaging key is cleared unless the mini transitions directly into another designated area in which
the case key is updated to the new location.

On its own, that is all this plugin does...which is totally useless on its own. However, another plugin,
called the Relay Plugin, looks for specific StatMessaging changes, compares them to some criteria, and
triggers other StatMessaging changes.

Great! More StatMessaging changes but how does that help us? Many plugins can be triggered by the appropriate
StatMessaging message. This means that any plugins that supports it can now be used by the Relay Plugin to
trigger effects based on the entry and/or exit information provdied by this plugin.

Lets take an easy example, we create two Hide Volume areas called Area1 and Area2. We put a light beside
each area called Light01 and Light02. For now the Replay plugin does not support wild cards so we need to
configure it for specific minis (by name). So lets assume we will have a mini called Jon. We can create a
Relay Pplugin configuration such as:

```
AssetLocation:Jon:Area1|0.1:org.lordashes.plugins.light:Light01:Torch:0.1:org.lordashes.plugins.light:Light02:None
AssetLocation:Jon:Area2|0.1:org.lordashes.plugins.light:Light02:Torch:0.1:org.lordashes.plugins.light:Light01:None
AssetLocation:Jon:|0.1:org.lordashes.plugins.light:Light01:None:0.1:org.lordashes.plugins.light:Light02:None
```

See the Replay Plugin documentation for details but basically the above configuration says:

When this plugin sends an AssetLocation notification for (mini) Jon entering Area1 then send a request to set
Light01 to Torch and Light02 to None. When this plugin sends an AssetLocation notification for (mini) Jon entering
Area2 then send a request to set Light01 to None and Light02 to Torch. When this plugin sends an AssetLocation
notification for (mini) Jon entering empty location (existing a location) request both Light01 and Light02 to
None.

### Configuring Your On Areas And Effects
 
The basic steps to configuring your own areas with effects are as follows: 
 
```
1. Create hide volumes on your board using Hide Volumes Menu plugin. Give each Hide Volume a unique name.
   This is how you define areas for this plugin regardless if you plan to use the Hide Volume to hide or not.
2. Place the minis that will affect trip entry and exit from areas. Rename them as desired.
3. Place any minis, props or other content that is to be affected by the triggered effect.
4. Create the Replay plugin configuration linking the inputs from this plugin to the desired effects.
```

Most plugins that use StatMessaging do not have their messages documented. This would normally make it hard
to write the effect StatMessaging request messages but there is a way learn the messages. StatMessaging plugin
provides a diagnostic mode that can be used to display or log all of the messages associated with a mini. To
display all of the creatures messages at the top of the screen, press LCTRL+Period. This is a toggle, so using
this keyboard combo again will turn the feature off. While this feature is on, anytime a mini is selected that
mini's messages are written to the top of the screen. LCTRL+Comma can be used to (one time) dump the mini's
messages to the log. To use this to figure out the request message for any effect, do the following:

```
1. Use the diagnostic feature to see the messages for a mini before the effect is added.
2. Manually add the desired effect.
3. Use the diagnostic feature again and compare the difference in messages.
```

## Limitations

```
1. Currently the Relay Plugin does not support wild card matching and thus the Relay Plugin configuration needs
   to have separate entries for each mini. For example, the simple configuration above for Jon would need to be
   copied and the name changed for each other mini that is to be able to trigger the same effects.
   
2. Plugins that don't have a StatMessaging interface cannot be triggered using this plugins. This is more of a
   Relay Plugin limitation than a limitation of this plugin but since this plugin is relying on Relay Plugin
   this limitation is inherited.
```
