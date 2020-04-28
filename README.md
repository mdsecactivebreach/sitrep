# SitRep
Extensible, configurable host triage.

## Purpose
SitRep is intended to provide a lightweight, extensible host triage alternative. Checks are loaded dynamically at runtime from stand-alone files. This allows operators to quickly modify existing checks, or add new checks as required.

Checks are grouped by category and can be marked as OpSec safe/unsafe. unsafe checks are only loaded if the /AllowUnsafe flag is provided. 

Interesting results are highlighted with a "[\*]" 

## Checks
Checks are separated into categories. This allows them to be displayed in appropriate groups. The following checks are currently available: 

**Environment**
* CurrentUser.cs - the current user
* DomainName.cs - the domain name
* HostName.cs - the hostname
* LoggedOnUsers.cs - List all logged on users
* OSVersion.cs - OS version information 
* VirtualEnvironment.cs - Checks if we are operating in a virtualised environment
* UserDomainGroups.cs - Gets the users domain group memberships
* userEnvironmentVariables.cs - Grabs the environment variables applied to the current process 
* SystemEnvironmentVariables.cs - Grabs system environment variables from the registry (HKLM)

**Defences**
* AVProcesses.cs - Checks if any known AV processes are running

**Permissions**
* Integrity.cs - Get the integrity level of the current process
* LocalAdmin.cs - Check if we are a local admin
* Privileges.cs - List our current privileges.
* UACLevel.cs - Get the UAC level

**Software**
* InstalledBrowsers.cs - Lists the browsers installed on the endpoint

**Credentials**
* CredentialManager.cs - Retrieve credentials stored in Windows Credential Manager for the current user

## Disabling Checks
All checks are enabled by default. However, as checks are loaded dynamically, it is possible to disable them.

**Disbling a check**

CheckBase includes a boolean "Enabled" property, which defaults to true. This can be set in the derived class by adding a constructor. The example below disables the CurrentUser check (CurrentUser.cs):

```
public CurrentUser()
{
    base.Enabled = false;
}
```
**Excluding checks from the build** 

As checks are loaded dynamically, it is possible to exclude a check from the build without other modifications. The easiest way to do this is to right-click on the check class in Visual Studio and select "exclude from project". The check can be re-added by selecting "include in project" from the same context menu. 

This approach has the advantage of removing the code from the compiled artifact.

## Example Usage

**Run all checks**
```
SitRep.exe /AllowUnsafe
```
**Run only OpSec safe checks (default)**
```
SitRep.exe
```
SitRep is designed to be executed via execute-assembly (or equivalent) 

![screenshot](https://github.com/mdsecactivebreach/sitrep/blob/master/execute-assembly-example.png)

## Adding Checks
Checks inherit from CheckBase and implement the ICheck interface. This enforces the patterns needed for the dynamic check loading. Other methods and classes can be added as required.

The ICheck interface exposes the following properties and methods:
* IsOpsecSafe (bool) - Indicates if the check is considered OpSec safe or not
* DisplayOrder (int) - The order in which to display the result of this check within its display group
* Check() - The method called to run the actual check

Derived classes must override the "ToString()" method defined in CheckBase. This method is called when displaying the output of each check.

Access to native methods is provided via classes in the "NativeMethods" folder. Each class is named after the dll it interacts with. 

An example, empty check is shown below

```
using SitRep.Interfaces;
using System;

namespace SitRep.Checks.Software
{
    class ExampleCheck : CheckBase, ICheck
    {
        public bool IsOpsecSafe => true;

        public int DisplayOrder => 1;

        public Enums.Enums.CheckType CheckType => Enums.Enums.CheckType.Credential;

        public void Check()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
```
## Contributing
PRs welcome. Please ensure checks are stand-alone (i.e. not dependent on the output of other checks). As far as possible, checks should be self-contained, with all single-use code present within the check class.

## Why no unit tests?
Have you ever tried mocking a domain-joined Windows endpoint? That's why. 

## Thanks
SitRep makes use of code from Seatbelt, SharpUp and random StackOverflow posts. Credits have been added where appropriate. 

