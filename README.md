# WPF Cast Server

This is a no-frills web application for playing back lists of audio files. It is based on my stand-alone 
[Cast Client](https://github.com/vivajohn/castclient) project. However, it offers the additional ability to save playlists.

Google Chrome cast is available in Windows only within the Chrome browser so one cannot make a desktop application
which offers casting ability. So this project has two parts: a web page for displaying and playing playlists and a
WPF desktop application for managing the audio files. The client web application is [here](https://github.com/vivajohn/wpfcastclient).

The desktop application has a server for playing back audio
files and internet radio stations. This was added because the way Chromecast works, from what I can understand,
is that it sends the url of the playback item to every speaker in the network. So if one wants to play an 
internet radio station on 3 speakers, each of the 3 speakers will access the radio station. This results in
occasional stuttering. So the desktop application plays the file or station and returns its stream to the client.
(It acts as a sort of proxy server.)

For security reasons, web pages cannot access the local file system. For this reason, the desktop application
manages saving and serving playlists. One can drag and drop files onto the web page, but this will be
intercepted by the desktop application and the list will be sent back to the web page.

You can see a demo [here](https://youtu.be/RXzwKv5zNLY).
 
**Features**
 1. Files may be uploaded using the upload button or dragging and dropping files onto the page
 2. Next/Previous buttons for moving through the list or click on an item to start playing it immediately
 3. Volume, looping and shuffle controls
 4. Play order may be changed by dragging and dropping within the list
 5. Playlists can be saved.

**Trouble-shooting:**
- If files don't play or only the first file plays, make sure that you do not have 'autoplay' disabled in your browser.

**Tech stuff:**
- C#, Angular 7, Typescript, Html, Css, Scss
- The web application was developed using the Angular CLI in Visual Studio Code 
- The desktop application was developed using WPF and .NET Core 3 (preview version) in Visual Studio 2019 (also preview version).
- Playback is done via a REST Api. For other data, the server and client communicate using Microsoft's SignalR package (which uses web sockets).

