# A comic file syncer for windows, based on rclone

Features
===============
 - Sync comic files from **several cloud storages** (see avaible [storage](https://rclone.org/overview/))
 - Can sync multiple remote storage to one final directory
 - **Check comics files** (cbr, cbz via 7zip, pdf via xpdf-utils) before adding them in the destination folder
 - Can be started as a **daemon**
- For now only a **CLI** is available

Pre-requisites
===============
The folowing software need to be installed throught [chocolatey](https://chocolatey.org/).  
__*Note: the exes will be seek in chocolatey default install folder.*__

- ``choco install rclone``
- ``choco install xpdf-utils``
- ``choco install 7zip ``

CLI
===============

Usage:
``comicsync copy remote1:path remote2:path --DestinationDir C://Dest``   
[Console readme](ConsoleApp/readme.md)