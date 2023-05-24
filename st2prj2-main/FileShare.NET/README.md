## FileShare library

### Library

Contains two classes each with one public method

* `Uploader`: To upload data from a file to a shared repository, where it can be access by others (including other computers)
  * Constructor takes a group name parameter - to be nice to others, please use group number and semester e.g. groupp_10_f23
  * `Save'` Takes a filename and a `FileStream`. 
    * The filename cannot be reused - to avoid that other groups overwrite your data.
    * `FileStream` should point to the data saved on the harddisk e.g. data saved on RaspBerryPi from the ADC
    * Return value is the filename the data is saved in - the same as the filename parameter - unless it has already be used.
* 'Downloader': To download data from a shared repository to a local file.
  * Constructor takes a group name parameter - to be nice to others, please use group number and semester e.g. groupp_10_f23. Note: Must be the same as when you upload.
  * `Load'` Takes a filename and a `FileStream`.
    * The filename must match one you have saved already
    * `FileStream` should point to a new file on you harddisk and should be opened in Write mode.
  * `GetFilenames` Load all files uploaded and return the names in a list.

### Demo application 

Usage can be found in FileShareApplication/Program.cs


### Usage

**From files:** 

1. Clone (or download from Gitlab).
2. Copy the FileShare folder into your project
3. In VS2022 (in your own project)
   1. If project don't show by it self. Add Existing project
4. Add reference to FileShare from your application.

**From Nuget**

1. First time add source as described below
2. Go to Nuget packages in Visual studio [Detail description](https://learn.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio)
3. Search for FileShare and add to project.

### From Nuget


This is deployed on [ECE Nuget server](https://swwao.orbit.au.dk/nuget/). This means you can add the Nuget server as normal and thereby use the packages here, as you will with any other Nuget package from nuget.org.

**Adding repository**

Run the following command in Powershell / terminal

$ dotnet nuget add source https://swwao.orbit.au.dk/nuget/v3/index.json -n ece-nuget

**Removing repository**

Run the following command in Powershell / terminal

$ dotnet nuget remove ece-nuget






