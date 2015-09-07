# Wox.Plugin.SimpleClock
A simple clock plugin for [Wox](https://www.getwox.com/)

Uses NAudio to play audio files.

Currently it does the following:
- Allows you to set/edit/delete alarms
- Sounds the alarm from an mp3 file (2 audio tracks are provided) set in the settings window of Wox
- Allows you to control a stopwatch

This plugin is not yet uploaded to Wox/Plugins, but will be in a couple of days when code is sorted out completely. 

TODOs:
- Test with various file formats. Mp3 and Wav are not enough. 
- Change the command hierarchy handling:
- - commandDepth must be refactored. The idea is OK, but the implementation is shit. 
- - reconsider a separate query parser. 