# bisportalservice
Simple service to execute a batch file which runs a java command. Monitors for changes, capable of restarting on change.

# install

c:\Windows\Microsoft.NET\Framework\v4.0.30319>installutil.exe BizportalService.exe

# uninstall

c:\Windows\Microsoft.NET\Framework\v4.0.30319>installutil.exe /uninstall BizportalService.exe

# example settings file

<?xml version="1.0" encoding="utf-8"?>
<Settings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Filename>c:\Source\Settings.xml</Filename>
  <BatchFile>c:\Source\startup.bat</BatchFile>
  <JarFile>c:\Source\demo-0.0.1-SNAPSHOT.jar</JarFile>
  <MonitorChanges>true</MonitorChanges>
</Settings>

# example batch file

java -jar %1





