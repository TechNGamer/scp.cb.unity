# Modding
## Mod Folder Structure
The mods folder will be located in the games directory.

- Mod Folder
    - Mod
      - info.json
      - Assembly (Optional)
        - MyCode.dll
      - AssetBundles (Optional)
        - MyRoom.assetbundle
        - objects.json
      - Language (Optional)
        - English.json

## File/Folder Information
### info.json

This is the file that tells the game what the mod is. It contains data relating to the name of the mod, along
with the UUID (aka GUID) of the mod. It has optional parameters that include a checksum of each file (excluding itself).
The json file looks something like that below.

```json5
{
  // The name of the mod doesn't have to be unique, but it should for the sake of the player.
  "name": "Core",
  // This is required, if there are any mods that have a blank UUID, or uses the same UUID, neither mod will be loaded.
  "id": "A81C1094-1B8E-4364-B5B5-8597C838A88A",
  // This section is to help load dependent mods first.
  "dependencies": [
    // This section uses the UUID of the mod that it is dependent on.
    "1EE8F12C-16AD-4DB2-BA55-2B963DD0EF03"
  ],
  // This part is optional, but it is required. It uses a SHA-512 checksum.
  "hashes": [
    {
      "file": "MyCode.dll",
      "hash": "bc626bff7dad0c181add398c8d22574dc3c019ef9967c884c3fc8ecd3121e694a4fca50d4108ec319bbedb4afe5859420fb78f491810a26cbc6ada03e5f47230"
    }
  ]
}
```
### Assembly
This folder contains the libraries that are used. It is recommended to number them so as to ensure that they are loaded properly
and without error.
### AssetBundles
This folder will contain asset bundles that the mod uses. Asset Bundles are a nice feature of Unity to enable extra content later
that can be used at Runtime. In short, this enables an easy way of creating content for other people to enjoy. It will allow them
to set up rooms and NPCs that have special shaders on them. This folder is loaded second as the assemblies should be loaded before
attempting to load AssetBundles so Unity can instantiate the object(s) properly.
### Languages
If any new strings are added, it is recommended to use a Key-Value pair to enable the usage of different languages for your mod.
Only 1 language is allowed per json file, but multiple json files are allowed. Each json file can have an unrelated name.
An example of a language JSON file is:
```json5
{
  "language": "en-us",
  "keys": [
    {
      "key": "scp-049-warning-sign",
      "value": "WARNING! SCP-049 has breached containment."
    },
    {
      "key": "added-lore-sign-1",
      "value": "Look at me, I am new, unofficial lore!"
    }
  ]
}
```
