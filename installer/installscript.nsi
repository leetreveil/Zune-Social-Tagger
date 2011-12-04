;NSIS Modern User Interface
;Start Menu Folder Selection Example Script
;Written by Joost Verburg

!define MULTIUSER_EXECUTIONLEVEL Admin
!include MultiUser.nsh

;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"

;--------------------------------

Function .onInit
  !insertmacro MULTIUSER_INIT
FunctionEnd

Function un.onInit
  !insertmacro MULTIUSER_UNINIT
FunctionEnd

;General

  ;Name and file
  Name "Zune Social Tagger"
  OutFile "zstinstaller.exe"

  ;Default installation folder
  InstallDir "C:\Program Files\Zune"
  
  ;Get installation folder from registry if available
  InstallDirRegKey HKCU "Software\Zune Social Tagger" ""

  ;Request application privileges for Windows Vista
  RequestExecutionLevel admin

;--------------------------------
;Variables

  Var StartMenuFolder

;--------------------------------
;Interface Settings

  !define MUI_ICON "zuneicondark.ico"
  !define MUI_ABORTWARNING
  !define MUI_WELCOMEFINISHPAGE_BITMAP "header.bmp"

Function LaunchLink
  ExecShell "" "$DESKTOP\Zune Social Tagger.lnk"
FunctionEnd

;--------------------------------
;Pages

  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_LICENSE "License.txt"
  !insertmacro MUI_PAGE_DIRECTORY
  
  ;Start Menu Folder Page Configuration
  !define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
  !define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\Zune Social Tagger" 
  !define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
  
  !insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder
  
  !insertmacro MUI_PAGE_INSTFILES
  
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES

  !define MUI_FINISHPAGE_RUN
  !define MUI_FINISHPAGE_RUN_TEXT "Run Zune Social Tagger Now"
  !define MUI_FINISHPAGE_RUN_FUNCTION "LaunchLink"
  !insertmacro MUI_PAGE_FINISH

;--------------------------------
;Languages
 
  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "Dummy Section" SecDummy

  SetOutPath "$INSTDIR"

  ;Remove any files that may interfere with the installation
  Delete "$INSTDIR\Zune Social Tagger.exe"
  Delete "$INSTDIR\Zune Social Tagger.exe.config"
  Delete "$INSTDIR\ZuneSocialTagger.Core.dll"
	
  
  ;ADD YOUR OWN FILES HERE...
  File "installfiles\Zune Social Tagger.exe"
  File "installfiles\Zune Social Tagger.exe.config"
  
  ;Store installation folder
  WriteRegStr HKCU "Software\Zune Social Tagger" "" $INSTDIR
  
  ;Create uninstaller
  WriteUninstaller "$INSTDIR\ZuneSocialTaggerUninstall.exe"
  
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
    
    ;Create shortcuts
    CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
    CreateShortCut "$SMPROGRAMS\$StartMenuFolder\Uninstall.lnk" "$INSTDIR\ZuneSocialTaggerUninstall.exe"
    CreateShortcut "$SMPROGRAMS\$StartMenuFolder\Zune Social Tagger.lnk" "$OUTDIR\Zune Social Tagger.exe"
    CreateShortcut "$DESKTOP\Zune Social Tagger.lnk" "$OUTDIR\Zune Social Tagger.exe"
  
  !insertmacro MUI_STARTMENU_WRITE_END

SectionEnd

;--------------------------------
;Descriptions

  ;Language strings
  LangString DESC_SecDummy ${LANG_ENGLISH} "A test section."

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${SecDummy} $(DESC_SecDummy)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END
 
;--------------------------------
;Uninstaller Section

Section "Uninstall"

  ;ADD YOUR OWN FILES HERE...

  Delete "$INSTDIR\ZuneSocialTaggerUninstall.exe"
  Delete "$INSTDIR\Zune Social Tagger.exe"
  Delete "$INSTDIR\Zune Social Tagger.exe.config"
  
  !insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder
    
  Delete "$DESKTOP\Zune Social Tagger.lnk"
  Delete "$SMPROGRAMS\$StartMenuFolder\Uninstall.lnk"
  Delete "$SMPROGRAMS\$StartMenuFolder\Zune Social Tagger.lnk"
  RMDir "$SMPROGRAMS\$StartMenuFolder"
  
  DeleteRegKey /ifempty HKCU "Software\Zune Social Tagger"

SectionEnd