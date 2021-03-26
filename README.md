# H2PC_TagExtraction
A application made to extract assets from cache files of H2v using BlamLib by KornnerStudios.
UI and small modifications to BlamLib by Himanshu-01.
The state of the project right now is pre-alpha so there are alot of issues/bugs that need addressing.


## KornnerStudios ##
You can go to this link

https://bitbucket.org/KornnerStudios/opensauce-release/wiki/Home

### REQUIREMENTS ###
* __[BoostSDK Libraries (boost_1_55_0)](https://www.boost.org/users/history/version_1_55_0.html)__
* __[SlimDX SDK (January 2012)](https://code.google.com/archive/p/slimdx/downloads)__
* __Copy the boost_1_55_0 folder to C:\Program Files (x86)\boost\\__

## COMPILATION / CONTRIBUTE ##

* __Contributors are Welcome :)__
* __Get the Requirements__
* __Visual Studio 15 was used (will be updated in near future)__
* __Hit Clone and Start Building__


## ChangeLog ##
### V2.2 ###
Features:
* __Static Tag Injection Beta released__
* __Changed Module to use Rebase Tables for relocation (much faster than plugin based)__
* __Sound Injection Support(Both runtime and static)__
* __More work on Undo Tag Post processing : sounds,lipsync,collision,physics,bsp and more__
### V2.1 ###
Features:
* __New UI combines Tag Extracting with Map Loading__
* __Users can select multiple tags for extracting__
* __Users must designate a maps folder on startup, directory saved to settings file in %appdata% folder__
* __If settings file exists will automatically load maps directory.__

## ChangeLog ##
### V2.0 ###
Features:
* __Added extended meta logic for dataRefs__
* __Can Now successfully extract most tags and rebase them__
* __Added Resyncer Dialog for Relinking and fixing specific tags,class Resyncer__
* __Added StringID refixer class Resync_SID__
* __Updated Write_Int_LE method__
* __DataStructures Updated(new classes tag_info,StringID_info)__

### V1.7 ###
Features:
* __Updated BlamLib version(With Fixed Depencies)__
* __Fixed the long awaited Last Character Bug__
* __Improved UI Systems(Still need to be better)__
* __Improved Meta Extraction and Injection UI__
* __Included a Tag Extractor UI__
* __Added Test Functions(Extract Import Info)__
* __Added a Dump Selected Tags Option__
* __Bitmaps Extraction Fix__
* __Sound Extraction (!snd) Added__

### V1.0 ###
Features:
* __Tag Extration__
* __Meta Extraction and Rebasing__
* __Simple Cache Viewer and UI__
* __And some Known Bugs like the dependencies and last character bug__


For any issue you may find using it, feel free to use "Issues" tab.

If you have more questions about the project, ask me on discord : Himanshu01#3268 .

### Huge Thanks to Kornmann for BlamLib ###
## Some Honourable Mentions to these people too :) ##

*__General_101__
*__Twinreaper__
*__NukeULater__
*__UF Beazt__


## H2PC Project Cartographer Team ##
### visit www.halo2.online ###
