# Business Central Application Profiler

This is an Application Profiler for Business Central. Originally written for Dynamics NAV and maintained by David Worthington, this port was updated to an (OnPrem) app and working with Business Central 15 onwards.

The code is based on Event Tracing for Windows (ETW) and consumes events raised from a Business Central or Dynamics NAV server.

The repository was restructured to contain the AL part, the DLL, and unmodified C/AL source code.

**Disclamer:** Use at own risk. No warranty or Guaranty. No support.

## Installing and Running
* Compile the dll (project EtwPerformanceProfiler)
* Create a zip archive with the contents of the dll\bin\Release folder
* In Business Central, open Control Add-ins and create an entry with the following values
  * Add-in Name: EtwPerformanceProfiler
  * Public Key Token: c78da9523a37b97f
  * Category: DotNet Interoperability
* Import the created archive as resource
* Compile and install the Performance Profiler Business Central app
* Search for 'Performance Profiler'

## Change Log
- Moved ETW Provider Name and Session Name configuration to DLL app.config
- Added UserName, Tenant, AppId, AppInfo to Event classes
- Added AL app
- Restructured repository

