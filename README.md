# OnlineResxTranslator
Let multiple users translate your resx files (.NET Resource files) in an online interface.  

## What does it do?
OnlineResxTranslator is a asp.NET 4.6.1 online plattform to translate multiple files from multiple projects in multiple languages per user, bundled in one nice little bootstrap-themed (and mobile friendly, too) online interface.  
Originally I built the project for multi-project solutions (like standard C# Project solutions with multiple projects in it), but the platform can also be used for just one project.  
So briefly summarized:  
Multi-user online translation platform (looks good on most browsers through the holy bootstrap and javascripts)  
  
There are also images under [Screens/](https://github.com/Nockiro/OnlineResxTranslator/blob/master/Screens/MainOverview.png) if you want to have a closer look at it.

## Quick start
(0. Might be necessary if nuget doesn't install all dependencies: run `Update-Package -reinstall` in the nuget package manager console, see also the troubleshooting section)  
1. Have a look at the web.config and replace the default platform name and the translation-containing folder with your strings.
2. Register your first account (Note: The first account will always be an admin-privileged account, all subsequent accounts will have standard user privileges)
3. Add a folder with your project files (probably you want them to be _not_ in the publicly accessible directory) in a directory of your choice (the choice you made two sentences earlier) and create a reference in the admin panel with your just new created admin account
4. Translate! Not yet created language files are automatically created if you choose its language in the menu (if you assigned multiple languages to your user)  
  
**Note**: You might want to disable the public registration feature after you set all up - you can do this if you set the setting `EnableOpenRegistration` in the app.config to false.  
**Also note**: If you want to create more users, you should do this over the registration function (also accessible over the manage menu over which you got to the admin panel)  
**Also also note**: If you want to add captions to your files to be seen in the file overview, have a look at the language code xml files in the project folders.  
  
## Troubleshooting  
Problems I've encountered:
- If Visual Studio doesn't find certain classes in certain namespaces ("type or namespace name 'bla' does not exist in the namespace 'Microsoft'" or similar), run `Update-Package -reinstall` in the nuget package manager console, in this way the project is forced to get all important packages again 
- If a translation file doesn't exist (e.g. because you imported an already translated language and the platform did not generate the files itself), hit the recalculate (⟳) button, the recalculation process will automatically create all not yet existing translation files
- You don't see a resource for translation although it's in the corresponding file: Check if the type of resource is excluded by the plaform in `XMLFile#NotArgs[]`

## Credits and Misc.
The fundamentals and idea I was originally starting to work from were taken from the (already archived) "[ResX Translation Helper](https://archive.codeplex.com/?p=resx)", which was licensed under the GPLv2 as therefore this project is, too.  
If you have suggestions or wishes, just open an issue and I'll have a look at it, if you have questions, don't hesitate to write me.
