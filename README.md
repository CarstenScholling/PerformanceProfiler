Business Central Application Profiler
=====================================
Sample for profiling Microsoft Dynamics NAV Application code. Consumes NAV Execution Events from ETW.

The sample consists of C# code for listening to NAV Server ETW events and Application objects for storing the events in a Table and presenting the results in a Page.

**Disclamer:** Use at own risk. No warranty or Guaranty. No support.

Installing and Running
=====================================
* Compile the dll (project EtwPerformanceProfiler)
* Create a zip archive with the contents of the dll\bin\Release folder
* In Business Central, open Control Add-ins and create an entry with the following values
  * Add-in Name: EtwPerformanceProfiler
  * Public Key Token: e5b5a7b2def5d864
  * Category: DotNet Interoperability
* Import the created archive as resource
* Compile and install the Performance Profiler Business Central app
* Search for 'Performance Profiler'

If you want to analyze ETL file here is an article, which describes how to collect ETL file for Dynamics NAV.
http://msdn.microsoft.com/en-us/library/dn271709(v=nav.71).aspx

- Moved Provider Name and Session Name configuration to DLL app.config
- Added UserName, Tenant, AppId, AppInfo to Event classes
