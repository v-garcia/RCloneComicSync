A program for syncing and checking comics files for your windows device.

Usage - ConsoleAppTest <action> -options   
Exemple - ``comicsync copy remote1:path remote2:path --DestinationDir C://Dest"``   

GlobalOption   Description
Help (-?)      Shows this help

Actions

  Copy <SourceFolders> -options - Creates a new album in the target container, creating the container if it does not exist

    Option                        Description
    SourceFolders* (-S)           The list of remotes path to sync 'remote1:path1;remote2:path2'
    DestinationFolder (-D)        The place where the checked files land on
    StartCopyEveryMinuts (-St)    If set copy keep copying every X minuts
    FilesToSync (-F)              Choose the files to sync (rclone include)
    _7ZipExePath (-_)             Path to 7Zip exe path
    PDFInfoExePath (-P)           Path to PDFInfo exe path
    RCloneLogPath (-R)            Where you want rclone write the logs
    RCloneExePath (-RC)           Path to rclone exe path
    WorkingFolder (-W)            The place where are stored the files before they are checked
    ExcludeDeletedFromCopy (-E)   Exclude deleted file from rclone copy [Default='True'] 

  ConfigRemotes -options - Creates a new album in the target container, creating the container if it does not exist

    Option               Description
    RCloneExePath (-R)   Path to rclone exe path

  Clean -options - Looks for deleted folder in destination folder and remove associated cache in the working directory

    Option               Description
    WorkingFolder (-W)   The place where are stored the files before they are checked

Examples

    Copy and checks the comics from several rclone remote to a local directory
    comicsync copy remote1:path;remote2:path --DestinationDir C://Dest