Implementing your own cloud backup utility is kind of a fun exercise. I've done a few Azure blob storage applications over the years. This time I wanted to try [Backblaze](https://www.backblaze.com/) because it's supposedly much cheaper than Azure. I've used Backblaze's desktop app before, but it's a bit redundant to OneDrive, which I think works pretty well in the latest Windows versions. I still have some oddities with OneDrive however, which motivated me to take more control and make a custom app. (I have files show up in my recycle bin as deleted, even though they aren't. When I try to restore them, I get the Replace File dialog, but I also can't find them through the File Explorer search box. This is pretty unnerving with old pictures I don't want to lose.) Again, this is mainly just an exercise that I'll stop working on as soon as it becomes not-fun.

Code tour:
- [BackupProvider](https://github.com/adamfoneil/Backblaze.CLI/blob/master/CloudBackup.CLI/Abstract/BackupProvider.cs) abstract class. This is abstract because I want to be able to "plug in" different cloud providers, and not be coupled to Backblaze necessarily. Maybe I want to implement an Azure solution eventually. By starting with an abstract class, I can implement some functionality common to all providers -- namely the ability to inspect local files -- then delegate provider specifics to a concrete class.
- [BackblazeBackupProvider](https://github.com/adamfoneil/Backblaze.CLI/blob/master/CloudBackup.CLI/Providers/BackblazeBackupProvider.cs) implements `BackupProvider` and uses [Backblaze Client for .NET](https://github.com/microcompiler/backblaze). They've done the hard work of making the Backblaze REST API much easier to consume in C#.
- [BackblazeTest](https://github.com/adamfoneil/Backblaze.CLI/blob/master/Testing/BackblazeTest.cs) is how I ran this without building the full console app. Note that I use a [linked json config file](https://github.com/adamfoneil/Backblaze.CLI/blob/master/Testing/Testing.csproj#L13) in order to keep my API key out of source control. Yes, I did mistakenly commit a config file with API keys in it earlier in development, but I've since recycled the key.