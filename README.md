CollectionDowngrader
=========================

> [!WARNING]
>
> This operation (Importing data from lazer to stable) is not officially supported by osu!, so use this at your own risk and don't forget to backup your data before downgrading!

A very simple and tiny program that converts osu! (lazer) collection data to osu! (stable) collection.db format.

## Usage

> [!NOTE]
>
> You should already know where your osu! installations are located and what you are trying to do.

`CollectionDowngrader.exe <path to osu! (lazer) client.realm> <output osu! (stable) collection.db path>`

### Example

This command reads the collection data from osu! (lazer) at the default installation path and creates a collection.db
 that can be used in osu! (stable).

````shell
CollectionDowngrader.exe %APPDATA%/osu/client.realm collection.db
````

> [!WARNING]
>
> If you already have some collections in your osu! (stable) installation, then you should ***NEVER*** directly overwrite
 the existing collection.db with a new one. Instead, use [CollectionManager][CollectionManager] for merging data in multiple collection databases.

## Downloads

[GitHub Releases](../../releases)

## How to build

Just utilize your IDEs.

## Credits / See also

* [kabiiQ/BeatmapExporter][BeatmapExporter]: Inspiration of this project, all code for osu! (lazer) realm schema and
 the database parsing.
* [Piotrekol/CollectionManager][CollectionManager]: Code for creating osu! (stable) collection.db files.

[BeatmapExporter]: https://github.com/kabiiQ/BeatmapExporter.git
[CollectionManager]: https://github.com/Piotrekol/CollectionManager.git
